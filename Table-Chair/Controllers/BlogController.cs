using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair.Examples;
using Table_Chair.Examples.BlogExample;
using Table_Chair_Application.Dtos.BlogDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        [HttpGet("getall")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BlogDto>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var blogs = await _blogService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<BlogDto>>.SuccessResponse(blogs));
        }

        [HttpGet("getbyid/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<BlogDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var blog = await _blogService.GetByIdAsync(id);
                return Ok(ApiResponse<BlogDto>.SuccessResponse(blog));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Blog with Id {Id} not found", id);
                return NotFound(ApiResponse<BlogDto>.Failure(ex.Message));
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        [SwaggerRequestExample(typeof(BlogCreateDto), typeof(BlogCreateDtoExample))]
        [SwaggerResponseExample(201, typeof(SuccessResponseExample))]
        [ProducesResponseType(typeof(ApiResponse<string>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] BlogCreateDto dto)
        {
            await _blogService.CreateAsync(dto);
            return StatusCode(201, ApiResponse<string>.SuccessResponse(string.Empty, "Blog muvaffaqiyatli yaratildi"));
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerRequestExample(typeof(BlogUpdateDto), typeof(BlogUpdateDtoExample))]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] BlogUpdateDto dto)
        {
            try
            {
                await _blogService.UpdateAsync(id, dto);
                return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Blog muvaffaqiyatli yangilandi"));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Blog with Id {Id} not found", id);
                return NotFound(ApiResponse<string>.Failure(ex.Message));
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _blogService.DeleteAsync(id);
                return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Blog muvaffaqiyatli o‘chirildi"));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Blog with Id {Id} not found", id);
                return NotFound(ApiResponse<string>.Failure(ex.Message));
            }
        }
    }
}

