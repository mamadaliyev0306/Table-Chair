using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IUserService
    {
        // Foydalanuvchi ma'lumotlari
        Task<UserResponseDto> GetUserProfileAsync(int userId); // Yangi metod
        Task<UserResponseDto> GetByEmailAsync(string email);
        Task<UserResponseDto> GetByUsernameAsync(string username);
        Task<UserResponseDto> GetByPhoneAsync(string phoneNumber);

        // Profil yangilash
        Task<bool> UpdateProfileAsync(int userId, UserUpdateDto updateDto); // Yangi metod
        Task<bool> DeleteOwnProfileAsync(int userId);
        Task<bool> DeleteProfileAsync(int userId);
        // Tekshiruv metodlari
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phoneNumber);
        Task<bool> UsernameExistsAsync(string username);

        // Email tasdiqlash
        Task<bool> VerifyEmailAsync(int userId);

    }
}
