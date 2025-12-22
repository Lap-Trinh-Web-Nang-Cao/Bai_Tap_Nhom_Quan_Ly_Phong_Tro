using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // TODO: b?t l?i [Authorize] khi hoàn thi?n login/token
    [AllowAnonymous]
    public class HoaDonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbIntrospectionService _dbInfo;

        public HoaDonController(ApplicationDbContext context, IDbIntrospectionService dbInfo)
        {
            _context = context;
            _dbInfo = dbInfo;
        }

        private Guid GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(id);
        }

        // GET /api/hoadon/nguoithue/{nguoiThueId}
        [HttpGet("nguoithue/{nguoiThueId:guid}")]
        public async Task<IActionResult> GetByTenantId([FromRoute] Guid nguoiThueId)
        {
            try
            {
                var hoaDonExists = await _dbInfo.TableExistsAsync("HoaDon");
                var hopDongExists = await _dbInfo.TableExistsAsync("HopDong");
                if (!hoaDonExists || !hopDongExists)
                {
                    var dbName = await _dbInfo.GetCurrentDatabaseNameAsync();
                    return BadRequest(new { success = false, message = $"Invalid object name '{(!hopDongExists ? "HopDong" : "HoaDon")}'. Backend ðang k?t n?i DB '{dbName}'. H?y ki?m tra connection string và ð?m b?o có b?ng dbo.HopDong và dbo.HoaDon." });
                }

                var query = from hd in _context.HoaDons
                            join hopdong in _context.HopDongs on hd.HopDongId equals hopdong.HopDongId
                            where hopdong.NguoiThueId == nguoiThueId
                            orderby hd.NgayLap descending
                            select new
                            {
                                HoaDonId = hd.HoaDonId,
                                ThangNam = (hd.Thang <10 ? "0" + hd.Thang : hd.Thang.ToString()) + "/" + hd.Nam,
                                TienThue = hd.TienPhong,
                                TienDien = hd.TienDien ??0,
                                TienNuoc = hd.TienNuoc ??0,
                                PhiKhac = hd.TienDichVu ??0,
                                TongTien = hd.TongTien,
                                TrangThai = hd.TrangThai == "DaThanhToan" ? "Ð? thanh toán" : (hd.TrangThai == "ChuaThanhToan" ? "Chýa thanh toán" : hd.TrangThai),
                                NgayThanhToan = hd.NgayThanhToan,
                                HanThanhToan = (DateTime?)null
                            };

                var list = await query.ToListAsync();
                return Ok(new { Success = true, Data = list, Message = "L?y danh sách hóa ðõn thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // GET /api/hoadon - L?y t?t c? hóa ðõn
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // TODO: Implement logic l?y t?t c? hóa ðõn
                return Ok(new { Success = true, Data = new List<object>(), Message = "L?y danh sách hóa ðõn thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // GET /api/hoadon/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                // TODO: Implement logic l?y 1 hóa ðõn by ID
                return Ok(new { Success = true, Data = (object)null, Message = "L?y hóa ðõn thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // POST /api/hoadon/{id}/thanhtoan
        [HttpPost("{id}/thanhtoan")]
        public async Task<IActionResult> PayInvoice(Guid id)
        {
            try
            {
                var invoice = await _context.HoaDons.FirstOrDefaultAsync(x => x.HoaDonId == id);
                if (invoice == null) return NotFound(new { Success = false, Message = "Không t?m th?y hóa ðõn" });

                invoice.TrangThai = "DaThanhToan";
                invoice.NgayThanhToan = DateTimeOffset.Now;
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Thanh toán hóa ðõn thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
