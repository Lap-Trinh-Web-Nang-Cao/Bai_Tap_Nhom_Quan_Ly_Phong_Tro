using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class ViPhamService : IViPhamService
    {
        private readonly ApplicationDbContext _context;

        public ViPhamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ViPham>> GetAllAsync()
        {
            return await _context.ViPhams.ToListAsync();
        }

        public async Task<ViPham?> GetByIdAsync(int id)
        {
            return await _context.ViPhams.FindAsync(id);
        }

        public async Task<ViPham> CreateAsync(ViPhamRequest request)
        {
            var viPham = new ViPham
            {
                TenViPham = request.TenViPham,
                MoTa = request.MoTa,
                HinhPhatTien = request.HinhPhatTien,
                SoDiemTru = request.SoDiemTru
            };

            _context.ViPhams.Add(viPham);
            await _context.SaveChangesAsync();
            return viPham;
        }

        public async Task<ViPham?> UpdateAsync(int id, ViPhamRequest request)
        {
            var existing = await _context.ViPhams.FindAsync(id);
            if (existing == null) return null;

            existing.TenViPham = request.TenViPham;
            existing.MoTa = request.MoTa;
            existing.HinhPhatTien = request.HinhPhatTien;
            existing.SoDiemTru = request.SoDiemTru;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ViPhams.FindAsync(id);
            if (existing == null) return false;

            // KIỂM TRA RÀNG BUỘC:
            // Nếu loại vi phạm này đã có trong bảng BaoCaoViPham (đã làm ở bài 1), không cho xóa
            bool isUsed = await _context.BaoCaoViPhams.AnyAsync(b => b.ViPhamId == id);
            if (isUsed)
            {
                throw new System.Exception("Không thể xóa loại vi phạm này vì đã tồn tại báo cáo liên quan.");
            }

            _context.ViPhams.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
