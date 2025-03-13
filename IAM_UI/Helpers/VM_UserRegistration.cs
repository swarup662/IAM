using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using IAM_UI.Helpers;
using IAM_UI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IAM_UI.Helpers
{
    public class VM_UserRegistration
    {

        public int BusinessUserID { get; set; }

        public int TenantID { get; set; }
        public string CompanyGroupName { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ClientName { get; set; }
        [DataType(DataType.Date)]
        public string ClientDOB { get; set; }
        [DataType(DataType.EmailAddress)]
        public string ClientEmailId { get; set; }
        [MinLength(10)]
        [MaxLength(10)]
        public string ClientContactNo { get; set; }
        public int ClientUniqueCodeTypeId { get; set; }
        public string ClientUniqueCode { get; set; }
        public string TenantProductKey { get; set; }



    }
    public class VM_UserLogin
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Please enter your username")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
    }
    public class VM_UserLoginResponse
    {
        public int UserID { get; set; }

        [JsonProperty("employee_Master_Key")]
        public int Employee_Master_Key { get; set; }
        public int Company { get; set; }

        public int DesignationId { get; set; }
        public string? CompanyId { get; set; }
        public string? Designation { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int JobRoleId { get; set; }
        public int UserTypeId { get; set; }
        public string? token { get; set; }
        public string? refresh_token { get; set; }

        public string? UserTypeList { get; set; }
		public string SessionId { get; set; }
		public DateTime RefreshTokenExpiry { get; set; }

        public string refreshtoken { get; set; }

        public DateTime refreshtokenExpiry { get; set; }

        public string? access_token { get; set; }

        public int? retVal { get; set; }

        public int? AppID { get; set; }



    }

    public class SessionDataModel
    {
        public string UserID { get; set; }
        public string Employee_Master_Key { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string JobRoleId { get; set; }
        public string UserTypeId { get; set; }
        public string token { get; set; }
        public string refresh_token { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }
    }
    public class Req_Companie
    {
        public int tenantid { get; set; }

    }
    public class model_companie
    {
        public string COMPANY_NAME { get; set; }

        public string CompanyId { get; set; }

        //public string encCompanyId { get; set; }

        //public string ADDRESS { get; set; }

        //public string PINNO { get; set; }



    }

    public class CompApp
    {
        
        [Required(ErrorMessage = "Please Select Company")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid company.")]
        public int CompanyID { get; set; }
        //  public string COMPANY_NAME { get; set; }
       
        [Required(ErrorMessage = "Please Select Application")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Application.")]
        public int Application { get; set; }
    }

   
    public class Global_Model
    {
        public int ApplicationId { get; set; }
        public int TenantID { get; set; }
        public int UserID { get; set; }
        public int CompanyID { get; set; }
        public int ModuleId { get; set; }
        public int UserTypeId { get; set; }
        public string Data { get; set; }
        public string Token { get; set; } = null;
        public string ModuleAccessData { get; set; } = null;
        public string CompanyList { get; set; } = null;
        public string? UserDetail { get; set; } = "";
        public string? Approval { get; set; } = "";
    }
    public class MenuModel
    {
        public int TenantID { get; set; }
        public int ApplicationId { get; set; }
        public int ModuleId { get; set; }
        public int ModuleParentId { get; set; }
        public string? ModuleName { get; set; }
        public string? ModuleLink { get; set; }
        public decimal ModuleHierarchy { get; set; }
        public string? ModuleTypeName { get; set; }
        public int ModuleTypeId { get; set; }
        public string? FullHiearchy { get; set; }
        public int Add { get; set; }
        public int Edit { get; set; }
        public int Delete { get; set; }
        public int Print { get; set; }
        public int View { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }


        // Add this property
        public List<MenuModel> Children { get; set; } = new List<MenuModel>();
    }
    public class UserTypeList
    {
        public int TenantID { get; set; }
        public int? ApplicationId { get; set; }
        public int CompanyID { get; set; }
        public int UserTypeId { get; set; }
        public string UserType { get; set; }
    }
    //public class CompanyList
    //{
    //    public int CompanyID { get; set; }
    //    public string CompanyName { get; set; }
    //}
    public class ProcessingResult
    {
        public List<CompanyList> CompanyList { get; set; }
        public List<MenuModel> MenuItems { get; set; }
        public int UserTypeId { get; set; }
        public int CompanyId { get; set; }
        public List<UserTypeList> UserTypes { get; set; }
        public UserDetail UserDetail { get; set; }
        public List<ApprovalList> Approval { get; set; }
    }

    public class UserDetail
    {
        public int TenantId { get; set; }
        public int UserCategoryId { get; set; }
        public int USER_MASTER_KEY { get; set; }
        //public int  CompanyId { get; set; }
        public string Email_ID { get; set; }
        public string User_Unique_ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime? DOB { get; set; }
        public string Mobile_No { get; set; }
        public string Gender { get; set; }
        public string GenderName { get; set; }
        public string CurrentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public int? IsAcceptedTerms { get; set; }

        public int CurrentCompanyId { get; set; }
        public int CurrentUserTypeId { get; set; }

        public int CurrentApplicationId { get; set; }
        public int CurrentTenantId { get; set; }

        // VM_UserLoginResponse All model data

        public int UserID { get; set; }

        [JsonProperty("user_Master_Key")]
        public int User_Master_Key { get; set; }

        public int? Company { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName{ get; set; }
        public string UserName { get; set; }
        public string? Password { get; set; }
        public int UserTypeId { get; set; }
        public string? token { get; set; }
        public string? refresh_token { get; set; }

        public string? UserTypeList { get; set; }
        public string SessionId { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }

        public string refreshtoken { get; set; }

        public DateTime refreshtokenExpiry { get; set; }

        public string? access_token { get; set; }

        public int? retVal { get; set; }

        public int? AppID { get; set; }



    }

    public class CompanyList : CustomSelectListItem
    {
        public int CompanyId { get; set; }
        public string COMPANY_NAME { get; set; }
    }

    public class CustomSelectListItem : SelectListItem
    {


        public string Description { get; set; }
    }
    public class CustomSelectListItem2
    {

        public int value { get; set; }
        public int label { get; set; }
        public int value2 { get; set; }
        public int ParentId { get; set; }
        public string text { get; set; }
        public string text2 { get; set; }
        public string text3 { get; set; }
    }
    public class GeneralModel
    {
        public int ModuleId { get; set; }
        public int userId { get; set; }
        public int UserTypeId { get; set; }
    }
    public class ApprovalList
    {
        public int TenantId { get; set; }
        public int ApplicationId { get; set; }
        public string CompanyKey { get; set; }
        public int ModuleId { get; set; }



    }


}
