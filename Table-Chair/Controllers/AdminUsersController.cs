using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;
        private readonly ILogger<AdminUsersController> _logger;

        public AdminUsersController(IAdminUserService adminUserService, ILogger<AdminUsersController> logger)
        {
            _adminUserService = adminUserService;
            _logger = logger;
        }

        // GET: api/adminusers/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Foydalanuvchini ID orqali olish")]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _adminUserService.GetUserByIdAsync(id);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user));
        }

        // GET: api/adminusers
        [HttpGet]
        [SwaggerOperation(Summary = "Foydalanuvchilar ro'yxatini filtrlab olish")]
        [ProducesResponseType(typeof(ApiResponse<AdminUserResponseDto>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter)
        {
            var result = await _adminUserService.GetUsersPaginatedAsync(filter);
            return Ok(ApiResponse<PaginatedList<AdminUserResponseDto>>.SuccessResponse(result));
        }

        // PUT: api/adminusers/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Foydalanuvchini yangilash")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Update(int id, [FromBody] AdminUserUpdateDto dto)
        {
            var updated = await _adminUserService.UpdateUserAsync(id, dto);
            if (!updated)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Foydalanuvchi yangilandi"));
        }

        // DELETE: api/adminusers/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Foydalanuvchini o'chirish (admin tomonidan)")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _adminUserService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Foydalanuvchi o'chirildi"));
        }

        // PATCH: api/adminusers/{id}/status?isActive=true
        [HttpPatch("{id}/status")]
        [SwaggerOperation(Summary = "Foydalanuvchi holatini o'zgartirish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SetStatus(int id, [FromQuery] bool isActive)
        {
            var updated = await _adminUserService.SetUserStatusAsync(id, isActive);
            if (!updated)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Foydalanuvchi holati yangilandi"));
        }

        // PATCH: api/adminusers/{id}/role?newRole=Admin
        [HttpPatch("{id}/role")]
        [SwaggerOperation(Summary = "Foydalanuvchi rolini o'zgartirish")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ChangeRole(int id, [FromQuery] Role newRole)
        {
            var updated = await _adminUserService.ChangeUserRoleAsync(id, newRole);
            if (!updated)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Rol yangilandi"));
        }

        // PATCH: api/adminusers/restore/{id}
        [HttpPatch("restore/{id}")]
        [SwaggerOperation(Summary = "Soft delete qilingan foydalanuvchini tiklash")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Restore(int id)
        {
            var restored = await _adminUserService.RestoreUserAsync(id);
            if (!restored)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi yoki tiklab bo'lmaydi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Foydalanuvchi tiklandi"));
        }

        // DELETE: api/adminusers/self/{id}
        [HttpDelete("self/{id}")]
        [SwaggerOperation(Summary = "Admin o'z profilini o'chirishi (soft delete)")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> DeleteOwnProfile(int id)
        {
            var deleted = await _adminUserService.DeleteOwnProfileAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));

            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Profil soft delete qilindi"));
        }
    }
}

