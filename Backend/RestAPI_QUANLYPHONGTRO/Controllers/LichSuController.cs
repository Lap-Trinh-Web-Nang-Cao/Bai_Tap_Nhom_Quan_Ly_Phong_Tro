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
            if (string.IsNullOrEmpty(idStr)) throw new UnauthorizedAccessException();
            return Guid.Parse(idStr);
        }

        // GET: api/lichsu/nguoithue/{userId}?limit=20
        // USER app currently calls this route
        [HttpGet("nguoithue/{userId}")]
        public async Task<IActionResult> GetByTenantId(Guid userId, [FromQuery] int limit = 20)
        {
            try
            {
                var history = await _service.GetUserHistoryAsync(userId, limit);
                return Ok(new { Success = true, Data = history, Message = "Lấy lịch sử hoạt động thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //1. Xem nhật ký hoạt động của mình
        // GET: api/lichsu/me?limit=20
        [HttpGet("me")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int limit = 20)
        {
            try
            {
                var userId = GetUserId();
                var history = await _service.GetUserHistoryAsync(userId, limit);
                return Ok(new { Success = true, Data = history, Message = "Lấy lịch sử hoạt động thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //2. Xóa lịch sử
        // DELETE: api/lichsu/me
        [HttpDelete("me")]
        public async Task<IActionResult> ClearMyHistory()
        {
            try
            {
                var userId = GetUserId();
                await _service.ClearUserHistoryAsync(userId);
                return Ok(new { Success = true, Message = "Đã xóa toàn bộ lịch sử hoạt động." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
