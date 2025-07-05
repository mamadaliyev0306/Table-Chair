using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IAdminUserService
    {
        // Id bo'yicha foydalanuvchi ma'lumotlari
        Task<AdminUserResponseDto> GetUserByIdAsync(int id);

        // Foydalanuvchilar ro'yxati
        Task<PaginatedList<AdminUserResponseDto>> GetUsersPaginatedAsync(UserFilterDto filter);

        // Foydalanuvchini yangilash
        Task<bool> UpdateUserAsync(int id, AdminUserUpdateDto userDto);

        // Foydalanuvchini o'chirish
        Task<bool> DeleteUserAsync(int id);

        // Foydalanuvchi holati
        Task<bool> SetUserStatusAsync(int userId, bool isActive); // Bitta metod bilan

        // Roli boshqaruvi
        Task<bool> ChangeUserRoleAsync(int userId, Role newRole);

        // Qidiruv va tekshirish
        Task<bool> CheckUserExistsAsync(int userId); // Yangi metod
        Task<bool> DeleteOwnProfileAsync(int userId);
        Task<bool> RestoreUserAsync(int userId);
        Task<List<UserActivityStatsDto>> GetUserActivityStatsAsync(DateRangeDto dateRange);
        Task<UserCountStatsDto> GetUserCountStatsAsync();
        Task<List<AdminUserResponseDto>> GetDeletedUsersAsync();
    }
}
