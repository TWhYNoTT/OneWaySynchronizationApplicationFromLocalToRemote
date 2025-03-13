using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services.Interfaces;

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
        public string BuildConnectionString(DatabaseSettings settings)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = settings.Server,
                InitialCatalog = settings.Database,
                TrustServerCertificate = true,
                Encrypt = true
            };

            // Check if we should use Windows Authentication
            if (string.IsNullOrWhiteSpace(settings.Username) && string.IsNullOrWhiteSpace(settings.Password))
            {
                // Use Windows Authentication
                builder.IntegratedSecurity = true;
            }
            else
            {
                // Use SQL Server Authentication
                builder.UserID = settings.Username;
                builder.Password = settings.Password;
            }

            return builder.ConnectionString;
        }

        public async Task<bool> TestConnectionAsync(DatabaseSettings settings)
        {
            string connectionString = BuildConnectionString(settings);

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Connection error: {ex.Message}");
                return false;
            }
        }
    }



        public class CustomerDataService : Interfaces.ICustomerDataService
        {
            // List of image column names in the Customers table
            private readonly List<string> _imageColumns = new List<string>
        {
            "PersonalImage",
            "IdentityImage",
            "Insurance",
            "Medicalhistorycard"
        };

            public async Task<int> GetTotalCustomerCountAsync(string connectionString)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT COUNT(*) FROM Customers", connection))
                    {
                        return (int)await command.ExecuteScalarAsync();
                    }
                }
            }

            public async Task<DataTable> GetCustomerBatchAsync(string connectionString, int offset, int batchSize)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = $@"
                    SELECT * FROM (
                        SELECT *, ROW_NUMBER() OVER (ORDER BY CustID) AS RowNum
                        FROM Customers
                    ) AS Temp
                    WHERE RowNum > {offset} AND RowNum <= {offset + batchSize}";

                    using (var command = new SqlCommand(query, connection))
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }

            public async Task<bool> CustomerExistsAsync(string connectionString, int custId)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE CustID = @CustID", connection))
                    {
                        command.Parameters.AddWithValue("@CustID", custId);
                        return (int)await command.ExecuteScalarAsync() > 0;
                    }
                }
            }

        public async Task InsertCustomerAsync(string connectionString, DataRow customerData)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Start a transaction since we need multiple commands
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Enable identity insert
                        using (var identityCommand = new SqlCommand("SET IDENTITY_INSERT Customers ON", connection, transaction))
                        {
                            await identityCommand.ExecuteNonQueryAsync();
                        }

                        // Build column names and parameter placeholders
                        var columns = new List<string>();
                        var parameters = new List<string>();

                        foreach (DataColumn col in customerData.Table.Columns)
                        {
                            // Skip the RowNum column we added for batching
                            if (col.ColumnName == "RowNum")
                                continue;

                            columns.Add($"[{col.ColumnName}]");
                            parameters.Add($"@{col.ColumnName}");
                        }

                        string sql = $"INSERT INTO Customers ({string.Join(", ", columns)}) VALUES ({string.Join(", ", parameters)})";

                        using (var command = new SqlCommand(sql, connection, transaction))
                        {
                            foreach (DataColumn col in customerData.Table.Columns)
                            {
                                // Skip the RowNum column we added for batching
                                if (col.ColumnName == "RowNum")
                                    continue;

                                // Handle image columns differently
                                if (_imageColumns.Contains(col.ColumnName))
                                {
                                    // For image columns, use SqlDbType.Image
                                    var param = new SqlParameter($"@{col.ColumnName}", SqlDbType.Image);

                                    if (customerData.IsNull(col))
                                    {
                                        param.Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        param.Value = customerData[col.ColumnName];
                                    }

                                    command.Parameters.Add(param);
                                }
                                else
                                {
                                    // For non-image columns, use AddWithValue
                                    command.Parameters.AddWithValue($"@{col.ColumnName}", customerData[col.ColumnName] ?? DBNull.Value);
                                }
                            }

                            await command.ExecuteNonQueryAsync();
                        }

                        // Disable identity insert after we're done
                        using (var identityCommand = new SqlCommand("SET IDENTITY_INSERT Customers OFF", connection, transaction))
                        {
                            await identityCommand.ExecuteNonQueryAsync();
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch
                    {
                        // If anything goes wrong, roll back
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task UpdateCustomerAsync(string connectionString, DataRow customerData)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Build SET clause for update
                    var setClauses = new List<string>();

                    foreach (DataColumn col in customerData.Table.Columns)
                    {
                        // Skip the RowNum column we added for batching and the primary key
                        if (col.ColumnName == "RowNum" || col.ColumnName == "CustID")
                            continue;

                        setClauses.Add($"[{col.ColumnName}] = @{col.ColumnName}");
                    }

                    string sql = $"UPDATE Customers SET {string.Join(", ", setClauses)} WHERE CustID = @CustID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        foreach (DataColumn col in customerData.Table.Columns)
                        {
                            // Skip the RowNum column we added for batching
                            if (col.ColumnName == "RowNum")
                                continue;

                            // Handle image columns differently
                            if (_imageColumns.Contains(col.ColumnName))
                            {
                                // For image columns, use SqlDbType.Image
                                var param = new SqlParameter($"@{col.ColumnName}", SqlDbType.Image);

                                if (customerData.IsNull(col))
                                {
                                    param.Value = DBNull.Value;
                                }
                                else
                                {
                                    param.Value = customerData[col.ColumnName];
                                }

                                command.Parameters.Add(param);
                            }
                            else
                            {
                                // For non-image columns, use AddWithValue
                                command.Parameters.AddWithValue($"@{col.ColumnName}", customerData[col.ColumnName] ?? DBNull.Value);
                            }
                        }

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }

            public async Task TruncateCustomersTableAsync(string connectionString)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("DELETE FROM Customers", connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }

            // Table existence check and creation
            public async Task<bool> TableExistsAsync(string connectionString)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = 'dbo' 
                    AND TABLE_NAME = 'Customers'";

                    using (var command = new SqlCommand(query, connection))
                    {
                        int count = (int)await command.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }

            public async Task CreateCustomersTableAsync(string connectionString)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Create table with the same schema
                    string createTableSql = @"
CREATE TABLE [dbo].[Customers](
	[CustName] [nvarchar](max) NULL,
	[CustDadName] [nvarchar](255) NULL,
	[CustMamName] [nvarchar](255) NULL,
	[CustJob] [nvarchar](255) NULL,
	[CustGender] [nvarchar](255) NULL,
	[CustNat] [nvarchar](255) NULL,
	[CustAddress] [nvarchar](max) NULL,
	[CustMobile1] [nvarchar](255) NULL,
	[CustMobile2] [nvarchar](255) NULL,
	[CustPhone1] [nvarchar](255) NULL,
	[CustPhone2] [nvarchar](255) NULL,
	[CustMail1] [nvarchar](255) NULL,
	[CustMail2] [nvarchar](255) NULL,
	[CustWattsUp] [nvarchar](255) NULL,
	[CustFacbook] [nvarchar](255) NULL,
	[CustBirthday] [date] NULL,
	[CutIDentity] [nvarchar](255) NULL,
	[CustMarital] [nvarchar](255) NULL,
	[CustFileDate] [date] NULL,
	[CustDr] [int] NULL,
	[CustSmsEnabled] [bit] NULL,
	[CustBlockedNote] [nvarchar](max) NULL,
	[CustResource] [nvarchar](max) NULL,
	[CustLevelStatue] [nvarchar](max) NULL,
	[CustWebSite] [nvarchar](max) NULL,
	[CustDrName] [nvarchar](255) NULL,
	[CustAge] [nvarchar](255) NULL,
	[CustResourceDetals] [nvarchar](max) NULL,
	[CustComment] [nvarchar](max) NULL,
	[CustCity] [nvarchar](255) NULL,
	[CustID] [int] IDENTITY(1,1) NOT NULL,
	[CustDrsName] [nvarchar](max) NULL,
	[CustUserName] [nvarchar](255) NULL,
	[CustOldID] [nvarchar](max) NULL,
	[CustAppBlocked] [bit] NULL,
	[CustAppBlockedNote] [nvarchar](max) NULL,
	[PersonalImage] [image] NULL,
	[IdentityImage] [image] NULL,
	[Insurance] [image] NULL,
	[Medicalhistorycard] [image] NULL,
	[HealthStatuDetails] [nvarchar](max) NULL,
	[HealthStatuNote] [nvarchar](max) NULL,
	[Somker] [bit] NULL,
	[BloodG] [nvarchar](255) NULL,
	[ChildNo] [int] NULL,
	[InsuranceCoName] [nvarchar](100) NULL,
	[InsuranceAproNo] [nvarchar](55) NULL,
	[InsurancePlic] [nvarchar](100) NULL,
	[InsuranceClass] [nvarchar](55) NULL,
	[PrevSickPseronal] [nvarchar](max) NULL,
	[PrevSickFamily] [nvarchar](max) NULL,
	[PrevSickConsoltion] [nvarchar](max) NULL,
	[PrevSickMedicneThss] [nvarchar](max) NULL,
	[PrevSickSpicalDo] [nvarchar](max) NULL,
	[LastMofiyDate] [datetime] NULL,
	[LastModifyUser] [nvarchar](255) NULL,
	[CustFileTime] [time](0) NULL,
	[IsESignature] [bit] NULL,
	[DadID] [nvarchar](max) NULL,
	[MamID] [nvarchar](max) NULL,
	[JobSource] [nvarchar](255) NULL,
	[Qualification] [nvarchar](255) NULL,
	[MotherIdentity] [nvarchar](255) NULL,
	[FatherIdentity] [nvarchar](255) NULL,
	[OtherIdentity] [nvarchar](255) NULL,
	[LastEditUser] [nvarchar](255) NULL,
	[DefaultDiscount] [decimal](18, 2) NULL,
	[SuretyName] [nvarchar](255) NULL,
	[SuretyID] [nvarchar](255) NULL,
	[SuretyAddress] [nvarchar](max) NULL,
	[PassportNo] [nvarchar](255) NULL,
	[CustInvoiceBlockedEnb] [bit] NULL,
	[CustInvoiceBlockedTxt] [nvarchar](max) NULL,
	[CustAllBlockedEnb] [bit] NULL,
	[CustAllBlockedTxt] [nvarchar](max) NULL,
	[CustBlocked] [bit] NULL,
	[RelativeName] [nvarchar](max) NULL,
	[RelativePhone] [nvarchar](max) NULL,
	[FileTypeID] [int] NULL,
	[FileTypeNo] [nvarchar](10) NULL,
	[FileTypeName] [nvarchar](255) NULL,
	[IdentityType] [nvarchar](10) NULL,
	[DefaultDiscountExpiredDate] [date] NULL,
	[Weight] [decimal](5, 2) NULL,
	[InsuranceExpiredDate] [date] NULL,
	[VATNumber] [nvarchar](50) NULL,
	[BaldiNo] [nvarchar](50) NULL,
	[InsuranceRelation] [nvarchar](50) NULL,
	[GLN] [nvarchar](255) NULL,
	[AddressDistrict] [nvarchar](127) NULL,
	[AddressStreet] [nvarchar](1000) NULL,
	[AddressBuildingNumber] [nvarchar](50) NULL,
	[AddressPostalCode] [nvarchar](200) NULL,
	[IsTaxRegistered] [bit] NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustID] ASC
))";

                    using (var command = new SqlCommand(createTableSql, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }

            // Methods for failed records management
            public async Task SaveFailedRecordsAsync(string filePath, IEnumerable<FailedRecord> failedRecords)
            {
                using (var writer = new StreamWriter(filePath))
                {
                    await writer.WriteLineAsync("CustID,Error,Timestamp");
                    foreach (var record in failedRecords)
                    {
                        await writer.WriteLineAsync($"{record.CustID},{record.ErrorMessage},{record.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    }
                }
            }

            public async Task<List<FailedRecord>> LoadFailedRecordsAsync(string filePath)
            {
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
                        string[] parts = line.Split(',', 3);
                        if (parts.Length >= 3 && int.TryParse(parts[0], out int custId))
                        {
                            result.Add(new FailedRecord
                            {
                                CustID = custId,
                                ErrorMessage = parts[1],
                                Timestamp = DateTime.TryParse(parts[2], out var timestamp) ? timestamp : DateTime.Now
                            });
                        }
                    }
                }

                return result;
            }

            public async Task<Customer> GetCustomerByIdAsync(string connectionString, int custId)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT * FROM Customers WHERE CustID = @CustID", connection))
                    {
                        command.Parameters.AddWithValue("@CustID", custId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapReaderToCustomer(reader);
                            }
                        }
                    }
                }

                return null;
            }

            private Customer MapReaderToCustomer(SqlDataReader reader)
            {
                var customer = new Customer();

                // Helper function to safely get value or default when column is null
                T GetValueOrDefault<T>(string columnName, T defaultValue = default)
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    return reader.IsDBNull(ordinal) ? defaultValue : (T)reader.GetValue(ordinal);
                }

                // Map all properties from the reader to the customer object
                customer.CustID = GetValueOrDefault<int>("CustID");
                customer.CustName = GetValueOrDefault<string>("CustName");
                customer.CustDadName = GetValueOrDefault<string>("CustDadName");
                customer.CustMamName = GetValueOrDefault<string>("CustMamName");
                customer.CustJob = GetValueOrDefault<string>("CustJob");
                customer.CustGender = GetValueOrDefault<string>("CustGender");
                customer.CustNat = GetValueOrDefault<string>("CustNat");
                customer.CustAddress = GetValueOrDefault<string>("CustAddress");
                customer.CustMobile1 = GetValueOrDefault<string>("CustMobile1");
                customer.CustMobile2 = GetValueOrDefault<string>("CustMobile2");
                customer.CustPhone1 = GetValueOrDefault<string>("CustPhone1");
                customer.CustPhone2 = GetValueOrDefault<string>("CustPhone2");
                customer.CustMail1 = GetValueOrDefault<string>("CustMail1");
                customer.CustMail2 = GetValueOrDefault<string>("CustMail2");
                customer.CustWattsUp = GetValueOrDefault<string>("CustWattsUp");
                customer.CustFacbook = GetValueOrDefault<string>("CustFacbook");
                customer.CustBirthday = GetValueOrDefault<DateTime?>("CustBirthday");
                customer.CutIDentity = GetValueOrDefault<string>("CutIDentity");
                customer.CustMarital = GetValueOrDefault<string>("CustMarital");
                customer.CustFileDate = GetValueOrDefault<DateTime?>("CustFileDate");
                customer.CustDr = GetValueOrDefault<int?>("CustDr");
                customer.CustSmsEnabled = GetValueOrDefault<bool?>("CustSmsEnabled");
                customer.CustBlockedNote = GetValueOrDefault<string>("CustBlockedNote");
                customer.CustResource = GetValueOrDefault<string>("CustResource");
                customer.CustLevelStatue = GetValueOrDefault<string>("CustLevelStatue");
                customer.CustWebSite = GetValueOrDefault<string>("CustWebSite");
                customer.CustDrName = GetValueOrDefault<string>("CustDrName");
                customer.CustAge = GetValueOrDefault<string>("CustAge");
                customer.CustResourceDetals = GetValueOrDefault<string>("CustResourceDetals");
                customer.CustComment = GetValueOrDefault<string>("CustComment");
                customer.CustCity = GetValueOrDefault<string>("CustCity");
                customer.CustDrsName = GetValueOrDefault<string>("CustDrsName");
                customer.CustUserName = GetValueOrDefault<string>("CustUserName");
                customer.CustOldID = GetValueOrDefault<string>("CustOldID");
                customer.CustAppBlocked = GetValueOrDefault<bool?>("CustAppBlocked");
                customer.CustAppBlockedNote = GetValueOrDefault<string>("CustAppBlockedNote");

                // Handle image data specifically
                int personalImageOrdinal = reader.GetOrdinal("PersonalImage");
                if (!reader.IsDBNull(personalImageOrdinal))
                {
                    customer.PersonalImage = (byte[])reader.GetValue(personalImageOrdinal);
                }

                int identityImageOrdinal = reader.GetOrdinal("IdentityImage");
                if (!reader.IsDBNull(identityImageOrdinal))
                {
                    customer.IdentityImage = (byte[])reader.GetValue(identityImageOrdinal);
                }

                int insuranceOrdinal = reader.GetOrdinal("Insurance");
                if (!reader.IsDBNull(insuranceOrdinal))
                {
                    customer.Insurance = (byte[])reader.GetValue(insuranceOrdinal);
                }

                int medicalCardOrdinal = reader.GetOrdinal("Medicalhistorycard");
                if (!reader.IsDBNull(medicalCardOrdinal))
                {
                    customer.Medicalhistorycard = (byte[])reader.GetValue(medicalCardOrdinal);
                }

                // Continue with other properties
                customer.HealthStatuDetails = GetValueOrDefault<string>("HealthStatuDetails");
                customer.HealthStatuNote = GetValueOrDefault<string>("HealthStatuNote");
                customer.Somker = GetValueOrDefault<bool?>("Somker");
                customer.BloodG = GetValueOrDefault<string>("BloodG");
                customer.ChildNo = GetValueOrDefault<int?>("ChildNo");
                customer.InsuranceCoName = GetValueOrDefault<string>("InsuranceCoName");
                customer.InsuranceAproNo = GetValueOrDefault<string>("InsuranceAproNo");
                customer.InsurancePlic = GetValueOrDefault<string>("InsurancePlic");
                customer.InsuranceClass = GetValueOrDefault<string>("InsuranceClass");
                customer.PrevSickPseronal = GetValueOrDefault<string>("PrevSickPseronal");
                customer.PrevSickFamily = GetValueOrDefault<string>("PrevSickFamily");
                customer.PrevSickConsoltion = GetValueOrDefault<string>("PrevSickConsoltion");
                customer.PrevSickMedicneThss = GetValueOrDefault<string>("PrevSickMedicneThss");
                customer.PrevSickSpicalDo = GetValueOrDefault<string>("PrevSickSpicalDo");
                customer.LastMofiyDate = GetValueOrDefault<DateTime?>("LastMofiyDate");
                customer.LastModifyUser = GetValueOrDefault<string>("LastModifyUser");
                customer.CustFileTime = GetValueOrDefault<TimeSpan?>("CustFileTime");
                customer.IsESignature = GetValueOrDefault<bool?>("IsESignature");
                customer.DadID = GetValueOrDefault<string>("DadID");
                customer.MamID = GetValueOrDefault<string>("MamID");
                customer.JobSource = GetValueOrDefault<string>("JobSource");
                customer.Qualification = GetValueOrDefault<string>("Qualification");
                customer.MotherIdentity = GetValueOrDefault<string>("MotherIdentity");
                customer.FatherIdentity = GetValueOrDefault<string>("FatherIdentity");
                customer.OtherIdentity = GetValueOrDefault<string>("OtherIdentity");
                customer.LastEditUser = GetValueOrDefault<string>("LastEditUser");
                customer.DefaultDiscount = GetValueOrDefault<decimal?>("DefaultDiscount");
                customer.SuretyName = GetValueOrDefault<string>("SuretyName");
                customer.SuretyID = GetValueOrDefault<string>("SuretyID");
                customer.SuretyAddress = GetValueOrDefault<string>("SuretyAddress");
                customer.PassportNo = GetValueOrDefault<string>("PassportNo");
                customer.CustInvoiceBlockedEnb = GetValueOrDefault<bool?>("CustInvoiceBlockedEnb");
                customer.CustInvoiceBlockedTxt = GetValueOrDefault<string>("CustInvoiceBlockedTxt");
                customer.CustAllBlockedEnb = GetValueOrDefault<bool?>("CustAllBlockedEnb");
                customer.CustAllBlockedTxt = GetValueOrDefault<string>("CustAllBlockedTxt");
                customer.CustBlocked = GetValueOrDefault<bool?>("CustBlocked");
                customer.RelativeName = GetValueOrDefault<string>("RelativeName");
                customer.RelativePhone = GetValueOrDefault<string>("RelativePhone");
                customer.FileTypeID = GetValueOrDefault<int?>("FileTypeID");
                customer.FileTypeNo = GetValueOrDefault<string>("FileTypeNo");
                customer.FileTypeName = GetValueOrDefault<string>("FileTypeName");
                customer.IdentityType = GetValueOrDefault<string>("IdentityType");
                customer.DefaultDiscountExpiredDate = GetValueOrDefault<DateTime?>("DefaultDiscountExpiredDate");
                customer.Weight = GetValueOrDefault<decimal?>("Weight");
                customer.InsuranceExpiredDate = GetValueOrDefault<DateTime?>("InsuranceExpiredDate");
                customer.VATNumber = GetValueOrDefault<string>("VATNumber");
                customer.BaldiNo = GetValueOrDefault<string>("BaldiNo");
                customer.InsuranceRelation = GetValueOrDefault<string>("InsuranceRelation");
                customer.GLN = GetValueOrDefault<string>("GLN");
                customer.AddressDistrict = GetValueOrDefault<string>("AddressDistrict");
                customer.AddressStreet = GetValueOrDefault<string>("AddressStreet");
                customer.AddressBuildingNumber = GetValueOrDefault<string>("AddressBuildingNumber");
                customer.AddressPostalCode = GetValueOrDefault<string>("AddressPostalCode");
                customer.IsTaxRegistered = GetValueOrDefault<bool?>("IsTaxRegistered");

                return customer;
            }
        }
    


    // Implementation of synchronization service
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ICustomerDataService _customerDataService;
        private const int BatchSize = 50;
        private readonly string _failedRecordsPath;

        public SynchronizationService(
            IDatabaseService databaseService,
            ICustomerDataService customerDataService)
        {
            _databaseService = databaseService;
            _customerDataService = customerDataService;

            // Create directory for failed records if it doesn't exist
            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CustomerSync");

            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);

            _failedRecordsPath = Path.Combine(appDataDir, "failed_records.csv");
        }

        public async Task<(bool localSuccess, bool remoteSuccess)> TestConnectionsAsync(AppSettings settings)
        {
            bool localSuccess = await _databaseService.TestConnectionAsync(settings.LocalDatabase);
            bool remoteSuccess = await _databaseService.TestConnectionAsync(settings.RemoteDatabase);

            return (localSuccess, remoteSuccess);
        }

        public async Task EnsureTableExistsAsync(string connectionString)
        {
            bool tableExists = await _customerDataService.TableExistsAsync(connectionString);

            if (!tableExists)
            {
                await _customerDataService.CreateCustomersTableAsync(connectionString);
            }
        }

        public async Task<SyncResult> SynchronizeAsync(
            AppSettings settings,
            IProgress<SyncProgress> progress,
            CancellationToken cancellationToken = default)
        {
            var result = new SyncResult();
            var progressInfo = new SyncProgress();
            var failedRecords = new List<FailedRecord>();

            try
            {
                // Build connection strings
                string localConnectionString = _databaseService.BuildConnectionString(settings.LocalDatabase);
                string remoteConnectionString = _databaseService.BuildConnectionString(settings.RemoteDatabase);

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
                progressInfo.StatusMessage = "Checking if Customers table exists in remote database...";
                progress?.Report(progressInfo);

                await EnsureTableExistsAsync(remoteConnectionString);

                // Step 1: Count total records for progress tracking
                progressInfo.StatusMessage = "Counting records to sync...";
                progress?.Report(progressInfo);

                result.TotalRecords = await _customerDataService.GetTotalCustomerCountAsync(localConnectionString);
                progressInfo.TotalRecords = result.TotalRecords;
                progress?.Report(progressInfo);

                // Step 2: If configured, truncate the remote table first
                if (settings.TruncateBeforeSync)
                {
                    progressInfo.StatusMessage = "Truncating remote table...";
                    progress?.Report(progressInfo);

                    await _customerDataService.TruncateCustomersTableAsync(remoteConnectionString);
                }

                // Step 3: Synchronize in batches
                progressInfo.StatusMessage = "Starting data synchronization...";
                progress?.Report(progressInfo);

                int offset = 0;
                while (offset < result.TotalRecords && !cancellationToken.IsCancellationRequested)
                {
                    // Get a batch of records
                    var customerBatch = await _customerDataService.GetCustomerBatchAsync(localConnectionString, offset, BatchSize);

                    // Process each record in the batch
                    foreach (DataRow row in customerBatch.Rows)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        try
                        {
                            int custId = Convert.ToInt32(row["CustID"]);

                            if (settings.TruncateBeforeSync)
                            {
                                // If we're truncating, we only need to insert
                                await _customerDataService.InsertCustomerAsync(remoteConnectionString, row);
                                result.AddedRecords++;
                            }
                            else
                            {
                                // Otherwise, check if record exists and update or insert
                                bool exists = await _customerDataService.CustomerExistsAsync(remoteConnectionString, custId);

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
                        catch (Exception ex)
                        {
                            result.ErrorRecords++;
                            string errorMsg = $"Error processing record {row["CustID"]}: {ex.Message}";
                            result.ErrorMessages.Add(errorMsg);

                            // Add to failed records for later retry
                            failedRecords.Add(new FailedRecord
                            {
                                CustID = Convert.ToInt32(row["CustID"]),
                                ErrorMessage = ex.Message,
                                Timestamp = DateTime.Now
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
                    await _customerDataService.SaveFailedRecordsAsync(_failedRecordsPath, failedRecords);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    progressInfo.StatusMessage = "Synchronization was cancelled.";
                    result.ErrorMessages.Add("Synchronization was cancelled by user.");
                }
                else
                {
                    progressInfo.StatusMessage = "Synchronization completed successfully.";
                }

                progress?.Report(progressInfo);
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Critical error during synchronization: {ex.Message}");
                progressInfo.StatusMessage = "Synchronization failed!";
                progress?.Report(progressInfo);
                return result;
            }
        }
    }

    // Implementation of log service
    public class LogService : ILogService
    {
        public async Task SaveLogAsync(string filePath, SyncResult result)
        {
            var log = new StringBuilder();
            log.AppendLine($"Synchronization Log - {DateTime.Now}");
            log.AppendLine("=======================================");
            log.AppendLine();
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