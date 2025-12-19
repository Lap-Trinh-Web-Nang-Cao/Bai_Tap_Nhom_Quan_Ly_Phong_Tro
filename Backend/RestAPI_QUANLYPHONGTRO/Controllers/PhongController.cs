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

            // Map Entity -> DTO để tránh circular reference
            var dtoList = result.Data.Select(p => new ViewModels.PhongDto
            {
                PhongId = p.PhongId,
                NhaTroId = p.NhaTroId,
                TieuDe = p.TieuDe ?? "",
                MoTa = p.MoTa,
                DienTich = p.DienTich,
                GiaTien = p.GiaTien,
                TienCoc = p.TienCoc,
                SoNguoiToiDa = p.SoNguoiToiDa ?? 1,
                TrangThai = p.TrangThai ?? "Trong",
                DiemTrungBinh = p.DiemTrungBinh,
                SoLuongDanhGia = p.SoLuongDanhGia ?? 0,
                IsDuyet = p.IsDuyet,
                IsBiKhoa = p.IsBiKhoa,
                CreatedAt = p.CreatedAt ?? DateTimeOffset.Now,
                UpdatedAt = p.UpdatedAt,
                NhaTro = p.NhaTro == null ? null : new ViewModels.NhaTroSimpleDto
                {
                    NhaTroId = p.NhaTro.NhaTroId,
                    TieuDe = p.NhaTro.TieuDe ?? "",
                    DiaChi = p.NhaTro.DiaChi
                }
            }).ToList();

            // Trả về format chuẩn cho frontend dễ paging, WRAP trong { success, data, message }
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
                Message = "Success"
            });
        }

        // 2. Xem chi tiết (Public)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var p = await _service.GetByIdAsync(id);
            if (p == null) return NotFound(new { Success = false, Message = "Không tìm thấy phòng trọ" });

            var dto = new ViewModels.PhongDto
            {
                PhongId = p.PhongId,
                NhaTroId = p.NhaTroId,
                TieuDe = p.TieuDe ?? "",
                MoTa = p.MoTa,
                DienTich = p.DienTich,
                GiaTien = p.GiaTien,
                TienCoc = p.TienCoc,
                SoNguoiToiDa = p.SoNguoiToiDa ?? 1,
                TrangThai = p.TrangThai ?? "Trong",
                DiemTrungBinh = p.DiemTrungBinh,
                SoLuongDanhGia = p.SoLuongDanhGia ?? 0,
                IsDuyet = p.IsDuyet,
                IsBiKhoa = p.IsBiKhoa,
                CreatedAt = p.CreatedAt ?? DateTimeOffset.Now,
                UpdatedAt = p.UpdatedAt,
                NhaTro = p.NhaTro == null ? null : new ViewModels.NhaTroSimpleDto
                {
                    NhaTroId = p.NhaTro.NhaTroId,
                    TieuDe = p.NhaTro.TieuDe ?? "",
                    DiaChi = p.NhaTro.DiaChi
                }
            };

            return Ok(new { Success = true, Data = dto, Message = "Success" });
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var adminId = GetUserId();
            var success = await _service.ApproveRoomAsync(id, adminId);
            if (!success) return NotFound();
            return Ok(new { message = "Đã duyệt phòng thành công" });
        }

        // 6. Khóa phòng (Admin)
        [HttpPut("lock/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Lock(Guid id, [FromQuery] bool isLocked = true)
        {
            var success = await _service.LockRoomAsync(id, isLocked);
            if (!success) return NotFound();
            return Ok(new { message = isLocked ? "Đã khóa phòng" : "Đã mở khóa phòng" });
        }
    }
}
