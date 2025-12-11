using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhGiaPhongController : ControllerBase
    {
        private readonly IDanhGiaPhongService _service;

        public DanhGiaPhongController(IDanhGiaPhongService service)
        {
            _service = service;
        }

        // Helper lấy UserID từ Token
        private Guid GetCurrentUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idStr)) throw new UnauthorizedAccessException();
            return Guid.Parse(idStr);
        }

        // 1. Xem đánh giá của phòng (Ai cũng xem được -> Không cần Authorize)
        [HttpGet("phong/{phongId}")]
        public async Task<IActionResult> GetByPhong(Guid phongId)
        {
            var list = await _service.GetByPhongIdAsync(phongId);
            return Ok(list);
        }

        // 2. Viết đánh giá (Phải đăng nhập)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateDanhGiaRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = GetCurrentUserId();
                var result = await _service.CreateAsync(request, userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // 3. Xóa đánh giá (Phải đăng nhập)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Lấy Role từ token để biết có phải Admin không
                // Lưu ý: Key của Role trong Claim có thể là "role" hoặc ClaimTypes.Role
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                bool isAdmin = role == "Admin"; // Hoặc so sánh với Enum ID nếu bạn lưu ID trong claim

                var success = await _service.DeleteAsync(id, userId, isAdmin);

                if (!success) return BadRequest("Không tìm thấy đánh giá hoặc bạn không có quyền xóa.");

                return NoContent();
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
