using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhongController : ControllerBase
    {
        private readonly IPhongService _service;

        public PhongController(IPhongService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id)) throw new UnauthorizedAccessException();
            return Guid.Parse(id);
        }

        // 1. Tìm kiếm phòng (Public)
        // GET: api/phong?nhaTroId=...&minPrice=1000000
        [HttpGet]
        public async Task<IActionResult> GetPublic(
            [FromQuery] Guid? nhaTroId,
            [FromQuery] long? minPrice,
            [FromQuery] long? maxPrice,
            [FromQuery] int page = 1,      // Mặc định trang 1
            [FromQuery] int pageSize = 10) // Mặc định 10 phòng/trang
        {
            var result = await _service.GetPublicRoomsAsync(nhaTroId, minPrice, maxPrice, page, pageSize);

            // Trả về format chuẩn cho frontend dễ paging
            return Ok(new
            {
                Data = result.Data,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
            });
        }

        // 2. Xem chi tiết (Public)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // Admin: Lấy danh sách phòng chờ duyệt (với phân trang)
        // TODO: Thêm lại [Authorize(Roles = "Admin")] khi hệ thống đăng nhập hoàn chỉnh
        [HttpGet("pending")]
        [AllowAnonymous] // Tạm thời cho phép truy cập không cần đăng nhập để test
        public async Task<IActionResult> GetPending(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string keyword = "")
        {
            try
            {
                var (data, totalCount) = await _service.GetPendingRoomsAsync(pageIndex, pageSize, keyword);
                
                return Ok(new
                {
                    data = data,
                    totalCount = totalCount,
                    pageIndex = pageIndex,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. Tạo phòng (Chủ trọ)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePhongRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = GetUserId();
                var result = await _service.CreateAsync(request, userId);
                return CreatedAtAction(nameof(GetDetail), new { id = result.PhongId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Lỗi không phải chủ nhà trọ
            }
        }

        // 4. Sửa phòng (Chủ trọ)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreatePhongRequest request)
        {
            try
            {
                var userId = GetUserId();
                var result = await _service.UpdateAsync(id, request, userId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 5. Duyệt phòng (Admin)
        [HttpPut("approve/{id}")]
        [AllowAnonymous] // Tạm thời cho phép test
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                var success = await _service.ApproveRoomAsync(id, Guid.Empty);
                if (!success)
                    return NotFound(new { success = false, message = "Phòng không tồn tại" });

                // Trả về ApiResponse-style để Admin client đọc Success
                return Ok(new { success = true, message = "Đã duyệt phòng thành công", data = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message, data = false });
            }
        }

        // 5b. Từ chối phòng (Admin)
        [HttpPut("{id}/reject")]
        [AllowAnonymous] // Tạm thời cho phép test
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRoomRequest request)
        {
            try
            {
                var success = await _service.RejectRoomAsync(id, request?.Reason ?? "");
                if (!success)
                    return NotFound(new { success = false, message = "Phòng không tồn tại" });

                return Ok(new { success = true, message = "Đã từ chối phòng", data = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message, data = false });
            }
        }

        // 6. Khóa phòng (Admin)
        [HttpPut("lock/{id}")]
        [AllowAnonymous] // Tạm thời cho phép test
        public async Task<IActionResult> Lock(Guid id, [FromQuery] bool isLocked = true)
        {
            try
            {
                var success = await _service.LockRoomAsync(id, isLocked);
                if (!success)
                    return NotFound(new { success = false, message = "Phòng không tồn tại" });

                return Ok(new
                {
                    success = true,
                    message = isLocked ? "Đã khóa phòng" : "Đã mở khóa phòng",
                    data = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message, data = false });
            }
        }

        // 7. Lấy thống kê phòng (Admin)
        [HttpGet("stats")]
        [AllowAnonymous] // Tạm thời cho phép test
        public async Task<IActionResult> GetStats()
        {
            try
            {
                // Thống kê trực tiếp từ DB (đúng với DashboardService)
                var (pageData, _) = await _service.GetPendingRoomsAsync(1, 100000, "");
                var allRoomsList = pageData.ToList();

                var pending = allRoomsList.Count(r => !r.IsDuyet && !r.IsBiKhoa);
                var approved = allRoomsList.Count(r => r.IsDuyet && !r.IsBiKhoa);
                var locked = allRoomsList.Count(r => r.IsBiKhoa);

                return Ok(new
                {
                    success = true,
                    message = "OK",
                    data = new
                    {
                        total = allRoomsList.Count,
                        pending = pending,
                        approved = approved,
                        locked = locked
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class RejectRoomRequest
    {
        public string Reason { get; set; }
    }
}
