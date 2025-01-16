using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;
using System.Security.Claims;

namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserService _userService;

        public CartsController(ICartService cartService, IUserService userService)
        {
            _cartService = cartService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập"
                    });
                }

                var user = await _userService.GetByUserIdAsync(int.Parse(userId));
                var cartItems = await _cartService.GetCartItemsAsync(int.Parse(userId));
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy giỏ hàng.", error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCart([FromQuery] int productId, [FromQuery] int quantity)
        {

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập"
                    });
                }

                var user = await _userService.GetByUserIdAsync(int.Parse(userId));
                var result = await _cartService.UpdateCartAsync(int.Parse(userId), productId, quantity);

                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật giỏ hàng.", error = ex.Message });
            }
        }
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập"
                    });
                }

                var user = await _userService.GetByUserIdAsync(int.Parse(userId));
                var result = await _cartService.RemoveFromCartAsync(int.Parse(userId), productId);

                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa sản phẩm.", error = ex.Message });
            }
        }
    }
}
