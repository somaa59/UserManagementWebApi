using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagementWebApi.DTO;
using UserManagementWebApi.DTO.Account;
using UserManagementWebApi.DTO.Account.Enum;
using UserManagementWebApi.Entities;
using UserManagementWebApi.Services.AuthService;

namespace UserManagementWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Fieldes 
        private readonly IAuthService _authService;
        #endregion
        #region constructor
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        #endregion
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] SignUpDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if(!result.State.Flag)
                return BadRequest(result.State.Message);

            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);
            if(!result.State.Flag)
                return BadRequest(result.State.Message);

            return Ok(result);
        }
    
    }
}
