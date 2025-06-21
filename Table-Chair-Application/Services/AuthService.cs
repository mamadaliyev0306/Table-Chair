using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Emails;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.PasswordHash;
using Table_Chair_Application.Repositorys;
using Table_Chair_Application.Repositorys.InterfaceRepositorys;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Application.Settings.Table_Chair_Application.Settings;
using Table_Chair_Application.Tokens;
using Table_Chair_Entity.Enums;
using Table_Chair_Entity.Models;

namespace Table_Chair_Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDistributedCache _distributedCache;
        private const string VerificationPrefix = "verify:email:";
        private const string RateLimitPrefix = "rate:email:";

        private const string TokenBlacklistPrefix = "blacklist:token:";
        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailService emailService,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ILogger<AuthService> logger,
            IConfiguration configuration,
            IConnectionMultiplexer connectionMultiplexer,
            IDistributedCache distributedCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
            _distributedCache = distributedCache;
            _redis = connectionMultiplexer;
            var jwtSettingsSection = configuration.GetSection("Jwt");
            if (jwtSettingsSection == null)
            {
                throw new InvalidOperationException("Jwt settings section is missing in the configuration.");
            }

            _jwtSettings = jwtSettingsSection.Get<JwtSettings>() ?? throw new InvalidOperationException("Jwt settings are not properly configured.");
        }

        public async Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Validation checks
                await ValidateRegistrationInput(registerDto);

                // Rate limiting
                if (!await CanSendVerificationEmail(registerDto.Email))
                {
                    throw new AppException("Too many requests. Please wait before trying again.");
                }

                // Create user
                var user = CreateUserFromDto(registerDto);

                // Generate verification code
                var verificationCode = GenerateRandomCode();
                await StoreVerificationData(user, verificationCode);

                // Send email
                await _emailService.SendVerificationEmail(user.Email, verificationCode);

                await transaction.CommitAsync();
                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Registration failed for {Email}", registerDto.Email);
                throw;
            }
        }

        public async Task<bool> VerifyEmailAsync(string email, string code)
        {
            var db = _redis.GetDatabase();
            var verificationJson = await db.StringGetAsync($"{VerificationPrefix}{email}");

            if (verificationJson.IsNullOrEmpty)
            {
                _logger.LogWarning("Verification data not found for {Email}", email);
                return false;
            }

            var verificationData = JsonSerializer.Deserialize<VerificationData>(verificationJson.ToString());
            if (verificationData == null)
            {
                _logger.LogWarning("Failed to deserialize verification data for {Email}", email);
                return false;
            }

            // Validate code
            if (!ValidateVerificationCode(verificationData, code))
            {
                _logger.LogWarning("Invalid verification code for {Email}", email);
                return false;
            }

            // Activate user
            var user = verificationData.User;
            user.IsActive = true;
            user.EmailVerified = true;
            user.EmailVerificationTokenExpires = DateTime.UtcNow.AddMinutes(15);
            user.LastPasswordChangeDate = DateTime.UtcNow;

            // Save to database
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Cleanup
            await db.KeyDeleteAsync($"{VerificationPrefix}{email}");

            _logger.LogInformation("Email verified successfully for {Email}", email);
            return true;
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            // Rate limit tekshirish
            if (!await CanSendVerificationEmail(email))
            {
                throw new AppException("Juda ko'p so'rovlar. Iltimos, biroz kutib turing.");
            }

            var db = _redis.GetDatabase();
            var verificationJson = await db.StringGetAsync($"{VerificationPrefix}{email}");

            if (verificationJson.IsNullOrEmpty)
                return false;

            var verificationData = JsonSerializer.Deserialize<VerificationData>(verificationJson);

            // Yangi kod yaratish
            var newCode = GenerateRandomCode();

            // Redis yangilash
            verificationData.Code = newCode;
            verificationData.CreatedAt = DateTime.UtcNow;

            await db.StringSetAsync(
                $"{VerificationPrefix}{email}",
                JsonSerializer.Serialize(verificationData),
                TimeSpan.FromMinutes(15));

            // Yangi kodni yuborish
            return await _emailService.SendVerificationEmail(email, newCode);
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            // Find user
            var user = await FindUserByLogin(loginDto.Login);

            // Ensure user is not null before proceeding
            if (user == null)
            {
                throw new AppException("Invalid login credentials");
            }

            // Validate credentials
            ValidateUserForLogin(user, loginDto.Password);

            // Generate tokens
            var tokenExpires = loginDto.RememberMe
                ? DateTime.UtcNow.AddDays(30)
                : DateTime.UtcNow.AddHours(2);

            return await GenerateAuthResponseAsync(user, tokenExpires);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // Validate token
            var storedToken = await ValidateRefreshToken(refreshToken);

            // Get user
            var user = await _unitOfWork.Users.GetByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("User not found or inactive for token {Token}", refreshToken);
                throw new AppException("User not found or inactive");
            }

            // Generate new tokens
            var tokenExpires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            var authResponse = await GenerateAuthResponseAsync(user, tokenExpires);

            // Revoke old token
            await RevokeRefreshToken(storedToken);

            _logger.LogInformation("Token refreshed successfully for user {UserId}", user.Id);
            return authResponse;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetValidTokenAsync(refreshToken);

            if (storedToken == null) return false;

            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;

            // Add to blacklist
            await AddToTokenBlacklist(storedToken.Token, storedToken.ExpiresAt);

            _unitOfWork.RefreshTokens.Update(storedToken);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Token revoked: {Token}", refreshToken);
            return true;
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var tokenEntity = await _unitOfWork.RefreshTokens.GetValidTokenAsync(refreshToken);

            if (tokenEntity != null)
            {
                await RevokeRefreshToken(tokenEntity);
                _logger.LogInformation("User logged out. Token revoked: {Token}", refreshToken);
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(email);
                if (user == null) return false;

                // Parolni tiklash tokeni yaratish
                var resetToken = _tokenService.GeneratePasswordResetToken(user.Id);
                user.ResetPasswordToken = resetToken;
                user.ResetPasswordTokenExpires = DateTime.UtcNow.AddMinutes(_jwtSettings.PasswordResetTokenExpirationMinutes);

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                // Email yuborish
                await _emailService.SendPasswordResetAsync(user.Email, user.Username, resetToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Parolni unutish jarayonida xato yuz berdi");
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(resetPasswordDto.Email);
                if (user == null ||
                    user.ResetPasswordToken != resetPasswordDto.Token ||
                    user.ResetPasswordTokenExpires < DateTime.UtcNow)
                    return false;

                // Yangi parolni o'rnatish
                user.PasswordHash = _passwordHasher.HashPassword(resetPasswordDto.NewPassword);
                user.ResetPasswordToken = null;
                user.ResetPasswordTokenExpires = null;
                user.LastPasswordChangeDate = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                // Barcha aktiv tokenlarni bekor qilish
                await _tokenService.RevokeAllRefreshTokensForUserAsync(user.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Parolni tiklashda xato yuz berdi");
                throw;
            }
        }
        #region Private Methods

        private async Task ValidateRegistrationInput(UserRegisterDto registerDto)
        {
            if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
                throw new AppException("Username is already taken");

            if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
                throw new AppException("Email is already registered");

            if (await _unitOfWork.Users.PhoneExistsAsync(registerDto.PhoneNumber))
                throw new AppException("Phone number is already registered");
        }

        private User CreateUserFromDto(UserRegisterDto registerDto)
        {
            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);
            user.Role = Table_Chair_Entity.Enums.Role.Customer;
            user.IsActive = false;
            user.EmailVerified = false;
            return user;
        }

        private async Task StoreVerificationData(User user, string verificationCode)
        {
            var verificationData = new VerificationData
            {
                User = user,
                Code = verificationCode,
                CreatedAt = DateTime.UtcNow
            };

            var db = _redis.GetDatabase();
            await db.StringSetAsync(
                $"{VerificationPrefix}{user.Email}",
                JsonSerializer.Serialize(verificationData),
                TimeSpan.FromMinutes(15));
        }

        private async Task<User?> FindUserByLogin(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                return null; // Handle null or empty login input gracefully
            }

            return login.Contains('@')
                ? await _unitOfWork.Users.GetByEmailAsync(login)
                : await _unitOfWork.Users.GetByUsernameAsync(login);
        }

        private void ValidateUserForLogin(User user, string password)
        {
            if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
                throw new AppException("Invalid login credentials");

            if (!user.IsActive)
                throw new AppException("User account is not active");

            if (!user.EmailVerified)
                throw new AppException("Please verify your email address");
        }

        private async Task<RefreshToken> ValidateRefreshToken(string refreshToken)
        {
            var db = _redis.GetDatabase();
            if (await db.KeyExistsAsync($"{TokenBlacklistPrefix}{refreshToken}"))
            {
                throw new AppException("This token has been revoked");
            }
            var storedToken = await _unitOfWork.RefreshTokens.GetValidTokenAsync(refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.IsExpired)
            {
                _logger.LogWarning("Invalid refresh token: {Token}", refreshToken);
                throw new AppException("Invalid refresh token");
            }

            return storedToken;
        }

        private async Task RevokeRefreshToken(RefreshToken token)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;

            // Add to blacklist
            await AddToTokenBlacklist(token.Token, token.ExpiresAt);

            _unitOfWork.RefreshTokens.Update(token);
            await _unitOfWork.CompleteAsync();
        }

        private async Task AddToTokenBlacklist(string token, DateTime expiresAt)
        {
            var db = _redis.GetDatabase();
            var ttl = expiresAt - DateTime.UtcNow;
            if (ttl > TimeSpan.Zero)
            {
                await db.StringSetAsync(
                    $"{TokenBlacklistPrefix}{token}",
                    "revoked",
                    ttl);
            }
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user, DateTime tokenExpires)
        {
            var accessToken = _tokenService.GenerateAccessToken(_mapper.Map<UserResponseDto>(user));
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.CompleteAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = tokenExpires,
                UserResponse = _mapper.Map<UserResponseDto>(user)
            };
        }

        private string GenerateRandomCode()
        {
            return new Random().Next(100000, 999999).ToString(); // 6-digit code
        }

        private bool ValidateVerificationCode(VerificationData verificationData, string code)
        {
            return verificationData.Code == code &&
                   verificationData.CreatedAt.AddMinutes(15) > DateTime.UtcNow;
        }

        private async Task<bool> CanSendVerificationEmail(string email)
        {
            var db = _redis.GetDatabase();
            var key = $"{RateLimitPrefix}{email}";
            var current = await db.StringIncrementAsync(key);

            if (current == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromHours(1));
            }

            return current <= 5; // Max 5 attempts per hour
        }

    }
    #endregion
}