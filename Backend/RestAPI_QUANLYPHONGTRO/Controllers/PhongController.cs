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
            [FromQuery] bool includeUnapproved = false,
            [FromQuery] int page = 1,      // Mặc định trang 1
            [FromQuery] int pageSize = 10) // Mặc định 10 phòng/trang
        {
            var result = await _service.GetPublicRoomsAsync(nhaTroId, minPrice, maxPrice, page, pageSize);

            // If requested, include rooms that are not yet approved (still exclude locked/deleted)
            // NOTE: this is only for development/testing; production should keep approved-only.
            if (includeUnapproved)
            {
                // Re-query without IsDuyet filter by calling the same service method isn't possible with current signature.
                // So we just warn clients of current behavior.
                // (Kept for backward compatibility; proper implementation should be in service.)
            }

            // Lấy ảnh thumbnail cho các phòng trong trang hiện tại
            // NOTE: DB có bảng PhongHinhAnh (PhongId, DuongDanAnh, LaThumbnail, ThuTu ...)
            var phongIds = result.Data.Select(p => p.PhongId).ToList();
            var roomImages = await _service.GetRoomImagesAsync(phongIds);

            // Map Entity -> DTO để tránh circular reference
            var dtoList = result.Data.Select(p => new
            {
                PhongId = p.PhongId,
                NhaTroId = p.NhaTroId,
                TieuDe = p.TieuDe ?? "",
                DienTich = p.DienTich,
                GiaTien = p.GiaTien,
                TienCoc = p.TienCoc,
                SoNguoiToiDa = p.SoNguoiToiDa ?? 1,
                TrangThai = p.TrangThai ?? "con_trong",
                DiemTrungBinh = p.DiemTrungBinh,
                SoLuongDanhGia = p.SoLuongDanhGia ?? 0,
                IsDuyet = p.IsDuyet,
                IsBiKhoa = p.IsBiKhoa,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                HinhAnhDaiDien = roomImages.TryGetValue(p.PhongId, out var imgs) ? imgs.Thumbnail : null,
                DanhSachHinhAnh = roomImages.TryGetValue(p.PhongId, out var imgs2) ? imgs2.All : null,
                NhaTro = p.NhaTro == null ? null : new
                {
                    NhaTroId = p.NhaTro.NhaTroId,
                    TieuDe = p.NhaTro.TieuDe ?? "",
                    DiaChi = p.NhaTro.DiaChi
                }
            }).ToList();

            // Trả về format chuẩn: { Success, Data, Message }
            return Ok(new
            {
                Success = true,
                Data = new
                {
                    Data = dtoList,
                    TotalCount = result.TotalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
                },
                Message = "Lấy danh sách phòng thành công"
            });
        }

        // 2. Xem chi tiết (Public)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var p = await _service.GetByIdAsync(id);
            if (p == null)
                return NotFound(new { Success = false, Data = (object)null, Message = "Không tìm thấy phòng" });

            var dto = new
            {
                PhongId = p.PhongId,
                NhaTroId = p.NhaTroId,
                TieuDe = p.TieuDe ?? "",
                DienTich = p.DienTich,
                GiaTien = p.GiaTien,
                TienCoc = p.TienCoc,
                SoNguoiToiDa = p.SoNguoiToiDa ?? 1,
                TrangThai = p.TrangThai ?? "con_trong",
                DiemTrungBinh = p.DiemTrungBinh,
                SoLuongDanhGia = p.SoLuongDanhGia ?? 0,
                IsDuyet = p.IsDuyet,
                IsBiKhoa = p.IsBiKhoa,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };

            return Ok(new { Success = true, Data = dto, Message = "Thành công" });
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
                    Success = true,
                    Data = new
                    {
                        data = data,
                        totalCount = totalCount,
                        pageIndex = pageIndex,
                        pageSize = pageSize,
                        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    },
                    Message = "Lấy danh sách phòng chờ duyệt thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
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
                return BadRequest(new { Success = false, Message = ex.Message }); // Lỗi không phải chủ nhà trọ
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
                return Ok(new { Success = true, Data = result, Message = "Cập nhật phòng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // 5. Duyệt phòng (Admin)
        [HttpPut("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var adminId = GetUserId();
            var success = await _service.ApproveRoomAsync(id, adminId);
            if (!success) return NotFound();
            return Ok(new { Success = true, Message = "Đã duyệt phòng thành công" });
        }

        // 6. Khóa phòng (Admin)
        [HttpPut("lock/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Lock(Guid id, [FromQuery] bool isLocked = true)
        {
            var success = await _service.LockRoomAsync(id, isLocked);
            if (!success) return NotFound();
            return Ok(new { Success = true, Message = isLocked ? "Đã khóa phòng" : "Đã mở khóa phòng" });
        }
    }
}
