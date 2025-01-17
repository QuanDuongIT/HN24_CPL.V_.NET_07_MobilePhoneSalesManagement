using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;
using ServerApp.BLL.Services.ViewModels;
using System.Security.Claims;

namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        // Tạo đơn hàng mới
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CustomerInfoVm customerInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy người dùng." });
                }
                var createdOrder = await _orderService.CreateOrderAsync(int.Parse(userId), customerInfo);
                return Ok(new { success = true, message = "Tạo đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, new { message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        // Lấy thông tin đơn hàng theo ID
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Order not found" });
            }
        }

        // Cập nhật trạng thái đơn hàng
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string status)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, status);
                return Ok(new { message = "Order status updated successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Order not found" });
            }
        }
    }
}
