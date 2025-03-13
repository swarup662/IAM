namespace IAM_UI.Models
{

    public class UserCreationViewModel
    {


        public int? TenantId { get; set; }
        public int? UserID { get; set; }
        public int? USER_MASTER_KEY { get; set; }
        public int? UserProfileID { get; set; }
        public string? User_Unique_ID { get; set; }
        public int UserCategoryId { get; set; }
        public string? UserCategoryName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? CurrentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public int? Gender { get; set; }
        public string? GenderName { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }


        public string? Pin { get; set; }

        public DateTime? DOB { get; set; }
        public string? Mobile_No { get; set; }
        public string? Email_ID { get; set; }

        public int? EmailTypeId { get; set; }
        public string? EmailTypeName { get; set; }
        public string? EmailTypeCode { get; set; }
        public string? Aadhar_no { get; set; }
        public int IsAcceptedTerms { get; set; }
        public int StatusKey { get; set; }


        public int? UserTypeId { get; set; }
        public string? UserTypeName { get; set; }

        public int? CreatedBy { get; set; }
        public int? CreatedDate { get; set; }

        #region Company
        public string? UserTypeIdString { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyIdString { get; set; }
        public string? CompanyName { get; set; }
        public int? empCompanyId { get; set; }
        public string? empCompanyName { get; set; }

        public int? empCompanyId_lbl3 { get; set; }
        public string? empCompanyName_lbl3 { get; set; }

        public int? application_ID { get; set; }
        public string? applicationName { get; set; }

        public int? UserTypeId_lbl3 { get; set; }
        public string? UserType_lbl3 { get; set; }
        #endregion
    }


    public class UserProfileAccessMapDtls2
    {
        public int? TenantId { get; set; }
        public int? USER_MASTER_KEY { get; set; }
        public int? UserProfileID { get; set; }
        public int? CompanyId { get; set; }
        public int? UserTypeId { get; set; }

        public int? ApplicationId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActionId { get; set; }

        public int? CreatedBy { get; set; }
        public int ActiveFlag { get; set; }


    }

    public class UserCreationModel
    {
        public int TenantID { get; set; }
        public int USER_MASTER_KEY { get; set; }
        public string? User_Unique_ID { get; set; }
        public int UserProfileID { get; set; }
        public int UserTypeId { get; set; }
        public int UserCategoryId { get; set; }
        public string? UserCategoryName { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int Pin { get; set; }
        public int AuthBlock { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public DateTime DOB { get; set; }
        public string? Mobile_No { get; set; }
        public string? Email_ID { get; set; }
        public int? EmailTypeId { get; set; }
        public string? EmailTypeName { get; set; }
        public string? EmailTypeCode { get; set; }
        public int Gender { get; set; }
        public string? GenderName { get; set; }
        public string? CurrentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Aadhar_no { get; set; }
        public int IsAcceptedTerms { get; set; }
        public int StatusKey { get; set; }
    }
    public class BlockUserModel
    {

        public int Key { get; set; } // 1 for permanent, 2 for temporary
        public int UserID { get; set; } // Employee ID
    }


}
