using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.ShippingAddressDtos;
using Table_Chair_Application.Services;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressService _shippingAddressService;

        public ShippingAddressController(IShippingAddressService shippingAddressService)
        {
            _shippingAddressService = shippingAddressService;
        }

        // POST: https://localhost:7179/api/shippingaddress/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ShippingAddressCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                await _shippingAddressService.CreateAsync(dto);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: https://localhost:7179/api/shippingaddress/delete
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _shippingAddressService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Shipping address with ID {id} not found.");
                }
                return NoContent(); // Successfully deleted
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: https://localhost:7179/api/shippingaddress/getbyId/id
        [HttpGet("getbyId/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _shippingAddressService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT:https://localhost:7179/api/shippingaddress/update/id
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingAddressUpdateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var result = await _shippingAddressService.UpdateAsync(id, dto);
                if (!result)
                {
                    return NotFound($"Shipping address with ID {id} not found.");
                }
                return NoContent(); // Successfully updated
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

