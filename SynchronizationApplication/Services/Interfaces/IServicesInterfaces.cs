using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services.Providers;

namespace SynchronizationApplication.Services.Interfaces
{
    // Interface for configuration management
    public interface IConfigurationService
    {
        Task<AppSettings> LoadSettingsAsync();
        Task SaveSettingsAsync(AppSettings settings);
    }

    // Interface for database connection
    public interface IDatabaseService
    {
        /// <summary>
        /// Tests a database connection
        /// </summary>
        Task<bool> TestConnectionAsync(DatabaseSettings settings);

        /// <summary>
        /// Builds a connection string from the database settings
        /// </summary>
        string BuildConnectionString(DatabaseSettings settings);

        /// <summary>
        /// Gets the database provider for the specified provider type
        /// </summary>
        IDatabaseProvider GetProviderForType(DatabaseProviderType providerType);

        /// <summary>
        /// Gets the database provider for the specified connection string
        /// </summary>
        IDatabaseProvider GetProviderForConnection(string connectionString);
    }


    // Interface for customer data access
    public interface ICustomerDataService
    {
        Task<int> GetTotalCustomerCountAsync(string connectionString);
        Task<DataTable> GetCustomerBatchAsync(string connectionString, int offset, int batchSize);
        Task<bool> CustomerExistsAsync(string connectionString, int custId);
        Task InsertCustomerAsync(string connectionString, DataRow customerData);
        Task UpdateCustomerAsync(string connectionString, DataRow customerData);
        Task DeleteCustomerAsync(string connectionString, int custId);
        Task TruncateCustomersTableAsync(string connectionString);
        Task<bool> TableExistsAsync(string connectionString, string tableName);
        Task CreateCustomersTableAsync(string connectionString);
        Task<Customer> GetCustomerByIdAsync(string connectionString, int custId);
    }

    // Interface for user data access
    public interface IUserDataService
    {
        Task<int> GetTotalUserCountAsync(string connectionString);
        Task<DataTable> GetUserBatchAsync(string connectionString, int offset, int batchSize);
        Task<bool> UserExistsAsync(string connectionString, int userId);
        Task InsertUserAsync(string connectionString, DataRow userData);
        Task UpdateUserAsync(string connectionString, DataRow userData);
        Task DeleteUserAsync(string connectionString, int userId);
        Task TruncateUsersTableAsync(string connectionString);
        Task<bool> TableExistsAsync(string connectionString, string tableName);
        Task CreateUsersTableAsync(string connectionString);
        Task<User> GetUserByIdAsync(string connectionString, int userId);
    }

    // Interface for synchronization operations
    public interface ISynchronizationService
    {
        Task<(bool localSuccess, bool remoteSuccess)> TestConnectionsAsync(AppSettings settings);
        Task EnsureTableExistsAsync(string connectionString, EntityType entityType);
        Task<SyncResult> SynchronizeAsync(
            AppSettings settings,
            EntityType entityType,
            IProgress<SyncProgress> progress,
            bool resumePreviousSync = false,
            CancellationToken cancellationToken = default);
        Task<bool> HasIncompleteSyncAsync(EntityType entityType);
        Task ResetSyncStateAsync(EntityType entityType);
    }

    // Interface for logging
    public interface ILogService
    {
        Task SaveLogAsync(string filePath, SyncResult result);
    }

    // Interface for system startup management
    public interface IStartupService
    {
        bool IsApplicationInStartup();
        void AddApplicationToStartup();
        void RemoveApplicationFromStartup();
    }

    // Interface for database trigger management
    public interface ITriggerService
    {
        /// <summary>
        /// Checks if the change tracking triggers exist in the database for the specified entity type
        /// </summary>
        Task<bool> DoTriggersExistAsync(string connectionString, EntityType entityType);

        /// <summary>
        /// Checks if the change tracking table exists in the database for the specified entity type
        /// </summary>
        Task<bool> DoesChangeTrackingTableExistAsync(string connectionString, EntityType entityType);

        /// <summary>
        /// Creates the change tracking table and triggers in the database for the specified entity type
        /// </summary>
        Task CreateTriggersAsync(string connectionString, EntityType entityType);

        /// <summary>
        /// Removes the change tracking table and triggers from the database for the specified entity type
        /// </summary>
        Task RemoveTriggersAsync(string connectionString, EntityType entityType);

        /// <summary>
        /// Gets details about the installed triggers for the specified entity type
        /// </summary>
        Task<List<string>> GetTriggerDetailsAsync(string connectionString, EntityType entityType);

        /// <summary>
        /// Ensures the change tracking table has the necessary columns for status tracking for the specified entity type
        /// </summary>
        Task EnsureStatusColumnsExistAsync(string connectionString, EntityType entityType);
    }

    // Interface for change tracking
    public interface IChangeTrackingService
    {
        /// <summary>
        /// Gets the list of changes since the specified date for the specified entity type
        /// </summary>
        Task<List<EntityChange>> GetChangesAsync(
            string connectionString,
            EntityType entityType,
            DateTime sinceDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the list of failed changes that can be retried for the specified entity type
        /// </summary>
        Task<List<EntityChange>> GetFailedChangesAsync(
            string connectionString,
            EntityType entityType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all changes with their status for display for the specified entity type
        /// </summary>
        Task<List<EntityChange>> GetAllChangesAsync(
            string connectionString,
            EntityType entityType,
            int maxRows = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Synchronizes the specified changes to the target database
        /// </summary>
        Task<SyncResult> SynchronizeChangesAsync(
            string sourceConnectionString,
            string targetConnectionString,
            List<EntityChange> changes,
            bool syncDeletes,
            IProgress<SyncProgress> progress,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the status of a change
        /// </summary>
        Task UpdateChangeStatusAsync(
            string connectionString,
            EntityType entityType,
            int logId,
            string status,
            string errorMessage);

        /// <summary>
        /// Retry failed changes
        /// </summary>
        Task<SyncResult> RetryFailedChangesAsync(
            string sourceConnectionString,
            string targetConnectionString,
            List<EntityChange> failedChanges,
            bool syncDeletes,
            IProgress<SyncProgress> progress,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Clear processed changes from the log (keeps only recent or unprocessed changes)
        /// </summary>
        Task ClearProcessedChangesAsync(
            string connectionString,
            EntityType entityType,
            int daysToKeep = 7);
    }
}