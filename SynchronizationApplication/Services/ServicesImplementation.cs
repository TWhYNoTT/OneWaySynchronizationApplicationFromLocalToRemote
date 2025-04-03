using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services.Interfaces;
using SynchronizationApplication.Services.Providers;

namespace SynchronizationApplication.Services
{
    // Implementation of configuration service
    public class ConfigurationService : IConfigurationService
    {
        private const string SettingsFileName = "appsettings.json";

        public async Task<AppSettings> LoadSettingsAsync()
        {
            if (!File.Exists(SettingsFileName))
            {
                return new AppSettings();
            }

            try
            {
                string json = await File.ReadAllTextAsync(SettingsFileName);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch (Exception)
            {
                // Return default settings if there's an error reading the file
                return new AppSettings();
            }
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(settings, options);
            await File.WriteAllTextAsync(SettingsFileName, json);
        }
    }




    

 
     
        // Implementation of database service
        public class DatabaseService : IDatabaseService
        {
            private readonly IDatabaseProvider _sqlServerProvider;
            private readonly IDatabaseProvider _mySqlProvider;

            // This dictionary will store the connection string to provider type mapping
            private readonly Dictionary<string, DatabaseProviderType> _connectionProviderMap = new Dictionary<string, DatabaseProviderType>();

            public DatabaseService()
            {
                _sqlServerProvider = new SqlServerProvider();
                _mySqlProvider = new MySqlProvider();
            }

            public string BuildConnectionString(DatabaseSettings settings)
            {
                var provider = GetProviderForType(settings.ProviderType);
                string connectionString = provider.BuildConnectionString(settings);

                // Store the mapping for future reference
                _connectionProviderMap[connectionString] = settings.ProviderType;

                return connectionString;
            }

            public async Task<bool> TestConnectionAsync(DatabaseSettings settings)
            {
                var provider = GetProviderForType(settings.ProviderType);
                string connectionString = provider.BuildConnectionString(settings);

                // Store the mapping for future reference
                _connectionProviderMap[connectionString] = settings.ProviderType;

                try
                {
                    return await provider.TestConnectionAsync(connectionString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection error: {ex.Message}");
                    return false;
                }
            }

            public IDatabaseProvider GetProviderForType(DatabaseProviderType providerType)
            {
                return providerType switch
                {
                    DatabaseProviderType.SqlServer => _sqlServerProvider,
                    DatabaseProviderType.MySQL => _mySqlProvider,
                    _ => throw new ArgumentException($"Unsupported provider type: {providerType}")
                };
            }

            public IDatabaseProvider GetProviderForConnection(string connectionString)
            {
                // Check if we have a stored mapping for this connection string
                if (_connectionProviderMap.TryGetValue(connectionString, out var providerType))
                {
                    return GetProviderForType(providerType);
                }

                // If no mapping is found, default to SQL Server as a safe fallback
                return _sqlServerProvider;
            }
        }
















    // Implementation of synchronization service
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ICustomerDataService _customerDataService;
        private readonly IUserDataService _userDataService;
        private const int BatchSize = 50;

        public SynchronizationService(
            IDatabaseService databaseService,
            ICustomerDataService customerDataService,
            IUserDataService userDataService)
        {
            _databaseService = databaseService;
            _customerDataService = customerDataService;
            _userDataService = userDataService;
        }

        public async Task<(bool localSuccess, bool remoteSuccess)> TestConnectionsAsync(AppSettings settings)
        {
            bool localSuccess = await _databaseService.TestConnectionAsync(settings.LocalDatabase);
            bool remoteSuccess = await _databaseService.TestConnectionAsync(settings.RemoteDatabase);

            return (localSuccess, remoteSuccess);
        }

        public async Task EnsureTableExistsAsync(string connectionString, EntityType entityType)
        {
            if (entityType == EntityType.Customer)
            {
                bool tableExists = await _customerDataService.TableExistsAsync(connectionString, "Customers");

                if (!tableExists)
                {
                    await _customerDataService.CreateCustomersTableAsync(connectionString);
                }
            }
            else // User
            {
                bool tableExists = await _userDataService.TableExistsAsync(connectionString, "Users");

                if (!tableExists)
                {
                    await _userDataService.CreateUsersTableAsync(connectionString);
                }
            }
        }

