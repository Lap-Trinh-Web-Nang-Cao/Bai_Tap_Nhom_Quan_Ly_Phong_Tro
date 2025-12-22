using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Toàn bộ Controller này yêu cầu đăng nhập
    public class DatPhongController : ControllerBase
    {
        private readonly IDatPhongService _service;

        public DatPhongController(IDatPhongService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(id);
        }

        // 1. Người thuê: Đặt phòng
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDatPhongRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _service.CreateBookingAsync(request, GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Ví dụ lỗi trùng phòng
            }
        }

        // 2. Người thuê: Xem lịch sử đặt của mình
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var result = await _service.GetMyBookingsAsync(GetUserId());
            return Ok(result);
        }

        // 3. Chủ trọ: Xem danh sách người khác đặt phòng mình
        [HttpGet("landlord-requests")]
        public async Task<IActionResult> GetLandlordRequests()
        {
            // API này trả về các đơn mà User hiện tại ĐÓNG VAI TRÒ LÀ CHỦ TRỌ
            var result = await _service.GetRequestsForLandlordAsync(GetUserId());
            return Ok(result);
        }

        // 4. Chủ trọ: Duyệt đơn (status = 2) hoặc Từ chối (status = 3)
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] int status)
        {
            var success = await _service.UpdateStatusAsync(id, status, GetUserId());
            if (!success) return BadRequest("Không tìm thấy đơn hoặc bạn không phải chủ trọ.");

            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }
    }
}
