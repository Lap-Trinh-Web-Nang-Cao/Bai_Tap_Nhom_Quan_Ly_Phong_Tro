using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TapTinController : ControllerBase
    {
        private readonly ITapTinService _service;

        public TapTinController(ITapTinService service)
        {
            _service = service;
        }

        private Guid? GetUserId()
        {
            // Có thể null nếu hệ thống cho phép khách upload (ít gặp)
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idStr)) return null;
            return Guid.Parse(idStr);
        }

        // Upload file (Yêu cầu đăng nhập)
        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")] // Đánh dấu API này nhận form-data
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
        {
            // Lưu ý: Dùng [FromForm] để nhận file
            if (request.File == null) return BadRequest("Vui lòng chọn file.");

            try
            {
                var userId = GetUserId();
                var result = await _service.UploadFileAsync(request.File, userId);

                // Trả về thông tin file vừa tạo
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Lấy thông tin file
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInfo(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
