using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class AdminUserRepository : UserRepository, IAdminUserRepository
    {
        private readonly FurnitureDbContext _context;
        public AdminUserRepository(FurnitureDbContext context) : base(context)
        {
            _context = context;
        }

        // In your service layer
        public async Task<bool> ChangeUserRoleAsync(int userId, Role newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;
            if (user.Role == newRole)
                return true;

            user.Role = newRole;
            user.UpdatedAt = DateTime.UtcNow;
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> CheckUserExistsAsync(int userId)
        {
            var result = await _context.Users.FindAsync(userId);
            if (result == null) return false;
            return true;
        }

        public async Task<bool> ForceUpdateUserAsync(int userId, AdminUserUpdateDto updateDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Entry(user).CurrentValues.SetValues(updateDto);
            return true;
        }

        public IQueryable<User> GetAdminQueryable()
        {
            return _context.Users.AsNoTracking();
        }

        public async Task<PaginatedList<User>> GetFilteredUsersAsync(UserFilterDto filter)
        {
            var query = _context.Users.AsNoTracking();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(u =>
                    u.Username.Contains(filter.SearchTerm) ||
                    u.Email.Contains(filter.SearchTerm) ||
                    u.FirstName.Contains(filter.SearchTerm) ||
                    (!string.IsNullOrEmpty(u.LastName) && u.LastName.Contains(filter.SearchTerm)));
            }

            if (filter.RoleFilter.HasValue)
            {
                query = query.Where(u => u.Role == filter.RoleFilter.Value);
            }

            if (filter.IsActiveFilter.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActiveFilter.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Return empty result if no items found
            if (totalCount == 0)
            {
                return new PaginatedList<User>(
                    new List<User>(),
                    0,
                    filter.PageNumber,
                    filter.PageSize);
            }

            // Apply pagination
            var items = await query
                .OrderBy(u => u.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedList<User>(items, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<bool> SetUserStatusAsync(int userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Only update if status is actually changing
            if (user.IsActive == isActive)
                return true;

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
        public async Task<bool> SoftDeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            if (user.IsDeleted)
                return true; // Allaqachon o‘chirilgan

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        // Faollik statistikasini hisoblash (UserLoginHistories jadvalisiz)
        public async Task<List<UserActivityStatsDto>> GetUserActivityStatsAsync(DateRangeDto dateRange)
        {
            var startDate = dateRange.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = dateRange.EndDate ?? DateTime.UtcNow;

            var result = new List<UserActivityStatsDto>();
            var days = (endDate - startDate).Days;

            for (var i = 0; i <= days; i++)
            {
                var currentDate = startDate.AddDays(i);
                var nextDate = currentDate.AddDays(1);

                var stats = new UserActivityStatsDto
                {
                    Date = currentDate,
                    NewUsers = await _context.Users
                        .CountAsync(u => u.CreatedAt >= currentDate && u.CreatedAt < nextDate),
                    ActiveUsers = await _context.Users
                        .CountAsync(u => u.LastLoginDate >= currentDate && u.LastLoginDate < nextDate),
                    // LoginCount ni hisoblamaymiz yoki LastLoginDate asosida taxmin qilamiz
                    LoginCount = await _context.Users
                        .CountAsync(u => u.LastLoginDate >= currentDate && u.LastLoginDate < nextDate)
                };

                result.Add(stats);
            }

            return result;
        }

        public async Task<UserCountStatsDto> GetUserCountStatsAsync()
        {
            var stats = new UserCountStatsDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.IsActive && !u.IsDeleted),
                InactiveUsers = await _context.Users.CountAsync(u => !u.IsActive && !u.IsDeleted),
                DeletedUsers = await _context.Users.CountAsync(u => u.IsDeleted),
                UsersByRole = await _context.Users
                    .Where(u => !u.IsDeleted && u.Role.HasValue)
                    .GroupBy(u => u.Role!.Value)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Role, x => x.Count)
            };

            return stats;
        }
        public async Task<List<User>> GetDeletedUsersAsync()
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.IsDeleted)
                .OrderByDescending(u => u.DeletedAt)
                .ToListAsync();
        }
    }
}
