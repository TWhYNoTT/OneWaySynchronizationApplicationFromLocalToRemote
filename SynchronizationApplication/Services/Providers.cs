using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using SynchronizationApplication.Models;

namespace SynchronizationApplication.Services.Providers
{
    /// <summary>
    /// Database provider type enumeration
    /// </summary>
    public enum DatabaseProviderType
    {
        SqlServer,
        MySQL
    }

    /// <summary>
    /// Interface for database providers
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>
        /// Gets the provider type
        /// </summary>
        DatabaseProviderType ProviderType { get; }

        /// <summary>
        /// Builds a connection string from the database settings
        /// </summary>
        string BuildConnectionString(DatabaseSettings settings);

        /// <summary>
        /// Tests a database connection
        /// </summary>
        Task<bool> TestConnectionAsync(string connectionString);

        /// <summary>
        /// Creates a data adapter for the specified command
        /// </summary>
        DbDataAdapter CreateDataAdapter(DbCommand command);

        /// <summary>
        /// Creates a connection for the specified connection string
        /// </summary>
        DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Creates a command for the specified connection and SQL
        /// </summary>
        DbCommand CreateCommand(string sql, DbConnection connection, DbTransaction transaction = null);

        /// <summary>
        /// Creates a parameter with the specified name and value
        /// </summary>
        DbParameter CreateParameter(string name, object value, DbType? dbType = null);

        /// <summary>
        /// Checks if a table exists in the database
        /// </summary>
        Task<bool> TableExistsAsync(string connectionString, string tableName);

        /// <summary>
        /// Gets SQL for creating the Customers table
        /// </summary>
        string GetCreateCustomersTableSql();

        /// <summary>
        /// Gets SQL for creating the Users table
        /// </summary>
        string GetCreateUsersTableSql();

        /// <summary>
        /// Gets SQL for paging data (used in batch processing)
        /// </summary>
        string GetBatchQuerySql(string tableName, int offset, int batchSize);

        /// <summary>
        /// Gets the SQL to enable identity insert
        /// </summary>
        string GetEnableIdentityInsertSql(string tableName);

        /// <summary>
        /// Gets the SQL to disable identity insert
        /// </summary>
        string GetDisableIdentityInsertSql(string tableName);

        /// <summary>
        /// Gets the appropriate DbType for image/blob data
        /// </summary>
        DbType GetImageDbType();

        /// <summary>
        /// Gets the change tracking table name for the entity type
        /// </summary>
        string GetChangeTrackingTableName(EntityType entityType);

        /// <summary>
        /// Gets the column name for the entity ID in the change tracking table
        /// </summary>
        string GetEntityIdColumnName(EntityType entityType);

