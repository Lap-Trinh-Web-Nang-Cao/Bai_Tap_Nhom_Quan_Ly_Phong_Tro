using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class PhuongService : IPhuongService
    {
        private readonly ApplicationDbContext _context;

        public PhuongService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Phuong>> GetAllAsync()
        {
            return await _context.Phuongs.ToListAsync();
        }

        public async Task<IEnumerable<Phuong>> GetByQuanIdAsync(int quanId)
        {
            // Chỉ lấy các phường thuộc quận được chọn
            return await _context.Phuongs
                .Where(x => x.QuanHuyenId == quanId)
                .OrderBy(x => x.Ten)
                .ToListAsync();
        }

        public async Task<Phuong?> GetByIdAsync(int id)
        {
            return await _context.Phuongs.FindAsync(id);
        }

        public async Task<Phuong> CreateAsync(PhuongRequest request)
        {
            // (Tuỳ chọn) Kiểm tra xem QuanHuyenId có tồn tại không trước khi thêm
            var quanExists = await _context.QuanHuyens.AnyAsync(q => q.QuanHuyenId == request.QuanHuyenId);
            if (!quanExists) throw new System.Exception("Mã quận huyện không tồn tại.");

            var item = new Phuong
            {
                QuanHuyenId = request.QuanHuyenId,
                Ten = request.Ten
            };

            _context.Phuongs.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Phuong?> UpdateAsync(int id, PhuongRequest request)
        {
            var existing = await _context.Phuongs.FindAsync(id);
            if (existing == null) return null;

            existing.Ten = request.Ten;
            existing.QuanHuyenId = request.QuanHuyenId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Phuongs.FindAsync(id);
            if (existing == null) return false;

            // Kiểm tra ràng buộc: Nếu có nhà trọ ở phường này thì không được xóa
            bool isUsed = await _context.NhaTros.AnyAsync(n => n.PhuongId == id);
            if (isUsed) throw new System.Exception("Không thể xóa phường này vì đang có nhà trọ trực thuộc.");

            _context.Phuongs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
