using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.CreateDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Exceptions; // NotFoundException uchun

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterSubscriptionController : ControllerBase
    {
        private readonly INewsletterSubscriptionService _newsletterSubscriptionService;

        public NewsletterSubscriptionController(INewsletterSubscriptionService newsletterSubscriptionService)
        {
            _newsletterSubscriptionService = newsletterSubscriptionService;
        }

        // POST: api/NewsletterSubscription
        // POST: https://localhost:7179/api/NewsletterSubscription/create
        [HttpPost("create")]
        public async Task<IActionResult> Add([FromBody] NewsletterSubscriptionCreateDto newsletterSubscriptionCreateDto)
        {
            await _newsletterSubscriptionService.AddNewsletterSubscription(newsletterSubscriptionCreateDto);
            return StatusCode(201); // Created
        }

        // GET: https://localhost:7179/api/NewsletterSubscription/getall
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<NewsletterSubscriptionDto>>> GetAll()
        {
            var subscriptions = await _newsletterSubscriptionService.GetAllAsync();
            return Ok(subscriptions);
        }

        // GET: https://localhost:7179/api/NewsletterSubscription/getbyId/id
        [HttpGet("getbyId/{id}")]
        public async Task<ActionResult<NewsletterSubscriptionDto>> GetById(int id)
        {
            try
            {
                var subscription = await _newsletterSubscriptionService.GetByIdAsync(id);
                return Ok(subscription);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT: https://localhost:7179/api/NewsletterSubscription/update
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] NewsletterSubscriptionDto newsletterSubscriptionDto)
        {
            try
            {
                await _newsletterSubscriptionService.UpdateAsync(newsletterSubscriptionDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: https://localhost:7179/api/NewsletterSubscription/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            try
            {
                await _newsletterSubscriptionService.DeleteNewsletterSubscriptionAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PATCH: https://localhost:7179/api/NewsletterSubscription/soft-delete/id
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                await _newsletterSubscriptionService.SoftDeleteNewsletterSubscriptionAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PATCH: https://localhost:7179/api/NewsletterSubscription/restore/id
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _newsletterSubscriptionService.RestoreNewsletterSubscriptionAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}

