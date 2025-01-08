using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;
using ServerApp.BLL.Services.ViewModels;

namespace ServerApp.PL.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("get-all-brands")]
        public async Task<ActionResult<IEnumerable<BrandVm>>> GetBrands()
        {
            try
            {
                return Ok(await _brandService.GetAllAsync());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }


        [HttpGet("get-brand-by-id/{id}")]
        public async Task<ActionResult<BrandVm>> GetBrand(int id)
        {
            try
            {

                return Ok(await _brandService.GetByBrandIdAsync(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }


        [HttpPost("add-new-brand")]
        public async Task<ActionResult<BrandVm>> PostBrand(BrandVm brandViewModel)
        {
            if (string.IsNullOrWhiteSpace(brandViewModel.Name))
            {
                return BadRequest("Title and Description cannot be empty.");
            }

            try
            {
                _brandService.AddBrandAsync(brandViewModel);
                return Ok(brandViewModel);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpPut("update-brand/{id}")]
        public async Task<IActionResult> PutBrand(int id, BrandVm brandViewModel)
        {
            if (id == null)
            {
                return BadRequest("Invalid Brand ID.");
            }

            try
            {
                await _brandService.UpdateBrandAsync(id, brandViewModel);
                return Ok(brandViewModel);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        [HttpDelete("delete-brand-by-id/{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                if (_brandService.DeleteByIdAsync(id))
                    return BadRequest();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

    }
}
