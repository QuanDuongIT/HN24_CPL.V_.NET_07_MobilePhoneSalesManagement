using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.PL.ViewModels.Authentication;

namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register-user")]
        public async Task<ServiceResult> Register([FromBody] UserVm register)
        {
            return await _authenticationService.RegisterUserAsync(register);
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVm loginVm)
        {
            return await _authenticationService.LoginUserAsync(loginVm);
        }

        [HttpGet("verify-email")]
        public async Task<ServiceResult> VerifyUserRegister([FromQuery] string email, [FromQuery] string code)
        {
            // Gọi service để xác minh mã xác nhận
            return await _authenticationService.VerifyEmail(email, code);
        }
    }
}
