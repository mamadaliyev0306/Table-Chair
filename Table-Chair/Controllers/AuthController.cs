using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.EmailDtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Microsoft.Extensions.Logging;
using Table_Chair.Examples.UserExamples;
using Table_Chair_Application.Responses;
using Table_Chair.Examples.PasswordExamples;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [SwaggerRequestExample(typeof(UserRegisterDto), typeof(UserRegisterDtoExample))]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Tasdiqlash kodi emailga yuborildi"));
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var result = await _authService.VerifyEmailAsync(dto.Email, dto.Code);
            return result
                ? Ok(ApiResponse<string>.SuccessResponse("Email muvaffaqiyatli tasdiqlandi"))
                : BadRequest(ApiResponse<string>.Failure("Noto'g'ri yoki eskirgan kod"));
        }

        [HttpPost("resend")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> ResendVerificationCode([FromBody] SendEmailVerificationRequest dto)
        {
            var result = await _authService.ResendVerificationEmailAsync(dto.Email);
            return result
                ? Ok(ApiResponse<string>.SuccessResponse("Yangi kod yuborildi"))
                : BadRequest(ApiResponse<string>.Failure("Xatolik yuz berdi"));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [SwaggerRequestExample(typeof(UserLoginDto), typeof(UserLoginDtoExample))]
        [SwaggerResponseExample(200, typeof(UserLoginDtoExample))]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
        }

        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest dto)
        {
            var result = await _authService.RevokeRefreshTokenAsync(dto.RefreshToken);
            return Ok(ApiResponse<string>.SuccessResponse("Token bekor qilindi"));
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);
            return Ok(ApiResponse<string>.SuccessResponse("Tizimdan chiqildi"));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(ApiResponse<string>.SuccessResponse("Reset link yuborildi"));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [SwaggerRequestExample(typeof(ResetPasswordDtoExample), typeof(ResetPasswordDtoExample))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Parol yangilandi"));
        }
    }
}


