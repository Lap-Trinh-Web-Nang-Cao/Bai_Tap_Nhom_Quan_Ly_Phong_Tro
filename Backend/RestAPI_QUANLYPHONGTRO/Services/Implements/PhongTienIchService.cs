using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class PhongTienIchService : IPhongTienIchService
    {
        private readonly ApplicationDbContext _context;

        public PhongTienIchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TienIch>> GetAmenitiesByRoomIdAsync(Guid phongId)
        {
            // Query lồng để lấy thông tin chi tiết của tiện ích
            return await _context.PhongTienIchs
                .Where(x => x.PhongId == phongId)
                .Join(_context.TienIchs,
                      pti => pti.TienIchId,
                      ti => ti.TienIchId,
                      (pti, ti) => ti) // Chỉ lấy object TienIch
                .ToListAsync();
        }

        public async Task<bool> AddAsync(PhongTienIchRequest request, Guid userId)
        {
            // 1. Check quyền sở hữu: Phòng này có thuộc về Nhà trọ của User này không?
            var isOwner = await _context.Phongs
                .Include(p => p.NhaTro) // Join sang Nhà trọ
                .AnyAsync(p => p.PhongId == request.PhongId && p.NhaTro.ChuTroId == userId); // Check ID chủ

            if (!isOwner) throw new Exception("Bạn không có quyền chỉnh sửa phòng này.");

            // 2. Check xem đã tồn tại chưa (tránh trùng lặp)
            var exists = await _context.PhongTienIchs
                .AnyAsync(x => x.PhongId == request.PhongId && x.TienIchId == request.TienIchId);

            if (exists) return false; // Đã có rồi thì bỏ qua

            // 3. Thêm mới
            var item = new PhongTienIch
            {
                PhongId = request.PhongId,
                TienIchId = request.TienIchId
            };

            _context.PhongTienIchs.Add(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAsync(Guid phongId, int tienIchId, Guid userId)
        {
            // 1. Check quyền sở hữu
            var isOwner = await _context.Phongs
                .Include(p => p.NhaTro)
                .AnyAsync(p => p.PhongId == phongId && p.NhaTro.ChuTroId == userId);

            if (!isOwner) throw new Exception("Bạn không có quyền chỉnh sửa phòng này.");

            // 2. Tìm và xóa
            var item = await _context.PhongTienIchs
                .FirstOrDefaultAsync(x => x.PhongId == phongId && x.TienIchId == tienIchId);

            if (item == null) return false;

            _context.PhongTienIchs.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
