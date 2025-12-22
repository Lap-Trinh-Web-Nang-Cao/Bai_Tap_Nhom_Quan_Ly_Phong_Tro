using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class LichSuService : ILichSuService
    {
        private readonly ApplicationDbContext _context;

        public LichSuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(Guid? userId, string hanhDong, string? tenBang, string? banGhiId, string? chiTiet)
        {
            var log = new LichSu
            {
                // LichSuId tự tăng
                NguoiDungId = userId,
                HanhDong = hanhDong,
                TenBang = tenBang,
                BanGhiId = banGhiId,
                ChiTiet = chiTiet,
                ThoiGian = DateTimeOffset.Now
            };

            _context.LichSus.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LichSu>> GetUserHistoryAsync(Guid userId, int limit = 20)
        {
            return await _context.LichSus
                .Where(x => x.NguoiDungId == userId)
                .OrderByDescending(x => x.ThoiGian) // Mới nhất lên đầu
                .Take(limit)
                .ToListAsync();
        }

        public async Task<bool> ClearUserHistoryAsync(Guid userId)
        {
            // Lấy tất cả lịch sử của user này
            var logs = _context.LichSus.Where(x => x.NguoiDungId == userId);

            // Xóa hàng loạt
            _context.LichSus.RemoveRange(logs);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
