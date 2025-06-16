using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;

namespace Table_Chair_Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AdminUserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<bool> ChangeUserRoleAsync(int userId, Role newRole)
        {
           var result = await _unitOfWork.AdminUsers.ChangeUserRoleAsync(userId, newRole);
            return result;
        }

        public async Task<bool> CheckUserExistsAsync(int userId)
        {
           var result = await _unitOfWork.AdminUsers.CheckUserExistsAsync(userId);
            return result;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var result = await _unitOfWork.AdminUsers.GetByIdAsync(id);
            if (result == null) 
                return false;
            _unitOfWork.AdminUsers.Delete(result);
           await  _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<AdminUserResponseDto> GetUserByIdAsync(int id)
        {
            var result = await _unitOfWork.AdminUsers.GetByIdAsync(id);
            return result == null
                ? throw new NotFoundException("Foydalanuvchi topilmadi")
                : _mapper.Map<AdminUserResponseDto>(result);
        }

        public async Task<PaginatedList<AdminUserResponseDto>> GetUsersPaginatedAsync(UserFilterDto filter)
        {
            var result = await _unitOfWork.AdminUsers.GetFilteredUsersAsync(filter);
            return result==null
                ? throw new NotFoundException("Foydalanuvchi topilmadi")
                : _mapper.Map<PaginatedList<AdminUserResponseDto>>(result);
        }

        public async Task<bool> SetUserStatusAsync(int userId, bool isActive)
        {
            var result = await _unitOfWork.AdminUsers.SetUserStatusAsync(userId, isActive);
            return result;
        }

        public async Task<bool> UpdateUserAsync(int id, AdminUserUpdateDto userDto)
        {
            if (userDto == null)
            {
                return false;
            }
            var result = await _unitOfWork.AdminUsers.GetByIdAsync(id);
            if (result == null)
            {
                return false;
            }
            result.UpdatedAt=DateTime.UtcNow;
           return await _unitOfWork.AdminUsers.ForceUpdateUserAsync(id, userDto);
        }
        public async Task<bool> DeleteOwnProfileAsync(int userId)
        {
            var user = await _unitOfWork.AdminUsers.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
                return false;

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AdminUsers.Update(user); // <-- Soft delete uchun Update ishlatiladi
            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<bool> RestoreUserAsync(int userId)
        {
            var user = await _unitOfWork.AdminUsers.GetByIdAsync(userId);
            if (user == null)
                return false;

            if (!user.IsDeleted)
                return true; // Allaqachon tiklangan

            user.IsDeleted = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
