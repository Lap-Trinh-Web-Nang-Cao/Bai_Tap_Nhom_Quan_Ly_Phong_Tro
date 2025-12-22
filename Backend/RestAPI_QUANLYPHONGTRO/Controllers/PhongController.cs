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

        // ===== PUBLIC ENDPOINTS (Khách vãng lai) =====

        /// <summary>
        /// Tìm kiếm phòng - Public (ai cũng xem được)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublic(
            [FromQuery] Guid? nhaTroId,
            [FromQuery] long? minPrice,
            [FromQuery] long? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetPublicRoomsAsync(nhaTroId, minPrice, maxPrice, page, pageSize);

            // ✅ Return consistent response format with Data (uppercase)
            return Ok(new
            {
                success = true,
                Data = result.Data,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
            });
        }

        /// <summary>
        /// Xem chi tiết phòng - Public
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            
            // ✅ Wrap in success response
            return Ok(new
            {
                success = true,
                Data = result
            });
        }

        // ===== CHỦ TRỌ ENDPOINTS =====

        /// <summary>
        /// Tạo phòng mới - Chủ trọ only
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "ChuTroOnly")]
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
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Sửa phòng - Chủ trọ only
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "ChuTroOnly")]
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

        // ===== ADMIN ONLY ENDPOINTS =====

        /// <summary>
        /// Lấy danh sách phòng chờ duyệt - Admin only
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetPending(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string keyword = "")
        {
            try
            {
                var (data, totalCount) = await _service.GetPendingRoomsAsync(pageIndex, pageSize, keyword);
                
                // ✅ Return consistent response format with Data (uppercase)
                return Ok(new
                {
                    success = true,
                    Data = data,
                    TotalCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Duyệt phòng - Admin only
        /// </summary>
        [HttpPut("approve/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                var success = await _service.ApproveRoomAsync(id, Guid.Empty);
                if (!success)
                    return NotFound(new { success = false, message = "Phòng không tồn tại" });

                return Ok(new { success = true, message = "Đã duyệt phòng thành công", data = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message, data = false });
            }
        }

        /// <summary>
        /// Từ chối phòng - Admin only
        /// </summary>
        [HttpPut("{id}/reject")]
        [Authorize(Policy = "AdminOnly")]
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

        /// <summary>
        /// Khóa/Mở khóa phòng - Admin only
        /// </summary>
        [HttpPut("lock/{id}")]
        [Authorize(Policy = "AdminOnly")]
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

        /// <summary>
        /// Lấy thống kê phòng - Admin only
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
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
        public string? Reason { get; set; }
    }
}
