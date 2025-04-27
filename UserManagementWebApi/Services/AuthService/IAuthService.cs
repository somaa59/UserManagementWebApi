using System.Security.Claims;
using UserManagementWebApi.DTO.Account;
using UserManagementWebApi.Entities;

namespace UserManagementWebApi.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(SignUpDto model);
        Task<AuthModel> LoginAsync(LoginDto model);
    }
}
