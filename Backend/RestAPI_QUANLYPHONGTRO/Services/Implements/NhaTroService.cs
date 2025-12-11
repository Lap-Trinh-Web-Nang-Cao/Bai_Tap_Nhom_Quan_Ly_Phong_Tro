using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class NhaTroService : INhaTroService
    {
        private readonly ApplicationDbContext _context;

        public NhaTroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NhaTro>> GetAllActiveAsync()
        {
            // Chỉ lấy những nhà trọ đang hoạt động (IsHoatDong == true hoặc null tùy quy ước)
            return await _context.NhaTros
                .Where(x => x.IsHoatDong == true)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<NhaTro?> GetByIdAsync(Guid id)
        {
            return await _context.NhaTros.FindAsync(id);
        }

        public async Task<IEnumerable<NhaTro>> GetByOwnerIdAsync(Guid chuTroId)
        {
            return await _context.NhaTros
                .Where(x => x.ChuTroId == chuTroId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<NhaTro> CreateAsync(NhaTroRequest request, Guid chuTroId)
        {
            var nhaTro = new NhaTro
            {
                NhaTroId = Guid.NewGuid(),
                ChuTroId = chuTroId, // Lấy từ Token
                TieuDe = request.TieuDe,
                DiaChi = request.DiaChi,
                QuanHuyenId = request.QuanHuyenId,
                PhuongId = request.PhuongId,
                CreatedAt = DateTimeOffset.Now,
                IsHoatDong = request.IsHoatDong ?? true // Mặc định là true nếu không gửi
            };

            _context.NhaTros.Add(nhaTro);
            await _context.SaveChangesAsync();
            return nhaTro;
        }

        public async Task<NhaTro?> UpdateAsync(Guid id, NhaTroRequest request, Guid chuTroId)
        {
            var existing = await _context.NhaTros.FindAsync(id);
            if (existing == null) return null;

            // Bảo mật: Chỉ chủ sở hữu mới được sửa
            if (existing.ChuTroId != chuTroId) return null; // Hoặc throw Exception tuỳ bạn

            existing.TieuDe = request.TieuDe;
            existing.DiaChi = request.DiaChi;
            existing.QuanHuyenId = request.QuanHuyenId;
            existing.PhuongId = request.PhuongId;

            if (request.IsHoatDong.HasValue)
                existing.IsHoatDong = request.IsHoatDong.Value;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid currentUserId, bool isAdmin)
        {
            var existing = await _context.NhaTros.FindAsync(id);
            if (existing == null) return false;

            // Chỉ Admin hoặc Chủ sở hữu mới được xóa
            if (!isAdmin && existing.ChuTroId != currentUserId) return false;

            _context.NhaTros.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
