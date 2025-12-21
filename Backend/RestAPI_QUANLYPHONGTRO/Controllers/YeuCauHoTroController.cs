using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        // === ADMIN ENDPOINTS (AllowAnonymous for admin panel) ===
        
        /// <summary>
        /// Admin: Lấy tất cả yêu cầu hỗ trợ
        /// </summary>
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }
        
        /// <summary>
        /// Admin: Lấy chi tiết yêu cầu
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var yeuCau = await _service.GetByIdAsync(id);
            if (yeuCau == null)
            {
                return NotFound(new { message = "Không tìm thấy yêu cầu hỗ trợ" });
            }
            return Ok(yeuCau);
        }
        
        /// <summary>
        /// Admin: Lấy thống kê yêu cầu hỗ trợ
        /// </summary>
        [HttpGet("statistics")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = await _service.GetStatisticsAsync();
            return Ok(stats);
        }
        
        /// <summary>
        /// Admin: Cập nhật trạng thái yêu cầu
        /// </summary>
        [HttpPut("admin-status/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminUpdateStatus(Guid id, [FromQuery] string status)
        {
            try
            {
                var success = await _service.AdminUpdateStatusAsync(id, status);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy yêu cầu" });
                }
                return Ok(new { success = true, message = $"Đã cập nhật trạng thái thành: {status}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // === USER ENDPOINTS (Require authentication) ===
        
        /// <summary>
        /// Người thuê: Tạo yêu cầu mới
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateYeuCauRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _service.CreateAsync(request, userId);

            return Ok(result);
        }

        /// <summary>
        /// Người thuê: Xem danh sách yêu cầu của mình
        /// </summary>
        [HttpGet("my-requests")]
        [Authorize]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = GetUserId();
            var list = await _service.GetMyRequestsAsync(userId);
            return Ok(list);
        }

        /// <summary>
        /// Chủ trọ: Xem danh sách yêu cầu gửi tới mình
        /// </summary>
        [HttpGet("landlord-inbox")]
        [Authorize]
        public async Task<IActionResult> GetLandlordInbox()
        {
            var userId = GetUserId();
            var list = await _service.GetRequestsForLandlordAsync(userId);
            return Ok(list);
        }

        /// <summary>
        /// Chủ trọ: Cập nhật trạng thái (VD: ?status=DangXuLy)
        /// </summary>
        [HttpPut("status/{id}")]
        [Authorize]
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
