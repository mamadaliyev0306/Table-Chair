using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SliderController : ControllerBase
    {
        private readonly ISliderService _sliderService;

        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        /// <summary>
        /// Yangi slider qo‘shish
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerRequestExample(typeof(CreateSliderDto), typeof(SliderCreateDtoExample))]
        public async Task<IActionResult> AddSliderAsync([FromBody] CreateSliderDto sliderDto)
        {
            await _sliderService.AddSliderAsync(sliderDto);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Slider muvaffaqiyatli qo‘shildi."));
        }

        /// <summary>
        /// Sliderni ID bo‘yicha olish
        /// </summary>
        [HttpGet("getbyId/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSliderByIdAsync(int id)
        {
            var slider = await _sliderService.GetSliderByIdAsync(id);
            return Ok(ApiResponse<SliderDto>.SuccessResponse(slider));
        }

        /// <summary>
        /// Barcha sliderlarni olish
        /// </summary>
        [HttpGet("getall")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSliderListAsync()
        {
            var sliders = await _sliderService.GetSliderListAsync();
            return Ok(ApiResponse<IEnumerable<SliderDto>>.SuccessResponse(sliders));
        }

        /// <summary>
        /// Sliderni yangilash
        /// </summary>
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [SwaggerRequestExample(typeof(SliderUpdateDto), typeof(SliderUpdateDtoExample))]
        public async Task<IActionResult> UpdateSliderAsync([FromBody] SliderUpdateDto sliderDto)
        {
            await _sliderService.UpdateSliderAsync(sliderDto);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Slider muvaffaqiyatli yangilandi."));
        }

        /// <summary>
        /// Sliderni o‘chirish
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteSliderAsync(int id)
        {
            await _sliderService.DeleteSliderAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Slider muvaffaqiyatli o‘chirildi."));
        }
    }
}


