using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;


namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IUserRepository:IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string? email);
        Task<User?> GetByUsernameAsync(string username);
        IQueryable<User> GetUsersByRoleQuery(Role role);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByPhoneAsync(string phoneNumber);
        Task<bool> PhoneExistsAsync(string phoneNumber);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> VerifyEmailAsync(int userId);
        Task<User> GetUserProfileAsync(int userId);
        Task<User> GetByVerificationTokenAsync(string token);
        Task<bool> ExistsUserIdAsync(int userId);
    }
}
