using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class VaiTroService : IVaiTroService
    {
        private readonly ApplicationDbContext _context;

        public VaiTroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VaiTro>> GetAllAsync()
        {
            return await _context.VaiTros.ToListAsync();
        }

        public async Task<VaiTro?> GetByIdAsync(int id)
        {
            return await _context.VaiTros.FindAsync(id);
        }

        public async Task<VaiTro> CreateAsync(VaiTroRequest request)
        {
            var vaiTro = new VaiTro
            {
                TenVaiTro = request.TenVaiTro
            };

            _context.VaiTros.Add(vaiTro);
            await _context.SaveChangesAsync();
            return vaiTro;
        }

        public async Task<VaiTro?> UpdateAsync(int id, VaiTroRequest request)
        {
            var existing = await _context.VaiTros.FindAsync(id);
            if (existing == null) return null;

            existing.TenVaiTro = request.TenVaiTro;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.VaiTros.FindAsync(id);
            if (existing == null) return false;

            // Kiểm tra ràng buộc: Nếu có User nào đang dùng vai trò này thì KHÔNG ĐƯỢC XÓA
            // (Đoạn này tránh lỗi SQL Foreign Key Constraint)
            bool isInUse = await _context.NguoiDungs.AnyAsync(u => u.VaiTroId == id);
            if (isInUse)
            {
                // Bạn có thể throw exception hoặc return false tuỳ cách xử lý lỗi
                throw new System.Exception("Không thể xóa vai trò đang có người sử dụng.");
            }

            _context.VaiTros.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
