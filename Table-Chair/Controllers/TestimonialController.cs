using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonialController : ControllerBase
    {
        private readonly ITestimonialService _testimonialService;

        public TestimonialController(ITestimonialService testimonialService)
        {
            _testimonialService = testimonialService;
        }

        /// <summary>
        /// Barcha testimonial (fikrlar) ro‘yxatini olish
        /// </summary>
        [HttpGet("getall")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<TestimonialDto>>>> GetAll()
        {
            var result = await _testimonialService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<TestimonialDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// ID bo‘yicha testimonial olish
        /// </summary>
        [HttpGet("getbyId/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<TestimonialDto>>> GetById(int id)
        {
            var result = await _testimonialService.GetByIdAsync(id);
            return Ok(ApiResponse<TestimonialDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Yangi testimonial qo‘shish
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerRequestExample(typeof(CreateTestimonialDto), typeof(CreateTestimonialDtoExample))]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] CreateTestimonialDto dto)
        {
            await _testimonialService.CreateAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Fikr muvaffaqiyatli qo‘shildi."));
        }

        /// <summary>
        /// Testimonialni yangilash
        /// </summary>
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerRequestExample(typeof(UpdateTestimonialDto), typeof(UpdateTestimonialDtoExample))]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] UpdateTestimonialDto dto)
        {

            await _testimonialService.UpdateAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Fikr muvaffaqiyatli yangilandi."));
        }

        /// <summary>
        /// Testimonialni o‘chirish (qattiq delete)
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            await _testimonialService.DeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Fikr muvaffaqiyatli o‘chirildi."));
        }

        /// <summary>
        /// Testimonialni soft delete qilish
        /// </summary>
        [HttpPatch("softdelete/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<string>>> SoftDelete(int id)
        {
            await _testimonialService.SoftDeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Fikr soft delete qilindi."));
        }

        /// <summary>
        /// Soft delete qilingan testimonialni tiklash
        /// </summary>
        [HttpPatch("restore/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<string>>> Restore(int id)
        {
            await _testimonialService.RestoreAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Fikr muvaffaqiyatli tiklandi."));
        }
    }
}

