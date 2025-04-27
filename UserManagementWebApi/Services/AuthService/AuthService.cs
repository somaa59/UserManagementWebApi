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
        private readonly RoleManager<IdentityRole> _roleManager;
        #endregion
        #region constructor
        public AuthService(UserManager<ApplicationUser> userManager ,IJwtService jwtService,RoleManager<IdentityRole> roleManager )
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
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
        public async Task<StateDto> AssignRoleAsync(AddRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return new StateDto { Flag = false, Message = " UserId or Role are Invalid" };
            if(await _userManager.IsInRoleAsync(user ,model.Role))
                return new StateDto { Flag = false, Message = " User already assignd to this Role" };

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                return new StateDto { Flag = false, Message = errors };
            }
            return new StateDto { Flag = true, Message = "Success" };
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
