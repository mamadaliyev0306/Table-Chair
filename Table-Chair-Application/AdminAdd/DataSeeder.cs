using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Table_Chair_Application.PasswordHash;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;

namespace Table_Chair_Application.Seeders
{
    public class DataSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<DataSeeder> _logger;
        private readonly IConfiguration _configuration;

        public DataSeeder(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ILogger<DataSeeder> logger,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SeedAdminAsync()
        {
            try
            {
                string adminUsername = _configuration["Admin:Username"] ?? "admin";
                string adminEmail = _configuration["Admin:Email"] ?? "admin@example.com";
                string adminPhone = _configuration["Admin:PhoneNumber"] ?? "998911234567";
                string adminPassword = _configuration["Admin:Password"] ?? "Admin@123";

                var existingAdmin = await _unitOfWork.Users.GetByEmailAsync(adminEmail);
                if (existingAdmin != null)
                {
                    _logger.LogInformation("Admin foydalanuvchi allaqachon mavjud.");
                    return;
                }

                var adminUser = new User
                {
                    Username = adminUsername,
                    Email = adminEmail,
                    PhoneNumber = adminPhone,
                    PasswordHash = _passwordHasher.HashPassword(adminPassword),
                    Role = Role.Admin,
                    IsActive = true,
                    EmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastPasswordChangeDate = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(adminUser);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Admin foydalanuvchi muvaffaqiyatli yaratildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin foydalanuvchi yaratishda xatolik yuz berdi.");
                throw;
            }
        }
    }
}
