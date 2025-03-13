using IAM_UI.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IAM_UI.Models
{
    public class Vm_Dashboard
    {
        public string enc { get; set; }

        public string enci { get; set; }

        public string UserType { get; set; }
        public string encEmpId { get; set; }

        public string encJobRoleId { get; set; }

        public string? CompanyId { get; set; }
        public string AppID { get;set; }
        public string SessionId { get;set; }
        public string? USH_KEY { get; set; }

        public List<model_companies> companies { get; set; }


    }
    public class model_companies
    {
        public string COMPANY_NAME { get; set; }

        public string CompanyId { get; set; }

        public string encCompanyId { get; set; }

        public string ADDRESS { get; set; }

        public string PINNO { get; set; }



    }

    public class HRMS
    {
        public int HrmsId { get;set; }
    }
    public class EMPLOYEE_STATUS
    {
        public int Tag_Approval { get; set; }
        public string Personnel_Name { get; set; }
        public string Entry_User_Name { get; set; }
        public string Approval_Status { get; set; }
        public string Reporting_Head { get; set; }
    }
    public class HEAD_LIST
    {
        public string Rec_Type { get; set; }
        public int? EmployeeId { get; set; }
        public int User_Type_Key { get; set; }
        public int Mast_Hrd_Draft_Personnel_Key { get; set; }
        public int Company_Key { get; set; }
    }
    public class APPROVAL
    {

        public int Count { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
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
    public class ApproveRejectModel
    {
        public int TenantId { get; set; }
        public int ApplicationdId { get; set; }
        public int CompanyId { get; set; }
        public string Rec_Type { get; set; }
        public int Row_Id { get; set; }
        public int ModuleId { get; set; }
        public int UserId { get; set; }
        public int UserTypeId { get; set; }
        public string? Remarks { get; set; }
    }


    public class SendToApproval
    {
        public string? Rec_Type { get; set; }
        public int? RowId { get; set; }
        public int? ModuleId { get; set; }

        public int? TenantId { get; set; }
        public int? CompanyId { get; set; }
        public int? ApplicationId { get; set; }
        public int? UserTypeId { get; set; }

        public int? UserId { get; set; }
        public string? Remarks { get; set; }
        public int OperationalMode { get; set; }


    }
}
