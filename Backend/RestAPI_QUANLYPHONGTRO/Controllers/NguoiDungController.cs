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

        // API Đăng ký (Ai cũng gọi được)
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.RegisterAsync(request);
            if (!result) return BadRequest(new { message = "Email đã tồn tại." });

            return Ok(new { message = "Đăng ký thành công!" });
        }

        // API Đăng nhập (Ai cũng gọi được)
        [HttpPost("login")]
        [AllowAnonymous]
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
                return BadRequest(new { message = ex.Message });
            }
        }

        // API Lấy thông tin bản thân
        [HttpGet("me")]
        [Authorize(Policy = "AuthenticatedOnly")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = Guid.Parse(userIdStr);
            var user = await _service.GetByIdAsync(userId);

            if (user == null) return NotFound();

            user.PasswordHash = null;
            return Ok(user);
        }

        // API Cập nhật hồ sơ bản thân
        [HttpPut("me")]
        [Authorize(Policy = "AuthenticatedOnly")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = Guid.Parse(userIdStr);

            var result = await _service.UpdateProfileAsync(userId, request);

            if (!result) return NotFound("Không tìm thấy người dùng.");

            return Ok(new { message = "Cập nhật thông tin thành công!" });
        }

        // API Đổi mật khẩu
        [HttpPut("change-password")]
        [Authorize(Policy = "AuthenticatedOnly")]
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
        /// GET: /api/nguoidung/statistics - Lấy thống kê người dùng
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                // Đếm theo vai trò
                var totalUsers = await _service.GetUsersAsync(1, int.MaxValue, "");
                var totalTenants = await _service.GetUsersAsync(1, int.MaxValue, "", 3, null);
                var totalLandlords = await _service.GetUsersAsync(1, int.MaxValue, "", 2, null);
                var totalAdmins = await _service.GetUsersAsync(1, int.MaxValue, "", 1, null);
                var lockedUsers = await _service.GetUsersAsync(1, int.MaxValue, "", null, true);
                var activeUsers = await _service.GetUsersAsync(1, int.MaxValue, "", null, false);

                return Ok(new
                {
                    TotalUsers = totalUsers.TotalCount,
                    TotalTenants = totalTenants.TotalCount,
                    TotalLandlords = totalLandlords.TotalCount,
                    TotalAdmins = totalAdmins.TotalCount,
                    LockedUsers = lockedUsers.TotalCount,
                    ActiveUsers = activeUsers.TotalCount,
                    VerifiedEmails = totalUsers.Items?.Where(u => u.IsEmailXacThuc).Count() ?? 0
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetStatistics Error: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi lấy thống kê", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: /api/nguoidung - Lấy danh sách users (phân trang) với filter
        /// </summary>
        [HttpGet]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> GetUsers(
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string keyword = "",
            [FromQuery] int? vaiTroId = null,
            [FromQuery] bool? isKhoa = null)
        {
            try
            {
                var result = await _service.GetUsersAsync(pageIndex, pageSize, keyword, vaiTroId, isKhoa);
                
                System.Diagnostics.Debug.WriteLine($"✅ GetUsers API: {result.TotalCount} records");
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetUsers Error: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi lấy danh sách người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: /api/nguoidung/{id} - Lấy chi tiết user
        /// </summary>
        [HttpGet("{id}")]
        // [Authorize]
        public async Task<IActionResult> GetUserDetail(string id)
        {
            if (!Guid.TryParse(id, out var userId))
                return BadRequest(new { success = false, message = "Invalid user ID" });

            try
            {
                var result = await _service.GetUserDetailAsync(userId);
                if (result == null)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi lấy thông tin người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: /api/nguoidung/admin-create - Admin tạo user mới
        /// </summary>
        [HttpPost("admin-create")]
        // [Authorize]  // ⚠️ TEMPORARILY DISABLED FOR TESTING
        public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserRequest request)
        {
            if (!ModelState.IsValid) 
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });

            try
            {
                var userId = await _service.CreateUserAsync(request);
                if (userId == null)
                    return BadRequest(new { success = false, message = "Email đã tồn tại" });

                return Ok(new { success = true, message = "Tạo người dùng thành công", data = new { nguoiDungId = userId } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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
                return BadRequest(new { success = false, message = "Invalid user ID" });

            try
            {
                var result = await _service.LockUserAsync(userId);
                if (!result)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(new { success = true, message = "Khóa tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi khóa tài khoản", error = ex.Message });
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
                return BadRequest(new { success = false, message = "Invalid user ID" });

            try
            {
                var result = await _service.UnlockUserAsync(userId);
                if (!result)
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });

                return Ok(new { success = true, message = "Mở khóa tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi mở khóa tài khoản", error = ex.Message });
            }
        }
    }
}
