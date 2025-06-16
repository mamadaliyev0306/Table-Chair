using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table_Chair_Application.Emails
{
    public interface IEmailService
    {
        // User related emails
        Task SendWelcomeEmailAsync(string email, string name);
        Task SendEmailVerificationAsync(string email, string name, string token);
        Task SendPasswordResetAsync(string email, string name, string token);
        Task SendAccountLockedEmailAsync(string email, string name);
        Task SendAccountUnlockedEmailAsync(string email, string name);

        // Notification emails
        Task SendOrderConfirmationAsync(string email, string name, string orderId);
        Task SendPasswordChangedNotificationAsync(string email, string name);

        // Admin emails
        Task SendNewUserNotificationToAdminAsync(string adminEmail, string newUserEmail);
        Task<bool> SendVerificationEmail(string email, string verificationCode);
        Task<List<DateTime>> GetEmailHistory(string email);
    }
}
