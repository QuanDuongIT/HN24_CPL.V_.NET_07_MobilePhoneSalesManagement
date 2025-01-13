using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.BLL.ViewModels.Authentication;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.PL.ViewModels.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServerApp.BLL.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceResult> RegisterUserAsync(UserVm register);
        Task<IActionResult> LoginUserAsync(LoginVm loginVm);
        Task<AuthResultVm> GenerateJwtToken(User user);
        Task<ServiceResult> VerifyEmail(string email, string code);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IUserDetailsService _userDetailsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork, IConfiguration configuration,
            ICacheService cacheService, IUserService userService, IEmailService emailService, IUserDetailsService userDetailsService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _cacheService = cacheService;
            _userService = userService;
            _emailService = emailService;
            _userDetailsService = userDetailsService;
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetCurrentBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            return "http://localhost:4200";
            //return $"{request.Scheme}://{request.Host}";
        }

        public async Task<ServiceResult> RegisterUserAsync(UserVm register)
        {
            var baseUrl = GetCurrentBaseUrl();
            try
            {
                var userExists = await _userManager.FindByEmailAsync(register.Email);

                if (userExists != null)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Email đã tồn tại."
                    };
                }

                var newUser = new User()
                {
                    Email = register.Email,
                    UserName = register.Email,
                    Status = false,
                };

                var result = await _userManager.CreateAsync(newUser, register.PasswordHash);

                if (result.Succeeded)
                {
                    await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        var confirmationCode = GenerateConfirmationCode();
                        // Thêm thông tin chi tiết người dùng
                        await _userDetailsService.AddUserDetailsAsync(newUser.UserId, register);

                        await _cacheService.SetAsync($"EmailVerification:{register.Email}", confirmationCode, TimeSpan.FromMinutes(15));

                        // Gửi email xác nhận
                        var verificationUrl = $"{baseUrl}/verify-email?email={register.Email}&code={confirmationCode}";
                        await _emailService.SendAsync(register.Email, "Verify your email", $"Click <a href='{verificationUrl}'>here</a> to verify your email.");

                        // Commit transaction nếu tất cả thao tác thành công
                        await _unitOfWork.CommitAsync();

                        return new ServiceResult
                        {
                            Success = true,
                            Message = "Đã gửi mã về email."
                        };
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction nếu có lỗi
                        await _unitOfWork.RollbackAsync();

                        return new ServiceResult
                        {
                            Success = false,
                            Message = "Có lỗi xảy ra trong quá trình xử lý: " + ex.Message
                        };
                    }
                }

                return new ServiceResult
                {
                    Success = false,
                    Message = "Tạo tài khoản thất bại."
                };
            }
            catch (Exception ex)
            {
                // Log the exception for further investigation
                return new ServiceResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<IActionResult> LoginUserAsync(LoginVm loginVm)
        {
            var user = await _userManager.FindByEmailAsync(loginVm.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginVm.Password))
            {
                var token = await GenerateJwtToken(user);
                return new OkObjectResult(token);
            }

            return new UnauthorizedResult();
        }

        public async Task<AuthResultVm> GenerateJwtToken(User user)
        {
            var authClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(5),
                claims: authClaim,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResultVm()
            {
                Token = jwtToken,
                ExpiresAt = token.ValidTo
            };
        }

        private string GenerateConfirmationCode()
        {
            var random = new Random();
            var code = random.Next(100000, 999999); // Tạo mã 6 chữ số
            return code.ToString();
        }


        public async Task<ServiceResult> VerifyEmail(string email, string code)
        {
            try
            {
                // Kiểm tra mã xác minh trong cache
                var cachedCode = await _cacheService.GetAsync<string>($"EmailVerification:{email}");
                if (cachedCode == null || cachedCode != code)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Xác minh tài khoản thất bại."
                    };
                }

                // Xác minh thành công
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    user.Status = true;
                    await _userManager.UpdateAsync(user);
                }

                // Xóa mã khỏi cache
                await _cacheService.RemoveAsync($"EmailVerification:{email}");

                // Chuyển hướng đến trang login với domain hiện tại
                return new ServiceResult
                {
                    Success = true,
                    Message = "Xác minh tài khoản thành công."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

    }
}
