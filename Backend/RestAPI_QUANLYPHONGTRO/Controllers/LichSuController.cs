using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class LichSuController : ControllerBase
    {
        private readonly ILichSuService _service;

        public LichSuController(ILichSuService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(idStr);
        }

        // 1. Xem nhật ký hoạt động của mình
        // GET: api/LichSu/me?limit=50
        [HttpGet("me")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int limit = 20)
        {
            var userId = GetUserId();
            var history = await _service.GetUserHistoryAsync(userId, limit);
            return Ok(history);
        }

        // 2. Xóa lịch sử (Dọn dẹp nhật ký)
        // DELETE: api/LichSu/me
        [HttpDelete("me")]
        public async Task<IActionResult> ClearMyHistory()
        {
            var userId = GetUserId();
            await _service.ClearUserHistoryAsync(userId);
            return Ok(new { message = "Đã xóa toàn bộ lịch sử hoạt động." });
        }

        // (Tuỳ chọn) API ghi log từ Frontend
        // Dùng khi Client muốn log hành động như "Click nút X", "Xem trang Y"
        // POST: api/LichSu/log
        /*
        [HttpPost("log")]
        public async Task<IActionResult> LogAction([FromBody] LogRequest request) 
        {
             var userId = GetUserId();
             await _service.AddLogAsync(userId, request.HanhDong, request.TenBang, request.RecordId, null);
             return Ok();
        }
        */
    }
}
