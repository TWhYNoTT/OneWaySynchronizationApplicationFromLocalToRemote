using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services.Interfaces;
using SynchronizationApplication.Services.Providers;

namespace SynchronizationApplication.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly IDatabaseService _databaseService;
        private readonly List<string> _imageColumns = new List<string>
        {
            "Image"
        };

        // Define the specific columns to sync for Users table
        private readonly List<string> _columnsToSync = new List<string>
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

        public UserDataService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<int> GetTotalUserCountAsync(string connectionString)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = provider.CreateCommand("SELECT COUNT(*) FROM Users", connection))
                {
                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        public async Task<DataTable> GetUserBatchAsync(string connectionString, int offset, int batchSize)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Only select the columns we want to sync
                string columnsToSelect = string.Join(", ", _columnsToSync);
                string query = $@"
                    SELECT {columnsToSelect} FROM (
                        SELECT *, ROW_NUMBER() OVER (ORDER BY UserID) AS RowNum
                        FROM Users
                    ) AS Temp
                    WHERE RowNum > {offset} AND RowNum <= {offset + batchSize}";

                // If the provider is MySQL, adjust the query
                if (provider.ProviderType == DatabaseProviderType.MySQL)
                {
                    query = $@"
                        SELECT {columnsToSelect} FROM Users
                        ORDER BY UserID
                        LIMIT {offset}, {batchSize}";
                }

                using (var command = provider.CreateCommand(query, connection))
                using (var adapter = provider.CreateDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public async Task<bool> UserExistsAsync(string connectionString, int userId)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = provider.CreateCommand("SELECT COUNT(*) FROM Users WHERE UserID = @UserID", connection))
                {
                    command.Parameters.Add(provider.CreateParameter("@UserID", userId));
                    return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
                }
            }
        }

        public async Task InsertUserAsync(string connectionString, DataRow userData)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Start a transaction since we need multiple commands
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Enable identity insert
                        using (var identityCommand = provider.CreateCommand(
                            provider.GetEnableIdentityInsertSql("Users"), connection, transaction))
                        {
                            await identityCommand.ExecuteNonQueryAsync();
                        }

                        // Build column names and parameter placeholders - only for columns we want to sync
                        var columns = new List<string>();
                        var parameters = new List<string>();

                        foreach (DataColumn col in userData.Table.Columns)
                        {
                            // Skip the RowNum column and columns not in our sync list
                            if (col.ColumnName == "RowNum" || !_columnsToSync.Contains(col.ColumnName))
                                continue;

                            columns.Add($"{col.ColumnName}");
                            parameters.Add($"@{col.ColumnName}");
                        }

                        string sql = $"INSERT INTO Users ({string.Join(", ", columns)}) VALUES ({string.Join(", ", parameters)})";

                        using (var command = provider.CreateCommand(sql, connection, transaction))
                        {
                            foreach (DataColumn col in userData.Table.Columns)
                            {
                                // Skip the RowNum column and columns not in our sync list
                                if (col.ColumnName == "RowNum" || !_columnsToSync.Contains(col.ColumnName))
                                    continue;

                                // Handle image columns differently
                                if (_imageColumns.Contains(col.ColumnName))
                                {
                                    var param = provider.CreateParameter(
                                        $"@{col.ColumnName}",
                                        userData.IsNull(col) ? DBNull.Value : userData[col.ColumnName],
                                        provider.GetImageDbType());

                                    command.Parameters.Add(param);
                                }
                                else
                                {
                                    // For non-image columns
                                    command.Parameters.Add(provider.CreateParameter(
                                        $"@{col.ColumnName}",
                                        userData.IsNull(col) ? DBNull.Value : userData[col.ColumnName]));
                                }
                            }

                            await command.ExecuteNonQueryAsync();
                        }

                        // Disable identity insert after we're done
                        using (var identityCommand = provider.CreateCommand(
                            provider.GetDisableIdentityInsertSql("Users"), connection, transaction))
                        {
                            await identityCommand.ExecuteNonQueryAsync();
                        }

                        // Commit the transaction
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        // If anything goes wrong, roll back
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task UpdateUserAsync(string connectionString, DataRow userData)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Build SET clause for update - only for columns we want to sync
                var setClauses = new List<string>();

                foreach (DataColumn col in userData.Table.Columns)
                {
                    // Skip the RowNum column, the primary key, and columns not in our sync list
                    if (col.ColumnName == "RowNum" || col.ColumnName == "UserID" || !_columnsToSync.Contains(col.ColumnName))
                        continue;

                    setClauses.Add($"{col.ColumnName} = @{col.ColumnName}");
                }

                string sql = $"UPDATE Users SET {string.Join(", ", setClauses)} WHERE UserID = @UserID";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    foreach (DataColumn col in userData.Table.Columns)
                    {
                        // Skip the RowNum column and columns not in our sync list, except for UserID which we need for the WHERE clause
                        if (col.ColumnName == "RowNum" || (!_columnsToSync.Contains(col.ColumnName) && col.ColumnName != "UserID"))
                            continue;

                        // Handle image columns differently
                        if (_imageColumns.Contains(col.ColumnName))
                        {
                            var param = provider.CreateParameter(
                                $"@{col.ColumnName}",
                                userData.IsNull(col) ? DBNull.Value : userData[col.ColumnName],
                                provider.GetImageDbType());

                            command.Parameters.Add(param);
                        }
                        else
                        {
                            // For non-image columns
                            command.Parameters.Add(provider.CreateParameter(
                                $"@{col.ColumnName}",
                                userData.IsNull(col) ? DBNull.Value : userData[col.ColumnName]));
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteUserAsync(string connectionString, int userId)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Simple DELETE statement to remove the user
                string sql = "DELETE FROM Users WHERE UserID = @UserID";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    command.Parameters.Add(provider.CreateParameter("@UserID", userId));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task TruncateUsersTableAsync(string connectionString)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = provider.CreateCommand("DELETE FROM Users", connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> TableExistsAsync(string connectionString, string tableName)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);
            return await provider.TableExistsAsync(connectionString, tableName);
        }

        public async Task CreateUsersTableAsync(string connectionString)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Create table with the appropriate schema for the provider
                string createTableSql = provider.GetCreateUsersTableSql();

                using (var command = provider.CreateCommand(createTableSql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<User> GetUserByIdAsync(string connectionString, int userId)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Only select the columns we want to sync
                string columnsToSelect = string.Join(", ", _columnsToSync);
                string sql = $"SELECT {columnsToSelect} FROM Users WHERE UserID = @UserID";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    command.Parameters.Add(provider.CreateParameter("@UserID", userId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapReaderToUser(reader);
                        }
                    }
                }
            }

            return null;
        }

        private User MapReaderToUser(DbDataReader reader)
        {
            var user = new User();

            // Helper function to safely get value or default when column is null
            T GetValueOrDefault<T>(string columnName, T defaultValue = default)
            {
                int ordinal;
                try
                {
                    ordinal = reader.GetOrdinal(columnName);
                }
                catch (IndexOutOfRangeException)
                {
                    return defaultValue;
                }

                return reader.IsDBNull(ordinal) ? defaultValue : (T)reader.GetValue(ordinal);
            }

            // Map only the properties we're syncing
            user.UserID = GetValueOrDefault<int>("UserID");
            user.UserName = GetValueOrDefault<string>("UserName");
            user.UserPass = GetValueOrDefault<string>("UserPass");
            user.UserSysName = GetValueOrDefault<string>("UserSysName");
            user.UserLevel = GetValueOrDefault<string>("UserLevel");
            user.UserPhone = GetValueOrDefault<string>("UserPhone");
            user.UserEnabled = GetValueOrDefault<bool?>("UserEnabled");
            user.UserNote = GetValueOrDefault<string>("UserNote");
            user.UserAddedDate = GetValueOrDefault<DateTime?>("UserAddedDate");
            user.Email = GetValueOrDefault<string>("Email");
            user.Gender = GetValueOrDefault<string>("Gender");
            user.PermissionType = GetValueOrDefault<string>("PermissionType");

            // Handle image data specifically
            int imageOrdinal = -1;
            try { imageOrdinal = reader.GetOrdinal("Image"); } catch { }
            if (imageOrdinal >= 0 && !reader.IsDBNull(imageOrdinal))
            {
                user.Image = (byte[])reader.GetValue(imageOrdinal);
            }

            return user;
        }
    }
}