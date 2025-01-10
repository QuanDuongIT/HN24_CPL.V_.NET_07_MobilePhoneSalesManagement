﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Exceptions;
using ServerApp.BLL.Services;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Models;
using System;
namespace ServerApp.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SpecificationTypesController : ControllerBase
    {
        private readonly ISpecificationTypeService _specificationTypeService;

        public SpecificationTypesController(ISpecificationTypeService specificationTypeService)
        {
            _specificationTypeService = specificationTypeService;
        }

        [HttpGet("get-all-specificationTypes")]
        public async Task<ActionResult<IEnumerable<SpecificationTypeVm>>> GetSpecificationTypes()
        {
            var result = await _specificationTypeService.GetAllSpecificationTypeAsync();

            return Ok(result); // 200 OK nếu có dữ liệu.
        }

        [HttpGet("get-specificationType-by-id/{id}")]
        public async Task<ActionResult<SpecificationTypeVm>> GetSpecificationType(int id)
        {
            var result = await _specificationTypeService.GetBySpecificationTypeIdAsync(id);

            if (result == null)
            {
                return NotFound(new { Message = $"SpecificationType with ID {id} not found." }); // 404 Not Found nếu không tìm thấy.
            }

            return Ok(result); // 200 OK nếu tìm thấy.
        }

        [HttpPost("add-new-specificationType")]
        public async Task<ActionResult<SpecificationTypeVm>> PostSpecificationType(InputSpecificationTypeVm specificationTypeVm)
        {
            var result = await _specificationTypeService.AddSpecificationTypeAsync(specificationTypeVm);

            if (result == null)
            {
                return BadRequest(new { Message = "Failed to create the specificationType." }); // 400 Bad Request nếu không tạo được.
            }

            return CreatedAtAction(nameof(GetSpecificationType), new { id = result.SpecificationTypeId }, result); // 201 Created nếu tạo thành công.
        }

        [HttpPut("update-specificationType/{id}")]
        public async Task<IActionResult> PutSpecificationType(int id, InputSpecificationTypeVm specificationTypeVm)
        {
            var result = await _specificationTypeService.UpdateSpecificationTypeAsync(id, specificationTypeVm);
            if (result == null)
            {
                return NotFound(new { Message = $"SpecificationType with ID {id} not found." }); // 404 Not Found nếu không tìm thấy.
            }

            return Ok(result); // 200 OK nếu cập nhật thành công.
        }

        [HttpDelete("delete-specificationType-by-id/{id}")]
        public async Task<IActionResult> DeleteSpecificationType(int id)
        {
            var result = await _specificationTypeService.DeleteSpecificationTypeAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"SpecificationType with ID {id} not found." }); // 404 Not Found nếu không tìm thấy.
            }

            return NoContent(); // 204 No Content nếu xóa thành công.
        }
    }
}