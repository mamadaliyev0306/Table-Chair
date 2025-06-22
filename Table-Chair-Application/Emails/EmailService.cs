using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Table_Chair_Application.Settings;
using Table_Chair_Application.Templates;
using Table_Chair_Entity.Models;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Table_Chair_Application.CacheServices;

namespace Table_Chair_Application.Emails
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailRepository _emailRepository;
        private readonly SmtpSettings _smtpSettings;
        private readonly IConnectionMultiplexer _redis;
        private readonly ICacheService _cacheService;
        private const string EmailHistoryPrefix = "email:history:";
        private const string RateLimitPrefix = "email:rate:";
        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IEmailSender emailSender,
            ILogger<EmailService> logger,
            IEmailRepository emailRepository,
            IOptions<SmtpSettings> options,
            ICacheService cacheService,
            IConnectionMultiplexer redis)
            
        {
            _emailSettings = emailSettings.Value;
            _emailSender = emailSender;
            _logger = logger;
            _emailRepository = emailRepository;
            _smtpSettings = options.Value;
            _redis = redis;
            _cacheService = cacheService;
        }

        public async Task SendEmailVerificationAsync(string email, string name, string token)
        {
            var verificationUrl = $"{_emailSettings.BaseUrl}/verify-email?token={token}";
            var message = EmailTemplates.GetEmailVerificationTemplate(name, verificationUrl);
            await SendEmailAsync(email, "Email manzilingizni tasdiqlang", message);
        }

        public async Task SendPasswordResetAsync(string email, string name, string token)
        {
            var resetUrl = $"{_emailSettings.BaseUrl}/reset-password?token={token}";
            var message = EmailTemplates.GetPasswordResetTemplate(name, resetUrl);
            await SendEmailAsync(email, "Parolingizni tiklash", message);
        }

        public async Task SendWelcomeEmailAsync(string email, string name)
        {
            var message = EmailTemplates.GetWelcomeTemplate(name);
            await SendEmailAsync(email, "Xush kelibsiz!", message);
        }

        public async Task SendAccountLockedEmailAsync(string email, string name)
        {
            var message = EmailTemplates.GetAccountLockedTemplate(name);
            await SendEmailAsync(email, "Hisobingiz bloklandi", message);
        }

        public async Task SendAccountUnlockedEmailAsync(string email, string name)
        {
            var message = EmailTemplates.GetAccountUnlockedTemplate(name);
            await SendEmailAsync(email, "Hisobingiz qayta faollashtirildi", message);
        }

        public async Task SendOrderConfirmationAsync(string email, string name, string orderId)
        {
            var orderUrl = $"{_emailSettings.BaseUrl}/orders/{orderId}";
            var message = EmailTemplates.GetOrderConfirmationTemplate(name, orderId, orderUrl);
            await SendEmailAsync(email, "Buyurtmangiz qabul qilindi", message);
        }

        public async Task SendPasswordChangedNotificationAsync(string email, string name)
        {
            var message = EmailTemplates.GetPasswordChangedTemplate(name);
            await SendEmailAsync(email, "Parolingiz o'zgartirildi", message);
        }

        public async Task SendNewUserNotificationToAdminAsync(string adminEmail, string newUserEmail)
        {
            var message = EmailTemplates.GetNewUserNotificationTemplate(newUserEmail);
            await SendEmailAsync(adminEmail, "Yangi foydalanuvchi ro'yxatdan o'tdi", message);
        }

        public async Task<bool> SendVerificationEmail(string email, string verificationCode)
        {
            try
            {
                // Rate limit tekshirish
                if (!await CanSendEmail(email))
                {
                    _logger.LogWarning($"Rate limit asosida email yuborish bloklandi: {email}");
                    return false;
                }

                // Email yuborish tarixini saqlash
                await LogEmailSent(email);

                // HTML formatdagi email
                var htmlMessage = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #4a6baf;'>Email Tasdiqlash</h2>
                    <p>Quyidagi tasdiqlash kodini ilovamizga kiriting:</p>
                    <div style='background: #f5f5f5; padding: 15px; text-align: center; 
                                margin: 20px 0; font-size: 24px; letter-spacing: 2px; 
                                font-weight: bold;'>
                        {verificationCode}
                    </div>
                    <p>Bu kod 15 daqiqadan keyin tugaydi.</p>
                    <p>Agar siz bu xabarni kutmaganingiz bo'lsa, e'tibor bermang.</p>
                </div>";

                return await SendEmailAsync(email, "Email Tasdiqlash Kodi", htmlMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Email yuborishda xatolik: {email}");
                return false;
            }
        }

        public async Task<List<DateTime>> GetEmailHistory(string email)
        {
            var emailHistoryKey = $"{EmailHistoryPrefix}{email}";

            var history = await _cacheService.ListRangeAsync(emailHistoryKey);

            return history
                .Select(x =>
                {
                    var json = x.ToString(); // Convert RedisValue to string  
                    if (string.IsNullOrEmpty(json)) // Handle null or empty values  
                    {
                        _logger.LogWarning($"Null or empty value found in email history for: {email}");
                        return (DateTime?)null; // Return null for invalid entries  
                    }

                    var historyItem = JsonSerializer.Deserialize<EmailHistoryItem>(json);
                    if (historyItem == null || historyItem.SentTime == default)
                    {
                        _logger.LogWarning($"Invalid or null EmailHistoryItem found for: {email}");
                        return (DateTime?)null; // Return null for invalid entries  
                    }

                    return historyItem.SentTime; // Return valid DateTime  
                })
                .Where(x => x.HasValue) // Filter out null values  
                .Select(a => a.Value) // Safely extract non-null DateTime values without null-forgiving operator  
                .ToList();
        }

        public async Task<int> GetEmailSentCount(string email, TimeSpan withinLast)
        {
            var history = await GetEmailHistory(email);
            var cutoff = DateTime.UtcNow - withinLast;
            return history.Count(t => t > cutoff);
        }

        private async Task<bool> CanSendEmail(string email)
        {
           // var db = _redis.GetDatabase();
            var key = $"{RateLimitPrefix}{email}";

            // So'rovlar sonini oshirish
            var current = await _cacheService.IncrementAsync(key);

            // Agar birinchi marta bo'lsa, muddat belgilash
            if (current == 1)
            {
                await _cacheService.KeyExpireAsync(key, TimeSpan.FromHours(1));
            }

            // 1 soatda maksimal 5 marta ruxsat berish
            return current <= 5;
        }

        private async Task LogEmailSent(string email)
        {
           // var db = _redis.GetDatabase();
            var emailHistoryKey = $"{EmailHistoryPrefix}{email}";
            var historyItem = new EmailHistoryItem
            {
                Email = email,
                SentTime = DateTime.UtcNow
            };

            await _cacheService.ListLeftPushAsync(emailHistoryKey, JsonSerializer.Serialize(historyItem));
            await _cacheService.KeyExpireAsync(emailHistoryKey, TimeSpan.FromDays(1));
        }

        private async Task<bool> SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.FromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                using var smtpClient = new SmtpClient(_smtpSettings.Server)
                {
                    Port = _smtpSettings.Port,
                    Credentials = new NetworkCredential(
                        _smtpSettings.Username,
                        _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl,
                    Timeout = 10000 // 10 soniya
                };

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email muvaffaqiyatli yuborildi: {email}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Email yuborishda xatolik: {email}");
                return false;
            }
        }
        private class EmailHistoryItem
        {
            public string Email { get; set; } = null!;
            public DateTime SentTime { get; set; }
        }
    }
}

