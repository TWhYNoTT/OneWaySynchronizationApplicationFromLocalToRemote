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
    public class CustomerDataService : ICustomerDataService
    {
        private readonly IDatabaseService _databaseService;
        private readonly List<string> _imageColumns = new List<string>
        {
            "PersonalImage",
            "IdentityImage",
            "Insurance",
            "Medicalhistorycard"
        };

        public CustomerDataService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<int> GetTotalCustomerCountAsync(string connectionString)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = provider.CreateCommand("SELECT COUNT(*) FROM Customers", connection))
                {
                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        public async Task<DataTable> GetCustomerBatchAsync(string connectionString, int offset, int batchSize)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = provider.GetBatchQuerySql("Customers", offset, batchSize);

                using (var command = provider.CreateCommand(query, connection))
                using (var adapter = provider.CreateDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public async Task<bool> CustomerExistsAsync(string connectionString, int custId)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = provider.CreateCommand("SELECT COUNT(*) FROM Customers WHERE CustID = @CustID", connection))
                {
                    command.Parameters.Add(provider.CreateParameter("@CustID", custId));
                    return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
                }
            }
        }

        public async Task InsertCustomerAsync(string connectionString, DataRow customerData)
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
                            provider.GetEnableIdentityInsertSql("Customers"), connection, transaction))
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

                            columns.Add($"{col.ColumnName}");
                            parameters.Add($"@{col.ColumnName}");
                        }

                        string sql = $"INSERT INTO Customers ({string.Join(", ", columns)}) VALUES ({string.Join(", ", parameters)})";

                        using (var command = provider.CreateCommand(sql, connection, transaction))
                        {
                            foreach (DataColumn col in customerData.Table.Columns)
                            {
                                // Skip the RowNum column we added for batching
                                if (col.ColumnName == "RowNum")
                                    continue;

                                // Handle image columns differently
                                if (_imageColumns.Contains(col.ColumnName))
                                {
                                    var param = provider.CreateParameter(
                                        $"@{col.ColumnName}",
                                        customerData.IsNull(col) ? DBNull.Value : customerData[col.ColumnName],
                                        provider.GetImageDbType());

                                    command.Parameters.Add(param);
                                }
                                else
                                {
                                    // For non-image columns
                                    command.Parameters.Add(provider.CreateParameter(
                                        $"@{col.ColumnName}",
                                        customerData.IsNull(col) ? DBNull.Value : customerData[col.ColumnName]));
                                }
                            }

                            await command.ExecuteNonQueryAsync();
                        }

                        // Disable identity insert after we're done
                        using (var identityCommand = provider.CreateCommand(
                            provider.GetDisableIdentityInsertSql("Customers"), connection, transaction))
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

        public async Task UpdateCustomerAsync(string connectionString, DataRow customerData)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Build SET clause for update
                var setClauses = new List<string>();

                foreach (DataColumn col in customerData.Table.Columns)
                {
                    // Skip the RowNum column we added for batching and the primary key
                    if (col.ColumnName == "RowNum" || col.ColumnName == "CustID")
                        continue;

                    setClauses.Add($"{col.ColumnName} = @{col.ColumnName}");
                }

                string sql = $"UPDATE Customers SET {string.Join(", ", setClauses)} WHERE CustID = @CustID";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    foreach (DataColumn col in customerData.Table.Columns)
                    {
                        // Skip the RowNum column we added for batching
                        if (col.ColumnName == "RowNum")
                            continue;

                        // Handle image columns differently
                        if (_imageColumns.Contains(col.ColumnName))
                        {
                            var param = provider.CreateParameter(
                                $"@{col.ColumnName}",
                                customerData.IsNull(col) ? DBNull.Value : customerData[col.ColumnName],
                                provider.GetImageDbType());

                            command.Parameters.Add(param);
                        }
                        else
                        {
                            // For non-image columns
                            command.Parameters.Add(provider.CreateParameter(
                                $"@{col.ColumnName}",
                                customerData.IsNull(col) ? DBNull.Value : customerData[col.ColumnName]));
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteCustomerAsync(string connectionString, int custId)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Simple DELETE statement to remove the customer
                string sql = "DELETE FROM Customers WHERE CustID = @CustID";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    command.Parameters.Add(provider.CreateParameter("@CustID", custId));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task TruncateCustomersTableAsync(string connectionString)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = provider.CreateCommand("DELETE FROM Customers", connection))
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

        public async Task CreateCustomersTableAsync(string connectionString)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                // Create table with the appropriate schema for the provider
                string createTableSql = provider.GetCreateCustomersTableSql();

                using (var command = provider.CreateCommand(createTableSql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Customer> GetCustomerByIdAsync(string connectionString, int custId)
        {
            var provider = _databaseService.GetProviderForConnection(connectionString);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = provider.CreateCommand("SELECT * FROM Customers WHERE CustID = @CustID", connection))
                {
                    command.Parameters.Add(provider.CreateParameter("@CustID", custId));

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

        private Customer MapReaderToCustomer(DbDataReader reader)
        {
            var customer = new Customer();

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
            int personalImageOrdinal = -1;
            try { personalImageOrdinal = reader.GetOrdinal("PersonalImage"); } catch { }
            if (personalImageOrdinal >= 0 && !reader.IsDBNull(personalImageOrdinal))
            {
                customer.PersonalImage = (byte[])reader.GetValue(personalImageOrdinal);
            }

            int identityImageOrdinal = -1;
            try { identityImageOrdinal = reader.GetOrdinal("IdentityImage"); } catch { }
            if (identityImageOrdinal >= 0 && !reader.IsDBNull(identityImageOrdinal))
            {
                customer.IdentityImage = (byte[])reader.GetValue(identityImageOrdinal);
            }

            int insuranceOrdinal = -1;
            try { insuranceOrdinal = reader.GetOrdinal("Insurance"); } catch { }
            if (insuranceOrdinal >= 0 && !reader.IsDBNull(insuranceOrdinal))
            {
                customer.Insurance = (byte[])reader.GetValue(insuranceOrdinal);
            }

            int medicalCardOrdinal = -1;
            try { medicalCardOrdinal = reader.GetOrdinal("Medicalhistorycard"); } catch { }
            if (medicalCardOrdinal >= 0 && !reader.IsDBNull(medicalCardOrdinal))
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
}



















