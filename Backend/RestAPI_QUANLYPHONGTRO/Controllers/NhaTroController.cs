using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaTroController : ControllerBase
    {
        private readonly INhaTroService _service;

        public NhaTroController(INhaTroService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id)) throw new UnauthorizedAccessException();
            return Guid.Parse(id);
        }

        // 1. Lấy danh sách Public (Ai cũng xem được)
        [HttpGet]
        public async Task<IActionResult> GetAllPublic()
        {
            var list = await _service.GetAllActiveAsync();
            return Ok(list);
        }

        // 2. Xem chi tiết (Ai cũng xem được)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // 3. Lấy danh sách CỦA TÔI (Phải đăng nhập)
        [HttpGet("my-houses")]
        [Authorize]
        public async Task<IActionResult> GetMyHouses()
        {
            var userId = GetUserId();
            var list = await _service.GetByOwnerIdAsync(userId);
            return Ok(list);
        }

        // 4. Tạo nhà trọ mới (Phải đăng nhập)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] NhaTroRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _service.CreateAsync(request, userId);

            return CreatedAtAction(nameof(GetDetail), new { id = result.NhaTroId }, result);
        }

        // 5. Cập nhật (Phải đăng nhập & Chính chủ)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] NhaTroRequest request)
        {
            var userId = GetUserId();
            var result = await _service.UpdateAsync(id, request, userId);

            if (result == null) return BadRequest("Không tìm thấy hoặc bạn không có quyền sửa.");

            return Ok(result);
        }

        // 6. Xóa (Admin hoặc Chính chủ)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            // Check Role Admin (nếu bạn có lưu Role trong Token)
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isAdmin = role == "Admin";

            var success = await _service.DeleteAsync(id, userId, isAdmin);

            if (!success) return BadRequest("Không xóa được (Không tìm thấy hoặc không đủ quyền).");

            return NoContent();
        }
    }
}
