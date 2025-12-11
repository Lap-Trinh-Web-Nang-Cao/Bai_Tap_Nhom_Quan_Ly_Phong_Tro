using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class LoaiHoTroService : ILoaiHoTroService
    {
        private readonly ApplicationDbContext _context;

        public LoaiHoTroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoaiHoTro>> GetAllAsync()
        {
            return await _context.LoaiHoTros.ToListAsync();
        }

        public async Task<LoaiHoTro?> GetByIdAsync(int id)
        {
            return await _context.LoaiHoTros.FindAsync(id);
        }

        public async Task<LoaiHoTro> CreateAsync(LoaiHoTroRequest request)
        {
            var item = new LoaiHoTro
            {
                // ID tự tăng nên không cần gán
                TenLoai = request.TenLoai
            };

            _context.LoaiHoTros.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<LoaiHoTro?> UpdateAsync(int id, LoaiHoTroRequest request)
        {
            var existingItem = await _context.LoaiHoTros.FindAsync(id);
            if (existingItem == null) return null;

            existingItem.TenLoai = request.TenLoai;

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.LoaiHoTros.FindAsync(id);
            if (item == null) return false;

            _context.LoaiHoTros.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
