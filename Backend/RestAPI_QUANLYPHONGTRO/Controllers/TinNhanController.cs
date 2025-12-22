using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Bắt buộc đăng nhập (Tắt để test Fake Login)
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
            if (!string.IsNullOrEmpty(id)) return Guid.Parse(id);

            // Fallback cho Fake Login
            return Guid.Parse("00000000-0000-0000-0000-000000000001");
        }

        // 1. Gửi tin nhắn
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(new { Success = false, Message = "Dữ liệu không hợp lệ" });
            try
            {
                var userId = GetUserId();
                var result = await _service.SendAsync(request, userId);
                return Ok(new { Success = true, Data = result, Message = "Gửi tin nhắn thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // 2. Lấy hội thoại với 1 người dùng cụ thể
        // GET: api/tinnhan/conversation/{otherUserId}
        [HttpGet("conversation/{otherUserId}")]
        public async Task<IActionResult> GetConversation(Guid otherUserId)
        {
            var userId = GetUserId();
            var list = await _service.GetConversationAsync(userId, otherUserId);
            return Ok(new { Success = true, Data = list });
        }

        // 3. Đánh dấu đã đọc (Khi user mở khung chat lên)
        // PUT: api/tinnhan/read/{otherUserId}
        [HttpPut("read/{otherUserId}")]
        public async Task<IActionResult> MarkRead(Guid otherUserId)
        {
            var userId = GetUserId();
            await _service.MarkAsReadAsync(userId, otherUserId);
            return Ok(new { Success = true, Message = "Đã đánh dấu đã đọc." });
        }

        // 4. Lấy danh sách các cuộc hội thoại
        [HttpGet("my-conversations")]
        public async Task<IActionResult> GetMyConversations()
        {
            var userId = GetUserId();
            var result = await _service.GetMyConversationsAsync(userId);
            return Ok(new { Success = true, Data = result });
        }
    }
}
