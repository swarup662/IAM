namespace IAM_API.Models
{
    public class MailSetupModel
    {
        public int? TenantMailSetupKey { get; set; }
        public int? Tenant_ID { get; set; }
        public int? Application_ID { get; set; }
        public int? PurposeApplicationId { get; set; }
        public string? ApplicationName { get; set; }
        public int? CompanyId { get; set; }
        public int? TenantMailSetupPurposeKey { get; set; }
        public string? PurposeName { get; set; }
        public string? PurposeDescription { get; set; }
        public int? Created_By { get; set; }
        public string? EditAllUser { get; set; }
        public int? PurposeId { get; set; }
        public string? SenderMail { get; set; }
        public string? SenderPassword { get; set; }
        public int? TenantMailSetupDtlsKey { get; set; }
        public string? ReceiverMail { get; set; }
        public List<MailTypeData>? Maildata { get; set; }

        // **Newly Added Fields**
        public string? MailSubject { get; set; }
        public string? MailBody { get; set; }
        public int? IsCC { get; set; }
        public int? IsAttachment { get; set; }
        public string? PageLink { get; set; }
        public string? Parameter1 { get; set; }
        public string? Parameter2 { get; set; }
        public string? Parameter3 { get; set; }
        public int? TimeSlotMinute { get; set; }
       
    }





    public class MailTypeData
    {
        public string? ReceiverMail { get; set; }
    }
}
