using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Dtos;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly IFaqService _faqService;
        private readonly ILogger<FaqController> _logger;

        public FaqController(IFaqService faqService, ILogger<FaqController> logger)
        {
            _faqService = faqService;
            _logger = logger;
        }
        // POST: https://localhost:7179/api/faq/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FaqCreateDto faqCreateDto)
        {
            await _faqService.CreateAsync(faqCreateDto);
            return Ok(new { message = "Faq created successfully." });
        }
        // PUT: https://localhost:7179/api/faq/update
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FaqUpdateDto faqUpdateDto)
        {
            await _faqService.UpdateAsync(id, faqUpdateDto);
            return Ok(new { message = "Faq updated successfully." });
        }
        // DELETE: https://localhost:7179/api/faq/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _faqService.DeleteAsync(id);
            return Ok(new { message = "Faq deleted successfully." });
        }
        // GET: https://localhost:7179/api/faq/getbyId/id
        [HttpGet("getbyId/{id}")]
        public async Task<ActionResult<FaqDto>> GetById(int id)
        {
            var faq = await _faqService.GetByIdAsync(id);
            return Ok(faq);
        }
        // GET: https://localhost:7179/api/faq/getall
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<FaqDto>>> GetAll([FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            var faqs = await _faqService.GetAllAsync(page: page, pageSize: pageSize);
            return Ok(faqs);
        }
        // GET: https://localhost:7179/api/faq/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<FaqDto>>> GetActive()
        {
            var activeFaqs = await _faqService.GetActiveAsync();
            return Ok(activeFaqs);
        }
        // GET: https://localhost:7179/api/faq/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var count = await _faqService.CountAsync();
            return Ok(count);
        }
        // GET: https://localhost:7179/api/faq/exists
        [HttpGet("exists")]
        public async Task<ActionResult<bool>> Exists([FromQuery] string field, [FromQuery] string value)
        {
            if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(value))
                return BadRequest("Field and value are required.");

            var exists = await _faqService.ExistsAsync(f => EF.Property<string>(f, field) == value);
            return Ok(exists);
        }
        // PATCH: https://localhost:7179/api/faq/soft-delete/id
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _faqService.SoftDeleteAsync(id);
            return Ok(new { message = "Faq soft deleted successfully." });
        }
        // PATCH: https://localhost:7179/api/faq/restore/id
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            await _faqService.RestoreAsync(id);
            return Ok(new { message = "Faq restored successfully." });
        }
        // GET: https://localhost:7179/api/faq/dynamic-filter
        [HttpGet("dynamic-filter")]
        public async Task<ActionResult<IEnumerable<FaqDto>>> DynamicFilter([FromBody] Dictionary<string, object> filters, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            var result = await _faqService.DynamicFilterAsync(filters, page, pageSize);
            return Ok(result);
        }
        // GET: https://localhost:7179/api/faq/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FaqDto>>> Search([FromQuery] string searchTerm)
        {
            var results = await _faqService.SearchAsync(searchTerm);
            return Ok(results);
        }
    }
}
