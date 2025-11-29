using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // BẮT BUỘC: Chỉ user đã đăng nhập mới vào được
    public class ChuTroThongTinPhapLyController : ControllerBase
    {
        private readonly IChuTroThongTinPhapLyService _service;

        public ChuTroThongTinPhapLyController(IChuTroThongTinPhapLyService service)
        {
            _service = service;
        }

        // Helper để lấy ID từ Token cho gọn code
        private Guid GetCurrentUserId()
        {
            // ClaimTypes.NameIdentifier thường chứa User ID khi cấu hình JWT chuẩn
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng trong Token.");

            return Guid.Parse(userIdString);
        }

        [HttpGet("me")] // Đổi route thành 'me' để lấy thông tin của chính mình
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _service.GetByIdAsync(userId);

                if (result == null) return NotFound("Bạn chưa cập nhật thông tin pháp lý.");
                return Ok(result);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] ChuTroThongTinPhapLy model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // --- ĐOẠN QUAN TRỌNG NHẤT ---
                // Lấy ID thật sự của người đang đăng nhập
                var currentUserId = GetCurrentUserId();

                // Cách 1 (Bảo mật tuyệt đối): Gán đè ID từ token vào model. 
                // Dù client gửi NguoiDungId là gì thì cũng bị ghi đè bằng ID chính chủ.
                model.NguoiDungId = currentUserId;

                // Cách 2 (Kiểm tra chặt chẽ): Nếu client gửi ID khác ID trong token thì báo lỗi
                // if (model.NguoiDungId != currentUserId)
                // {
                //     return StatusCode(403, "Bạn không được phép sửa hồ sơ của người khác!");
                // }
                // -----------------------------

                // Một logic bảo mật phụ: Nếu hồ sơ đã "DaDuyet", user thường không được phép tự ý sửa nữa
                // Bạn có thể check thêm ở đây:
                /*
                var existing = await _service.GetByIdAsync(currentUserId);
                if (existing != null && existing.TrangThaiXacThuc == "DaDuyet")
                {
                     return BadRequest("Hồ sơ đã được duyệt, vui lòng liên hệ Admin để thay đổi.");
                }
                */

                var result = await _service.CreateOrUpdateAsync(model);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Vui lòng đăng nhập lại.");
            }
        }

        // API Admin duyệt thì giữ nguyên, nhưng cần thêm Role Admin
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được gọi
        [HttpPut("approve/{userId}")]
        public async Task<IActionResult> Approve(Guid userId, [FromQuery] string status)
        {
            var result = await _service.ApproveAsync(userId, status);
            if (!result) return NotFound();
            return Ok(new { message = $"Đã cập nhật trạng thái user {userId} thành: {status}" });
        }
    }
}
