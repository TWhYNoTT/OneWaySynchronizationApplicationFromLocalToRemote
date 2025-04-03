using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SynchronizationApplication.Models
{


    public static class SyncStateManager
    {
        // Get the path to the sync state file for the specified entity type
        public static string GetSyncStatePath(EntityType entityType)
        {
            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CustomerUserSync");

            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);

            string fileName = entityType == EntityType.Customer ? "customer_sync_state.json" : "user_sync_state.json";
            return Path.Combine(appDataDir, fileName);
        }

        // Save sync state for the specified entity type
        public static async Task SaveSyncStateAsync(SyncState state)
        {
            string filePath = GetSyncStatePath(state.EntityType);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(state, options);
            await File.WriteAllTextAsync(filePath, json);
        }

        // Load sync state for the specified entity type
        public static async Task<SyncState> LoadSyncStateAsync(EntityType entityType)
        {
            string filePath = GetSyncStatePath(entityType);

            if (!File.Exists(filePath))
                return new SyncState { EntityType = entityType }; // Return default state with correct entity type

            try
            {
                string json = await File.ReadAllTextAsync(filePath);
                var state = JsonSerializer.Deserialize<SyncState>(json);

                if (state != null)
                {
                    // Ensure the entity type is set correctly
                    state.EntityType = entityType;
                    return state;
                }

                return new SyncState { EntityType = entityType };
            }
            catch
            {
                // If there's an error reading the file, return default state
                return new SyncState { EntityType = entityType };
            }
        }

        // Reset sync state for the specified entity type
        public static async Task ResetSyncStateAsync(EntityType entityType)
        {
            await SaveSyncStateAsync(new SyncState { EntityType = entityType });
        }

        // Check if there's an incomplete sync for the specified entity type
        public static async Task<bool> HasIncompleteSyncAsync(EntityType entityType)
        {
            var state = await LoadSyncStateAsync(entityType);
            return !state.IsComplete && state.LastProcessedOffset > 0;
        }
    }




}