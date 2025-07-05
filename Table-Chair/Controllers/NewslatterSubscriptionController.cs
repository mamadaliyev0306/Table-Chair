using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NewsletterSubscriptionController : ControllerBase
{
    private readonly INewsletterSubscriptionService _service;

    public NewsletterSubscriptionController(INewsletterSubscriptionService service)
    {
        _service = service;
    }

    [HttpPost("create")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Yangiliklarga obuna bo‘lish")]
    [ProducesResponseType(typeof(ApiResponse<string>), 201)]
    public async Task<IActionResult> Add([FromBody] NewsletterSubscriptionCreateDto dto)
    {
        await _service.AddNewsletterSubscription(dto);
        return StatusCode(201, ApiResponse<string>.SuccessResponse("Obuna yaratildi."));
    }

    [HttpGet("getall")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Barcha obunalarni olish (admin)")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NewsletterSubscriptionDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<NewsletterSubscriptionDto>>.SuccessResponse(result));
    }

    [HttpGet("getbyId/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Obunani ID bo‘yicha olish")]
    [ProducesResponseType(typeof(ApiResponse<NewsletterSubscriptionDto>), 200)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ApiResponse<NewsletterSubscriptionDto>.SuccessResponse(result));
    }

    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Obunani yangilash")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Update([FromBody] NewsletterSubscriptionUpdateDto dto)
    {
        await _service.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Obunani o‘chirish (hard delete)")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> HardDelete(int id)
    {
        await _service.DeleteNewsletterSubscriptionAsync(id);
        return NoContent();
    }

    [HttpPatch("soft-delete/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Obunani soft delete qilish")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> SoftDelete(int id)
    {
        await _service.SoftDeleteNewsletterSubscriptionAsync(id);
        return NoContent();
    }

    [HttpPatch("restore/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Obunani tiklash (restore)")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Restore(int id)
    {
        await _service.RestoreNewsletterSubscriptionAsync(id);
        return NoContent();
    }
}


