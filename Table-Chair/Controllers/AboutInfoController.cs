using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos.AboutInfoDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AboutInfoController : ControllerBase
{
    private readonly IAboutInfoService _aboutInfoService;

    public AboutInfoController(IAboutInfoService aboutInfoService)
    {
        _aboutInfoService = aboutInfoService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AboutInfoDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _aboutInfoService.GetAllAsync();
        return Ok(result); 
    }

    // GET: api/AboutInfo/{id}
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AboutInfoDto>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _aboutInfoService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ValidationErrorResponse), 400)]
    [SwaggerRequestExample(typeof(AboutInfoCreateDto), typeof(AboutInfoCreateDtoExample))]
    public async Task<IActionResult> Create([FromBody] AboutInfoCreateDto dto)
    {

        await _aboutInfoService.CreateAsync(dto);
        return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "AboutInfo successfully created!"));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ValidationErrorResponse), 400)]
    public async Task<IActionResult> Update(int id, [FromBody] AboutInfoUpdateDto dto)
    {
        await _aboutInfoService.UpdateAsync(id, dto);
        return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "AboutInfo successfully updated!"));
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> Delete(int id)
    {
        await _aboutInfoService.DeleteAsync(id);
        return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "AboutInfo successfully deleted!"));
    }
}


