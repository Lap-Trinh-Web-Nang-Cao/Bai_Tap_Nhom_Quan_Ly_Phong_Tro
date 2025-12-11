using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Bảo vệ nghiêm ngặt
    public class UserRoleController : ControllerBase
    {
        private readonly INguoiDungVaiTroService _service;

        public UserRoleController(INguoiDungVaiTroService service)
        {
            _service = service;
        }

        // Lấy danh sách Role của 1 User
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserRoles(Guid userId)
        {
            var roles = await _service.GetRolesByUserIdAsync(userId);
            return Ok(roles);
        }

        // Gán Role cho User
        [HttpPost]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _service.AddRoleToUserAsync(request);
            if (!success) return BadRequest("Người dùng đã có vai trò này rồi.");

            return Ok(new { message = "Phân quyền thành công!" });
        }

        // Gỡ Role khỏi User
        // DELETE: api/UserRole?userId=...&roleId=...
        [HttpDelete]
        public async Task<IActionResult> RemoveRole([FromQuery] Guid userId, [FromQuery] int roleId)
        {
            var success = await _service.RemoveRoleFromUserAsync(userId, roleId);
            if (!success) return NotFound("Không tìm thấy thông tin phân quyền này.");

            return Ok(new { message = "Đã gỡ quyền thành công." });
        }
    }
}
