using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserManagementWebApi.Helpers;
using System.Text;
using Microsoft.Extensions.Options;

namespace UserManagementWebApi.Services.AuthService
{
    public class JwtService : IJwtService
    {
        #region Fieldes 
        private readonly JWT _jwt;
        #endregion
        #region Constructor  
        public JwtService(IOptions<JWT> jwt)
        {
            _jwt = jwt.Value;
        }
        #endregion
        public async Task<JwtSecurityToken> GenerateTokenAsync(IEnumerable<Claim> claims)
        {
            var SignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            SigningCredentials signincred =
                new SigningCredentials(SignInKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _jwt.Issuer,
                    audience: _jwt.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(_jwt.DurationInMinutes),
                    signingCredentials: signincred
                );

            return token;
        }
    }
}
