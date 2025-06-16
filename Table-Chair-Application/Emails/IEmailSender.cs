using System.Threading.Tasks;

namespace Table_Chair_Application.Emails
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage, string? plainTextMessage = null);
    }
}
