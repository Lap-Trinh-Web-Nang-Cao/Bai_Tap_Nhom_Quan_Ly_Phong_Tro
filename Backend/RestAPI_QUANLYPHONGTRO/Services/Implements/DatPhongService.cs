using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class DatPhongService : IDatPhongService
    {
        private readonly ApplicationDbContext _context;

        public DatPhongService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DatPhong> CreateBookingAsync(CreateDatPhongRequest request, Guid userId)
        {
            // 1. (Nâng cao) Kiểm tra xem phòng có bị trùng lịch không
            /*
            var isConflict = await _context.DatPhongs.AnyAsync(d => 
                d.PhongId == request.PhongId && 
                d.TrangThaiId != 3 && // Không tính đơn đã hủy
                (request.BatDau < d.KetThuc && request.KetThuc > d.BatDau) // Logic giao nhau thời gian
            );
            if (isConflict) throw new Exception("Phòng đã có người đặt trong khoảng thời gian này.");
            */

            // 2. Tạo đơn đặt phòng
            var datPhong = new DatPhong
            {
                DatPhongId = Guid.NewGuid(),
                PhongId = request.PhongId,
                NguoiThueId = userId, // Lấy từ Token
                ChuTroId = request.ChuTroId,
                Loai = request.Loai,
                BatDau = request.BatDau,
                KetThuc = request.KetThuc,
                ThoiGianTao = DateTimeOffset.Now,
                TrangThaiId = 1, // Mặc định: 1 = Chờ xác nhận
                GhiChu = request.GhiChu,

                // Giả lập số đơn hàng (nên dùng Sequence trong SQL hoặc logic riêng)
                SoDatPhong = new Random().Next(100000, 999999)
            };

            _context.DatPhongs.Add(datPhong);
            await _context.SaveChangesAsync();
            return datPhong;
        }

        public async Task<IEnumerable<DatPhong>> GetMyBookingsAsync(Guid userId)
        {
            return await _context.DatPhongs
                .Where(x => x.NguoiThueId == userId)
                .OrderByDescending(x => x.ThoiGianTao)
                .ToListAsync();
        }

        public async Task<IEnumerable<DatPhong>> GetRequestsForLandlordAsync(Guid chuTroId)
        {
            return await _context.DatPhongs
                .Where(x => x.ChuTroId == chuTroId)
                .OrderByDescending(x => x.ThoiGianTao)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid datPhongId, int trangThaiId, Guid currentUserId)
        {
            var booking = await _context.DatPhongs.FindAsync(datPhongId);
            if (booking == null) return false;

            // Bảo mật: Chỉ chủ trọ của đơn này mới được duyệt
            if (booking.ChuTroId != currentUserId) return false;

            booking.TrangThaiId = trangThaiId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
