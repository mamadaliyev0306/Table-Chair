using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Tokens
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
        Task<RefreshToken> GenerateRefreshTokenAsync(int userId);
        Task<string> GenerateEmailVerificationTokenAsync(int userId);
        Task<string> GeneratePasswordResetTokenAsync(int userId);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
