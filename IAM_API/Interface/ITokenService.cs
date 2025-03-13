using IAM_API.Models;

namespace IAM_API.Interface
{
    public interface ITokenService
    {
       // string GenerateAccessToken(Login login, TenantAuthSetting tenantAuthSetting);
        string GenerateRefreshToken( string Id);
        bool ValidateRefreshToken(string refreshToken,out string Id);
    }
}
