using Microsoft.AspNetCore.Mvc;
using ServerApp.BLL.Services;
using ServerApp.BLL.Services.ViewModels;
namespace ServerApp.PL.Controllers
{
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
                var brand = await _brandService.GetAllBrandAsync();

                if (brand.Any())
                {
                    return StatusCode(200, brand);
                }

                return StatusCode(200, "No data");
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
                var brand = await _brandService.GetByBrandIdAsync(id);

                if (brand!=null)
                {
                    return StatusCode(200, brand);
                }

                return StatusCode(200, "No data");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }


        [HttpPost("add-new-brand")]
        public async Task<ActionResult<BrandVm>> PostBrand(AddBrandVm brandViewModel)
        {

            if (string.IsNullOrWhiteSpace(brandViewModel.Name))
            {
                return BadRequest("Title and Description cannot be empty.");
            }
            try
            {
                var result = await _brandService.AddBrandAsync(brandViewModel);

                if (result != null)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        //[HttpPut("update-brand/{id}")]
        //public async Task<IActionResult> PutBrand(int id, AddBrandVm brandViewModel)
        //{
        //    if (id == null)
        //    {
        //        return BadRequest("Invalid Brand ID.");
        //    }


        //    if (string.IsNullOrWhiteSpace(brandViewModel.Name))
        //    {
        //        return BadRequest("Title and Description cannot be empty.");
        //    }
        //    try
        //    {
        //        var result = await _brandService.UpdateBrandAsync(id,brandViewModel);

        //        if (result != null)
        //        {
        //            return Ok(result);
        //        }

        //        return BadRequest(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, $"Internal server error: {e.Message}");
        //    }
        //}

        [HttpDelete("delete-brand-by-id/{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                var Brand = await _brandService.GetByIdAsync(id);
                if (Brand == null)
                {
                    return BadRequest("Brand not found.");
                }

                await _brandService.DeleteAsync(Brand);

                return Ok(Brand);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
