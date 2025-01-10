using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.BLL.Services;
using ServerApp.DAL.Models;

namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all-products")]
        public async Task<ActionResult<IEnumerable<ProductVm>>> GetProducts()
        {
            var result = await _productService.GetAllProductAsync();
            return Ok(result); // 200 OK nếu có dữ liệu.
        }
        [HttpPost("filter")]
        public async Task<ActionResult<IEnumerable<ProductVm>>> FilterProducts([FromBody] FilterRequest filterRequest)
        {
            try
            {
                var filteredProducts = await _productService.FilterProductsAsync(filterRequest);
                return Ok(filteredProducts);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpGet("get-product-by-id/{id}")]
        public async Task<ActionResult<ProductVm>> GetProduct(int id)
        {
            var result = await _productService.GetByProductIdAsync(id);

            if (result == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." }); // 404 Not Found nếu không tìm thấy.
            }

            return Ok(result); // 200 OK nếu tìm thấy.
        }
        [HttpPost("add-new-product")]
        public async Task<ActionResult<ProductVm>> PostProduct(InputProductVm productVm)
        {
            var result = await _productService.AddProductAsync(productVm);

            if (result == null)
            {
                return BadRequest(new { Message = "Failed to create the product." }); // 400 Bad Request nếu không tạo được.
            }

            return CreatedAtAction(nameof(GetProduct), new { id = result.ProductId }, result); // 201 Created nếu tạo thành công.
        }

        [HttpDelete("delete-product-by-id/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found." }); // 404 Not Found nếu không tìm thấy.
            }

            return NoContent(); // 204 No Content nếu xóa thành công.
        }
    }
}
