using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.ShippingAddressDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Microsoft.AspNetCore.Authorization;

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

        /// <summary>
        /// Yangi yetkazib berish manzili yaratish
        /// </summary>
        /// <param name="dto">Manzil ma'lumotlari</param>
        /// <returns>Natija</returns>
        [HttpPost("create")]
        [AllowAnonymous] // foydalanuvchi login qilmagan bo‘lishi mumkin
        public async Task<IActionResult> Create([FromBody] ShippingAddressCreateDto dto)
        {
            _logger.LogInformation("Yangi manzil qo‘shilmoqda");

            if (dto == null)
                return BadRequest("Invalid data.");

            await _shippingAddressService.CreateAsync(dto);
            return Ok(new { message = "Manzil muvaffaqiyatli qo‘shildi" });
        }

        /// <summary>
        /// Manzilni ID bo‘yicha o‘chirish
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Manzil o‘chirish so‘rovi. ID: {Id}", id);

            var result = await _shippingAddressService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = $"ID {id} bilan manzil topilmadi" });

            return NoContent();
        }

        /// <summary>
        /// ID bo‘yicha manzilni olish
        /// </summary>
        [HttpGet("getbyId/{id}")]
        [AllowAnonymous] // ochiq
        public async Task<IActionResult> GetById(int id)
        {
            
            _logger.LogInformation("ID: {Id} bilan manzilni olish", id);

            var result = await _shippingAddressService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Manzilni yangilash
        /// </summary>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingAddressUpdateDto dto)
        {
            _logger.LogInformation("ID: {Id} bilan manzil yangilash", id);

            if (dto == null)
                return BadRequest("Invalid data.");

            var result = await _shippingAddressService.UpdateAsync(id, dto);
            if (!result)
                return NotFound(new { message = $"ID {id} bilan manzil topilmadi" });

            return Ok(new { message = "Manzil muvaffaqiyatli yangilandi" });
        }
    }
}
