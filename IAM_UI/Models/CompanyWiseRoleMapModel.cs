namespace IAM_UI.Models
{
    public class CompanyWiseRoleMapModel
    {

        public int? UserTypeId { get; set; }

        public int? UserTypeName { get; set; }


        public int? UserCategoryId { get; set; }
        public string? UserCategoryName { get; set; }

        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }

         public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        public int? DesignationId { get; set; }
        public string? DesignationName { get; set; }

  
        public int? JobRoleId { get; set; }
        public string? JobRoleName { get; set; }



    }
}
