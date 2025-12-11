using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập mới được đăng ký Token
    public class TokenThongBaoController : ControllerBase
    {
        private readonly ITokenThongBaoService _service;

        public TokenThongBaoController(ITokenThongBaoService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id)) throw new UnauthorizedAccessException();
            return Guid.Parse(id);
        }

        // 1. Đăng ký Token (Gọi khi Login thành công hoặc khi mở App)
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] DeviceTokenRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            await _service.RegisterTokenAsync(userId, request.Token);

            return Ok(new { message = "Đã đăng ký thiết bị nhận thông báo." });
        }

        // 2. Hủy Token (Gọi khi Logout)
        [HttpDelete]
        public async Task<IActionResult> Unregister([FromBody] DeviceTokenRequest request)
        {
            // API này không cần UserId, chỉ cần biết Token nào cần xóa
            await _service.RemoveTokenAsync(request.Token);
            return Ok(new { message = "Đã hủy đăng ký thiết bị." });
        }
    }
}
