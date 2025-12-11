using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class QuanHuyenService : IQuanHuyenService
    {
        private readonly ApplicationDbContext _context;

        public QuanHuyenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuanHuyen>> GetAllAsync()
        {
            // Sắp xếp theo tên cho dễ nhìn
            return await _context.QuanHuyens.OrderBy(x => x.Ten).ToListAsync();
        }

        public async Task<QuanHuyen?> GetByIdAsync(int id)
        {
            return await _context.QuanHuyens.FindAsync(id);
        }

        public async Task<QuanHuyen> CreateAsync(QuanHuyenRequest request)
        {
            var item = new QuanHuyen
            {
                Ten = request.Ten
            };

            _context.QuanHuyens.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<QuanHuyen?> UpdateAsync(int id, QuanHuyenRequest request)
        {
            var existing = await _context.QuanHuyens.FindAsync(id);
            if (existing == null) return null;

            existing.Ten = request.Ten;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.QuanHuyens.FindAsync(id);
            if (existing == null) return false;

            // Kiểm tra ràng buộc: Nếu đã có Nhà trọ thuộc quận này thì không cho xóa
            bool isUsed = await _context.NhaTros.AnyAsync(n => n.QuanHuyenId == id);
            if (isUsed) throw new System.Exception("Không thể xóa quận huyện này vì đang có nhà trọ trực thuộc.");

            _context.QuanHuyens.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
