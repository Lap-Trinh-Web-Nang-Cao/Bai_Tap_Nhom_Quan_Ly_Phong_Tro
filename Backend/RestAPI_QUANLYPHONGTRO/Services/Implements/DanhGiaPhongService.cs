using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class DanhGiaPhongService : IDanhGiaPhongService
    {
        private readonly ApplicationDbContext _context;

        public DanhGiaPhongService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DanhGiaPhong> CreateAsync(CreateDanhGiaRequest request, Guid userId)
        {
            // (Tuỳ chọn) Bạn có thể check xem userId này đã thuê phòng này chưa mới cho đánh giá
            // Code check logic thuê phòng ở đây...

            var danhGia = new DanhGiaPhong
            {
                DanhGiaId = Guid.NewGuid(),
                PhongId = request.PhongId,
                NguoiDanhGia = userId, // Lấy từ Token truyền vào
                Diem = request.Diem,
                NoiDung = request.NoiDung,
                ThoiGian = DateTimeOffset.Now
            };

            _context.DanhGiaPhongs.Add(danhGia);
            await _context.SaveChangesAsync();
            return danhGia;
        }

        public async Task<IEnumerable<DanhGiaPhong>> GetByPhongIdAsync(Guid phongId)
        {
            // Lấy danh sách, sắp xếp mới nhất lên đầu
            return await _context.DanhGiaPhongs
                .Where(x => x.PhongId == phongId)
                .OrderByDescending(x => x.ThoiGian)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(Guid danhGiaId, Guid userId, bool isAdmin)
        {
            var danhGia = await _context.DanhGiaPhongs.FindAsync(danhGiaId);
            if (danhGia == null) return false;

            // Chỉ cho phép xóa nếu là Admin HOẶC là người viết đánh giá đó
            if (!isAdmin && danhGia.NguoiDanhGia != userId)
            {
                return false; // Không có quyền
            }

            _context.DanhGiaPhongs.Remove(danhGia);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
