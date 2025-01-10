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
            try
            {
                var productsWithSpecifications = await _productService.GetAllProductAsync();

                if(productsWithSpecifications.Any())
                {
                    return StatusCode(200, productsWithSpecifications);
                }

                return StatusCode(200, "No data");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
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



    }
}
