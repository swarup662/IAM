using System;

namespace IAM_UI.Models
{
    public class SaasBillingEmpModel
    {
        public int? UsersForBillingId { get; set; }
        public int? TenantId { get; set; }
        public int? ApplicationId { get; set; }
        public int? CompanyId { get; set; }
        public string? COMPANY_NAME { get; set; }
        public int? Mast_State_Key { get; set; }
        public string? StateName { get; set; }
        public string? GST_NO { get; set; }
        public string? PAN_NO { get; set; }
        public string? CIN_NO { get; set; }
        public string? MOBILE { get; set; }
        public string? PHONENO { get; set; }
        public int? ApplicationUserRateId { get; set; }
        public decimal? Rate { get; set; }
        public int? SAC_Code { get; set; }
        public string? ADDRESS { get; set; }
        public DateTime? Date { get; set; }
        public int? Month { get; set; }
        public string? MonthName { get; set; }
        public int? Year { get; set; }
        public int? ActiveEmployee { get; set; }
        public int? ResignedEmployee { get; set; }
        public int? TotalEmployee { get; set; }
        public decimal? EmployeeValue { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? Amount { get; set; }

    }
}
