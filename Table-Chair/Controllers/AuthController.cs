using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.AdditionDtos;
using Table_Chair_Application.Dtos.EmailDtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize] // Barcha endpointlar uchun token talab qilinadi
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // faqat Register va Login AllowAnonymous qoldiramiz
        // POST:https://localhost:7179/api/Auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(userRegisterDto);
                return Ok(new
                {
                    Message = "Tasdiqlash kodi emailga yuborildi"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Register Error: " + ex.Message);
                return BadRequest(new {ex.Message });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyDto)
        {
            var result = await _authService.VerifyEmailAsync(verifyDto.Email, verifyDto.Code);
            return result ? Ok(new { Success = true })
                         : BadRequest(new { Success = false, Message = "Noto'g'ri kod" });
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendVerificationCode([FromBody] SendEmailVerificationRequest resendDto)
        {
            var result = await _authService.ResendVerificationEmailAsync(resendDto.Email);
            return result ? Ok(new { Success = true })
                         : BadRequest(new { Success = false, Message = "Xatolik yuz berdi" });
        }

        // POST:https://localhost:7179/api/Auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(userLoginDto);
                _logger.LogInformation($"{result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Login Error: " + ex.Message);
                return BadRequest(new { ex.Message });
            }
        }
        // POST:https://localhost:7179/api/Auth/refresh-token
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Refresh Token Error: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }
        // POST:https://localhost:7179/api/Auth/revoke-token
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var result = await _authService.RevokeRefreshTokenAsync(refreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Revoke Token Error: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }
        // POST:https://localhost:7179/api/Auth/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                var result = await _authService.ForgotPasswordAsync(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Forgot Password Error: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }
        // POST:https://localhost:7179/api/Auth/reset-password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(resetDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Reset Password Error: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}

