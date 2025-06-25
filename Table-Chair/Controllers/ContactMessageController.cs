using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Table_Chair.Examples;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ContactMessageController : ControllerBase
    {
        private readonly IContactMessageService _contactMessageService;
        private readonly ILogger<ContactMessageController> _logger;

        public ContactMessageController(IContactMessageService contactMessageService, ILogger<ContactMessageController> logger)
        {
            _contactMessageService = contactMessageService;
            _logger = logger;
        }

        // ✅ GET: api/contactmessage/getall
        [HttpGet("getall")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Barcha kontakt xabarlarini olish (faqat Admin)")]
        [ProducesResponseType(typeof(ApiResponse<List<ContactMessageDto>>), 200)]
        [SwaggerResponseExample(200, typeof(ContactMessageListExample))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _contactMessageService.GetAllAsync();
            return Ok(ApiResponse<List<ContactMessageDto>>.SuccessResponse(result));
        }

        [HttpGet("getbyid/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Xabar ID bo‘yicha olish (faqat Admin)")]
        [ProducesResponseType(typeof(ApiResponse<ContactMessageDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [SwaggerResponseExample(200, typeof(ContactMessageExample))]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _contactMessageService.GetByIdAsync(id);
            return Ok(ApiResponse<ContactMessageDto?>.SuccessResponse(result));
        }
        [HttpPost("create")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Yangi contact xabari jo‘natish (login talab qilinmaydi)")]
        [ProducesResponseType(typeof(ApiResponse<string>), 201)]
        [SwaggerResponseExample(201, typeof(SuccessMessageExample))]
        public async Task<IActionResult> Create([FromBody] ContactMessageCreateDto dto)
        {
            await _contactMessageService.CreateAsync(dto);
            _logger.LogInformation("Yangi kontakt xabari yaratildi.");
            return StatusCode(201, ApiResponse<string>.SuccessResponse("Xabaringiz qabul qilindi"));
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Contact xabarini o‘chirish (faqat Admin)")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Delete(int id)
        {
            await _contactMessageService.DeleteAsync(id);
            _logger.LogInformation("Contact message deleted. Id: {Id}", id);
            return NoContent();
        }
    }
}
