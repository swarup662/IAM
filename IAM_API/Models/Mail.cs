namespace IAM_API.Models
{
  

    public class Mail
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

        public MailUserMaster? MailUserMaster { get; set; }
        public List<ReceiverEmail>? ReceiverEmail { get; set; }

    }


    public class MailUserMaster
    {
        public int? USER_MASTER_KEY { get; set; }
        public string? Email_ID { get; set; }

        public string? Username { get; set; }
        public string? UserPassword { get; set; }

        public string? UserMailTypeCode { get; set; }
        public string? FullName { get; set; }
        public string? Pin { get; set; }

    }


    public class ReceiverEmail
    {
        public int? TenantMailSetupKey { get; set; }
        public int? TenantMailSetupDtlsKey { get; set; }
        public string? ReceiverMail { get; set; }

    }


    public class TenantAppProfile
    {
        public int TenantID { get; set; }
        public int ApplicationId { get; set; }
        public string? EmailId { get; set; }
        public string? Password { get; set; }
    }

    public class OldPassword
    {
        public string? Email_ID { get; set; }
        public string? User_Name { get; set; }
        public int USER_MASTER_KEY { get; set; }
        public string OldPass { get; set; }

    }

    public class EmailTimeLimit
    {
        public string? EmailSetUpDtls_Key { get; set; }
        public string? timeslotMinute { get; set; }
        public int TenantID { get; set; }
    }

    public class ChangePass
    {
        public int? TenantId { get; set; }
        public int? User_Master_Key { get; set; }
        public string? New_Password { get; set; }
        public string? Old_Password { get; set; }
        public string? Confirm_Password { get; set; }
        public int CreatedBy { get; set; }  
    }

    public class Link
    {
        public int id { get; set; }
    }

    public class EmailBody
    {
        //  public string? MessageBody { get; set; }
        public int MessageBodyID { get; set; }

    }

    public class EmailId
    {
        public int? EmailID { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

    }

    public class ApiTriggerMailModel
    {
        public int TenantId { get; set; }
        public int ApplicationId { get; set; }
        public int CompanyId { get; set; }
        public int ApiTriggerMailKey { get; set; }
        public int PurposeId { get; set; }
        public string? PurposeName { get; set; }
        public string? SenderMail { get; set; }
        public string? SenderPassword { get; set; }
        public string? ReceiverMail { get; set; }
    }

   
}
