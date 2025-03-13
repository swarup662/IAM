using IAM_API.Interface;
using IAM_API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json; 

namespace IAM_API.Repository
{
    public class TokenServiceRepo : ITokenService
    {
        private readonly string _refreshTokenSecret;
        private readonly int _refreshTokenExpirationDays;

        public TokenServiceRepo(IOptions<TokenSettings> tokenSettingsOptions)
        {
            var tokenSettings = tokenSettingsOptions.Value;
            _refreshTokenSecret = tokenSettings.RefreshTokenSecret;
            _refreshTokenExpirationDays = tokenSettings.RefreshTokenExpirationDays;
        }

        public string GenerateRefreshToken(string TenantId)
        {
            var refreshToken = Guid.NewGuid().ToString();
            var expirationDate = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

            var refreshTokenData = new
            {
                Token = refreshToken,
                TenantId = TenantId,
                ExpirationDate = expirationDate
            };

            return JsonConvert.SerializeObject(refreshTokenData);
        }

        public bool ValidateRefreshToken(string refreshToken,out string tenantId)
        {
            tenantId = null;
             

            dynamic tokenData = JsonConvert.DeserializeObject<dynamic>(refreshToken);

            if (tokenData == null)
            {
                return false;
            }

            var expirationDate = (DateTime)tokenData.ExpirationDate;
            tenantId = (string)tokenData.TenantId; 
            return expirationDate > DateTime.UtcNow;
        }
    }
}
