using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class TrangThaiDatPhongService : ITrangThaiDatPhongService
    {
        private readonly ApplicationDbContext _context;

        public TrangThaiDatPhongService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrangThaiDatPhong>> GetAllAsync()
        {
            return await _context.TrangThaiDatPhongs.ToListAsync();
        }

        public async Task<TrangThaiDatPhong?> GetByIdAsync(int id)
        {
            return await _context.TrangThaiDatPhongs.FindAsync(id);
        }

        public async Task<TrangThaiDatPhong?> UpdateAsync(int id, TrangThaiRequest request)
        {
            var existing = await _context.TrangThaiDatPhongs.FindAsync(id);
            if (existing == null) return null;

            existing.TenTrangThai = request.TenTrangThai;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
