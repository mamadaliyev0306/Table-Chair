using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FaqController : ControllerBase
{
    private readonly IFaqService _faqService;

    public FaqController(IFaqService faqService)
    {
        _faqService = faqService;
    }
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Yangi FAQ yaratish (Admin only)")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> Create([FromBody] FaqCreateDto dto)
    {
        await _faqService.CreateAsync(dto);
        return Ok(ApiResponse<string>.SuccessResponse("FAQ muvaffaqiyatli yaratildi"));
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "FAQ yangilash (Admin only)")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> Update(int id, [FromBody] FaqUpdateDto dto)
    {
        await _faqService.UpdateAsync(id, dto);
        return Ok(ApiResponse<string>.SuccessResponse("FAQ yangilandi"));
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "FAQ’ni o‘chirish (Admin only)")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> Delete(int id)
    {
        await _faqService.DeleteAsync(id);
        return Ok(ApiResponse<string>.SuccessResponse("FAQ o‘chirildi"));
    }
    [HttpGet("getbyId/{id}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "ID bo‘yicha FAQ olish")]
    [ProducesResponseType(typeof(ApiResponse<FaqDto>), 200)]
    public async Task<IActionResult> GetById(int id)
    {
        var faq = await _faqService.GetByIdAsync(id);
        return Ok(ApiResponse<FaqDto>.SuccessResponse(faq));
    }

    [HttpGet("getall")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Barcha FAQ’larni olish (Admin)")]
    [ProducesResponseType(typeof(ApiResponse<List<FaqDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int? page = null, [FromQuery] int? pageSize = null)
    {
        var list = await _faqService.GetAllAsync(page:page,pageSize: pageSize);
        return Ok(ApiResponse<List<FaqDto>>.SuccessResponse(list.ToList()));
    }
    [HttpGet("active")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Aktiv FAQ’larni olish")]
    [ProducesResponseType(typeof(ApiResponse<List<FaqDto>>), 200)]
    public async Task<IActionResult> GetActive()
    {
        var list = await _faqService.GetActiveAsync();
        return Ok(ApiResponse<List<FaqDto>>.SuccessResponse(list.ToList()));
    }
    [HttpGet("count")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Jami FAQ sonini olish (Admin only)")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<IActionResult> Count()
    {
        var count = await _faqService.CountAsync();
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpGet("exists")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Ma’lum maydon bo‘yicha mavjudligini tekshirish")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> Exists([FromQuery] string field, [FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(value))
            return BadRequest(ApiResponse<string>.FailResponse("Field va value bo‘sh bo‘lmasligi kerak"));

        var exists = await _faqService.ExistsAsync(f => EF.Property<string>(f, field) == value);
        return Ok(ApiResponse<bool>.SuccessResponse(exists));
    }

    [HttpPatch("soft-delete/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "FAQ’ni soft-delete qilish (Admin only)")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> SoftDelete(int id)
    {
        await _faqService.SoftDeleteAsync(id);
        return Ok(ApiResponse<string>.SuccessResponse("Soft delete qilindi"));
    }

    [HttpPatch("restore/{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "FAQ’ni tiklash (Admin only)")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> Restore(int id)
    {
        await _faqService.RestoreAsync(id);
        return Ok(ApiResponse<string>.SuccessResponse("Tiklandi"));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "FAQ bo‘yicha qidiruv")]
    [ProducesResponseType(typeof(ApiResponse<List<FaqDto>>), 200)]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        var results = await _faqService.SearchAsync(searchTerm);
        return Ok(ApiResponse<List<FaqDto>>.SuccessResponse(results.ToList()));
    }

    [HttpPost("dynamic-filter")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Dinamika filter bilan FAQlarni olish")]
    [ProducesResponseType(typeof(ApiResponse<List<FaqDto>>), 200)]
    public async Task<IActionResult> DynamicFilter([FromBody] Dictionary<string, object> filters, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
    {
        var result = await _faqService.DynamicFilterAsync(filters, page, pageSize);
        return Ok(ApiResponse<List<FaqDto>>.SuccessResponse(result.ToList()));
    }
}

