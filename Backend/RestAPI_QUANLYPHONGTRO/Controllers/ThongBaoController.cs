using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using System.Security.Claims;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Tạm thời comment để test từ giao diện demo (Fake Login)
    public class ThongBaoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static readonly Guid FakeUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public ThongBaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId()
        {
            // Thử lấy từ Token
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(id)) return Guid.Parse(id);

            // Nếu không có Token (Fake Login), dùng ID mặc định để demo "thông báo thật"
            return FakeUserId;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = GetUserId();
            var notifications = await _context.ThongBaos
                .Where(n => n.NguoiDungId == userId)
                .OrderByDescending(n => n.ThoiGianTao)
                .Take(50)
                .ToListAsync();

            return Ok(new { Success = true, Data = notifications });
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();
            var count = await _context.ThongBaos
                .CountAsync(n => n.NguoiDungId == userId && !n.DaXem);

            return Ok(new { Success = true, Data = count });
        }

        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = GetUserId();
            var notification = await _context.ThongBaos
                .FirstOrDefaultAsync(n => n.ThongBaoId == id && n.NguoiDungId == userId);

            if (notification == null) return NotFound();

            notification.DaXem = true;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }

        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();
            var unreadNotifications = await _context.ThongBaos
                .Where(n => n.NguoiDungId == userId && !n.DaXem)
                .ToListAsync();

            foreach (var n in unreadNotifications)
            {
                n.DaXem = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { Success = true });
        }

        // Action giúp tạo thông báo nhanh từ UI (để test live)
        [HttpPost("create-test")]
        public async Task<IActionResult> CreateTest([FromQuery] string title, [FromQuery] string content)
        {
            var thongBao = new ThongBao
            {
                ThongBaoId = Guid.NewGuid(),
                NguoiDungId = FakeUserId,
                TieuDe = title ?? "Thông báo mới",
                NoiDung = content ?? "Đây là nội dung thông báo thật từ DB.",
                Loai = "info",
                ThoiGianTao = DateTimeOffset.Now,
                DaXem = false
            };
            _context.ThongBaos.Add(thongBao);
            await _context.SaveChangesAsync();
            return Ok(new { Success = true, Data = thongBao });
        }
    }
}
