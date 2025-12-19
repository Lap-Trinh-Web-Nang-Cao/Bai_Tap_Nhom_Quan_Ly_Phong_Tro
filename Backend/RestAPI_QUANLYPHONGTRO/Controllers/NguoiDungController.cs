using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungService _service;

        public NguoiDungController(INguoiDungService service)
        {
            _service = service;
        }

        // API Đăng ký (Ai cũng gọi được -> Không cần [Authorize])
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.RegisterAsync(request);
            if (!result) return BadRequest(new { message = "Email đã tồn tại." });

            return Ok(new { message = "Đăng ký thành công!" });
        }

        // API Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _service.LoginAsync(request);
                if (token == null) return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Trả về lỗi nếu bị khóa
            }
        }

        // API Lấy thông tin bản thân (Cần đăng nhập -> Có [Authorize])
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            // Lấy ID từ Token
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = Guid.Parse(userIdStr);
            var user = await _service.GetByIdAsync(userId);

            if (user == null) return NotFound();

            // Ẩn mật khẩu trước khi trả về
            user.PasswordHash = null;

            return Ok(user);
        }

        [HttpPut("me")] // PUT: api/nguoidung/me
        [Authorize] // Bắt buộc đăng nhập
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            // Lấy ID từ Token (đảm bảo chỉ sửa của chính mình)
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userId = Guid.Parse(userIdStr);

            var result = await _service.UpdateProfileAsync(userId, request);

            if (!result) return NotFound("Không tìm thấy người dùng.");

            return Ok(new { message = "Cập nhật thông tin thành công!" });
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Lấy ID từ Token
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userId = Guid.Parse(userIdStr);

            var result = await _service.ChangePasswordAsync(userId, request);

            if (!result)
            {
                return BadRequest(new { message = "Mật khẩu cũ không chính xác." });
            }

            return Ok(new { message = "Đổi mật khẩu thành công!" });
        }

        // ===== ADMIN ENDPOINTS =====

        /// <summary>
        /// GET: /api/nguoidung - Lấy danh sách users (phân trang)
        /// </summary>
        [HttpGet]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> GetUsers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string keyword = "")
        {
            try
            {
                var result = await _service.GetUsersAsync(pageIndex, pageSize, keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi lấy danh sách người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: /api/nguoidung/{id}/lock - Khóa tài khoản
        /// </summary>
        [HttpPut("{id}/lock")]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> LockUser(string id)
        {
            if (!Guid.TryParse(id, out var userId))
                return BadRequest(new { message = "Invalid user ID" });

            try
            {
                var result = await _service.LockUserAsync(userId);
                if (!result)
                    return NotFound(new { message = "Người dùng không tồn tại" });

                return Ok(new { message = "Khóa tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi khóa tài khoản", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: /api/nguoidung/{id}/unlock - Mở khóa tài khoản
        /// </summary>
        [HttpPut("{id}/unlock")]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> UnlockUser(string id)
        {
            if (!Guid.TryParse(id, out var userId))
                return BadRequest(new { message = "Invalid user ID" });

            try
            {
                var result = await _service.UnlockUserAsync(userId);
                if (!result)
                    return NotFound(new { message = "Người dùng không tồn tại" });

                return Ok(new { message = "Mở khóa tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi mở khóa tài khoản", error = ex.Message });
            }
        }
    }
}