        public async Task<SyncResult> SynchronizeAsync(
            AppSettings settings,
            EntityType entityType,
            IProgress<SyncProgress> progress,
            bool resumePreviousSync = false,
            CancellationToken cancellationToken = default)
        {
            var result = new SyncResult { EntityType = entityType };
            var progressInfo = new SyncProgress { EntityType = entityType };
            var failedRecords = new List<FailedRecord>();
            var syncState = new SyncState { EntityType = entityType };

            try
            {
                // Build connection strings
                string localConnectionString = _databaseService.BuildConnectionString(settings.LocalDatabase);
                string remoteConnectionString = _databaseService.BuildConnectionString(settings.RemoteDatabase);

                // Load previous sync state if resuming
                int startOffset = 0;
                if (resumePreviousSync)
                {
                    syncState = await SyncStateManager.LoadSyncStateAsync(entityType);
                    if (!syncState.IsComplete)
                    {
                        startOffset = syncState.LastProcessedOffset;
                    }
                }
                else
                {
                    // Create a new sync state
                    syncState = new SyncState
                    {
                        LastProcessedOffset = 0,
                        LastSyncDate = DateTime.Now,
                        IsComplete = false,
                        EntityType = entityType
                    };

                    // Save initial state
                    await SyncStateManager.SaveSyncStateAsync(syncState);
                }

                // Step 0: Test connections
                progressInfo.StatusMessage = "Testing database connections...";
                progress?.Report(progressInfo);

                var (localSuccess, remoteSuccess) = await TestConnectionsAsync(settings);

                if (!localSuccess)
                {
                    result.ErrorMessages.Add("Cannot connect to local database. Please check your settings.");
                    progressInfo.StatusMessage = "Connection to local database failed!";
                    progress?.Report(progressInfo);
                    return result;
                }

                if (!remoteSuccess)
                {
                    result.ErrorMessages.Add("Cannot connect to remote database. Please check your settings.");
                    progressInfo.StatusMessage = "Connection to remote database failed!";
                    progress?.Report(progressInfo);
                    return result;
                }

                // Step 0.5: Ensure table exists in remote database
                string entityName = entityType == EntityType.Customer ? "Customers" : "Users";
                progressInfo.StatusMessage = $"Checking if {entityName} table exists in remote database...";
                progress?.Report(progressInfo);

                await EnsureTableExistsAsync(remoteConnectionString, entityType);

                // Step 1: Count total records for progress tracking
                progressInfo.StatusMessage = "Counting records to sync...";
                progress?.Report(progressInfo);

                if (entityType == EntityType.Customer)
                {
                    result.TotalRecords = await _customerDataService.GetTotalCustomerCountAsync(localConnectionString);
                }
                else // User
                {
                    result.TotalRecords = await _userDataService.GetTotalUserCountAsync(localConnectionString);
                }

                progressInfo.TotalRecords = result.TotalRecords;

                // If resuming, update processed count
                if (resumePreviousSync && startOffset > 0)
                {
                    result.ProcessedRecords = startOffset;
                    progressInfo.CurrentRecord = startOffset;
                    progressInfo.StatusMessage = $"Resuming from record {startOffset} of {result.TotalRecords}...";
                }

                progress?.Report(progressInfo);

                // Step 2: If configured, truncate the remote table first (only if not resuming)
                if (settings.TruncateBeforeSync && !resumePreviousSync)
                {
                    progressInfo.StatusMessage = $"Truncating remote {entityName} table...";
                    progress?.Report(progressInfo);

                    if (entityType == EntityType.Customer)
                    {
                        await _customerDataService.TruncateCustomersTableAsync(remoteConnectionString);
                    }
                    else // User
                    {
                        await _userDataService.TruncateUsersTableAsync(remoteConnectionString);
                    }
                }

                // Step 3: Synchronize in batches
                progressInfo.StatusMessage = "Starting data synchronization...";
                progress?.Report(progressInfo);

                int offset = startOffset;
                while (offset < result.TotalRecords && !cancellationToken.IsCancellationRequested)
                {
                    // Get a batch of records
                    DataTable batch;
                    if (entityType == EntityType.Customer)
                    {
                        batch = await _customerDataService.GetCustomerBatchAsync(localConnectionString, offset, BatchSize);
                    }
                    else // User
                    {
                        batch = await _userDataService.GetUserBatchAsync(localConnectionString, offset, BatchSize);
                    }

                    // Update sync state with current offset
                    syncState.LastProcessedOffset = offset;
                    await SyncStateManager.SaveSyncStateAsync(syncState);

                    // Process each record in the batch
                    foreach (DataRow row in batch.Rows)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        try
                        {
                            int id;
                            if (entityType == EntityType.Customer)
                            {
                                id = Convert.ToInt32(row["CustID"]);

                                if (settings.TruncateBeforeSync && !resumePreviousSync)
                                {
                                    // If we're truncating, we only need to insert
                                    await _customerDataService.InsertCustomerAsync(remoteConnectionString, row);
                                    result.AddedRecords++;
                                }
                                else
                                {
                                    // Otherwise, check if record exists and update or insert
                                    bool exists = await _customerDataService.CustomerExistsAsync(remoteConnectionString, id);

                                    if (exists)
                                    {
                                        await _customerDataService.UpdateCustomerAsync(remoteConnectionString, row);
                                        result.UpdatedRecords++;
                                    }
                                    else
                                    {
                                        await _customerDataService.InsertCustomerAsync(remoteConnectionString, row);
                                        result.AddedRecords++;
                                    }
                                }
                            }
                            else // User
                            {
                                id = Convert.ToInt32(row["UserID"]);

                                if (settings.TruncateBeforeSync && !resumePreviousSync)
                                {
                                    // If we're truncating, we only need to insert
                                    await _userDataService.InsertUserAsync(remoteConnectionString, row);
                                    result.AddedRecords++;
                                }
                                else
                                {
                                    // Otherwise, check if record exists and update or insert
                                    bool exists = await _userDataService.UserExistsAsync(remoteConnectionString, id);

                                    if (exists)
                                    {
                                        await _userDataService.UpdateUserAsync(remoteConnectionString, row);
                                        result.UpdatedRecords++;
                                    }
                                    else
                                    {
                                        await _userDataService.InsertUserAsync(remoteConnectionString, row);
                                        result.AddedRecords++;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result.ErrorRecords++;
                            string idColumnName = entityType == EntityType.Customer ? "CustID" : "UserID";
                            string errorMsg = $"Error processing {entityName.TrimEnd('s')} {row[idColumnName]}: {ex.Message}";
                            result.ErrorMessages.Add(errorMsg);

                            // Add to failed records for later retry
                            failedRecords.Add(new FailedRecord
                            {
                                EntityID = Convert.ToInt32(row[idColumnName]),
                                ErrorMessage = ex.Message,
                                Timestamp = DateTime.Now,
                                IsResolved = false,
                                EntityType = entityType
                            });
                        }

                        // Update progress
                        result.ProcessedRecords++;
                        progressInfo.CurrentRecord = result.ProcessedRecords;
                        progressInfo.StatusMessage = $"Processed {result.ProcessedRecords} of {result.TotalRecords} records...";
                        progress?.Report(progressInfo);
                    }

                    offset += BatchSize;
                }

                // Save failed records for later retry
                if (failedRecords.Count > 0)
                {
                    // Load existing failed records (if any)
                    var existingFailedRecords = await FailedRecordsManager.LoadFailedRecordsAsync(entityType);

                    // Combine with new failed records
                    existingFailedRecords.AddRange(failedRecords);

                    // Save all failed records
                    await FailedRecordsManager.SaveFailedRecordsAsync(existingFailedRecords);
                }

                // Update sync state
                if (cancellationToken.IsCancellationRequested)
                {
                    progressInfo.StatusMessage = "Synchronization was cancelled.";
                    result.ErrorMessages.Add("Synchronization was cancelled by user.");

                    // Keep the incomplete status in the sync state
                    syncState.IsComplete = false;
                    await SyncStateManager.SaveSyncStateAsync(syncState);
                }
                else
                {
                    progressInfo.StatusMessage = "Synchronization completed successfully.";

                    // Mark as complete
                    syncState.IsComplete = true;
                    syncState.LastSyncDate = DateTime.Now;
                    await SyncStateManager.SaveSyncStateAsync(syncState);
                }

                progress?.Report(progressInfo);
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Critical error during synchronization: {ex.Message}");
                progressInfo.StatusMessage = "Synchronization failed!";
                progress?.Report(progressInfo);

                // Mark the sync state as incomplete
                syncState.IsComplete = false;
                await SyncStateManager.SaveSyncStateAsync(syncState);

                return result;
            }
        }

        public async Task<bool> HasIncompleteSyncAsync(EntityType entityType)
        {
            return await SyncStateManager.HasIncompleteSyncAsync(entityType);
        }

        public async Task ResetSyncStateAsync(EntityType entityType)
        {
            await SyncStateManager.ResetSyncStateAsync(entityType);
        }
    }




    // Implementation of log service
    public class LogService : ILogService
    {
        public async Task SaveLogAsync(string filePath, SyncResult result)
        {
            var log = new StringBuilder();
            string entityName = result.EntityType == EntityType.Customer ? "Customer" : "User";

            log.AppendLine($"{entityName} Synchronization Log - {DateTime.Now}");
            log.AppendLine("=======================================");
            log.AppendLine();
            log.AppendLine($"Entity Type: {entityName}");
            log.AppendLine($"Total Records: {result.TotalRecords}");
            log.AppendLine($"Processed Records: {result.ProcessedRecords}");
            log.AppendLine($"Added Records: {result.AddedRecords}");
            log.AppendLine($"Updated Records: {result.UpdatedRecords}");
            log.AppendLine($"Skipped Records: {result.SkippedRecords}");
            log.AppendLine($"Records with Errors: {result.ErrorRecords}");
            log.AppendLine();

            if (result.ErrorMessages.Count > 0)
            {
                log.AppendLine("ERROR DETAILS:");
                log.AppendLine("=======================================");
                foreach (var error in result.ErrorMessages)
                {
                    log.AppendLine(error);
                }
            }
            else
            {
                log.AppendLine("No errors occurred during synchronization.");
            }

            await File.WriteAllTextAsync(filePath, log.ToString());
        }
    }
}















