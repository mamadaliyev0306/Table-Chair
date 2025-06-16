using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.PasswordHash;
using Table_Chair_Application.Repositorys;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> GetByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            return user == null
                ? throw new NotFoundException("Foydalanuvchi topilmadi")
                : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> GetByUsernameAsync(string username)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            return user == null
                ? throw new NotFoundException("Foydalanuvchi topilmadi")
                : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _unitOfWork.Users.EmailExistsAsync(email);
        }

        public async Task<UserResponseDto> GetByPhoneAsync(string phoneNumber)
        {
            var user = await _unitOfWork.Users.GetByPhoneAsync(phoneNumber);
            return user == null
                ? throw new NotFoundException("Foydalanuvchi topilmadi")
                : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> PhoneExistsAsync(string phoneNumber)
        {
            return await _unitOfWork.Users.PhoneExistsAsync(phoneNumber);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _unitOfWork.Users.UsernameExistsAsync(username);
        }

        public async Task<UserResponseDto> GetUserProfileAsync(int userId)
        {
            var result = await _unitOfWork.Users.GetUserProfileAsync(userId);
            return result == null
                ? throw new NotFoundException("Not Found")
                : _mapper.Map<UserResponseDto>(result);
        }

        public async Task<bool> UpdateProfileAsync(int userId, UserUpdateDto updateDto)
        {
           var result = await _unitOfWork.Users.GetByIdAsync(userId);
            if(result == null) 
                return false;
            result.UpdatedAt = DateTime.UtcNow;
            _mapper.Map(updateDto,result);
            _unitOfWork.Users.Update(result);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> VerifyEmailAsync(int userId)
        {
            return await _unitOfWork.Users.VerifyEmailAsync(userId);
        }

        public async Task<bool> DeleteOwnProfileAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
                return false;
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteProfileAsync(int userId)
        {
            var result = await _unitOfWork.Users.GetByIdAsync(userId);
            if (result == null)
                return false;
            _unitOfWork.Users.Delete(result);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}