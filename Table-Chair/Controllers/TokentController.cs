using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Models;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Barcha Token controller endpointlar uchun token talab qilinadi
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        // POST:  https://localhost:7179/api/token/generate-access-token
        [HttpPost("generate-access-token")]
        public IActionResult GenerateAccessToken([FromBody] UserResponseDto refreshTokenDto)
        {
            if (refreshTokenDto == null)
            {
                return BadRequest("Foydalanuvchi ma'lumotlari berilmagan.");
            }

            var accessToken = _tokenService.GenerateAccessToken(refreshTokenDto);
            return Ok(new { AccessToken = accessToken });
        }
        // POST:  https://localhost:7179/api/token/generate-refresh-token
        [HttpPost("generate-refresh-token")]
        public IActionResult GenerateRefreshToken()
        {
            var refreshToken = _tokenService.GenerateRefreshToken();
            return Ok(new { RefreshToken = refreshToken });
        }
        // POST:  https://localhost:7179/api/token/generate-email-verification-token
        [HttpPost("generate-email-verification-token")]
        public IActionResult GenerateEmailVerificationToken([FromBody] int userId)
        {
            var emailVerificationToken = _tokenService.GenerateEmailVerificationToken(userId);
            return Ok(new { EmailVerificationToken = emailVerificationToken });
        }
        // POST:  https://localhost:7179/api/token/generate-password-reset-token
        [HttpPost("generate-password-reset-token")]
        public IActionResult GeneratePasswordResetToken([FromBody] int userId)
        {
            var passwordResetToken = _tokenService.GeneratePasswordResetToken(userId);
            return Ok(new { PasswordResetToken = passwordResetToken });
        }
        // POST:  https://localhost:7179/api/token/validate-access-token
        [HttpPost("validate-access-token")]
        public IActionResult ValidateAccessToken([FromBody] string token)
        {
            var userId = _tokenService.ValidateAccessToken(token);
            if (userId == null)
            {
                return Unauthorized("Token noto‘g‘ri yoki muddati o‘tgan.");
            }
            return Ok(new { UserId = userId });
        }
        // POST:  https://localhost:7179/api/token/validate-email-verification-token
        [HttpPost("validate-email-verification-token")]
        public IActionResult ValidateEmailVerificationToken([FromBody] string token)
        {
            var userId = _tokenService.ValidateEmailVerificationToken(token);
            if (userId == null)
            {
                return Unauthorized("Token noto‘g‘ri yoki muddati o‘tgan.");
            }
            return Ok(new { UserId = userId });
        }
        // POST:  https://localhost:7179/api/token/validate-password-reset-token
        [HttpPost("validate-password-reset-token")]
        public IActionResult ValidatePasswordResetToken([FromBody] string token)
        {
            var userId = _tokenService.ValidatePasswordResetToken(token);
            if (userId == null)
            {
                return Unauthorized("Token noto‘g‘ri yoki muddati o‘tgan.");
            }
            return Ok(new { UserId = userId });
        }
        // POST:  https://localhost:7179/api/token/is-refresh-token-valid
        [HttpPost("is-refresh-token-valid")]
        public async Task<IActionResult> IsRefreshTokenValidAsync([FromBody] string token)
        {
            var isValid = await _tokenService.IsRefreshTokenValidAsync(token);
            if (!isValid)
            {
                return Unauthorized("Refresh token noto‘g‘ri yoki muddati o‘tgan.");
            }
            return Ok(new { IsValid = isValid });
        }
        // POST:  https://localhost:7179/api/token/revoke-all-refresh-tokens
        [HttpPost("revoke-all-refresh-tokens")]
        public async Task<IActionResult> RevokeAllRefreshTokensAsync([FromBody] int userId)
        {
            var result = await _tokenService.RevokeAllRefreshTokensForUserAsync(userId);
            if (!result)
            {
                return BadRequest("Refresh tokenlar bekor qilindi.");
            }
            return Ok("Barcha refresh tokenlar bekor qilindi.");
        }
    }
}


