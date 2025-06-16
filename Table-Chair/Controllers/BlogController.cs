using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.BlogDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Faqat ro‘yxatdan o‘tgan foydalanuvchilarga ruxsat
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        // ✅ GET: api/Blog - Barcha bloglarni olish
        // GET:https://localhost:7179/api/blog/getall
        [HttpGet("getall")]
        [AllowAnonymous] // Hamma kirishi mumkin
        [ProducesResponseType(typeof(IEnumerable<BlogDto>), 200)]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetAll()
        {
            var blogs = await _blogService.GetAllAsync();
            return Ok(blogs);
        }

        // ✅ GET: api/Blog/{id} - ID bo‘yicha blogni olish
        // GET:https://localhost:7179/api/blog/getbyid/id
        [HttpGet("getbyid/{id}")]
        [AllowAnonymous] // Hamma kirishi mumkin
        [ProducesResponseType(typeof(BlogDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BlogDto>> GetById(int id)
        {
            try
            {
                var blog = await _blogService.GetByIdAsync(id);
                return Ok(blog);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Blog with Id {id} not found.");
                return NotFound(new { message = ex.Message });
            }
        }

        // ✅ POST: api/Blog - Yangi blog qo‘shish (faqat Adminlar)
        // POST:https://localhost:7179/api/blog/create
        [HttpPost("create")]
        [Authorize(Roles = "Admin")] // Faqat Admin qo‘sha oladi
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult> Create([FromBody] BlogCreateDto blogCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _blogService.CreateAsync(blogCreateDto);
            return StatusCode(201); // Created
        }

        // ✅ PUT: api/Blog/{id} - Mavjud blogni yangilash (faqat Adminlar)
        // PUT:https://localhost:7179/api/blog/update
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")] // Faqat Admin yangilashi mumkin
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Update(int id, [FromBody] BlogUpdateDto blogUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _blogService.UpdateAsync(id, blogUpdateDto);
                return NoContent(); // 204
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Blog with Id {id} not found.");
                return NotFound(new { message = ex.Message });
            }
        }

        // ✅ DELETE: api/Blog/{id} - Blogni o‘chirish (faqat Adminlar)
        // DELETE:https://localhost:7179/api/blog/delete/id
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")] // Faqat Admin o‘chira oladi
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _blogService.DeleteAsync(id);
                return NoContent(); // 204
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Blog with Id {id} not found.");
                return NotFound(new { message = ex.Message });
            }
        }
    }
}

