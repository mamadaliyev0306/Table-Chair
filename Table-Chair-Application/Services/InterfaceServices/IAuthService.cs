using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;

namespace Table_Chair_Application.Services.InterfaceServices
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> VerifyEmailAsync(string email, string code);
        Task<bool> ResendVerificationEmailAsync(string email);
        Task LogoutAsync(string refreshToken);
    }
}
