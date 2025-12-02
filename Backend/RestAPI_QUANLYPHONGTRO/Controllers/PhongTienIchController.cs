using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhongTienIchController : ControllerBase
    {
        private readonly IPhongTienIchService _service;

        public PhongTienIchController(IPhongTienIchService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id)) throw new UnauthorizedAccessException();
            return Guid.Parse(id);
        }

        // 1. Lấy danh sách tiện ích của phòng (Public)
        [HttpGet("{phongId}")]
        public async Task<IActionResult> GetByRoom(Guid phongId)
        {
            var result = await _service.GetAmenitiesByRoomIdAsync(phongId);
            return Ok(result);
        }

        // 2. Thêm tiện ích vào phòng (Chủ trọ)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] PhongTienIchRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                var success = await _service.AddAsync(request, userId);

                if (!success) return BadRequest("Tiện ích này đã có trong phòng rồi.");

                return Ok(new { message = "Thêm tiện ích thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Lỗi không có quyền
            }
        }

        // 3. Xóa tiện ích khỏi phòng (Chủ trọ)
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Remove([FromQuery] Guid phongId, [FromQuery] int tienIchId)
        {
            try
            {
                var userId = GetUserId();
                var success = await _service.RemoveAsync(phongId, tienIchId, userId);

                if (!success) return NotFound("Không tìm thấy tiện ích trong phòng này.");

                return Ok(new { message = "Xóa tiện ích thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
