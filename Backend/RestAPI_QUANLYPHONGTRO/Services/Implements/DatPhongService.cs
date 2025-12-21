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

            // 3. Thông báo cho NGƯỜI THUÊ (Confirm request sent)
            var thongBaoKhach = new ThongBao
            {
                ThongBaoId = Guid.NewGuid(),
                NguoiDungId = userId,
                TieuDe = "Gửi yêu cầu thành công!",
                NoiDung = $"Yêu cầu đặt phòng của bạn đã được gửi tới chủ trọ. Vui lòng đợi phản hồi.",
                Loai = "success",
                ThoiGianTao = DateTimeOffset.Now,
                DaXem = false,
                RedirectUrl = "/KhachThue/LichDaDat"
            };
            _context.ThongBaos.Add(thongBaoKhach);

            // 4. Thông báo cho CHỦ TRỌ (New request alert)
            var thongBaoChuTro = new ThongBao
            {
                ThongBaoId = Guid.NewGuid(),
                NguoiDungId = request.ChuTroId,
                TieuDe = "Yêu cầu đặt phòng mới",
                NoiDung = $"Bạn có một yêu cầu mới cho phòng. Vui lòng kiểm tra và phản hồi.",
                Loai = "info",
                ThoiGianTao = DateTimeOffset.Now,
                DaXem = false,
                RedirectUrl = "/ChuTro/ManageBookings"
            };
            _context.ThongBaos.Add(thongBaoChuTro);

            await _context.SaveChangesAsync();
            return datPhong;
        }

        public async Task<IEnumerable<BookingDetailDto>> GetMyBookingsAsync(Guid userId)
        {
            var query = from d in _context.DatPhongs
                        join p in _context.Phongs on d.PhongId equals p.PhongId
                        join n in _context.NhaTros on p.NhaTroId equals n.NhaTroId
                        join c in _context.NguoiDungs on d.ChuTroId equals c.NguoiDungId
                        where d.NguoiThueId == userId
                        orderby d.ThoiGianTao descending
                        select new BookingDetailDto
                        {
                            DatPhongId = d.DatPhongId,
                            PhongId = d.PhongId,
                            TieuDePhong = p.TieuDe,
                            DiaChi = n.DiaChi,
                            Loai = d.Loai,
                            BatDau = d.BatDau,
                            KetThuc = d.KetThuc,
                            ThoiGianTao = d.ThoiGianTao ?? DateTimeOffset.Now,
                            TrangThaiId = d.TrangThaiId,
                            TenTrangThai = d.TrangThaiId == 1 ? "Chờ xác nhận" : (d.TrangThaiId == 2 ? "Đã xác nhận" : "Bị từ chối"),
                            SdtChuTro = c.DienThoai,
                            GhiChu = d.GhiChu
                        };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<BookingDetailDto>> GetRequestsForLandlordAsync(Guid chuTroId)
        {
            var query = from d in _context.DatPhongs
                        join p in _context.Phongs on d.PhongId equals p.PhongId
                        join n in _context.NhaTros on p.NhaTroId equals n.NhaTroId
                        join t in _context.NguoiDungs on d.NguoiThueId equals t.NguoiDungId
                        join h in _context.HoSoNguoiDungs on t.NguoiDungId equals h.NguoiDungId into hs
                        from h in hs.DefaultIfEmpty()
                        where d.ChuTroId == chuTroId
                        orderby d.ThoiGianTao descending
                        select new BookingDetailDto
                        {
                            DatPhongId = d.DatPhongId,
                            PhongId = d.PhongId,
                            TieuDePhong = p.TieuDe,
                            DiaChi = n.DiaChi,
                            Loai = d.Loai,
                            BatDau = d.BatDau,
                            KetThuc = d.KetThuc,
                            ThoiGianTao = d.ThoiGianTao ?? DateTimeOffset.Now,
                            TrangThaiId = d.TrangThaiId,
                            TenTrangThai = d.TrangThaiId == 1 ? "Chờ xác nhận" : (d.TrangThaiId == 2 ? "Đã xác nhận" : "Bị từ chối"),
                            HoTenNguoiThue = h.HoTen ?? "Người dùng",
                            SdtNguoiThue = t.DienThoai,
                            GhiChu = d.GhiChu
                        };

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid datPhongId, int trangThaiId, Guid currentUserId)
        {
            var booking = await _context.DatPhongs.FindAsync(datPhongId);
            if (booking == null) return false;

            // Bảo mật: Chỉ chủ trọ của đơn này mới được duyệt
            if (booking.ChuTroId != currentUserId) return false;

            booking.TrangThaiId = trangThaiId;

            // Tạo thông báo cho người thuê về việc thay đổi trạng thái
            string statusName = trangThaiId == 2 ? "đã được DUYỆT" : (trangThaiId == 3 ? "bị TỪ CHỐI" : "đã thay đổi");
            var thongBao = new ThongBao
            {
                ThongBaoId = Guid.NewGuid(),
                NguoiDungId = booking.NguoiThueId,
                TieuDe = "Cập nhật trạng thái đặt phòng",
                NoiDung = $"Yêu cầu đặt phòng của bạn {statusName} bởi chủ trọ.",
                Loai = trangThaiId == 2 ? "success" : "info",
                ThoiGianTao = DateTimeOffset.Now,
                DaXem = false,
                RedirectUrl = "/KhachThue/LichDaDat"
            };
            _context.ThongBaos.Add(thongBao);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
