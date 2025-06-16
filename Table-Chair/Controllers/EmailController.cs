using Microsoft.AspNetCore.Mvc;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.EmailDtos;
using Table_Chair_Application.Emails;


namespace Table_Chair.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        // POST: https://localhost:7179/api/email/send-password-reset
        [HttpPost("send-password-reset")]
        public async Task<IActionResult> SendPasswordReset([FromBody] SendPasswordResetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendPasswordResetAsync(request.Email, request.Name, request.Token);
            return Ok(new { Message = "Parol tiklash email yuborildi." });
        }
        // POST: https://localhost:7179/api/email/send-welcome
        [HttpPost("send-welcome")]
        public async Task<IActionResult> SendWelcomeEmail([FromBody] SendWelcomeEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendWelcomeEmailAsync(request.Email, request.Name);
            return Ok(new { Message = "Xush kelibsiz email yuborildi." });
        }
        // POST: https://localhost:7179/api/email/send-account-locked
        [HttpPost("send-account-locked")]
        public async Task<IActionResult> SendAccountLockedEmail([FromBody] SendAccountLockedEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendAccountLockedEmailAsync(request.Email, request.Name);
            return Ok(new { Message = "Hisob bloklandi haqida email yuborildi." });
        }
        // POST: https://localhost:7179/api/email/send-account-unlocked
        [HttpPost("send-account-unlocked")]
        public async Task<IActionResult> SendAccountUnlockedEmail([FromBody] SendAccountUnlockedEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendAccountUnlockedEmailAsync(request.Email, request.Name);
            return Ok(new { Message = "Hisob blokdan chiqarildi haqida email yuborildi." });
        }
        // POST: https://localhost:7179/api/email/send-order-confirmation
        [HttpPost("send-order-confirmation")]
        public async Task<IActionResult> SendOrderConfirmation([FromBody] SendOrderConfirmationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendOrderConfirmationAsync(request.Email, request.Name, request.OrderId);
            return Ok(new { Message = "Buyurtma tasdiqlash email yuborildi." });
        }
        // POST: https://localhost:7179/api/email/send-password-changed
        [HttpPost("send-password-changed")]
        public async Task<IActionResult> SendPasswordChangedNotification([FromBody] SendPasswordChangedNotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendPasswordChangedNotificationAsync(request.Email, request.Name);
            return Ok(new { Message = "Parol o'zgargani haqida email yuborildi." });
        }
        // POST: https://localhost:7179/api/email/send-new-user-notification
        [HttpPost("send-new-user-notification")]
        public async Task<IActionResult> SendNewUserNotificationToAdmin([FromBody] SendNewUserNotificationToAdminRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendNewUserNotificationToAdminAsync(request.AdminEmail, request.NewUserEmail);
            return Ok(new { Message = "Admin'ga yangi foydalanuvchi haqida email yuborildi." });
        }
    }
}

