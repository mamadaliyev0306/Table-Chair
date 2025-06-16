using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Services.InterfaceServices;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        // GET:  https://localhost:7179/api/users/email-exists
        [HttpGet("email-exists")]
        public async Task<IActionResult> EmailExists([FromQuery] string email)
        {
            var exists = await _userService.EmailExistsAsync(email);
            return Ok(exists);
        }
        // GET:  https://localhost:7179/api/users/username-exists
        [HttpGet("username-exists")]
        public async Task<IActionResult> UsernameExists([FromQuery] string username)
        {
            var exists = await _userService.UsernameExistsAsync(username);
            return Ok(exists);
        }
        // GET:  https://localhost:7179/api/users/phone-exists
        [HttpGet("phone-exists")]
        public async Task<IActionResult> PhoneExists([FromQuery] string phone)
        {
            var exists = await _userService.PhoneExistsAsync(phone);
            return Ok(exists);
        }
        // GET:  https://localhost:7179/api/users/profile/userId
        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            try
            {
                var result = await _userService.GetUserProfileAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user profile with ID {UserId}", userId);
                return NotFound(ex.Message);
            }
        }
        // PUT:  https://localhost:7179/api/users/update-profile/userId
        [HttpPut("update-profile/{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UserUpdateDto dto)
        {
            var result = await _userService.UpdateProfileAsync(userId, dto);
            if (result)
                return Ok("Profil yangilandi");

            return NotFound("Foydalanuvchi topilmadi");
        }
        // DELETE:  https://localhost:7179/api/users/softdelete-profile/userId
        [HttpDelete("softdelete-profile/{userId}")]
        public async Task<IActionResult> SoftDeleteProfile(int userId)
        {
            var result = await _userService.DeleteOwnProfileAsync(userId);
            if (result)
                return Ok("Profil o'chirildi");

            return NotFound("Foydalanuvchi topilmadi yoki allaqachon o'chirilgan");
        }
        // DELETE:  https://localhost:7179/api/users/delete-profile/userId
        [HttpDelete("delete-profile/{userId}")]
        public async Task<IActionResult> DeleteProfile(int userId)
        {
            var result = await _userService.DeleteProfileAsync(userId);
            if (result)
                return Ok("Profil to'liq o'chirildi");

            return NotFound("Foydalanuvchi topilmadi");
        }
        // PUT:  https://localhost:7179/api/users/verify-email/userId
        [HttpPut("verify-email/{userId}")]
        public async Task<IActionResult> VerifyEmail(int userId)
        {
            var result = await _userService.VerifyEmailAsync(userId);
            if (result)
                return Ok("Email tasdiqlandi");

            return BadRequest("Email tasdiqlanmadi");
        }
        // GET:  https://localhost:7179/api/users/by-email
        [HttpGet("by-email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "User not found with email {Email}", email);
                return NotFound(ex.Message);
            }
        }
        // GET:  https://localhost:7179/api/users/by-username
        [HttpGet("by-username")]
        public async Task<IActionResult> GetByUsername([FromQuery] string username)
        {
            try
            {
                var user = await _userService.GetByUsernameAsync(username);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "User not found with username {Username}", username);
                return NotFound(ex.Message);
            }
        }
        // GET:  https://localhost:7179/api/users/by-phone
        [HttpGet("by-phone")]
        public async Task<IActionResult> GetByPhone([FromQuery] string phone)
        {
            try
            {
                var user = await _userService.GetByPhoneAsync(phone);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "User not found with phone {Phone}", phone);
                return NotFound(ex.Message);
            }
        }
    }
}
