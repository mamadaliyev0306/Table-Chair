using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Table_Chair_Application.Dtos.UserDtos;
using Table_Chair_Application.Services.InterfaceServices;
using Table_Chair_Entity.Enums;

namespace Table_Chair.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;
        private readonly ILogger<AdminUsersController> _logger;
        public AdminUsersController(IAdminUserService adminUserService, ILogger<AdminUsersController> logger)
        {
            _adminUserService = adminUserService;
            _logger = logger;
        }
        // GET: https://localhost:7179/api/adminusers/GetById/id
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _adminUserService.GetUserByIdAsync(id);
            return Ok(result);
        }

        // GET:https://localhost:7179/api/adminusers/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterDto filter)
        {
            var result = await _adminUserService.GetUsersPaginatedAsync(filter);
            return Ok(result);
        }

        // PUT: https://localhost:7179/api/adminusers/Update/id
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminUserUpdateDto dto)
        {
            var success = await _adminUserService.UpdateUserAsync(id, dto);
            return success ? Ok("Foydalanuvchi yangilandi") : NotFound("Foydalanuvchi topilmadi");
        }

        // DELETE: https://localhost:7179/api/adminusers/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _adminUserService.DeleteUserAsync(id);
            return success ? Ok("Foydalanuvchi o‘chirildi") : NotFound("Foydalanuvchi topilmadi");
        }

        // PATCH: https://localhost:7179/api/adminusers/id/status?isActive=true
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> SetStatus(int id, [FromQuery] bool isActive)
        {
            var success = await _adminUserService.SetUserStatusAsync(id, isActive);
            return success ? Ok("Holat yangilandi") : NotFound("Foydalanuvchi topilmadi");
        }

        // PATCH: https://localhost:7179/api/adminusers/id/role?newRole=Admin
        [HttpPatch("{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromQuery] Role newRole)
        {
            var success = await _adminUserService.ChangeUserRoleAsync(id, newRole);
            return success ? Ok("Rol yangilandi") : NotFound("Foydalanuvchi topilmadi");
        }

        // PATCH: https://localhost:7179/api/adminusers/restore/id
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _adminUserService.RestoreUserAsync(id);
            return success ? Ok("Foydalanuvchi tiklandi") : NotFound("Foydalanuvchi topilmadi yoki allaqachon tiklangan");
        }

        // DELETE: https://localhost:7179/api/adminusers/self/id
        [HttpDelete("self/{id}")]
        public async Task<IActionResult> DeleteOwnProfile(int id)
        {
            var success = await _adminUserService.DeleteOwnProfileAsync(id);
            return success ? Ok("Profil soft delete qilindi") : NotFound("Foydalanuvchi topilmadi");
        }
    }
}
