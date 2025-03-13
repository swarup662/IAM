namespace IAM_UI.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class returnDetails
    {
        public string returndata { get; set; }
        public string? returnUserAuthorization { get; set; }
        public string? UniqueUserId { get; set; }
        public string? returnCategoryId { get; set; }

        public string? returnUserParentCompany { get; set; }
    }
    public class returndata
    {
        public string? returnMessage { get; set; }
        public string? authToken { get; set; }
        public string? refreshToken { get; set; }

    }
    public class TrustedHostsOptions
    {
        public List<string> TrustedHosts { get; set; }
    }

    public class ParentCompanyModel
    {
        public long TenantId { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
