using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // QUAN TRỌNG: Chỉ Role Admin mới được truy cập
    // (Giả sử trong Token bạn đã lưu Claim Role là "Admin")
    [Authorize(Roles = "Admin")]
    public class HanhDongAdminController : ControllerBase
    {
        private readonly IHanhDongAdminService _service;

        public HanhDongAdminController(IHanhDongAdminService service)
        {
            _service = service;
        }

        // Xem 100 hoạt động gần nhất
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentLogs([FromQuery] int count = 100)
        {
            var logs = await _service.GetLatestLogsAsync(count);
            return Ok(logs);
        }

        // Xem lịch sử của một đối tượng cụ thể (Ví dụ: xem lịch sử sửa xóa của User A)
        [HttpGet("history/{recordId}")]
        public async Task<IActionResult> GetHistoryByRecord(string recordId)
        {
            var logs = await _service.GetLogsByRecordIdAsync(recordId);
            return Ok(logs);
        }
    }
}
