namespace IAM_API.Models
{
    public class Login
    {
        public int TenantId { get; set; } = 0;
        public int User_Key { get; set; } = 0;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public DateTime DateUpto { get; set; }
    public string? msg { get; set; }
    public int UserCatagoryId { get; set; } = 0;
    public int LockedStatus { get; set; } = 0;
        
    }
    public class TenantAuthSetting
    {
        public int TenantID { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Issuer { get; set; }

    }
    public class returnDetails
    {
        public string? returnMessage { get; set; }
        public string? clientId { get; set; }
        public string? encryptedClientsecret { get; set; }
        public string? authToken { get; set; }
    }
    public class TokenSettings
    {
        public string? RefreshTokenSecret { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
    public class encryptedPayload
    {
        public string? Data { get; set; }
    }
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ParentCompanyModel
    {
        public long TenantId { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
