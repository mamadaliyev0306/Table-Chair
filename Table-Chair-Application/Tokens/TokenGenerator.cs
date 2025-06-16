using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Tokens
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public TokenGenerator(
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        // Access token generation
        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Updated code to handle potential null reference for 'user.LastName'
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName ?? string.Empty), 
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("phoneNumber", user.PhoneNumber),
                new Claim("emailVerified", user.EmailVerified.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Refresh token generation
        public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RefreshTokenExpirationDays"])),
                UserId = userId,
                ReplacedByToken = string.Empty
            };

            // Check if there is an existing refresh token and update it
            var existingToken = await _unitOfWork.RefreshTokens.GetByIdAsync(userId);
            if (existingToken != null)
            {
                existingToken.ReplacedByToken = refreshToken.Token;
                _unitOfWork.RefreshTokens.Update(existingToken);
            }

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync();
            return refreshToken;
        }

        // Email verification token generation
        public async Task<string> GenerateEmailVerificationTokenAsync(int userId)
        {
            var token = Guid.NewGuid().ToString();
            var user = await _unitOfWork.Users.GetByIdAsync(userId); // Use Users repository
            if (user == null)
                throw new NotFoundException("User not found");

            user.EmailVerificationToken = token;  // Store token in User model
            user.EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(1);
            _unitOfWork.Users.Update(user);  // Update User entity
            await _unitOfWork.CompleteAsync();
            return token;
        }

        // Password reset token generation
        public async Task<string> GeneratePasswordResetTokenAsync(int userId)
        {
            var token = Guid.NewGuid().ToString();
            var user = await _unitOfWork.Users.GetByIdAsync(userId); // Use Users repository
            if (user == null)
                throw new NotFoundException("User not found");

            user.ResetPasswordToken = token;  // Store reset token in User model
            user.ResetPasswordTokenExpires = DateTime.UtcNow.AddHours(1);
            _unitOfWork.Users.Update(user);  // Update User entity
            await _unitOfWork.CompleteAsync();
            return token;
        }

        // Get claims from expired token
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"))),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}

