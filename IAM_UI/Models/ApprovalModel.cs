using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IAM_UI.Models
{
    public class ApprovalModel : SelectListItem
    {
        public int TenantID { get; set; }
        public int UserKey { get; set; }

        public int Application_Name_Id { get; set; }

        public int Application_Main_Menu_Id { get; set; }
        // ADDED ON 30-11-2024
        public int Application_Module_Id { get; set; }
        public int Approval_Type { get; set; }

        public int Approval_Path_setup_type { get; set; }

        public int Approval_choose_ { get; set; }
        public int Approval_level_One_key { get; set; }
        public int Approval_level_three_key { get; set; }
        public string? COMPANY_KEY { get; set; }
  //      public int TenantId { get; set; }

        public int Approval_level_two_key { get; set; }

        public int UserType_key { get; set; }

        public string? StepNo { get; set; }

        public int employee_master_key { get; set; }

        public string? Names { get; set; }
        //public int APPROVAL_LVL1_KEY { get; set; }
        public string? ApprovalTypeDesc { get; set; }
        public string? ModuleName { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? arr_opt { get; set; }

        public string? Employee_Name { get; set; }

        public int? OprMode_Id  { get; set; }

    }

}
