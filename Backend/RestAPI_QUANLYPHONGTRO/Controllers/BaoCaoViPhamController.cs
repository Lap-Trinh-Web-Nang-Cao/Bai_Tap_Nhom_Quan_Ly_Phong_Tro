using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoViPhamController : ControllerBase
    {
        private readonly IBaoCaoViPhamService _service;
        private readonly ApplicationDbContext _context;

        public BaoCaoViPhamController(IBaoCaoViPhamService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả báo cáo vi phạm với thông tin mở rộng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, 
            [FromQuery] string trangThai = "", [FromQuery] string loaiThucThe = "", [FromQuery] string keyword = "")
        {
            try
            {
                var query = _context.BaoCaoViPhams.AsQueryable();

                // Filter by trạng thái
                if (!string.IsNullOrEmpty(trangThai))
                {
                    query = query.Where(b => b.TrangThai == trangThai);
                }

                // Filter by loại thực thể
                if (!string.IsNullOrEmpty(loaiThucThe))
                {
                    query = query.Where(b => b.LoaiThucThe == loaiThucThe);
                }

                // Search by keyword
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(b => b.TieuDe.Contains(keyword) || b.MoTa.Contains(keyword));
                }

                var totalRecords = await query.CountAsync();

                var items = await query
                    .OrderByDescending(b => b.ThoiGianBaoCao)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Get related data
                var nguoiBaoCaoIds = items.Select(b => b.NguoiBaoCao).Distinct().ToList();
                var nguoiDungs = await _context.NguoiDungs
                    .Where(n => nguoiBaoCaoIds.Contains(n.NguoiDungId))
                    .Include(n => n.HoSoNguoiDung)
                    .ToDictionaryAsync(n => n.NguoiDungId);

                // Get phong names
                var phongIds = items.Where(b => b.LoaiThucThe == "PHONG" && b.ThucTheId.HasValue)
                    .Select(b => b.ThucTheId.Value).Distinct().ToList();
                var phongs = await _context.Phongs
                    .Where(p => phongIds.Contains(p.PhongId))
                    .ToDictionaryAsync(p => p.PhongId, p => p.TieuDe);

                // Get vi pham names
                var viPhamIds = items.Where(b => b.ViPhamId.HasValue).Select(b => b.ViPhamId.Value).Distinct().ToList();
                var viPhams = await _context.ViPhams
                    .Where(v => viPhamIds.Contains(v.ViPhamId))
                    .ToDictionaryAsync(v => v.ViPhamId, v => v.TenViPham);

                var result = items.Select(b => new
                {
                    b.BaoCaoId,
                    b.SoBaoCao,
                    b.LoaiThucThe,
                    b.ThucTheId,
                    b.NguoiBaoCao,
                    b.ViPhamId,
                    b.TieuDe,
                    MoTa = b.MoTa ?? "",
                    b.TrangThai,
                    b.KetQua,
                    b.NguoiXuLy,
                    b.ThoiGianBaoCao,
                    b.ThoiGianXuLy,
                    TenNguoiBaoCao = nguoiDungs.ContainsKey(b.NguoiBaoCao) && nguoiDungs[b.NguoiBaoCao].HoSoNguoiDung != null
                        ? nguoiDungs[b.NguoiBaoCao].HoSoNguoiDung.HoTen
                        : (nguoiDungs.ContainsKey(b.NguoiBaoCao) ? nguoiDungs[b.NguoiBaoCao].Email : "Ẩn danh"),
                    EmailNguoiBaoCao = nguoiDungs.ContainsKey(b.NguoiBaoCao) ? nguoiDungs[b.NguoiBaoCao].Email : "",
                    TenDoiTuong = b.LoaiThucThe == "PHONG" && b.ThucTheId.HasValue && phongs.ContainsKey(b.ThucTheId.Value)
                        ? phongs[b.ThucTheId.Value]
                        : (b.LoaiThucThe == "NGUOIDUNG" && b.ThucTheId.HasValue && nguoiDungs.ContainsKey(b.ThucTheId.Value)
                            ? (nguoiDungs[b.ThucTheId.Value].HoSoNguoiDung?.HoTen ?? nguoiDungs[b.ThucTheId.Value].Email)
                            : "N/A"),
                    TenLoaiViPham = b.ViPhamId.HasValue && viPhams.ContainsKey(b.ViPhamId.Value) ? viPhams[b.ViPhamId.Value] : "",
                    TenNguoiXuLy = ""
                }).ToList();

                return Ok(new
                {
                    Items = result,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Lấy chi tiết báo cáo vi phạm
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var baoCao = await _context.BaoCaoViPhams.FindAsync(id);
                if (baoCao == null) return NotFound(new { message = "Không tìm thấy báo cáo" });

                // Get người báo cáo
                var nguoiBaoCao = await _context.NguoiDungs
                    .Include(n => n.HoSoNguoiDung)
                    .FirstOrDefaultAsync(n => n.NguoiDungId == baoCao.NguoiBaoCao);

                // Get đối tượng bị báo cáo
                string tenDoiTuong = "N/A";
                string chiTietDoiTuong = "";

                if (baoCao.LoaiThucThe == "PHONG" && baoCao.ThucTheId.HasValue)
                {
                    var phong = await _context.Phongs
                        .Include(p => p.NhaTro)
                        .FirstOrDefaultAsync(p => p.PhongId == baoCao.ThucTheId.Value);
                    if (phong != null)
                    {
                        tenDoiTuong = phong.TieuDe ?? "Phòng trọ";
                        chiTietDoiTuong = phong.NhaTro?.DiaChi ?? "";
                    }
                }
                else if (baoCao.LoaiThucThe == "NGUOIDUNG" && baoCao.ThucTheId.HasValue)
                {
                    var nguoiDung = await _context.NguoiDungs
                        .Include(n => n.HoSoNguoiDung)
                        .FirstOrDefaultAsync(n => n.NguoiDungId == baoCao.ThucTheId.Value);
                    if (nguoiDung != null)
                    {
                        tenDoiTuong = nguoiDung.HoSoNguoiDung?.HoTen ?? nguoiDung.Email ?? "Người dùng";
                        chiTietDoiTuong = nguoiDung.Email ?? "";
                    }
                }

                // Get loại vi phạm
                string tenLoaiViPham = "";
                string moTaLoaiViPham = "";
                if (baoCao.ViPhamId.HasValue)
                {
                    var viPham = await _context.ViPhams.FindAsync(baoCao.ViPhamId.Value);
                    if (viPham != null)
                    {
                        tenLoaiViPham = viPham.TenViPham;
                        moTaLoaiViPham = viPham.MoTa ?? "";
                    }
                }

                // Get người xử lý
                string tenNguoiXuLy = "";
                if (baoCao.NguoiXuLy.HasValue)
                {
                    var nguoiXuLy = await _context.NguoiDungs
                        .Include(n => n.HoSoNguoiDung)
                        .FirstOrDefaultAsync(n => n.NguoiDungId == baoCao.NguoiXuLy.Value);
                    if (nguoiXuLy != null)
                    {
                        tenNguoiXuLy = nguoiXuLy.HoSoNguoiDung?.HoTen ?? nguoiXuLy.Email ?? "Admin";
                    }
                }

                return Ok(new
                {
                    baoCao.BaoCaoId,
                    baoCao.SoBaoCao,
                    baoCao.LoaiThucThe,
                    baoCao.ThucTheId,
                    baoCao.TieuDe,
                    baoCao.MoTa,
                    baoCao.TrangThai,
                    baoCao.KetQua,
                    baoCao.ThoiGianBaoCao,
                    baoCao.ThoiGianXuLy,
                    // Người báo cáo
                    NguoiBaoCaoId = baoCao.NguoiBaoCao,
                    TenNguoiBaoCao = nguoiBaoCao?.HoSoNguoiDung?.HoTen ?? nguoiBaoCao?.Email ?? "Ẩn danh",
                    EmailNguoiBaoCao = nguoiBaoCao?.Email ?? "",
                    DienThoaiNguoiBaoCao = nguoiBaoCao?.DienThoai ?? "",
                    AvatarNguoiBaoCao = "",
                    // Đối tượng
                    TenDoiTuong = tenDoiTuong,
                    ChiTietDoiTuong = chiTietDoiTuong,
                    // Loại vi phạm
                    baoCao.ViPhamId,
                    TenLoaiViPham = tenLoaiViPham,
                    MoTaLoaiViPham = moTaLoaiViPham,
                    // Người xử lý
                    NguoiXuLyId = baoCao.NguoiXuLy,
                    TenNguoiXuLy = tenNguoiXuLy
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Tạo báo cáo vi phạm mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BaoCaoViPham baoCao)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdBaoCao = await _service.CreateBaoCaoAsync(baoCao);
            return CreatedAtAction(nameof(GetById), new { id = createdBaoCao.BaoCaoId }, createdBaoCao);
        }

        /// <summary>
        /// Xử lý báo cáo vi phạm (chấp nhận)
        /// </summary>
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> Resolve(Guid id, [FromBody] XuLyBaoCaoRequest request)
        {
            try
            {
                var baoCao = await _context.BaoCaoViPhams.FindAsync(id);
                if (baoCao == null) return NotFound(new { Success = false, Message = "Không tìm thấy báo cáo" });

                // Cập nhật trạng thái
                baoCao.TrangThai = "DaXuLy";
                baoCao.KetQua = request?.KetQua ?? "Đã xử lý";
                baoCao.ViPhamId = request?.ViPhamId;
                baoCao.ThoiGianXuLy = DateTimeOffset.Now;
                // TODO: Get admin ID from token
                // baoCao.NguoiXuLy = adminId;

                // Xử lý khóa tài khoản nếu cần
                if (request?.KhoaTaiKhoan == true && baoCao.LoaiThucThe == "NGUOIDUNG" && baoCao.ThucTheId.HasValue)
                {
                    var nguoiDung = await _context.NguoiDungs.FindAsync(baoCao.ThucTheId.Value);
                    if (nguoiDung != null)
                    {
                        nguoiDung.IsKhoa = true;
                        nguoiDung.UpdatedAt = DateTimeOffset.Now;
                    }
                }

                // Xử lý khóa bài đăng nếu cần
                if (request?.KhoaBaiDang == true && baoCao.LoaiThucThe == "PHONG" && baoCao.ThucTheId.HasValue)
                {
                    var phong = await _context.Phongs.FindAsync(baoCao.ThucTheId.Value);
                    if (phong != null)
                    {
                        phong.IsBiKhoa = true;
                        phong.IsDuyet = false;
                        phong.UpdatedAt = DateTimeOffset.Now;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Đã xử lý báo cáo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Từ chối báo cáo vi phạm
        /// </summary>
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] XuLyBaoCaoRequest request)
        {
            try
            {
                var baoCao = await _context.BaoCaoViPhams.FindAsync(id);
                if (baoCao == null) return NotFound(new { Success = false, Message = "Không tìm thấy báo cáo" });

                baoCao.TrangThai = "TuChoi";
                baoCao.KetQua = request?.KetQua ?? "Báo cáo không hợp lệ";
                baoCao.ThoiGianXuLy = DateTimeOffset.Now;

                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Đã từ chối báo cáo" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu đang xử lý
        /// </summary>
        [HttpPut("{id}/processing")]
        public async Task<IActionResult> MarkAsProcessing(Guid id)
        {
            try
            {
                var baoCao = await _context.BaoCaoViPhams.FindAsync(id);
                if (baoCao == null) return NotFound(new { Success = false, Message = "Không tìm thấy báo cáo" });

                baoCao.TrangThai = "DangXuLy";
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Đã cập nhật trạng thái" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Thống kê báo cáo vi phạm
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var stats = new
                {
                    TongSoBaoCao = await _context.BaoCaoViPhams.CountAsync(),
                    ChoXuLy = await _context.BaoCaoViPhams.CountAsync(b => b.TrangThai == "ChoXuLy"),
                    DangXuLy = await _context.BaoCaoViPhams.CountAsync(b => b.TrangThai == "DangXuLy"),
                    DaXuLy = await _context.BaoCaoViPhams.CountAsync(b => b.TrangThai == "DaXuLy"),
                    TuChoi = await _context.BaoCaoViPhams.CountAsync(b => b.TrangThai == "TuChoi"),
                    BaoCaoPhong = await _context.BaoCaoViPhams.CountAsync(b => b.LoaiThucThe == "PHONG"),
                    BaoCaoNguoiDung = await _context.BaoCaoViPhams.CountAsync(b => b.LoaiThucThe == "NGUOIDUNG")
                };

                return Ok(new { Success = true, Data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật báo cáo vi phạm
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BaoCaoViPham baoCao)
        {
            var updatedBaoCao = await _service.UpdateBaoCaoAsync(id, baoCao);
            if (updatedBaoCao == null) return NotFound();

            return Ok(updatedBaoCao);
        }

        /// <summary>
        /// Xóa báo cáo vi phạm
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _service.DeleteBaoCaoAsync(id);
            if (!isDeleted) return NotFound();

            return NoContent();
        }
    }

    /// <summary>
    /// Request để xử lý báo cáo
    /// </summary>
    public class XuLyBaoCaoRequest
    {
        public Guid? BaoCaoId { get; set; }
        public string? HanhDong { get; set; }
        public string? KetQua { get; set; }
        public int? ViPhamId { get; set; }
        public bool KhoaTaiKhoan { get; set; }
        public bool KhoaBaiDang { get; set; }
    }
}
