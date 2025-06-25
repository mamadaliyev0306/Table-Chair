using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Table_Chair_Application.Dtos.EmailDtos;
using Table_Chair_Application.Emails;
using Table_Chair_Application.Responses;

namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }
        [HttpPost("send-password-reset")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Parol tiklash uchun email yuborish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> SendPasswordReset([FromBody] SendPasswordResetRequest request)
        {
            await _emailService.SendPasswordResetAsync(request.Email, request.Name, request.Token);
            return Ok(ApiResponse<string>.SuccessResponse("Parol tiklash email yuborildi."));
        }
        [HttpPost("send-order-confirmation")]
        [Authorize]
        [SwaggerOperation(Summary = "Buyurtma tasdiqlash uchun email yuborish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SendOrderConfirmation([FromBody] SendOrderConfirmationRequest request)
        {
            await _emailService.SendOrderConfirmationAsync(request.Email, request.Name, request.OrderId);
            return Ok(ApiResponse<string>.SuccessResponse("Buyurtma tasdiqlash email yuborildi."));
        }
        [HttpPost("send-password-changed")]
        [Authorize]
        [SwaggerOperation(Summary = "Parol o‘zgargani haqida email yuborish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SendPasswordChangedNotification([FromBody] SendPasswordChangedNotificationRequest request)
        {
            await _emailService.SendPasswordChangedNotificationAsync(request.Email, request.Name);
            return Ok(ApiResponse<string>.SuccessResponse("Parol o‘zgargani haqida email yuborildi."));
        }

        [HttpPost("send-new-user-notification")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Adminga yangi foydalanuvchi haqida email yuborish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SendNewUserNotificationToAdmin([FromBody] SendNewUserNotificationToAdminRequest request)
        {
            await _emailService.SendNewUserNotificationToAdminAsync(request.AdminEmail, request.NewUserEmail);
            return Ok(ApiResponse<string>.SuccessResponse("Adminga yangi foydalanuvchi haqida email yuborildi."));
        }
    }
}


