using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    // TODO: b?t l?i [Authorize] khi hoàn thi?n login/token
    [AllowAnonymous]
    public class HopDongController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbIntrospectionService _dbInfo;

        public HopDongController(ApplicationDbContext context, IDbIntrospectionService dbInfo)
        {
            _context = context;
            _dbInfo = dbInfo;
        }

        // GET /api/hopdong/nguoithue/{nguoiThueId}/hieuluc
        [HttpGet("nguoithue/{nguoiThueId:guid}/hieuluc")]
        public async Task<IActionResult> GetActiveContractByTenantId([FromRoute] Guid nguoiThueId)
        {
            try
            {
                var exists = await _dbInfo.TableExistsAsync("HopDong");
                if (!exists)
                {
                    var dbName = await _dbInfo.GetCurrentDatabaseNameAsync();
                    return BadRequest(new { success = false, message = $"Invalid object name 'HopDong'. Backend ðang k?t n?i DB '{dbName}'. H?y ki?m tra connection string và ð?m b?o có b?ng dbo.HopDong." });
                }

                var now = DateTime.Now.Date;
                var hd = await _context.HopDongs
                    .Where(x => x.NguoiThueId == nguoiThueId)
                    .OrderByDescending(x => x.NgayBatDau)
                    .FirstOrDefaultAsync(x => x.NgayBatDau.Date <= now && (x.NgayKetThuc == null || x.NgayKetThuc.Value.Date >= now));

                if (hd == null)
                {
                    return Ok(new { Success = true, Data = (object)null, Message = "Chýa có h?p ð?ng ðang hi?u l?c" });
                }

                var dto = new
                {
                    HopDongId = hd.HopDongId,
                    TieuDePhong = $"Ph?ng {hd.PhongId.ToString().Substring(0,8).ToUpper()}",
                    DiaChi = "",
                    TenChuTro = "",
                    SdtChuTro = "",
                    NgayBatDau = hd.NgayBatDau,
                    NgayKetThuc = hd.NgayKetThuc ?? hd.NgayBatDau,
                    GiaThue = hd.TienThue,
                    TienCoc = hd.TienCoc ??0,
                    FileHopDongUrl = "",
                    TrangThai = "Ðang hi?u l?c"
                };

                return Ok(new { Success = true, Data = dto, Message = "L?y h?p ð?ng thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
