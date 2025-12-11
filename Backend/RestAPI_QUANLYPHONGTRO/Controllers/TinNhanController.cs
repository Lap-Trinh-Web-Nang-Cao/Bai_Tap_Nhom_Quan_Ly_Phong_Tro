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
    public class TinNhanController : ControllerBase
    {
        private readonly ITinNhanService _service;

        public TinNhanController(ITinNhanService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id)) throw new UnauthorizedAccessException();
            return Guid.Parse(id);
        }

        // 1. Gửi tin nhắn
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = GetUserId();
                var result = await _service.SendAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 2. Lấy hội thoại với 1 người dùng cụ thể
        // GET: api/tinnhan/conversation/{otherUserId}
        [HttpGet("conversation/{otherUserId}")]
        public async Task<IActionResult> GetConversation(Guid otherUserId)
        {
            var userId = GetUserId();
            var list = await _service.GetConversationAsync(userId, otherUserId);
            return Ok(list);
        }

        // 3. Đánh dấu đã đọc (Khi user mở khung chat lên)
        // PUT: api/tinnhan/read/{otherUserId}
        [HttpPut("read/{otherUserId}")]
        public async Task<IActionResult> MarkRead(Guid otherUserId)
        {
            var userId = GetUserId();
            await _service.MarkAsReadAsync(userId, otherUserId);
            return Ok(new { message = "Đã đánh dấu đã đọc." });
        }
    }
}
