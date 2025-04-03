using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services.Interfaces;
using Microsoft.Data.SqlClient;
using SynchronizationApplication.Services.Providers;

namespace SynchronizationApplication.Services
{
    /// <summary>
    /// Service for managing database triggers for change tracking
    /// </summary>
    public class TriggerService : ITriggerService
    {
        private readonly IDatabaseService _databaseService;

        // Define the specific columns to sync for Users table
        private readonly List<string> _userColumnsToSync = new List<string>
        {
            "UserID",
            "UserName",
            "UserPass",
            "UserSysName",
            "UserLevel",
            "UserPhone",
            "UserEnabled",
            "UserNote",
            "UserAddedDate",
            "Image",
            "Email",
            "Gender",
            "PermissionType"
        };

        public TriggerService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> DoTriggersExistAsync(string connectionString, EntityType entityType)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetTableName(entityType);
            string prefix = entityType == EntityType.Customer ? "trg_Customers" : "trg_Users";

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Check for INSERT trigger
                using (var command = provider.CreateCommand(
                    $"SELECT COUNT(*) FROM sys.triggers WHERE name = '{prefix}_Insert'", connection))
                {
                    bool insertTriggerExists = Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;

                    // Check for UPDATE trigger
                    using (var updateCmd = provider.CreateCommand(
                        $"SELECT COUNT(*) FROM sys.triggers WHERE name = '{prefix}_Update'", connection))
                    {
                        bool updateTriggerExists = Convert.ToInt32(await updateCmd.ExecuteScalarAsync()) > 0;

                        // Check for DELETE trigger
                        using (var deleteCmd = provider.CreateCommand(
                            $"SELECT COUNT(*) FROM sys.triggers WHERE name = '{prefix}_Delete'", connection))
                        {
                            bool deleteTriggerExists = Convert.ToInt32(await deleteCmd.ExecuteScalarAsync()) > 0;

                            // All triggers must exist
                            return insertTriggerExists && updateTriggerExists && deleteTriggerExists;
                        }
                    }
                }
            }
        }

        public async Task<bool> DoesChangeTrackingTableExistAsync(string connectionString, EntityType entityType)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetChangeTrackingTableName(entityType);
            return await provider.TableExistsAsync(connectionString, tableName);
        }

        public async Task EnsureStatusColumnsExistAsync(string connectionString, EntityType entityType)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetChangeTrackingTableName(entityType);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Check if the status column exists
                bool statusExists = false;

                string checkColumnSql = $@"
                    SELECT COUNT(*) 
                    FROM sys.columns 
                    WHERE object_id = OBJECT_ID('{tableName}') AND name = 'Status'";

                using (var command = provider.CreateCommand(checkColumnSql, connection))
                {
                    statusExists = Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
                }

                // If the column doesn't exist, add the status columns
                if (!statusExists)
                {
                    string addColumnsSql = $@"
                        ALTER TABLE [dbo].[{tableName}] ADD
                            [Status] [varchar](20) NOT NULL DEFAULT 'Pending',
                            [ProcessedTime] [datetime] NULL,
                            [ErrorMessage] [nvarchar](max) NULL";

                    using (var command = provider.CreateCommand(addColumnsSql, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task CreateTriggersAsync(string connectionString, EntityType entityType)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetTableName(entityType);
            string logTableName = provider.GetChangeTrackingTableName(entityType);
            string idColumnName = provider.GetEntityIdColumnName(entityType);
            string triggerPrefix = entityType == EntityType.Customer ? "trg_Customers" : "trg_Users";

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Start a transaction to ensure all operations succeed or fail together
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // First, create the change tracking table if it doesn't exist
                        bool tableExists = await DoesChangeTrackingTableExistAsync(connectionString, entityType);

                        if (!tableExists)
                        {
                            string createTableSql = $@"
                                CREATE TABLE [dbo].[{logTableName}] (
                                    [LogID] [int] IDENTITY(1,1) NOT NULL,
                                    [{idColumnName}] [int] NOT NULL,
                                    [ChangeType] [varchar](10) NOT NULL,
                                    [ChangeTime] [datetime] NOT NULL,
                                    [ChangedBy] [int] NULL,
                                    [Status] [varchar](20) NOT NULL DEFAULT 'Pending',
                                    [ProcessedTime] [datetime] NULL,
                                    [ErrorMessage] [nvarchar](max) NULL,
                                    CONSTRAINT [PK_{logTableName}] PRIMARY KEY CLUSTERED ([LogID] ASC)
                                );";

                            using (var command = provider.CreateCommand(createTableSql, connection, transaction))
                            {
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            // Ensure status columns exist if the table already exists
                            string checkColumnSql = $@"
                                IF NOT EXISTS (SELECT * FROM sys.columns 
                                              WHERE object_id = OBJECT_ID('{logTableName}') AND name = 'Status')
                                BEGIN
                                    ALTER TABLE [dbo].[{logTableName}] ADD
                                        [Status] [varchar](20) NOT NULL DEFAULT 'Pending',
                                        [ProcessedTime] [datetime] NULL,
                                        [ErrorMessage] [nvarchar](max) NULL
                                END";

                            using (var command = provider.CreateCommand(checkColumnSql, connection, transaction))
                            {
                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        // Create the appropriate triggers based on entity type
                        if (entityType == EntityType.Customer)
                        {
                            // Create standard triggers for Customer table
                            await CreateStandardTriggersAsync(connection, transaction, provider, tableName, logTableName, idColumnName, triggerPrefix);
                        }
                        else // User
                        {
                            // Create modified triggers for User table to only track specified columns
                            await CreateUserTriggersAsync(connection, transaction, provider, tableName, logTableName, idColumnName, triggerPrefix);
                        }

                        // Commit the transaction
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        // If anything fails, roll back the transaction
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private async Task CreateStandardTriggersAsync(
            DbConnection connection,
            DbTransaction transaction,
            IDatabaseProvider provider,
            string tableName,
            string logTableName,
            string idColumnName,
            string triggerPrefix)
        {
            // Create the INSERT trigger
            string insertTriggerSql = $@"
                CREATE OR ALTER TRIGGER [dbo].[{triggerPrefix}_Insert]
                ON [dbo].[{tableName}]
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    INSERT INTO [dbo].[{logTableName}] ({idColumnName}, ChangeType, ChangeTime)
                    SELECT {idColumnName}, 'Insert', GETDATE()
                    FROM inserted;
                END;";

            using (var command = provider.CreateCommand(insertTriggerSql, connection, transaction))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Create the UPDATE trigger
            string updateTriggerSql = $@"
                CREATE OR ALTER TRIGGER [dbo].[{triggerPrefix}_Update]
                ON [dbo].[{tableName}]
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    INSERT INTO [dbo].[{logTableName}] ({idColumnName}, ChangeType, ChangeTime)
                    SELECT {idColumnName}, 'Update', GETDATE()
                    FROM inserted;
                END;";

            using (var command = provider.CreateCommand(updateTriggerSql, connection, transaction))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Create the DELETE trigger
            string deleteTriggerSql = $@"
                CREATE OR ALTER TRIGGER [dbo].[{triggerPrefix}_Delete]
                ON [dbo].[{tableName}]
                AFTER DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    INSERT INTO [dbo].[{logTableName}] ({idColumnName}, ChangeType, ChangeTime)
                    SELECT {idColumnName}, 'Delete', GETDATE()
                    FROM deleted;
                END;";

            using (var command = provider.CreateCommand(deleteTriggerSql, connection, transaction))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateUserTriggersAsync(
            DbConnection connection,
            DbTransaction transaction,
            IDatabaseProvider provider,
            string tableName,
            string logTableName,
            string idColumnName,
            string triggerPrefix)
        {
            // Create the INSERT trigger - for User table we still track all inserts
            string insertTriggerSql = $@"
                CREATE OR ALTER TRIGGER [dbo].[{triggerPrefix}_Insert]
                ON [dbo].[{tableName}]
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    INSERT INTO [dbo].[{logTableName}] ({idColumnName}, ChangeType, ChangeTime)
                    SELECT {idColumnName}, 'Insert', GETDATE()
                    FROM inserted;
                END;";

            using (var command = provider.CreateCommand(insertTriggerSql, connection, transaction))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Create the UPDATE trigger with specific columns for User table
            // Build the UPDATE condition properly: UPDATE(Col1) OR UPDATE(Col2) OR UPDATE(Col3)...
            List<string> updateConditions = new List<string>();
            foreach (string column in _userColumnsToSync)
            {
                updateConditions.Add($"UPDATE({column})");
            }
            string updateConditionSql = string.Join(" OR ", updateConditions);

            string updateTriggerSql = $@"
                CREATE OR ALTER TRIGGER [dbo].[{triggerPrefix}_Update]
                ON [dbo].[{tableName}]
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    -- Only track changes if relevant columns were updated
                    IF {updateConditionSql}
                    BEGIN
                        INSERT INTO [dbo].[{logTableName}] ({idColumnName}, ChangeType, ChangeTime)
                        SELECT {idColumnName}, 'Update', GETDATE()
                        FROM inserted;
                    END
                END;";

            using (var command = provider.CreateCommand(updateTriggerSql, connection, transaction))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Create the DELETE trigger - for User table we still track all deletes
            string deleteTriggerSql = $@"
                CREATE OR ALTER TRIGGER [dbo].[{triggerPrefix}_Delete]
                ON [dbo].[{tableName}]
                AFTER DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    INSERT INTO [dbo].[{logTableName}] ({idColumnName}, ChangeType, ChangeTime)
                    SELECT {idColumnName}, 'Delete', GETDATE()
                    FROM deleted;
                END;";

            using (var command = provider.CreateCommand(deleteTriggerSql, connection, transaction))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoveTriggersAsync(string connectionString, EntityType entityType)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string triggerPrefix = entityType == EntityType.Customer ? "trg_Customers" : "trg_Users";
            string logTableName = provider.GetChangeTrackingTableName(entityType);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Start a transaction to ensure all operations succeed or fail together
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Drop the INSERT trigger if it exists
                        string dropInsertTriggerSql = $@"
                            IF EXISTS (SELECT * FROM sys.triggers WHERE name = '{triggerPrefix}_Insert')
                            DROP TRIGGER [dbo].[{triggerPrefix}_Insert];";

                        using (var command = provider.CreateCommand(dropInsertTriggerSql, connection, transaction))
                        {
                            await command.ExecuteNonQueryAsync();
                        }

                        // Drop the UPDATE trigger if it exists
                        string dropUpdateTriggerSql = $@"
                            IF EXISTS (SELECT * FROM sys.triggers WHERE name = '{triggerPrefix}_Update')
                            DROP TRIGGER [dbo].[{triggerPrefix}_Update];";

                        using (var command = provider.CreateCommand(dropUpdateTriggerSql, connection, transaction))
                        {
                            await command.ExecuteNonQueryAsync();
                        }

                        // Drop the DELETE trigger if it exists
                        string dropDeleteTriggerSql = $@"
                            IF EXISTS (SELECT * FROM sys.triggers WHERE name = '{triggerPrefix}_Delete')
                            DROP TRIGGER [dbo].[{triggerPrefix}_Delete];";

                        using (var command = provider.CreateCommand(dropDeleteTriggerSql, connection, transaction))
                        {
                            await command.ExecuteNonQueryAsync();
                        }

                        // Drop the change tracking table if it exists
                        string dropTableSql = $@"
                            IF EXISTS (SELECT * FROM sys.tables WHERE name = '{logTableName}')
                            DROP TABLE [dbo].[{logTableName}];";

                        using (var command = provider.CreateCommand(dropTableSql, connection, transaction))
                        {
                            await command.ExecuteNonQueryAsync();
                        }

                        // Commit the transaction
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        // If anything fails, roll back the transaction
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<List<string>> GetTriggerDetailsAsync(string connectionString, EntityType entityType)
        {
            var result = new List<string>();
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetTableName(entityType);
            string logTableName = provider.GetChangeTrackingTableName(entityType);
            string triggerPrefix = entityType == EntityType.Customer ? "trg_Customers" : "trg_Users";
            string entityName = entityType == EntityType.Customer ? "Customer" : "User";

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Check if the change tracking table exists
                bool tableExists = await DoesChangeTrackingTableExistAsync(connectionString, entityType);
                result.Add($"{entityName} Change Tracking Table: {(tableExists ? "Exists" : "Not Found")}");

                if (tableExists)
                {
                    // Check for status columns
                    string checkColumnSql = $@"
                        SELECT COUNT(*) 
                        FROM sys.columns 
                        WHERE object_id = OBJECT_ID('{logTableName}') AND name = 'Status'";

                    using (var command = provider.CreateCommand(checkColumnSql, connection))
                    {
                        bool hasStatusColumn = Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
                        result.Add($"Status Tracking: {(hasStatusColumn ? "Enabled" : "Not Enabled")}");
                    }

                    // Get the row count
                    using (var command = provider.CreateCommand(
                        $"SELECT COUNT(*) FROM [dbo].[{logTableName}]", connection))
                    {
                        int rowCount = Convert.ToInt32(await command.ExecuteScalarAsync());
                        result.Add($"Change Log Entries: {rowCount}");
                    }

                    // Get status counts
                    string statusQuery = $@"
                        SELECT Status, COUNT(*) as Count
                        FROM [dbo].[{logTableName}]
                        GROUP BY Status
                        ORDER BY Status";

                    try
                    {
                        using (var command = provider.CreateCommand(statusQuery, connection))
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (reader.HasRows)
                                {
                                    result.Add("Change Status Summary:");
                                    while (await reader.ReadAsync())
                                    {
                                        string status = reader.GetString(0);
                                        int count = reader.GetInt32(1);
                                        result.Add($"  - {status}: {count}");
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // If this fails, status columns may not exist
                    }

                    // Get the latest changes
                    using (var command = provider.CreateCommand(
                        $@"SELECT TOP 5 {provider.GetEntityIdColumnName(entityType)}, ChangeType, ChangeTime 
                          FROM [dbo].[{logTableName}] 
                          ORDER BY ChangeTime DESC", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            result.Add("Last 5 changes:");
                            int count = 0;

                            while (await reader.ReadAsync())
                            {
                                int entityId = reader.GetInt32(0);
                                string changeType = reader.GetString(1);
                                DateTime changeTime = reader.GetDateTime(2);

                                result.Add($"  - {entityName} {entityId}: {changeType} at {changeTime}");
                                count++;
                            }

                            if (count == 0)
                            {
                                result.Add("  (No changes recorded yet)");
                            }
                        }
                    }
                }

                // Check if the triggers exist
                using (var command = provider.CreateCommand(
                    $@"SELECT name, is_disabled 
                      FROM sys.triggers 
                      WHERE name IN ('{triggerPrefix}_Insert', '{triggerPrefix}_Update', '{triggerPrefix}_Delete')",
                    connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        result.Add("Triggers:");
                        int count = 0;

                        while (await reader.ReadAsync())
                        {
                            string name = reader.GetString(0);
                            bool isDisabled = reader.GetBoolean(1);

                            result.Add($"  - {name}: {(isDisabled ? "Disabled" : "Enabled")}");
                            count++;
                        }

                        if (count == 0)
                        {
                            result.Add("  (No triggers found)");
                        }
                    }
                }
            }

            // If this is a User entity, add information about which columns are being synced
            if (entityType == EntityType.User)
            {
                result.Add("\nUser Columns Being Synchronized:");
                foreach (var column in _userColumnsToSync)
                {
                    result.Add($"  - {column}");
                }
            }

            return result;
        }
    }


    /// <summary>
    /// Service for tracking and synchronizing changes
    /// </summary>
    public class ChangeTrackingService : IChangeTrackingService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ICustomerDataService _customerDataService;
        private readonly IUserDataService _userDataService;

        // SQL Server minimum date is 1753-01-01
        private static readonly DateTime MinSqlDate = new DateTime(1753, 1, 1);

        // Define the specific columns to sync for Users table
        private readonly List<string> _userColumnsToSync = new List<string>
        {
            "UserID",
            "UserName",
            "UserPass",
            "UserSysName",
            "UserLevel",
            "UserPhone",
            "UserEnabled",
            "UserNote",
            "UserAddedDate",
            "Image",
            "Email",
            "Gender",
            "PermissionType"
        };

        public ChangeTrackingService(
            IDatabaseService databaseService,
            ICustomerDataService customerDataService,
            IUserDataService userDataService)
        {
            _databaseService = databaseService;
            _customerDataService = customerDataService;
            _userDataService = userDataService;
        }

        public async Task<List<EntityChange>> GetChangesAsync(
            string connectionString,
            EntityType entityType,
            DateTime sinceDate,
            CancellationToken cancellationToken = default)
        {
            // Ensure sinceDate is valid for SQL Server
            if (sinceDate < MinSqlDate)
            {
                sinceDate = MinSqlDate;
            }

            var result = new List<EntityChange>();
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetChangeTrackingTableName(entityType);
            string idColumnName = provider.GetEntityIdColumnName(entityType);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                // Check if the change tracking table exists
                bool tableExists = await provider.TableExistsAsync(connectionString, tableName);

                if (!tableExists)
                {
                    // No changes tracked yet
                    return result;
                }

                // Query the change log for pending or failed changes since the specified date
                string sql = $@"
                SELECT LogID, {idColumnName}, ChangeType, ChangeTime, ChangedBy, 
                       Status, ProcessedTime, ErrorMessage
                FROM [dbo].[{tableName}]
                WHERE (Status = 'Pending' OR Status = 'Failed') 
                  AND ChangeTime > @SinceDate
                ORDER BY ChangeTime ASC";

                try
                {
                    using (var command = provider.CreateCommand(sql, connection))
                    {
                        command.Parameters.Add(provider.CreateParameter("@SinceDate", sinceDate));

                        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                EntityChange change = CreateEntityChange(entityType);

                                change.LogID = reader.GetInt32(0);
                                change.EntityID = reader.GetInt32(1);
                                change.ChangeType = ParseChangeType(reader.GetString(2));
                                change.ChangeTime = reader.GetDateTime(3);
                                change.ChangedBy = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                                change.Status = reader.GetString(5);
                                change.ProcessedTime = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                                change.ErrorMessage = reader.IsDBNull(7) ? null : reader.GetString(7);

                                result.Add(change);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Status columns might not exist yet
                    // Fall back to the simpler query without status
                    string basicSql = $@"
                    SELECT LogID, {idColumnName}, ChangeType, ChangeTime, ChangedBy
                    FROM [dbo].[{tableName}]
                    WHERE ChangeTime > @SinceDate
                    ORDER BY ChangeTime ASC";

                    using (var command = provider.CreateCommand(basicSql, connection))
                    {
                        command.Parameters.Add(provider.CreateParameter("@SinceDate", sinceDate));

                        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                EntityChange change = CreateEntityChange(entityType);

                                change.LogID = reader.GetInt32(0);
                                change.EntityID = reader.GetInt32(1);
                                change.ChangeType = ParseChangeType(reader.GetString(2));
                                change.ChangeTime = reader.GetDateTime(3);
                                change.ChangedBy = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                                change.Status = "Pending"; // Default status

                                result.Add(change);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public async Task<List<EntityChange>> GetFailedChangesAsync(
            string connectionString,
            EntityType entityType,
            CancellationToken cancellationToken = default)
        {
            var result = new List<EntityChange>();
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetChangeTrackingTableName(entityType);
            string idColumnName = provider.GetEntityIdColumnName(entityType);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                // Check if the change tracking table exists
                bool tableExists = await provider.TableExistsAsync(connectionString, tableName);

                if (!tableExists)
                {
                    // No changes tracked yet
                    return result;
                }

                // Query for failed changes
                string sql = $@"
                SELECT LogID, {idColumnName}, ChangeType, ChangeTime, ChangedBy, 
                       Status, ProcessedTime, ErrorMessage
                FROM [dbo].[{tableName}]
                WHERE Status = 'Failed'
                ORDER BY ChangeTime DESC";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            EntityChange change = CreateEntityChange(entityType);

                            change.LogID = reader.GetInt32(0);
                            change.EntityID = reader.GetInt32(1);
                            change.ChangeType = ParseChangeType(reader.GetString(2));
                            change.ChangeTime = reader.GetDateTime(3);
                            change.ChangedBy = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                            change.Status = reader.GetString(5);
                            change.ProcessedTime = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                            change.ErrorMessage = reader.IsDBNull(7) ? null : reader.GetString(7);
                            change.IsSelected = true; // Select by default for retry

                            result.Add(change);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<List<EntityChange>> GetAllChangesAsync(
            string connectionString,
            EntityType entityType,
            int maxRows = 100,
            CancellationToken cancellationToken = default)
        {
            var result = new List<EntityChange>();
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetChangeTrackingTableName(entityType);
            string idColumnName = provider.GetEntityIdColumnName(entityType);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                // Check if the change tracking table exists
                bool tableExists = await provider.TableExistsAsync(connectionString, tableName);

                if (!tableExists)
                {
                    // No changes tracked yet
                    return result;
                }

                // Query for all changes, limited by maxRows
                string sql = $@"
                SELECT TOP {maxRows} LogID, {idColumnName}, ChangeType, ChangeTime, ChangedBy, 
                       Status, ProcessedTime, ErrorMessage
                FROM [dbo].[{tableName}]
                ORDER BY ChangeTime DESC";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await reader.ReadAsync(cancellationToken))
                        {
                            EntityChange change = CreateEntityChange(entityType);

                            change.LogID = reader.GetInt32(0);
                            change.EntityID = reader.GetInt32(1);
                            change.ChangeType = ParseChangeType(reader.GetString(2));
                            change.ChangeTime = reader.GetDateTime(3);
                            change.ChangedBy = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                            change.Status = reader.IsDBNull(5) ? "Pending" : reader.GetString(5);
                            change.ProcessedTime = reader.IsDBNull(6) ? null : reader.GetDateTime(6);
                            change.ErrorMessage = reader.IsDBNull(7) ? null : reader.GetString(7);

                            result.Add(change);
                        }
                    }
                }
            }

            return result;
        }

        private ChangeType ParseChangeType(string changeTypeStr)
        {
            return changeTypeStr.ToLower() switch
            {
                "insert" => ChangeType.Insert,
                "update" => ChangeType.Update,
                "delete" => ChangeType.Delete,
                _ => throw new ArgumentException($"Unknown change type: {changeTypeStr}")
            };
        }

        private EntityChange CreateEntityChange(EntityType entityType)
        {
            return entityType == EntityType.Customer
                ? new CustomerChange()
                : (EntityChange)new UserChange();
        }

        public async Task<SyncResult> SynchronizeChangesAsync(
            string sourceConnectionString,
            string targetConnectionString,
            List<EntityChange> changes,
            bool syncDeletes,
            IProgress<SyncProgress> progress,
            CancellationToken cancellationToken = default)
        {
            if (changes == null || changes.Count == 0)
            {
                return new SyncResult() { TotalRecords = 0 };
            }

            // Determine the entity type from the first change
            EntityType entityType = changes[0].GetEntityType();

            var result = new SyncResult
            {
                TotalRecords = changes.Count,
                EntityType = entityType
            };

            // Create progress reporter
            var progressInfo = new SyncProgress
            {
                TotalRecords = changes.Count,
                StatusMessage = "Starting auto sync...",
                EntityType = entityType
            };

            progress?.Report(progressInfo);

            // Process each change one by one
            for (int i = 0; i < changes.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    result.ErrorMessages.Add("Auto sync was cancelled by the user.");
                    return result;
                }

                var change = changes[i];
                progressInfo.CurrentRecord = i + 1;
                string entityName = entityType == EntityType.Customer ? "customer" : "user";
                progressInfo.StatusMessage = $"Processing change {i + 1} of {changes.Count}: {change.ChangeType} for {entityName} {change.EntityID}";
                progress?.Report(progressInfo);

                try
                {
                    // Handle the change based on its type
                    switch (change.ChangeType)
                    {
                        case ChangeType.Insert:
                        case ChangeType.Update:
                            await SynchronizeInsertOrUpdate(sourceConnectionString, targetConnectionString, change.EntityID, entityType);
                            result.ProcessedRecords++;

                            if (change.ChangeType == ChangeType.Insert)
                                result.AddedRecords++;
                            else
                                result.UpdatedRecords++;

                            // Mark as successful
                            await UpdateChangeStatusAsync(sourceConnectionString, entityType, change.LogID, "Success", null);
                            break;

                        case ChangeType.Delete:
                            if (syncDeletes)
                            {
                                await SynchronizeDelete(targetConnectionString, change.EntityID, entityType);
                                result.ProcessedRecords++;

                                // Mark as successful
                                await UpdateChangeStatusAsync(sourceConnectionString, entityType, change.LogID, "Success", null);
                            }
                            else
                            {
                                result.SkippedRecords++;

                                // Mark as skipped
                                await UpdateChangeStatusAsync(sourceConnectionString, entityType, change.LogID, "Skipped", "Delete operations are disabled");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorRecords++;
                    string errorMessage = $"Error processing {change.ChangeType} for {entityName} {change.EntityID}: {ex.Message}";
                    result.ErrorMessages.Add(errorMessage);

                    // Mark as failed with the error message
                    await UpdateChangeStatusAsync(sourceConnectionString, entityType, change.LogID, "Failed", errorMessage);
                }
            }

            // Update progress one last time
            progressInfo.StatusMessage = "Auto sync completed.";
            progress?.Report(progressInfo);

            return result;
        }

        public async Task<SyncResult> RetryFailedChangesAsync(
            string sourceConnectionString,
            string targetConnectionString,
            List<EntityChange> failedChanges,
            bool syncDeletes,
            IProgress<SyncProgress> progress,
            CancellationToken cancellationToken = default)
        {
            // Filter only the selected changes
            var selectedChanges = failedChanges.FindAll(c => c.IsSelected);

            if (selectedChanges.Count == 0)
            {
                return new SyncResult
                {
                    TotalRecords = 0,
                    ProcessedRecords = 0,
                    ErrorMessages = new List<string> { "No changes selected for retry." }
                };
            }

            // Use the standard synchronization method for the selected changes
            return await SynchronizeChangesAsync(
                sourceConnectionString,
                targetConnectionString,
                selectedChanges,
                syncDeletes,
                progress,
                cancellationToken);
        }

        public async Task UpdateChangeStatusAsync(
            string connectionString,
            EntityType entityType,
            int logId,
            string status,
            string errorMessage)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetChangeTrackingTableName(entityType);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Ensure we have status columns
                try
                {
                    string sql = $@"
                    UPDATE {tableName} 
                    SET Status = @Status, 
                        ProcessedTime = @ProcessedTime, 
                        ErrorMessage = @ErrorMessage
                    WHERE LogID = @LogID";

                    using (var command = provider.CreateCommand(sql, connection))
                    {
                        command.Parameters.Add(provider.CreateParameter("@Status", status));
                        command.Parameters.Add(provider.CreateParameter("@ProcessedTime", DateTime.Now));
                        command.Parameters.Add(provider.CreateParameter("@ErrorMessage", errorMessage ?? (object)DBNull.Value));
                        command.Parameters.Add(provider.CreateParameter("@LogID", logId));

                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch
                {
                    // Status columns might not exist yet - try to add them
                    string checkColumnSql = $@"
                    IF NOT EXISTS (SELECT * FROM sys.columns 
                                  WHERE object_id = OBJECT_ID('{tableName}') AND name = 'Status')
                    BEGIN
                        ALTER TABLE [dbo].[{tableName}] ADD
                            [Status] [varchar](20) NOT NULL DEFAULT 'Pending',
                            [ProcessedTime] [datetime] NULL,
                            [ErrorMessage] [nvarchar](max) NULL
                    END";

                    using (var checkCommand = provider.CreateCommand(checkColumnSql, connection))
                    {
                        await checkCommand.ExecuteNonQueryAsync();
                    }

                    // Now try the update again
                    string updateSql = $@"
                    UPDATE {tableName} 
                    SET Status = @Status, 
                        ProcessedTime = @ProcessedTime, 
                        ErrorMessage = @ErrorMessage
                    WHERE LogID = @LogID";

                    using (var updateCommand = provider.CreateCommand(updateSql, connection))
                    {
                        updateCommand.Parameters.Add(provider.CreateParameter("@Status", status));
                        updateCommand.Parameters.Add(provider.CreateParameter("@ProcessedTime", DateTime.Now));
                        updateCommand.Parameters.Add(provider.CreateParameter("@ErrorMessage", errorMessage ?? (object)DBNull.Value));
                        updateCommand.Parameters.Add(provider.CreateParameter("@LogID", logId));

                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task ClearProcessedChangesAsync(string connectionString, EntityType entityType, int daysToKeep = 7)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);
            string tableName = provider.GetChangeTrackingTableName(entityType);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Delete processed records older than the specified number of days
                string sql = $@"
                DELETE FROM {tableName} 
                WHERE Status IN ('Success', 'Skipped') 
                AND ChangeTime < @CutoffDate";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    command.Parameters.Add(provider.CreateParameter("@CutoffDate", DateTime.Now.AddDays(-daysToKeep)));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task SynchronizeInsertOrUpdate(string sourceConnectionString, string targetConnectionString, int entityId, EntityType entityType)
        {
            // Get the data from the source database based on entity type
            if (entityType == EntityType.Customer)
            {
                // Get the customer data from the source database
                Customer customer = await _customerDataService.GetCustomerByIdAsync(sourceConnectionString, entityId);

                if (customer == null)
                {
                    throw new Exception($"Customer {entityId} not found in the source database.");
                }

                // Check if the customer exists in the target database
                bool exists = await _customerDataService.CustomerExistsAsync(targetConnectionString, entityId);

                // Convert the customer object to a DataRow for the data service
                DataTable table = new DataTable();

                // Add all customer properties as columns
                foreach (var prop in typeof(Customer).GetProperties())
                {
                    Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    table.Columns.Add(prop.Name, propType);
                }

                // Create a new row and populate it
                DataRow row = table.NewRow();
                foreach (var prop in typeof(Customer).GetProperties())
                {
                    var value = prop.GetValue(customer);
                    row[prop.Name] = value ?? DBNull.Value;
                }

                table.Rows.Add(row);

                // Insert or update the customer in the target database
                if (exists)
                {
                    await _customerDataService.UpdateCustomerAsync(targetConnectionString, row);
                }
                else
                {
                    await _customerDataService.InsertCustomerAsync(targetConnectionString, row);
                }
            }
            else // User - only sync the specific columns
            {
                // Get the user data from the source database
                User user = await _userDataService.GetUserByIdAsync(sourceConnectionString, entityId);

                if (user == null)
                {
                    throw new Exception($"User {entityId} not found in the source database.");
                }

                // Check if the user exists in the target database
                bool exists = await _userDataService.UserExistsAsync(targetConnectionString, entityId);

                // Convert the user object to a DataRow for the data service
                DataTable table = new DataTable();

                // Add only the columns we want to sync
                foreach (var colName in _userColumnsToSync)
                {
                    var prop = typeof(User).GetProperty(colName);
                    if (prop != null)
                    {
                        Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        table.Columns.Add(prop.Name, propType);
                    }
                }

                // Create a new row and populate it with only the columns we want to sync
                DataRow row = table.NewRow();
                foreach (var colName in _userColumnsToSync)
                {
                    var prop = typeof(User).GetProperty(colName);
                    if (prop != null)
                    {
                        var value = prop.GetValue(user);
                        row[prop.Name] = value ?? DBNull.Value;
                    }
                }

                table.Rows.Add(row);

                // Insert or update the user in the target database
                if (exists)
                {
                    await _userDataService.UpdateUserAsync(targetConnectionString, row);
                }
                else
                {
                    await _userDataService.InsertUserAsync(targetConnectionString, row);
                }
            }
        }

        private async Task SynchronizeDelete(string targetConnectionString, int entityId, EntityType entityType)
        {
            if (entityType == EntityType.Customer)
            {
                // Check if the customer exists in the target database
                bool exists = await _customerDataService.CustomerExistsAsync(targetConnectionString, entityId);

                if (exists)
                {
                    // Delete the customer from the target database
                    await _customerDataService.DeleteCustomerAsync(targetConnectionString, entityId);
                }
            }
            else // User
            {
                // Check if the user exists in the target database
                bool exists = await _userDataService.UserExistsAsync(targetConnectionString, entityId);

                if (exists)
                {
                    // Delete the user from the target database
                    await _userDataService.DeleteUserAsync(targetConnectionString, entityId);
                }
            }
        }
    }

}











