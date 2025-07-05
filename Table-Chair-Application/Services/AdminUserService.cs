using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminUserService> _logger;

        public AdminUserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AdminUserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> ChangeUserRoleAsync(int userId, Role newRole)
        {
            _logger.LogInformation("Changing role for user with ID {UserId} to {NewRole}", userId, newRole);
            var success = await _unitOfWork.AdminUsers.ChangeUserRoleAsync(userId, newRole);
            if (!success)
                throw new NotFoundException("Foydalanuvchi topilmadi yoki rol o'zgartirib bo'lmadi");

            return true;
        }

        public async Task<bool> CheckUserExistsAsync(int userId)
        {
            return await _unitOfWork.AdminUsers.CheckUserExistsAsync(userId);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.AdminUsers.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("DeleteUserAsync: User not found with ID {UserId}", id);
                throw new NotFoundException("Foydalanuvchi topilmadi");
            }
            user.DeletedAt = DateTime.UtcNow;
            _unitOfWork.AdminUsers.Delete(user);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("User with ID {UserId} hard deleted successfully", id);
            return true;
        }

        public async Task<AdminUserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.AdminUsers.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("GetUserByIdAsync: User not found with ID {UserId}", id);
                throw new NotFoundException("Foydalanuvchi topilmadi");
            }

            return _mapper.Map<AdminUserResponseDto>(user);
        }

        public async Task<PaginatedList<AdminUserResponseDto>> GetUsersPaginatedAsync(UserFilterDto filter)
        {
            var users = await _unitOfWork.AdminUsers.GetFilteredUsersAsync(filter);

            // Bo'sh ro'yxat uchun exception o'rniga bo'sh javob qaytaramiz
            if (users == null || users.Items.Count == 0)
            {
                return new PaginatedList<AdminUserResponseDto>(
                    new List<AdminUserResponseDto>(),
                    0,
                    filter.PageNumber,
                    filter.PageSize);
            }

            return _mapper.Map<PaginatedList<AdminUserResponseDto>>(users);
        }

        public async Task<bool> SetUserStatusAsync(int userId, bool isActive)
        {
            var result = await _unitOfWork.AdminUsers.SetUserStatusAsync(userId, isActive);
            if (!result)
            {
                _logger.LogWarning("SetUserStatusAsync: Failed to update status for user with ID {UserId}", userId);
                throw new NotFoundException("Foydalanuvchi topilmadi yoki holatni o'zgartirib bo'lmadi");
            }
            return true;
        }

        public async Task<bool> UpdateUserAsync(int id, AdminUserUpdateDto userDto)
        {
            if (userDto == null)
            {
                _logger.LogWarning("UpdateUserAsync called with null DTO");
                throw new ValidationException("Yaroqsiz malumotlar yuborildi");
            }

            var user = await _unitOfWork.AdminUsers.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("UpdateUserAsync: User not found with ID {UserId}", id);
                throw new NotFoundException("Foydalanuvchi topilmadi");
            }

            user.UpdatedAt = DateTime.UtcNow;
            var updated = await _unitOfWork.AdminUsers.ForceUpdateUserAsync(id, userDto);
            return updated;
        }

        public async Task<bool> DeleteOwnProfileAsync(int userId)
        {
            var user = await _unitOfWork.AdminUsers.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("DeleteOwnProfileAsync: User not found or already deleted with ID {UserId}", userId);
                throw new NotFoundException("Foydalanuvchi topilmadi yoki allaqachon o'chirilgan");
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            _unitOfWork.AdminUsers.Update(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> RestoreUserAsync(int userId)
        {
            var user = await _unitOfWork.AdminUsers.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("RestoreUserAsync: User not found with ID {UserId}", userId);
                throw new NotFoundException("Foydalanuvchi topilmadi");
            }

            if (!user.IsDeleted)
                return true;

            user.IsDeleted = false;
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.AdminUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<List<UserActivityStatsDto>> GetUserActivityStatsAsync(DateRangeDto dateRange)
        {
            try
            {
                // Sana oralig'ini tekshirish
                if (dateRange.StartDate > dateRange.EndDate)
                {
                    throw new ArgumentException("Boshlang'ich sana tugash sanasidan katta bo'lishi mumkin emas");
                }

                // Maksimum 1 yillik ma'lumot olish mumkin
                if ((dateRange.EndDate ?? DateTime.UtcNow) - (dateRange.StartDate ?? DateTime.UtcNow.AddDays(-30)) > TimeSpan.FromDays(365))
                {
                    throw new ArgumentException("Sana oraligi 1 yildan oshmasligi kerak");
                }

                // UTC vaqtini tasdiqlash
                if (dateRange.StartDate.HasValue)
                    dateRange.StartDate = DateTime.SpecifyKind(dateRange.StartDate.Value, DateTimeKind.Utc);
                if (dateRange.EndDate.HasValue)
                    dateRange.EndDate = DateTime.SpecifyKind(dateRange.EndDate.Value, DateTimeKind.Utc);

                return await _unitOfWork.AdminUsers.GetUserActivityStatsAsync(dateRange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Foydalanuvchilar faollik statistikasini olishda xatolik");
                throw new AppException("Statistika olishda xatolik yuz berdi. Iltimos, qaytadan urunib ko'ring.");
            }
        }
        public async Task<UserCountStatsDto> GetUserCountStatsAsync()
        {
            try
            {
                var stats = await _unitOfWork.AdminUsers.GetUserCountStatsAsync();
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Foydalanuvchilar statistikasini olishda xatolik");
                throw new NotFoundException(ex + "Foydalanuvchilar statistikasini olishda xatolik");
            }
        }
        public async Task<List<AdminUserResponseDto>> GetDeletedUsersAsync()
        {
            try
            {
                var deletedUsers = await _unitOfWork.AdminUsers.GetDeletedUsersAsync();
                var list = _mapper.Map<List<AdminUserResponseDto>>(deletedUsers);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "O'chirilgan foydalanuvchilarni olishda xatolik");
                throw new NotFoundException(ex + "O'chirilgan foydalanuvchilarni olishda xatolik");
            }
        }
    }
}
