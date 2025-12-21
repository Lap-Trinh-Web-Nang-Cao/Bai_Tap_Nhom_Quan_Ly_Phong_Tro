using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class HoSoNguoiDungController : ControllerBase
    {
        private readonly IHoSoNguoiDungService _service;

        public HoSoNguoiDungController(IHoSoNguoiDungService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(idStr);
        }

        // GET: api/hosonguoidung/me
        // Lấy thông tin hồ sơ của chính mình
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetUserId();
            var profile = await _service.GetProfileAsync(userId);

            if (profile == null)
            {
                // Nếu chưa có hồ sơ, trả về 404 hoặc object rỗng tuỳ logic Frontend
                return NotFound(new { message = "Chưa cập nhật hồ sơ cá nhân" });
            }

            return Ok(profile);
        }

        // POST: api/hosonguoidung
        // Tạo mới hoặc Cập nhật hồ sơ
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] HoSoNguoiDungRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _service.UpsertProfileAsync(userId, request);

            return Ok(result);
        }
    }
}
