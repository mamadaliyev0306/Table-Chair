using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.ShippingAddressDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Microsoft.AspNetCore.Authorization;
using Table_Chair_Application.Responses;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingAddressController : ControllerBase
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly ILogger<ShippingAddressController> _logger;

        public ShippingAddressController(
            IShippingAddressService shippingAddressService,
            ILogger<ShippingAddressController> logger)
        {
            _shippingAddressService = shippingAddressService;
            _logger = logger;
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] ShippingAddressCreateDto dto)
        {
            _logger.LogInformation("Yangi manzil qo‘shilmoqda");

            if (dto == null)
                return BadRequest(ApiResponse<string>.FailResponse("Invalid data."));

            await _shippingAddressService.CreateAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Manzil muvaffaqiyatli qo‘shildi"));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Manzil o‘chirish so‘rovi. ID: {Id}", id);

            var result = await _shippingAddressService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<string>.FailResponse($"ID {id} bilan manzil topilmadi"));

            return NoContent(); // Success bo'lsa, hech narsa qaytarilmaydi
        }

        [HttpGet("getbyId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("ID: {Id} bilan manzilni olish", id);

            var result = await _shippingAddressService.GetByIdAsync(id);
            return Ok(ApiResponse<ShippingAddressDto>.SuccessResponse(result));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingAddressUpdateDto dto)
        {
            _logger.LogInformation("ID: {Id} bilan manzil yangilash", id);

            if (dto == null)
                return BadRequest(ApiResponse<string>.FailResponse("Invalid data."));

            var result = await _shippingAddressService.UpdateAsync(id, dto);
            if (!result)
                return NotFound(ApiResponse<string>.FailResponse($"ID {id} bilan manzil topilmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Manzil muvaffaqiyatli yangilandi"));
        }
    }

}
