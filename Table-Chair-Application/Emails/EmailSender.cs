using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Table_Chair_Application.Settings;

namespace Table_Chair_Application.Emails
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<SmtpSettings> smtpSettings, ILogger<EmailSender> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string? plainTextMessage = null)
        {
            try
            {
                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = _smtpSettings.EnableSsl;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    if (!string.IsNullOrEmpty(plainTextMessage))
                    {
                        mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainTextMessage, null, "text/plain"));
                    }

                    await client.SendMailAsync(mailMessage);
                }

                _logger.LogInformation($"Email yuborildi: {subject} - {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Xatolik yuz berdi email yuborishda: {ex.Message} - {subject} - {email}");
                throw;
            }
        }
    }
}
