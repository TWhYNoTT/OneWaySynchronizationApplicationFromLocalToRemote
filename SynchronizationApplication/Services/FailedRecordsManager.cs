using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchronizationApplication.Models;

namespace SynchronizationApplication.Services
{
    public static class FailedRecordsManager
    {
        // Get the path to the failed records file for the specified entity type
        public static string GetFailedRecordsPath(EntityType entityType)
        {
            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CustomerUserSync");

            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);

            string fileName = entityType == EntityType.Customer ? "failed_customer_records.csv" : "failed_user_records.csv";
            return Path.Combine(appDataDir, fileName);
        }

        // Save only unresolved failed records
        public static async Task SaveFailedRecordsAsync(IEnumerable<FailedRecord> failedRecords)
        {
            if (failedRecords == null || !failedRecords.Any())
                return;

            // Group records by entity type
            var groupedRecords = failedRecords.GroupBy(r => r.EntityType);

            foreach (var group in groupedRecords)
            {
                var entityType = group.Key;
                var entityRecords = group.ToList();
                string filePath = GetFailedRecordsPath(entityType);

                // Only save unresolved records
                var unresolvedRecords = entityRecords.Where(r => !r.IsResolved).ToList();

                using (var writer = new StreamWriter(filePath))
                {
                    await writer.WriteLineAsync("EntityID,Error,Timestamp,EntityType");
                    foreach (var record in unresolvedRecords)
                    {
                        await writer.WriteLineAsync($"{record.EntityID},{record.ErrorMessage.Replace(",", ";")},{record.Timestamp:yyyy-MM-dd HH:mm:ss},{(int)record.EntityType}");
                    }
                }
            }
        }

        // Load failed records for the specified entity type
        public static async Task<List<FailedRecord>> LoadFailedRecordsAsync(EntityType entityType)
        {
            string filePath = GetFailedRecordsPath(entityType);
            var result = new List<FailedRecord>();

            if (!File.Exists(filePath))
                return result;

            using (var reader = new StreamReader(filePath))
            {
                // Skip header
                await reader.ReadLineAsync();

                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // Split by commas, but handle the case where error message might contain commas
                    int firstCommaIndex = line.IndexOf(',');
                    if (firstCommaIndex < 0) continue;

                    string entityIdStr = line.Substring(0, firstCommaIndex);

                    int lastCommaIndex = line.LastIndexOf(',');
                    if (lastCommaIndex <= firstCommaIndex) continue;

                    string timestampStr = line.Substring(lastCommaIndex + 1);
                    int entityTypeIndex = timestampStr.LastIndexOf(',');

                    string errorMsg;
                    EntityType recordEntityType;

                    if (entityTypeIndex > 0)
                    {
                        // New format with EntityType column
                        errorMsg = line.Substring(firstCommaIndex + 1, lastCommaIndex - firstCommaIndex - 1);
                        string entityTypeStr = timestampStr.Substring(entityTypeIndex + 1);
                        timestampStr = timestampStr.Substring(0, entityTypeIndex);
                        recordEntityType = int.TryParse(entityTypeStr, out int entityTypeValue) ?
                            (EntityType)entityTypeValue : entityType;
                    }
                    else
                    {
                        // Old format without EntityType column
                        errorMsg = line.Substring(firstCommaIndex + 1, lastCommaIndex - firstCommaIndex - 1);
                        recordEntityType = entityType;
                    }

                    if (int.TryParse(entityIdStr, out int entityId) &&
                        DateTime.TryParse(timestampStr, out var timestamp))
                    {
                        result.Add(new FailedRecord
                        {
                            EntityID = entityId,
                            ErrorMessage = errorMsg,
                            Timestamp = timestamp,
                            IsResolved = false,
                            EntityType = recordEntityType
                        });
                    }
                }
            }

            return result;
        }

        // Load all failed records
        public static async Task<List<FailedRecord>> LoadAllFailedRecordsAsync()
        {
            var customerRecords = await LoadFailedRecordsAsync(EntityType.Customer);
            var userRecords = await LoadFailedRecordsAsync(EntityType.User);

            // Combine both lists
            var allRecords = new List<FailedRecord>();
            allRecords.AddRange(customerRecords);
            allRecords.AddRange(userRecords);

            return allRecords;
        }

        // Check if there are any failed records for the specified entity type
        public static async Task<bool> HasFailedRecordsAsync(EntityType entityType)
        {
            var records = await LoadFailedRecordsAsync(entityType);
            return records.Count > 0;
        }

        // Check if there are any failed records of any type
        public static async Task<bool> HasAnyFailedRecordsAsync()
        {
            return await HasFailedRecordsAsync(EntityType.Customer) ||
                   await HasFailedRecordsAsync(EntityType.User);
        }
    }
}