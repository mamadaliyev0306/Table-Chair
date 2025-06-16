using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class EmailRepository : IEmailRepository
    {
        private readonly FurnitureDbContext _context;

        public EmailRepository(FurnitureDbContext context)
        {
            _context = context;
        }

        public async Task SaveVerificationCodeAsync(EmailVerification emailVerification)
        {
            await _context.EmailVerifications.AddAsync(emailVerification);
            await _context.SaveChangesAsync();
        }

        public async Task<EmailVerification> GetVerificationCodeByEmailAsync(string email)
        {
            var result = await _context.EmailVerifications
                .FirstOrDefaultAsync(ev => ev.Email == email && ev.ExpiryDate > DateTime.UtcNow);

            if (result == null)
            {
                throw new InvalidOperationException($"No valid verification code found for email: {email}");
            }

            return result;
        }

        public async Task MarkEmailAsVerifiedAsync(string email)
        {
            var emailVerification = await _context.EmailVerifications
                .FirstOrDefaultAsync(ev => ev.Email == email);

            if (emailVerification != null)
            {
                // Emailni tasdiqlash
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    user.EmailVerified = true;
                    await _context.SaveChangesAsync();
                }

                // Tasdiqlash kodini o'chirish
                _context.EmailVerifications.Remove(emailVerification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
