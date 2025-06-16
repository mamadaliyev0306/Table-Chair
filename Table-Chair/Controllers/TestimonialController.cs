using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
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

        // GET:  https://localhost:7179/api/testimonial/getall
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<TestimonialDto>>> GetAll()
        {
            try
            {
                var result = await _testimonialService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET:  https://localhost:7179/api/testimonial/getbyId/id
        [HttpGet("getbyId/{id}")]
        public async Task<ActionResult<TestimonialDto>> GetById(int id)
        {
            try
            {
                var result = await _testimonialService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound($"Testimonial with ID {id} not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST:  https://localhost:7179/api/testimonial/create
        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] CreateTestimonialDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Testimonial data is null.");
                }

                await _testimonialService.CreateAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT:  https://localhost:7179/api/testimonial/update/id
        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateTestimonialDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Testimonial data is null.");
                }

                await _testimonialService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: https://localhost:7179/api/testimonial/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _testimonialService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //DELETE : https://localhost:7179/api/testimonial/softdelete/id
        [HttpPatch("softdelete/{id}")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            try
            {
                await _testimonialService.SoftDeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PATCH:  https://localhost:7179/api/testimonial/restore/id
        [HttpPatch("restore/{id}")]
        public async Task<ActionResult> Restore(int id)
        {
            try
            {
                await _testimonialService.RestoreAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

