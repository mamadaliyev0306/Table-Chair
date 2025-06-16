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
    public class SliderController : ControllerBase
    {
        private readonly ISliderService _sliderService;

        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        // POST: https://localhost:7179/api/slider/create
        [HttpPost("create")]
        public async Task<IActionResult> AddSliderAsync([FromBody] CreateSliderDto sliderDto)
        {
            if (sliderDto == null)
            {
                return BadRequest("Slider data is required.");
            }

            try
            {
                await _sliderService.AddSliderAsync(sliderDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: https://localhost:7179/api/slider/getbyId/id
        [HttpGet("getbyId/{id}")]
        public async Task<IActionResult> GetSliderByIdAsync(int id)
        {
            try
            {
                var slider = await _sliderService.GetSliderByIdAsync(id);
                return Ok(slider);
            }
            catch (Exception ex)
            {
                return NotFound($"Slider with Id {id} not found. Error: {ex.Message}");
            }
        }

        // GET: https://localhost:7179/api/slider/getall
        [HttpGet("getall")]
        public async Task<IActionResult> GetSliderListAsync()
        {
            try
            {
                var sliders = await _sliderService.GetSliderListAsync();
                return Ok(sliders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: https://localhost:7179/api/slider/update/id
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateSliderAsync(int id, [FromBody] SliderDto sliderDto)
        {
            if (sliderDto == null || sliderDto.Id != id)
            {
                return BadRequest("Invalid slider data.");
            }

            try
            {
                await _sliderService.UpdateSliderAsync(sliderDto);
                return NoContent(); // Successfully updated
            }
            catch (Exception ex)
            {
                return NotFound($"Slider with Id {id} not found. Error: {ex.Message}");
            }
        }

        // Delete : https://localhost:7179/api/slider/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSliderAsync(int id)
        {
            try
            {
                await _sliderService.DeleteSliderAsync(id);
                return NoContent(); // Successfully deleted
            }
            catch (Exception ex)
            {
                return NotFound($"Slider with Id {id} not found. Error: {ex.Message}");
            }
        }
    }
}

