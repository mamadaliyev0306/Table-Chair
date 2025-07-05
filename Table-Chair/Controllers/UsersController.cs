using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Exceptions;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("email-exists")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Email mavjudligini tekshirish")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> EmailExists([FromQuery] string email)
        {
            var exists = await _userService.EmailExistsAsync(email);
            return Ok(ApiResponse<bool>.SuccessResponse(exists));
        }

        [HttpGet("username-exists")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Username mavjudligini tekshirish")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> UsernameExists([FromQuery] string username)
        {
            var exists = await _userService.UsernameExistsAsync(username);
            return Ok(ApiResponse<bool>.SuccessResponse(exists));
        }

        [HttpGet("phone-exists")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Telefon raqami mavjudligini tekshirish")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> PhoneExists([FromQuery] string phone)
        {
            var exists = await _userService.PhoneExistsAsync(phone);
            return Ok(ApiResponse<bool>.SuccessResponse(exists));
        }
        [HttpGet("profile/{userId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Foydalanuvchi profilini olish")]
        [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
        public async Task<IActionResult> GetProfile(int userId)
        {
            try
            {
                var profile = await _userService.GetUserProfileAsync(userId);
                return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<string>.FailResponse(ex.Message));
            }
        }

        [HttpPut("update-profile/{userId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Foydalanuvchi profilini yangilash")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UserUpdateDto dto)
        {
            var updated = await _userService.UpdateProfileAsync(userId, dto);
            if (!updated)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Profil muvaffaqiyatli yangilandi"));
        }

        [HttpDelete("softdelete-profile/{userId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Foydalanuvchi profilini soft delete qilish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SoftDeleteProfile(int userId)
        {
            var deleted = await _userService.DeleteOwnProfileAsync(userId);
            if (!deleted)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi yoki allaqachon o‘chirilgan"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Profil soft delete qilindi"));
        }

        [HttpDelete("delete-profile/{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Foydalanuvchini to'liq o'chirish (admin)")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteProfile(int userId)
        {
            var result = await _userService.DeleteProfileAsync(userId);
            if (!result)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Profil to‘liq o‘chirildi"));
        }

        [HttpPut("verify-email/{userId}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Emailni tasdiqlash")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> VerifyEmail(int userId)
        {
            var verified = await _userService.VerifyEmailAsync(userId);
            if (!verified)
                return BadRequest(ApiResponse<string>.Failure("Email tasdiqlanmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Email muvaffaqiyatli tasdiqlandi"));
        }

        [HttpGet("by-email")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user));
        }

        [HttpGet("by-username")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUsername([FromQuery] string username)
        {
            try
            {
                var user = await _userService.GetByUsernameAsync(username);
                return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<UserResponseDto>.FailResponse(ex.Message));
            }
        }

        [HttpGet("by-phone")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByPhone([FromQuery] string phone)
        {
            var user = await _userService.GetByPhoneAsync(phone);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user));
        }
    }
}

