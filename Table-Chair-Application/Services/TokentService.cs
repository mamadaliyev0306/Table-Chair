using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Settings;
using Table_Chair_Application.Settings.Table_Chair_Application.Settings;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IOptions<JwtSettings> jwtSettings,
            IUnitOfWork unitOfWork,
            ILogger<TokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Generate access token for user
        public string GenerateAccessToken(UserResponseDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Generate a random refresh token
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        // Generate email verification token for user
        public string GenerateEmailVerificationToken(int userId)
        {
            return GenerateJwtToken(userId, _jwtSettings.EmailVerificationTokenExpirationHours);
        }

        // Generate password reset token for user
        public string GeneratePasswordResetToken(int userId)
        {
            return GenerateJwtToken(userId, _jwtSettings.PasswordResetTokenExpirationMinutes);
        }

        // Validate access token
        public int? ValidateAccessToken(string token)
        {
            return ValidateJwtToken(token, validateLifetime: true);
        }

        // Validate email verification token
        public int? ValidateEmailVerificationToken(string token)
        {
            return ValidateJwtToken(token, validateLifetime: true);
        }

        // Validate password reset token
        public int? ValidatePasswordResetToken(string token)
        {
            return ValidateJwtToken(token, validateLifetime: true);
        }

        // Check if the refresh token is valid
        public async Task<bool> IsRefreshTokenValidAsync(string token)
        {
            return await _unitOfWork.RefreshTokens.IsTokenValid(token);
        }

        // Revoke all refresh tokens for a user
        public async Task<bool> RevokeAllRefreshTokensForUserAsync(int userId)
        {
            return await _unitOfWork.RefreshTokens.RevokeAllRefreshTokensForUserAsync(userId);
        }
        public async Task<bool> RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _unitOfWork.RefreshTokens.GetValidTokenAsync(token);
            if (refreshToken == null) return false;

            await _unitOfWork.RefreshTokens.RevokeTokenAsync(refreshToken.Id, "system",
            "User session cleanup");
            return true;
        }
        // Generate JWT token with specific expiration
        private string GenerateJwtToken(int userId, int expirationHours)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expirationHours),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Validate JWT token and extract user ID
        private int? ValidateJwtToken(string token, bool validateLifetime)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = validateLifetime,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate JWT token.");
                return null;
            }

        }
        public async Task CleanupOldTokensAsync(int userId)
        {
            // Faqat oxirgi 5 ta tokenni saqlab, qolganlarini o'chirish
            var tokens = await _unitOfWork.RefreshTokens
                .GetUserRefreshTokensAsync(userId);

            await  _unitOfWork.RefreshTokens.RemoveExpiredTokensAsync(tokens);
            await _unitOfWork.CompleteAsync();
        }
    }
}

