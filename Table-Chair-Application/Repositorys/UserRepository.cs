using Microsoft.EntityFrameworkCore;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly FurnitureDbContext _context;

        public UserRepository(FurnitureDbContext context) : base(context)
        {
            _context = context;
        }



        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email));
        }


        public async Task<User?> GetByPhoneAsync(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetByUsernameAsync(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public IQueryable<User> GetUsersByRoleQuery(Role role)
        {
            return _context.Users
                .Where(u => u.Role == role)
                .AsQueryable();
        }

        public async Task<bool> PhoneExistsAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == username);
        }

        public async Task<bool> VerifyEmailAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.EmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        public async Task<User> GetUserProfileAsync(int userId)
        {
            var result = await _context.Users.FirstOrDefaultAsync(a=>a.Id == userId);
            return result== null
                ? throw new NotFoundException("Foydalanuvchi topilmadi")
                : result;

        }

        public Task<User> GetByVerificationTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> ExistsUserIdAsync(int userId)=>
            await _context.Users.AnyAsync(u=>u.Id == userId);
    }
}
