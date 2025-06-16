using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize] // 🔒 faqat token bilan kirish mumkin
    public class ContactMessageController : ControllerBase
    {
        private readonly IContactMessageService _contactMessageService;
        private readonly ILogger<ContactMessageController> _logger;

        public ContactMessageController(IContactMessageService contactMessageService, ILogger<ContactMessageController> logger)
        {
            _contactMessageService = contactMessageService;
            _logger = logger;
        }

        // GET: api/ContactMessage
        // GET: https://localhost:7179/api/contactmessage/getall
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _contactMessageService.GetAllAsync();
            return Ok(result);
        }
        // GET: https://localhost:7179/api/contactmessage/getbyId/id
        // GET: api/ContactMessage/5
        [HttpGet("getbyId/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _contactMessageService.GetByIdAsync(id);
            return Ok(result);
        }
        // POST: https://localhost:7179/api/contactmessage/create
        // POST: api/ContactMessage
        [HttpPost("create")]
        [AllowAnonymous] // ❗ Manashu joyda AllowAnonymous qo'ydim, Contact yozishi uchun login bo'lishi shart emas
        public async Task<IActionResult> Create([FromBody] ContactMessageCreateDto dto)
        {
            await _contactMessageService.CreateAsync(dto);
            _logger.LogInformation("New contact message created.");
            return StatusCode(201); // Created
        }
        // DELETE: https://localhost:7179/api/contactmessage/delete/id
        // DELETE: api/ContactMessage/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contactMessageService.DeleteAsync(id);
            _logger.LogInformation("Contact message deleted. Id: {Id}", id);
            return NoContent();
        }
    }
}
