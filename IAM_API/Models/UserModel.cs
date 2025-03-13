using System.ComponentModel.DataAnnotations;

namespace IAM_API.Models
{
    public class UserModel
    {


        public int TenantId { get; set; }
        public int UserCategoryId { get; set; }
        public int GroupHeadId { get; set; }
        public string? GroupHeadName { get; set; }
        public int GroupHeadPersonnelId { get; set; }


        public string? DIN { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }

        public string FathersName { get; set; }
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN Number.")]

        public string PAN { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Invalid Aadhar Number.")]
        public string Aadhar { get; set; }

        public string EmailId { get; set; }

        public string MobileNo { get; set; }
        public string? SelectedDepartments { get; set; }


        public int ResidentialStatusId { get; set; }


        public string? OtherCompanyDirectors1 { get; set; }

        public string? OtherCompanyDirectors2 { get; set; }

        public string? OtherCompanyDirectors3 { get; set; }

        public string? OtherCompanyDirectors4 { get; set; }
        public string? OtherCompanyDirectors5 { get; set; }

        public string User_Name { get; set; }
        public string PWD { get; set; }
        public int ActiveFlag { get; set; }

        public string EffectiveDate { get; set; }
        public string? userName { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public int UserTypeId { get; set; }
        public string? UserTypeName { get; set; }



        public List<CompanyUserCombination> Combinations { get; set; }

    }

    public class CompanyUserCombination
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }  // Name attribute of CompanyId checkbox
        public string UserTypeId { get; set; }
        public string UserTypeName { get; set; }  // Name attribute of UserTypeId checkbox
    }
}
