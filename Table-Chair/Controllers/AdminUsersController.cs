using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using Table_Chair_Application.Dtos;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Responses;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

            // Replace the problematic line with the following:
            _logger.LogInformation("GetAll called with filters: {Filter}", JsonSerializer.Serialize(filter, new JsonSerializerOptions { WriteIndented = false }));
            var result = await _adminUserService.GetUsersPaginatedAsync(filter);
            return Ok(ApiResponse<PaginatedList<AdminUserResponseDto>>.SuccessResponse(result));
        }

    

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Foydalanuvchini yangilash")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Update(int id, [FromBody] AdminUserUpdateDto dto)
        {
            var updated = await _adminUserService.UpdateUserAsync(id, dto);
            if (!updated)
            {
                _logger.LogWarning("Update failed. User {UserId} not found", id);
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));
            }

            
            _logger.LogInformation("Admin updated user {UserId} with data: {Dto}", id, dto);
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
            {
                _logger.LogWarning("Delete failed. User {UserId} not found", id);
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));
            }

            _logger.LogWarning("Admin deleted user {UserId}", id);
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
            {
                _logger.LogWarning("SetStatus failed. User {UserId} not found", id);
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));
            }

            _logger.LogInformation("User {UserId} status set to {IsActive}", id, isActive);
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
            {
                _logger.LogWarning("Failed to change role. User {UserId} not found", id);
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));
            }

            _logger.LogInformation("Role of User {UserId} changed to {NewRole}", id, newRole);
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
            {
                _logger.LogWarning("Restore failed. User {UserId} not found or cannot be restored", id);
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi yoki tiklab bo'lmaydi"));
            }

            _logger.LogInformation("User {UserId} restored by admin", id);
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
            {
                _logger.LogWarning("DeleteOwnProfile failed. User {UserId} not found", id);
                return NotFound(ApiResponse<string>.Failure("Foydalanuvchi topilmadi"));
            }

            _logger.LogInformation("Admin user {UserId} soft-deleted own profile", id);
            return Ok(ApiResponse<string>.SuccessResponse(string.Empty, "Profil soft delete qilindi"));
        }
        // GET: api/admin/users/deleted
        [HttpGet("deleted")]
        [SwaggerOperation(Summary = "O'chirilgan foydalanuvchilar ro'yxati")]
        [ProducesResponseType(typeof(ApiResponse<List<AdminUserResponseDto>>), 200)]
        public async Task<IActionResult> GetDeletedUsers()
        {
            var result = await _adminUserService.GetDeletedUsersAsync();
            return Ok(ApiResponse<List<AdminUserResponseDto>>.SuccessResponse(result));
        }


        #region Statistics & Reports

        // GET: api/admin/users/stats/count
        [HttpGet("stats/count")]
        [SwaggerOperation(Summary = "Foydalanuvchilar soni statistikasi")]
        [ProducesResponseType(typeof(ApiResponse<UserCountStatsDto>), 200)]
        public async Task<IActionResult> GetUserCountStats()
        {
            var result = await _adminUserService.GetUserCountStatsAsync();
            return Ok(ApiResponse<UserCountStatsDto>.SuccessResponse(result));
        }

        // GET: api/admin/users/stats/activity
        [HttpGet("stats/activity")]
        [SwaggerOperation(Summary = "Foydalanuvchilar faolligi statistikasi")]
        [ProducesResponseType(typeof(ApiResponse<List<UserActivityStatsDto>>), 200)]
        public async Task<IActionResult> GetUserActivityStats([FromQuery] DateRangeDto dateRange)
        {
            _logger.LogInformation("User activity stats requested for range {Start} - {End}", dateRange.StartDate, dateRange.EndDate);
            var result = await _adminUserService.GetUserActivityStatsAsync(dateRange);
            return Ok(ApiResponse<List<UserActivityStatsDto>>.SuccessResponse(result));
        }

        #endregion

    }
}

