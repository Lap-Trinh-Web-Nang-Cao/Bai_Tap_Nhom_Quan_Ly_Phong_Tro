using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class HanhDongAdminService : IHanhDongAdminService
    {
        private readonly ApplicationDbContext _context;

        public HanhDongAdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(Guid adminId, string hanhDong, string? bang, string? recordId, string? chiTiet)
        {
            var log = new HanhDongAdmin
            {
                // HanhDongId tự tăng (Identity) nên không cần gán
                AdminId = adminId,
                HanhDong = hanhDong,
                MucTieuBang = bang,
                BanGhiId = recordId,
                ChiTiet = chiTiet,
                ThoiGian = DateTimeOffset.Now
            };

            _context.HanhDongAdmins.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<HanhDongAdmin>> GetLatestLogsAsync(int count)
        {
            return await _context.HanhDongAdmins
                .OrderByDescending(x => x.ThoiGian) // Mới nhất lên đầu
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<HanhDongAdmin>> GetLogsByRecordIdAsync(string recordId)
        {
            return await _context.HanhDongAdmins
               .Where(x => x.BanGhiId == recordId)
               .OrderByDescending(x => x.ThoiGian)
               .ToListAsync();
        }
    }
}