        /// <summary>
        /// Gets the table name for the entity type
        /// </summary>
        string GetTableName(EntityType entityType);
    }

    /// <summary>
    /// SQL Server database provider implementation
    /// </summary>
    public class SqlServerProvider : IDatabaseProvider
    {
        public DatabaseProviderType ProviderType => DatabaseProviderType.SqlServer;

        public string BuildConnectionString(DatabaseSettings settings)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = settings.Server,
                InitialCatalog = settings.Database
            };

            // Check if we should use Windows Authentication
            if (string.IsNullOrWhiteSpace(settings.Username) && string.IsNullOrWhiteSpace(settings.Password))
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = settings.Username;
                builder.Password = settings.Password;
            }

            // Add these properties carefully in case they're not supported in the current version
            try
            {
                builder.TrustServerCertificate = true;
            }
            catch
            {
                // Ignore if not supported
            }

            try
            {
                builder.Encrypt = false; // Change to false to avoid encryption issues
            }
            catch
            {
                // Ignore if not supported
            }

            return builder.ConnectionString;
        }
        public async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            return new SqlDataAdapter((SqlCommand)command);
        }

        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public DbCommand CreateCommand(string sql, DbConnection connection, DbTransaction transaction = null)
        {
            var command = new SqlCommand(sql, (SqlConnection)connection);
            if (transaction != null)
            {
                command.Transaction = (SqlTransaction)transaction;
            }
            return command;
        }

        public DbParameter CreateParameter(string name, object value, DbType? dbType = null)
        {
            var parameter = new SqlParameter(name, value ?? DBNull.Value);
            if (dbType.HasValue)
            {
                parameter.DbType = dbType.Value;
            }
            return parameter;
        }

        public async Task<bool> TableExistsAsync(string connectionString, string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = 'dbo' 
                    AND TABLE_NAME = @TableName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    return (int)await command.ExecuteScalarAsync() > 0;
                }
            }
        }

        public string GetCreateCustomersTableSql()
        {
            return @"
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
        }

        public string GetCreateUsersTableSql()
        {
            // Create table with only the columns we want to sync
            return @"
CREATE TABLE [dbo].[Users](
    [UserID] [int] IDENTITY(1,1) NOT NULL,
    [UserName] [nvarchar](255) NULL,
    [UserPass] [nvarchar](255) NULL,
    [UserSysName] [nvarchar](255) NULL,
    [UserLevel] [nvarchar](255) NULL,
    [UserPhone] [nvarchar](255) NULL,
    [UserEnabled] [bit] NULL,
    [UserNote] [nvarchar](max) NULL,
    [UserAddedDate] [datetime] NULL,
    [Image] [image] NULL,
    [Email] [nvarchar](255) NULL,
    [Gender] [nvarchar](50) NULL,
    [PermissionType] [nvarchar](255) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
    [UserID] ASC
))";
        }

        public string GetBatchQuerySql(string tableName, int offset, int batchSize)
        {
            return $@"
                SELECT * FROM (
                    SELECT *, ROW_NUMBER() OVER (ORDER BY {(tableName == "Users" ? "UserID" : "CustID")}) AS RowNum
                    FROM {tableName}
                ) AS Temp
                WHERE RowNum > {offset} AND RowNum <= {offset + batchSize}";
        }

        public string GetEnableIdentityInsertSql(string tableName)
        {
            return $"SET IDENTITY_INSERT {tableName} ON";
        }

        public string GetDisableIdentityInsertSql(string tableName)
        {
            return $"SET IDENTITY_INSERT {tableName} OFF";
        }

        public DbType GetImageDbType()
        {
            return DbType.Binary;
        }

        public string GetChangeTrackingTableName(EntityType entityType)
        {
            return entityType == EntityType.Customer ? "CustomerChangeLog" : "UserChangeLog";
        }

        public string GetEntityIdColumnName(EntityType entityType)
        {
            return entityType == EntityType.Customer ? "CustID" : "UserID";
        }

        public string GetTableName(EntityType entityType)
        {
            return entityType == EntityType.Customer ? "Customers" : "Users";
        }
    }

    /// <summary>
    /// MySQL database provider implementation
    /// </summary>
    public class MySqlProvider : IDatabaseProvider
    {
        public DatabaseProviderType ProviderType => DatabaseProviderType.MySQL;

        public string BuildConnectionString(DatabaseSettings settings)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = settings.Server,
                Database = settings.Database,
                UserID = settings.Username,
                Password = settings.Password,
                SslMode = MySqlSslMode.Preferred,
                AllowPublicKeyRetrieval = true,
                AllowUserVariables = true,
                

            };

            return builder.ConnectionString;
        }

        public async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            return new MySqlDataAdapter((MySqlCommand)command);
        }

        public DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public DbCommand CreateCommand(string sql, DbConnection connection, DbTransaction transaction = null)
        {
            var command = new MySqlCommand(sql, (MySqlConnection)connection);
            if (transaction != null)
            {
                command.Transaction = (MySqlTransaction)transaction;
            }
            return command;
        }

        public DbParameter CreateParameter(string name, object value, DbType? dbType = null)
        {
            var parameter = new MySqlParameter(name, value ?? DBNull.Value);
            if (dbType.HasValue)
            {
                parameter.DbType = dbType.Value;
            }
            return parameter;
        }

        public async Task<bool> TableExistsAsync(string connectionString, string tableName)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT COUNT(*)
                    FROM information_schema.tables
                    WHERE table_schema = DATABASE()
                    AND table_name = @TableName";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
                }
            }
        }

        public string GetCreateCustomersTableSql()
        {
            return @"
CREATE TABLE `Customers` (
    `CustName` LONGTEXT NULL,
    `CustDadName` VARCHAR(255) NULL,
    `CustMamName` VARCHAR(255) NULL,
    `CustJob` VARCHAR(255) NULL,
    `CustGender` VARCHAR(255) NULL,
    `CustNat` VARCHAR(255) NULL,
    `CustAddress` LONGTEXT NULL,
    `CustMobile1` VARCHAR(255) NULL,
    `CustMobile2` VARCHAR(255) NULL,
    `CustPhone1` VARCHAR(255) NULL,
    `CustPhone2` VARCHAR(255) NULL,
    `CustMail1` VARCHAR(255) NULL,
    `CustMail2` VARCHAR(255) NULL,
    `CustWattsUp` VARCHAR(255) NULL,
    `CustFacbook` VARCHAR(255) NULL,
    `CustBirthday` DATE NULL,
    `CutIDentity` VARCHAR(255) NULL,
    `CustMarital` VARCHAR(255) NULL,
    `CustFileDate` DATE NULL,
    `CustDr` INT NULL,
    `CustSmsEnabled` BOOLEAN NULL,
    `CustBlockedNote` LONGTEXT NULL,
    `CustResource` LONGTEXT NULL,
    `CustLevelStatue` LONGTEXT NULL,
    `CustWebSite` LONGTEXT NULL,
    `CustDrName` VARCHAR(255) NULL,
    `CustAge` VARCHAR(255) NULL,
    `CustResourceDetals` LONGTEXT NULL,
    `CustComment` LONGTEXT NULL,
    `CustCity` VARCHAR(255) NULL,
    `CustID` INT NOT NULL AUTO_INCREMENT,
    `CustDrsName` LONGTEXT NULL,
    `CustUserName` VARCHAR(255) NULL,
    `CustOldID` LONGTEXT NULL,
    `CustAppBlocked` BOOLEAN NULL,
    `CustAppBlockedNote` LONGTEXT NULL,
    `PersonalImage` LONGBLOB NULL,
    `IdentityImage` LONGBLOB NULL,
    `Insurance` LONGBLOB NULL,
    `Medicalhistorycard` LONGBLOB NULL,
    `HealthStatuDetails` LONGTEXT NULL,
    `HealthStatuNote` LONGTEXT NULL,
    `Somker` BOOLEAN NULL,
    `BloodG` VARCHAR(255) NULL,
    `ChildNo` INT NULL,
    `InsuranceCoName` VARCHAR(100) NULL,
    `InsuranceAproNo` VARCHAR(55) NULL,
    `InsurancePlic` VARCHAR(100) NULL,
    `InsuranceClass` VARCHAR(55) NULL,
    `PrevSickPseronal` LONGTEXT NULL,
    `PrevSickFamily` LONGTEXT NULL,
    `PrevSickConsoltion` LONGTEXT NULL,
    `PrevSickMedicneThss` LONGTEXT NULL,
    `PrevSickSpicalDo` LONGTEXT NULL,
    `LastMofiyDate` DATETIME NULL,
    `LastModifyUser` VARCHAR(255) NULL,
    `CustFileTime` TIME NULL,
    `IsESignature` BOOLEAN NULL,
    `DadID` LONGTEXT NULL,
    `MamID` LONGTEXT NULL,
    `JobSource` VARCHAR(255) NULL,
    `Qualification` VARCHAR(255) NULL,
    `MotherIdentity` VARCHAR(255) NULL,
    `FatherIdentity` VARCHAR(255) NULL,
    `OtherIdentity` VARCHAR(255) NULL,
    `LastEditUser` VARCHAR(255) NULL,
    `DefaultDiscount` DECIMAL(18, 2) NULL,
    `SuretyName` VARCHAR(255) NULL,
    `SuretyID` VARCHAR(255) NULL,
    `SuretyAddress` LONGTEXT NULL,
    `PassportNo` VARCHAR(255) NULL,
    `CustInvoiceBlockedEnb` BOOLEAN NULL,
    `CustInvoiceBlockedTxt` LONGTEXT NULL,
    `CustAllBlockedEnb` BOOLEAN NULL,
    `CustAllBlockedTxt` LONGTEXT NULL,
    `CustBlocked` BOOLEAN NULL,
    `RelativeName` LONGTEXT NULL,
    `RelativePhone` LONGTEXT NULL,
    `FileTypeID` INT NULL,
    `FileTypeNo` VARCHAR(10) NULL,
    `FileTypeName` VARCHAR(255) NULL,
    `IdentityType` VARCHAR(10) NULL,
    `DefaultDiscountExpiredDate` DATE NULL,
    `Weight` DECIMAL(5, 2) NULL,
    `InsuranceExpiredDate` DATE NULL,
    `VATNumber` VARCHAR(50) NULL,
    `BaldiNo` VARCHAR(50) NULL,
    `InsuranceRelation` VARCHAR(50) NULL,
    `GLN` VARCHAR(255) NULL,
    `AddressDistrict` VARCHAR(127) NULL,
    `AddressStreet` VARCHAR(1000) NULL,
    `AddressBuildingNumber` VARCHAR(50) NULL,
    `AddressPostalCode` VARCHAR(200) NULL,
    `IsTaxRegistered` BOOLEAN NULL,
    PRIMARY KEY (`CustID`)
) ENGINE=InnoDB;";
        }

        public string GetCreateUsersTableSql()
        {
            // Create table with only the columns we want to sync
            return @"
CREATE TABLE `Users` (
    `UserID` INT NOT NULL AUTO_INCREMENT,
    `UserName` VARCHAR(255) NULL,
    `UserPass` VARCHAR(255) NULL,
    `UserSysName` VARCHAR(255) NULL,
    `UserLevel` VARCHAR(255) NULL,
    `UserPhone` VARCHAR(255) NULL,
    `UserEnabled` BOOLEAN NULL,
    `UserNote` LONGTEXT NULL,
    `UserAddedDate` DATETIME NULL,
    `Image` LONGBLOB NULL,
    `Email` VARCHAR(255) NULL,
    `Gender` VARCHAR(50) NULL,
    `PermissionType` VARCHAR(255) NULL,
    PRIMARY KEY (`UserID`)
) ENGINE=InnoDB;";
        }

        public string GetBatchQuerySql(string tableName, int offset, int batchSize)
        {
            string idColumn = tableName == "Users" ? "UserID" : "CustID";
            return $@"
                SELECT * FROM {tableName}
                ORDER BY {idColumn}
                LIMIT {offset}, {batchSize}";
        }

        public string GetEnableIdentityInsertSql(string tableName)
        {
            // MySQL doesn't have IDENTITY_INSERT, but we need to set SQL_MODE to allow setting AUTO_INCREMENT values
            return "SET SESSION sql_mode = 'NO_AUTO_VALUE_ON_ZERO'";
        }

        public string GetDisableIdentityInsertSql(string tableName)
        {
            // Reset SQL_MODE to default
            return "SET SESSION sql_mode = ''";
        }

        public DbType GetImageDbType()
        {
            return DbType.Binary;
        }

        public string GetChangeTrackingTableName(EntityType entityType)
        {
            return entityType == EntityType.Customer ? "CustomerChangeLog" : "UserChangeLog";
        }

        public string GetEntityIdColumnName(EntityType entityType)
        {
            return entityType == EntityType.Customer ? "CustID" : "UserID";
        }

        public string GetTableName(EntityType entityType)
        {
            return entityType == EntityType.Customer ? "Customers" : "Users";
        }
    }
}





