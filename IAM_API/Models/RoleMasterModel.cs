namespace IAM_API.Models
{
    public class RoleMasterModel
    {
        public int SerialNo { get; set; }
        public int TenantId { get; set; }
        public int UserTypeId { get; set; }
        public string? UserTypeCode { get; set; }
        public string? UserTypeName { get; set; }
        public string? UserTypeDescription { get; set; }
        public int LevelUpto { get; set; }
        public bool AutoInApproval { get; set; }
        //public int? ActiveFlag { get; set; }

        public Decimal? Hierarchy { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
