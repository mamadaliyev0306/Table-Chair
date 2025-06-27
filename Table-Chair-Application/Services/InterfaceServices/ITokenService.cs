using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface ITokenService
    {
        // Token generation
        string GenerateAccessToken(UserResponseDto user);
        string GenerateRefreshToken();
        string GenerateEmailVerificationToken(int userId);
        string GeneratePasswordResetToken(int userId);
        (string Token, DateTime ExpiresAt) GenerateAccessTokenWithExpiry(UserResponseDto user);

        // Token validation
        int? ValidateAccessToken(string token);
        int? ValidateEmailVerificationToken(string token);
        int? ValidatePasswordResetToken(string token);

        // Token management
        Task<bool> IsRefreshTokenValidAsync(string token);
        Task<bool> RevokeAllRefreshTokensForUserAsync(int userId);
    }
}
