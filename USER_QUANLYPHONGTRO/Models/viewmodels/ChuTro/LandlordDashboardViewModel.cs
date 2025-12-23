using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordDashboardViewModel
    {
        // Tổng quan
        public int TotalRooms { get; set; }
        public int TongSoPhong { get; set; }
        public int ViewsToday { get; set; }
        public int UpcomingSchedules { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal DoanhThuThang { get; set; }

        // Thống kê phòng
        public int SoPhongTrong { get; set; }
        public int SoPhongDaThue { get; set; }
        public int SoPhongDangSuaChua { get; set; }

        // Thống kê đơn đặt
        public int DonChoXacNhan { get; set; }
        public int DonDaCoc { get; set; }
        public int HopDongHieuLuc { get; set; }

        // Danh sách phòng chờ duyệt
        public List<PendingRoomItem> PendingRooms { get; set; } = new List<PendingRoomItem>();

        // Lịch xem hôm nay
        public List<TodayScheduleItem> TodaySchedules { get; set; } = new List<TodayScheduleItem>();

        // Tin nhắn gần đây
        public List<RecentMessageItem> RecentMessages { get; set; } = new List<RecentMessageItem>();

        // Yêu cầu sửa chữa
        public List<MaintenanceRequestItem> MaintenanceRequests { get; set; } = new List<MaintenanceRequestItem>();
    }

    // ----------------- CHILD MODELS -----------------

    public class PendingRoomItem
    {
        public Guid PhongId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public DateTime SubmitDate { get; set; }
    }

    public class TodayScheduleItem
    {
        public Guid DatPhongId { get; set; }
        public string TenantName { get; set; }
        public string RoomName { get; set; }
        public string ViewTime { get; set; }
        public string Status { get; set; }
    }

    public class RecentMessageItem
    {
        public Guid UserId { get; set; }
        public string SenderName { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
    }

    public class MaintenanceRequestItem
    {
        public Guid YeuCauId { get; set; }
        public string RoomName { get; set; }
        public string Title { get; set; }
        public string ReporterName { get; set; }
        public string Priority { get; set; }
    }
}
