using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Exceptions;
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
            if (users == null || !users.Items.Any()) 
                throw new NotFoundException("Hech qanday foydalanuvchi topilmadi");

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
            user.UpdatedAt = DateTime.UtcNow;
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
    }
}
