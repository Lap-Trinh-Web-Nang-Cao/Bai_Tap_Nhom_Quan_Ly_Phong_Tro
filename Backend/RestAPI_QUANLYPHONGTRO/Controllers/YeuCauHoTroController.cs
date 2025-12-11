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
    public class YeuCauHoTroController : ControllerBase
    {
        private readonly IYeuCauHoTroService _service;

        public YeuCauHoTroController(IYeuCauHoTroService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id)) throw new UnauthorizedAccessException();
            return Guid.Parse(id);
        }

        // 1. Người thuê: Tạo yêu cầu
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateYeuCauRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _service.CreateAsync(request, userId);

            return Ok(result);
        }

        // 2. Người thuê: Xem danh sách yêu cầu của mình
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = GetUserId();
            var list = await _service.GetMyRequestsAsync(userId);
            return Ok(list);
        }

        // 3. Chủ trọ: Xem danh sách yêu cầu gửi tới mình
        [HttpGet("landlord-inbox")]
        public async Task<IActionResult> GetLandlordInbox()
        {
            var userId = GetUserId();
            var list = await _service.GetRequestsForLandlordAsync(userId);
            return Ok(list);
        }

        // 4. Chủ trọ: Cập nhật trạng thái (VD: ?status=DangXuLy)
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] string status)
        {
            try
            {
                var userId = GetUserId();
                var success = await _service.UpdateStatusAsync(id, status, userId);

                if (!success) return NotFound("Không tìm thấy yêu cầu.");

                return Ok(new { message = $"Đã cập nhật trạng thái thành: {status}" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Lỗi không có quyền
            }
        }
    }
}
