using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        [HttpPost("validate-email-verification-token")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Email verification tokenni tekshirish", Description = "Token yaroqliligini tekshiradi")]
        public IActionResult ValidateEmailVerificationToken([FromQuery] string token)
        {
            var userId = _tokenService.ValidateEmailVerificationToken(token);
            if (userId == null)
                return Unauthorized(ApiResponse<string>.Failure("Token noto‘g‘ri yoki muddati o‘tgan."));

            return Ok(ApiResponse<TokenValidationResultDto>.SuccessResponse(
                                  new TokenValidationResultDto { UserId = userId.Value }, "Token yaroqli"));
        }

        [HttpPost("validate-password-reset-token")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Parol tiklash tokenini tekshirish", Description = "Token yaroqliligini tekshiradi")]
        public IActionResult ValidatePasswordResetToken([FromQuery] string token)
        {
            var userId = _tokenService.ValidatePasswordResetToken(token);
            if (userId == null)
                return Unauthorized(ApiResponse<string>.Failure("Token noto‘g‘ri yoki muddati o‘tgan."));

            return Ok(ApiResponse<TokenValidationResultDto>.SuccessResponse(
                            new TokenValidationResultDto { UserId = userId.Value }, "Token yaroqli"));
        }

    }
}
