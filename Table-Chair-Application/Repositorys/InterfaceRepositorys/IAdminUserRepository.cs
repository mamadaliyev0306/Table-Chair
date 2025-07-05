using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IAdminUserRepository:IUserRepository
    {
        Task<bool> SetUserStatusAsync(int userId, bool isActive);
        Task<bool> ChangeUserRoleAsync(int userId, Role newRole);
        Task<bool> ForceUpdateUserAsync(int userId, AdminUserUpdateDto updateDto);
        IQueryable<User> GetAdminQueryable();
        Task<PaginatedList<User>> GetFilteredUsersAsync(UserFilterDto filter);
        Task<bool> CheckUserExistsAsync(int userId);
        Task<UserCountStatsDto> GetUserCountStatsAsync();
        Task<List<User>> GetDeletedUsersAsync();
        Task<List<UserActivityStatsDto>> GetUserActivityStatsAsync(DateRangeDto dateRange);
    }
}
