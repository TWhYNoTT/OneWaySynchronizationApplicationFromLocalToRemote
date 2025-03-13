using System;
using System.Collections.Generic;

namespace SynchronizationApplication.Models
{
    // Model for database connection settings
    public class DatabaseSettings
    {
        public string Server { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Application configuration model
    public class AppSettings
    {
        public DatabaseSettings LocalDatabase { get; set; } = new DatabaseSettings();
        public DatabaseSettings RemoteDatabase { get; set; } = new DatabaseSettings();
        public bool TruncateBeforeSync { get; set; } = false;
    }

    // Model for synchronization results
    public class SyncResult
    {
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int AddedRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public int SkippedRecords { get; set; }
        public int ErrorRecords { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public bool IsSuccessful => ErrorRecords == 0;
    }

    // Model for synchronization progress
    public class SyncProgress
    {
        public int CurrentRecord { get; set; }
        public int TotalRecords { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public int PercentComplete => TotalRecords > 0 ? (int)((CurrentRecord / (double)TotalRecords) * 100) : 0;
    }


    public class FailedRecord
    {
        public int CustID { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsResolved { get; set; }
        public bool IsSelected { get; set; } // For UI selection
    }

    // Model representing a customer record
    public class Customer
    {
        public int CustID { get; set; }
        public string? CustName { get; set; }
        public string? CustDadName { get; set; }
        public string? CustMamName { get; set; }
        public string? CustJob { get; set; }
        public string? CustGender { get; set; }
        public string? CustNat { get; set; }
        public string? CustAddress { get; set; }
        public string? CustMobile1 { get; set; }
        public string? CustMobile2 { get; set; }
        public string? CustPhone1 { get; set; }
        public string? CustPhone2 { get; set; }
        public string? CustMail1 { get; set; }
        public string? CustMail2 { get; set; }
        public string? CustWattsUp { get; set; }
        public string? CustFacbook { get; set; }
        public DateTime? CustBirthday { get; set; }
        public string? CutIDentity { get; set; }
        public string? CustMarital { get; set; }
        public DateTime? CustFileDate { get; set; }
        public int? CustDr { get; set; }
        public bool? CustSmsEnabled { get; set; }
        public string? CustBlockedNote { get; set; }
        public string? CustResource { get; set; }
        public string? CustLevelStatue { get; set; }
        public string? CustWebSite { get; set; }
        public string? CustDrName { get; set; }
        public string? CustAge { get; set; }
        public string? CustResourceDetals { get; set; }
        public string? CustComment { get; set; }
        public string? CustCity { get; set; }
        public string? CustDrsName { get; set; }
        public string? CustUserName { get; set; }
        public string? CustOldID { get; set; }
        public bool? CustAppBlocked { get; set; }
        public string? CustAppBlockedNote { get; set; }
        public byte[]? PersonalImage { get; set; }
        public byte[]? IdentityImage { get; set; }
        public byte[]? Insurance { get; set; }
        public byte[]? Medicalhistorycard { get; set; }
        public string? HealthStatuDetails { get; set; }
        public string? HealthStatuNote { get; set; }
        public bool? Somker { get; set; }
        public string? BloodG { get; set; }
        public int? ChildNo { get; set; }
        public string? InsuranceCoName { get; set; }
        public string? InsuranceAproNo { get; set; }
        public string? InsurancePlic { get; set; }
        public string? InsuranceClass { get; set; }
        public string? PrevSickPseronal { get; set; }
        public string? PrevSickFamily { get; set; }
        public string? PrevSickConsoltion { get; set; }
        public string? PrevSickMedicneThss { get; set; }
        public string? PrevSickSpicalDo { get; set; }
        public DateTime? LastMofiyDate { get; set; }
        public string? LastModifyUser { get; set; }
        public TimeSpan? CustFileTime { get; set; }
        public bool? IsESignature { get; set; }
        public string? DadID { get; set; }
        public string? MamID { get; set; }
        public string? JobSource { get; set; }
        public string? Qualification { get; set; }
        public string? MotherIdentity { get; set; }
        public string? FatherIdentity { get; set; }
        public string? OtherIdentity { get; set; }
        public string? LastEditUser { get; set; }
        public decimal? DefaultDiscount { get; set; }
        public string? SuretyName { get; set; }
        public string? SuretyID { get; set; }
        public string? SuretyAddress { get; set; }
        public string? PassportNo { get; set; }
        public bool? CustInvoiceBlockedEnb { get; set; }
        public string? CustInvoiceBlockedTxt { get; set; }
        public bool? CustAllBlockedEnb { get; set; }
        public string? CustAllBlockedTxt { get; set; }
        public bool? CustBlocked { get; set; }
        public string? RelativeName { get; set; }
        public string? RelativePhone { get; set; }
        public int? FileTypeID { get; set; }
        public string? FileTypeNo { get; set; }
        public string? FileTypeName { get; set; }
        public string? IdentityType { get; set; }
        public DateTime? DefaultDiscountExpiredDate { get; set; }
        public decimal? Weight { get; set; }
        public DateTime? InsuranceExpiredDate { get; set; }
        public string? VATNumber { get; set; }
        public string? BaldiNo { get; set; }
        public string? InsuranceRelation { get; set; }
        public string? GLN { get; set; }
        public string? AddressDistrict { get; set; }
        public string? AddressStreet { get; set; }
        public string? AddressBuildingNumber { get; set; }
        public string? AddressPostalCode { get; set; }
        public bool? IsTaxRegistered { get; set; }
    }
}