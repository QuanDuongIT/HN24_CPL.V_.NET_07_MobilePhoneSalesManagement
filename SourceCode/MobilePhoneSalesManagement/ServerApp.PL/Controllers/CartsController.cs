using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.BLL.Services;
using ServerApp.DAL.Models;
using ServerApp.BLL.Services.InterfaceServices;

namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Lấy tất cả người dùng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetAllCarts()
        {
            var carts = await _cartService.GetAllAsync();
            if (carts == null || !carts.Any())
            {
                return NotFound("No carts found.");
            }
            return Ok(carts);
        }

        // Lấy người dùng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCartById(int id)
        {
            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null)
            {
                return NotFound($"Cart with ID {id} not found.");
            }
            return Ok(cart);
        }

        // Thêm người dùng mới
        [HttpPost]
        public async Task<ActionResult<Cart>> AddCart([FromBody] CartVm cart)
        {
            if (cart == null)
            {
                return BadRequest("Cart data is null.");
            }

            var result = await _cartService.AddCartAsync(cart);
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest("Error while adding cart.");
        }

        // Cập nhật thông tin người dùng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, CartVm cartVm)
        {
            if (cartVm == null)
            {
                return BadRequest("Car data is invalid.");
            }

            var updated = await _cartService.UpdateCartAsync(id, cartVm);
            if (updated)
            {
                return NotFound($"Car with ID {id} not found.");
            }

            return NoContent();
        }
        // Xóa người dùng theo ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCart(int id)
        {
            var deleted = await _cartService.DeleteAsync(id);
            if (deleted == 0)
            {
                return NotFound($"Car with ID {id} not found.");
            }

            return NoContent();  // Trả về 204 khi xóa thành công
        }
    }
}
