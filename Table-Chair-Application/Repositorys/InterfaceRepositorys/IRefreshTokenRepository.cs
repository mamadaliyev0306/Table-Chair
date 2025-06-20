using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys.InterfaceRepositorys
{
    public interface IRefreshTokenRepository:IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetValidTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetUserRefreshTokensAsync(int userId);
        Task RevokeTokenAsync(int id, string revokedBy, string? reason = null);
        Task RevokeAllForUserAsync(int userId, string revokedBy, string? reason = null);
        Task<bool> IsTokenValid(string token);
        Task<bool> RevokeAllRefreshTokensForUserAsync(int userId);
        Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync();
        Task<int> RemoveExpiredTokensAsync();
    }
}
