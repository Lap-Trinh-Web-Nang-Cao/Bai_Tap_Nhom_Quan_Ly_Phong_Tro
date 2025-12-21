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
            if (string.IsNullOrEmpty(id)) throw new UnauthorizedAccessException();
            return Guid.Parse(id);
        }

        //1. Người thuê: Đặt phòng
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDatPhongRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Errors = ModelState, Message = "Dữ liệu không hợp lệ" });

            try
            {
                var result = await _service.CreateBookingAsync(request, GetUserId());
                return Ok(new { Success = true, Data = result, Message = "Đặt lịch xem phòng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //2. Người thuê: Xem lịch sử đặt của mình
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            try
            {
                var result = await _service.GetMyBookingsAsync(GetUserId());
                return Ok(new { Success = true, Data = result, Message = "Lấy danh sách lịch hẹn thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //2.1 Người thuê: Xem lịch sử đặt của một người thuê cụ thể
        [HttpGet("nguoithue/{userId}")]
        public async Task<IActionResult> GetByTenantId(Guid userId)
        {
            try
            {
                var result = await _service.GetMyBookingsAsync(userId);
                return Ok(new { Success = true, Data = result, Message = "Lấy danh sách lịch hẹn thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //3. Chủ trọ: Xem danh sách người khác đặt phòng mình
        [HttpGet("landlord-requests")]
        public async Task<IActionResult> GetLandlordRequests()
        {
            try
            {
                var result = await _service.GetRequestsForLandlordAsync(GetUserId());
                return Ok(new { Success = true, Data = result, Message = "Lấy danh sách yêu cầu đặt phòng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        //4. Chủ trọ: Duyệt đơn (status =2) hoặc Từ chối (status =3)
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] int status)
        {
            try
            {
                var success = await _service.UpdateStatusAsync(id, status, GetUserId());
                if (!success)
                    return BadRequest(new { Success = false, Message = "Không tìm thấy đơn hoặc bạn không phải chủ trọ." });

                return Ok(new { Success = true, Message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
