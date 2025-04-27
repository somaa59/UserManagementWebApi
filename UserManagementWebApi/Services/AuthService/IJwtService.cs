using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UserManagementWebApi.Services.AuthService
{
    public interface IJwtService
    {
        Task<JwtSecurityToken> GenerateTokenAsync(IEnumerable<Claim> claims);
    }
}
