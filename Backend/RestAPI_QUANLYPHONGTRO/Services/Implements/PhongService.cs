using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class PhongService : IPhongService
    {
        private readonly ApplicationDbContext _context;

        public PhongService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Phong> Data, int TotalCount)> GetPublicRoomsAsync(
            Guid? nhaTroId,
            long? minPrice,
            long? maxPrice,
            int pageIndex,
            int pageSize)
        {
            // 1. Query cơ bản (chưa thực thi)
            var query = _context.Phongs
                .Include(p => p.NhaTro) // Join để lấy thêm địa chỉ nhà trọ nếu cần
                .Where(x => x.IsDuyet == true && x.IsBiKhoa == false);

            // 2. Các bộ lọc
            if (nhaTroId.HasValue) query = query.Where(x => x.NhaTroId == nhaTroId.Value);
            if (minPrice.HasValue) query = query.Where(x => x.GiaTien >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(x => x.GiaTien <= maxPrice.Value);

            // 3. Đếm tổng số bản ghi (để tính số trang)
            int totalCount = await query.CountAsync();

            // 4. Phân trang (Skip & Take)
            var data = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<Phong?> GetByIdAsync(Guid id)
        {
            return await _context.Phongs.FindAsync(id);
        }

        public async Task<Phong> CreateAsync(CreatePhongRequest request, Guid userId)
        {
            // 1. Kiểm tra quyền sở hữu Nhà trọ
            // (Phải đảm bảo User này là chủ của cái Nhà trọ kia)
            var nhaTro = await _context.NhaTros.FirstOrDefaultAsync(n => n.NhaTroId == request.NhaTroId && n.ChuTroId == userId);

            if (nhaTro == null)
                throw new Exception("Bạn không phải chủ sở hữu nhà trọ này hoặc nhà trọ không tồn tại.");

            // 2. Tạo phòng
            var phong = new Phong
            {
                PhongId = Guid.NewGuid(),
                NhaTroId = request.NhaTroId,
                TieuDe = request.TieuDe,
                DienTich = request.DienTich,
                GiaTien = request.GiaTien,
                TienCoc = request.TienCoc,
                SoNguoiToiDa = request.SoNguoiToiDa,

                TrangThai = "Trong", // Mặc định là Trống
                CreatedAt = DateTimeOffset.Now,

                // Mặc định cần Admin duyệt mới hiện
                IsDuyet = false,
                IsBiKhoa = false,

                // Khởi tạo điểm đánh giá
                DiemTrungBinh = 0,
                SoLuongDanhGia = 0
            };

            _context.Phongs.Add(phong);
            await _context.SaveChangesAsync();
            return phong;
        }

        public async Task<Phong?> UpdateAsync(Guid id, CreatePhongRequest request, Guid userId)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return null;

            // Kiểm tra quyền: Phải query ngược lại bảng NhaTro để xem User có phải chủ không
            var isOwner = await _context.NhaTros.AnyAsync(n => n.NhaTroId == phong.NhaTroId && n.ChuTroId == userId);
            if (!isOwner) throw new Exception("Không có quyền chỉnh sửa.");

            phong.TieuDe = request.TieuDe;
            phong.DienTich = request.DienTich;
            phong.GiaTien = request.GiaTien;
            phong.TienCoc = request.TienCoc;
            phong.SoNguoiToiDa = request.SoNguoiToiDa;
            phong.UpdatedAt = DateTimeOffset.Now;

            // Lưu ý: Nếu sửa thông tin quan trọng, có thể cần reset IsDuyet = false để Admin duyệt lại
            // phong.IsDuyet = false; 

            await _context.SaveChangesAsync();
            return phong;
        }

        public async Task<bool> ApproveRoomAsync(Guid id, Guid adminId)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return false;

            phong.IsDuyet = true;
            phong.NguoiDuyet = adminId;
            phong.ThoiGianDuyet = DateTimeOffset.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LockRoomAsync(Guid id, bool isLocked)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return false;

            phong.IsBiKhoa = isLocked;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null) return false;

            // Thay vì _context.Phongs.Remove(phong);
            phong.IsDeleted = true; // Đánh dấu đã xóa

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
