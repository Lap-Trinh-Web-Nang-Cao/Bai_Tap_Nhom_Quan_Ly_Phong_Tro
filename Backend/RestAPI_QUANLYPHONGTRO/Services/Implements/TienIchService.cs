using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class TienIchService : ITienIchService
    {
        private readonly ApplicationDbContext _context;

        public TienIchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TienIch>> GetAllAsync()
        {
            return await _context.TienIchs.ToListAsync();
        }

        public async Task<TienIch?> GetByIdAsync(int id)
        {
            return await _context.TienIchs.FindAsync(id);
        }

        public async Task<TienIch> CreateAsync(TienIchRequest request)
        {
            var item = new TienIch
            {
                Ten = request.Ten
            };

            _context.TienIchs.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<TienIch?> UpdateAsync(int id, TienIchRequest request)
        {
            var existing = await _context.TienIchs.FindAsync(id);
            if (existing == null) return null;

            existing.Ten = request.Ten;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.TienIchs.FindAsync(id);
            if (item == null) return false;

            _context.TienIchs.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
