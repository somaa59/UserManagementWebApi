using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using UserManagementWebApi.Data;
using UserManagementWebApi.DTO;
using UserManagementWebApi.DTO.Account;
using UserManagementWebApi.DTO.Account.Enum;
using UserManagementWebApi.Entities;

namespace UserManagementWebApi.Services.AuthService
{
    public class AuthService : IAuthService
    {
        #region Fieldes 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        #endregion
        #region constructor
        public AuthService(UserManager<ApplicationUser> userManager ,IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }
        #endregion
        public async Task<AuthModel> RegisterAsync(SignUpDto model)
        {
            var state = new StateDto();
            if (model.UserName is null)
                model.UserName = new MailAddress(model.Email).User;

            if (await _userManager.FindByEmailAsync(model.Email) is not null )
            {
                state.Message = "Email is already registered ";
                return new AuthModel() { State =state };
            }
            if (await _userManager.FindByNameAsync(model.UserName) is not null )
            {
                state.Message = "UserName is already registered ";
                return new AuthModel() { State =state };
            }
            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.UserName
            };
            var result =await _userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded) {
                var errors = string.Join(",",result.Errors.Select(e=>e.Description));
                state.Message = errors;
                return new AuthModel() { State = state };
            }
            //assign to user role by default  
            await _userManager.AddToRoleAsync(user,Roles.User.ToString());
            //create claims
            var claims = await GetUserClaimsAsync(user);
            //Generate Token
            var jwtSecurityToken = await _jwtService.GenerateTokenAsync(claims);

            state.Message = "Register Success";
            state.Flag = true;
            return new AuthModel() {
                State =state,
                UserName = user.UserName,
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                Roles =new List<string> { Roles.User.ToString() },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }
        public async Task<AuthModel> LoginAsync(LoginDto model)
        {
            var authModel =new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
               authModel.State = new StateDto {Flag=false, Message = "Email Or Password is incorrect" };
                return authModel;
            }
            //create claims
            var claims = await GetUserClaimsAsync(user);
            //Generate Token
            var jwtSecurityToken = await _jwtService.GenerateTokenAsync(claims);

            authModel.State = new StateDto { Flag = true, Message = "Login Success" };
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            return authModel;
        }
        #region private Section
        private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            var UserClaims =await _userManager.GetClaimsAsync(user);
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(UserClaims);
            return claims;
        }
        #endregion
    }
}
