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
            try
            {
                // Validatsiya
                if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
                    throw new AppException("Bu foydalanuvchi nomi band");

                if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
                    throw new AppException("Bu email allaqachon ro'yxatdan o'tgan");

                if (await _unitOfWork.Users.PhoneExistsAsync(registerDto.PhoneNumber))
                    throw new AppException("Bu telefon raqam allaqachon ro'yxatdan o'tgan");

                // Rate limit tekshirish
                if (!await CanSendVerificationEmail(registerDto.Email))
                {
                    throw new AppException("Juda ko'p so'rovlar. Iltimos, biroz kutib turing.");
                }

                // User yaratish (lekin hali saqlamaymiz)
                var user = _mapper.Map<User>(registerDto);
                user.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);
                user.Role = Table_Chair_Entity.Enums.Role.Customer;
                user.IsActive = false;
                user.EmailVerified = false;

                // Tasdiqlash kodi yaratish
                var verificationCode = GenerateRandomCode();

                // Redisga saqlash
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

                // Emailga kod yuborish
                await _emailService.SendVerificationEmail(user.Email, verificationCode);

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ro'yxatdan o'tishda xato yuz berdi");
                throw;
            }
        }

        public async Task<bool> VerifyEmailAsync(string email, string code)
        {
            var db = _redis.GetDatabase();
            var verificationJson = await db.StringGetAsync($"{VerificationPrefix}{email}");

            if (verificationJson.IsNullOrEmpty)
                return false;

            var verificationData = JsonSerializer.Deserialize<VerificationData>(verificationJson);

            // Kodni tekshirish
            if (verificationData.Code != code ||
                DateTime.Parse(verificationData.CreatedAt.ToString()).AddMinutes(15) < DateTime.UtcNow)
            {
                return false;
            }

            // User ma'lumotlarini olish
            var userJson = JsonSerializer.Serialize(verificationData.User);
            var user = JsonSerializer.Deserialize<User>(userJson);
            user.IsActive = true;
            user.EmailVerified = true;
            user.EmailVerificationTokenExpires = DateTime.UtcNow.AddMinutes(15);
            user.LastPasswordChangeDate = DateTime.UtcNow;
            user.EmailVerificationToken = _tokenService.GenerateEmailVerificationToken(user.Id);
            // Databasega saqlash
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Redisdan o'chirish
            await db.KeyDeleteAsync($"{VerificationPrefix}{email}");

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
            verificationData.CreatedAt=DateTime.UtcNow;

            await db.StringSetAsync(
                $"{VerificationPrefix}{email}",
                JsonSerializer.Serialize(verificationData),
                TimeSpan.FromMinutes(15));

            // Yangi kodni yuborish
            return await _emailService.SendVerificationEmail(email, newCode);
        }

        private async Task<bool> CanSendVerificationEmail(string email)
        {
            var db = _redis.GetDatabase();
            var key = $"{RateLimitPrefix}{email}";

            // So'rovlar sonini oshirish
            var current = await db.StringIncrementAsync(key);

            // Agar birinchi marta bo'lsa, muddat belgilash
            if (current == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromHours(1));
            }

            // 1 soatda maksimal 5 marta ruxsat berish
            return current <= 5;
        }


        private string GenerateRandomCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6 raqamli kod
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                // Foydalanuvchini topish
                User? user = loginDto.Login.Contains('@')
                    ? await _unitOfWork.Users.GetByEmailAsync(loginDto.Login)
                    : await _unitOfWork.Users.GetByUsernameAsync(loginDto.Login);

                if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
                    throw new AppException("Login yoki parol noto'g'ri");

                if (!user.IsActive)
                    throw new AppException("Foydalanuvchi hisobi faol emas");

                if (!user.EmailVerified)
                    throw new AppException("Iltimos, email manzilingizni tasdiqlang");

                var tokenExpires = loginDto.RememberMe
                        ? DateTime.UtcNow.AddDays(30)
                         : DateTime.UtcNow.AddHours(2);
                return await GenerateAuthResponseAsync(user,tokenExpires);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tizimga kirishda xato yuz berdi");
                throw;
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Refresh tokenni tekshirish
                var storedToken = await _unitOfWork.RefreshTokens.GetValidTokenAsync(refreshToken);
                if (storedToken == null)
                    throw new AppException("Yaroqsiz refresh token");

                // Foydalanuvchini tekshirish
                var user = await _unitOfWork.Users.GetByIdAsync(storedToken.UserId);
                if (user == null || !user.IsActive)
                    throw new AppException("Foydalanuvchi topilmadi yoki faol emas");

                // Yangi tokenlar generatsiya qilish
                var tokenExpires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
                var authResponse = await GenerateAuthResponseAsync(user, tokenExpires);


                // Eski tokenni bekor qilish
                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;
                _unitOfWork.RefreshTokens.Update(storedToken);

                await _unitOfWork.CompleteAsync();

                return authResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token yangilashda xato yuz berdi");
                throw;
            }
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var storedToken = await _unitOfWork.RefreshTokens.GetValidTokenAsync(refreshToken);
                if (storedToken == null) return false;

                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;
                _unitOfWork.RefreshTokens.Update(storedToken);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tokenni bekor qilishda xato yuz berdi");
                throw;
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
                user.ResetPasswordTokenExpires = DateTime.UtcNow.AddHours(_jwtSettings.PasswordResetTokenExpirationHours);

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

        public async Task LogoutAsync(string refreshToken)
        {
            var tokenEntity = await _unitOfWork.RefreshTokens.GetValidTokenAsync(refreshToken);
            if (tokenEntity != null)
            {
                _unitOfWork.RefreshTokens.Delete(tokenEntity);
                await _unitOfWork.CompleteAsync();
            }
        }


        private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user,DateTime TokenExpires)
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
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                UserResponse = _mapper.Map<UserResponseDto>(user)
            };
        }


    }
}