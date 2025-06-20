using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.DbContextModels;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Repositorys
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly FurnitureDbContext _context;

        public RefreshTokenRepository(FurnitureDbContext context):base(context) 
        {
            _context = context;
        }
        public async Task<RefreshToken?> GetValidTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .IgnoreQueryFilters()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == token &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);
        }



        public async Task<IEnumerable<RefreshToken>> GetUserRefreshTokensAsync(int userId)
            {
                return await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId)
                    .OrderByDescending(rt => rt.User.CreatedAt)
                    .ToListAsync();
            }
        public async Task RevokeTokenAsync(int id, string revokedBy, string? reason = null)
        {
            var token = await GetByIdAsync(id);
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token), "Token not found.");
            }
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.ReplacedByToken = revokedBy;
            Update(token);
        }

            public async Task RevokeAllForUserAsync(int userId, string revokedBy, string? reason = null)
            {
                var tokens = await GetUserRefreshTokensAsync(userId);
                foreach (var token in tokens.Where(t => !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow))
                {
                    await RevokeTokenAsync(token.Id, revokedBy, reason);
                }
            }

            public async Task<bool> IsTokenValid(string token)
            {
                var storedToken = await GetValidTokenAsync(token);
                return storedToken != null;
            }

        public async Task<bool> RevokeAllRefreshTokensForUserAsync(int userId)
        {
            return await _context.RefreshTokens
                .AllAsync(a=>a.UserId == userId);
        }

        public async Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync()
        {
            return await _context.RefreshTokens
                .Where(rt => rt.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RemoveExpiredTokensAsync(IEnumerable<RefreshToken> tokens)
        {
            var tokensToRemove = new List<RefreshToken>();

            // 1. Faol va amal qilish muddati tugamagan tokenlar (oxirgi 3 tasini saqlaymiz)
            var validTokens = tokens
                .Where(t => !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            tokensToRemove.AddRange(validTokens.Skip(3)); // faqat oxirgi 3 tasini saqlaymiz

            // 2. Amal qilish muddati tugagan yoki revoked bo‘lgan tokenlarni o‘chirishga qo‘shish
            tokensToRemove.AddRange(
                tokens.Where(t => t.IsRevoked || t.ExpiresAt <= DateTime.UtcNow)
            );

            // 3. O‘chirish
            if (tokensToRemove.Any())
            {
                _context.RefreshTokens.RemoveRange(tokensToRemove);
            }

            await Task.CompletedTask; // Yoki bu yerda haqiqiy async ish yo‘q, sync ham bo'lishi mumkin
        }



    }
}
