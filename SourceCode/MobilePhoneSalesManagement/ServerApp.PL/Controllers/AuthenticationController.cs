using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;
using ServerApp.BLL.ViewModels.Authentication;
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
        public async Task<IActionResult> Register([FromBody] RegisterVm register)
        {
            return await _authenticationService.RegisterUserAsync(register);
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVm loginVm)
        {
            return await _authenticationService.LoginUserAsync(loginVm);
        }
    }
}
