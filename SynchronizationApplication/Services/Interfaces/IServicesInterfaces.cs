using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SynchronizationApplication.Models;

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
        Task<bool> TestConnectionAsync(DatabaseSettings settings);
        string BuildConnectionString(DatabaseSettings settings);
    }

    // Interface for customer data access
    public interface ICustomerDataService
    {
        Task<int> GetTotalCustomerCountAsync(string connectionString);
        Task<DataTable> GetCustomerBatchAsync(string connectionString, int offset, int batchSize);
        Task<bool> CustomerExistsAsync(string connectionString, int custId);
        Task InsertCustomerAsync(string connectionString, DataRow customerData);
        Task UpdateCustomerAsync(string connectionString, DataRow customerData);
        Task TruncateCustomersTableAsync(string connectionString);

        // New methods for table checking/creation
        Task<bool> TableExistsAsync(string connectionString);
        Task CreateCustomersTableAsync(string connectionString);

        // Methods for failed records management
        Task SaveFailedRecordsAsync(string filePath, IEnumerable<FailedRecord> failedRecords);
        Task<List<FailedRecord>> LoadFailedRecordsAsync(string filePath);
        Task<Customer> GetCustomerByIdAsync(string connectionString, int custId);
    }

    // Interface for synchronization operations
    public interface ISynchronizationService
    {
        Task<SyncResult> SynchronizeAsync(
            AppSettings settings,
            IProgress<SyncProgress> progress,
            CancellationToken cancellationToken = default);

        Task<(bool localSuccess, bool remoteSuccess)> TestConnectionsAsync(AppSettings settings);

        // Method to check and create table if needed
        Task EnsureTableExistsAsync(string connectionString);
    }

    // Interface for logging
    public interface ILogService
    {
        Task SaveLogAsync(string filePath, SyncResult result);
    }
}