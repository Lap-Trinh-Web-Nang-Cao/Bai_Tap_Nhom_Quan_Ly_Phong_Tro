using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordDashboardViewModel
    {
        // 1. Số liệu thống kê (Statistics)
        public int TongSoPhong { get; set; }          // Thay cho TotalRooms
        public int LuotXemHomNay { get; set; }        // Thay cho ViewsToday
        public int LichHenSapToi { get; set; }        // Thay cho UpcomingSchedules
        public decimal DoanhThuThang { get; set; }    // Thay cho RevenueThisMonth

        // 2. Các danh sách dữ liệu (Lists)
        public List<PhongChoDuyetItem> DanhSachPhongCho { get; set; }   // PendingRooms
        public List<LichHenItem> LichHenHomNay { get; set; }            // TodaySchedules
        public List<TinNhanItem> TinNhanGanDay { get; set; }           // RecentMessages
        public List<YeuCauSuaChuaItem> YeuCauSuaChua { get; set; }      // MaintenanceRequests
    }

    // --- CÁC CLASS CON (Đã đổi tên tiếng Việt) ---

    public class PhongChoDuyetItem
    {
        public Guid MaPhong { get; set; }         // PhongId
        public string TenPhong { get; set; }      // Name
        public string HinhAnh { get; set; }       // ImageUrl
        public DateTime NgayDang { get; set; }    // SubmitDate
    }

    public class LichHenItem
    {
        public string TenKhach { get; set; }      // TenantName
        public string TenPhong { get; set; }      // RoomName
        public string ThoiGianXem { get; set; }   // ViewTime (để string cho dễ format giờ)
        public string TrangThai { get; set; }     // Status (Chờ xác nhận/Đã duyệt...)
    }

    public class TinNhanItem
    {
        public string TenNguoiGui { get; set; }   // SenderName
        public string AnhDaiDien { get; set; }    // Avatar
        public string NoiDung { get; set; }       // Content
        public string ThoiGian { get; set; }      // Time (VD: "5 phút trước")
    }

    public class YeuCauSuaChuaItem
    {
        public string TieuDe { get; set; }        // Title
        public string TenPhong { get; set; }      // RoomName
        public string NguoiBaoCao { get; set; }   // ReporterName
        public string MucDoUuTien { get; set; }   // Priority (Thấp/Trung bình/Cao)
    }
}