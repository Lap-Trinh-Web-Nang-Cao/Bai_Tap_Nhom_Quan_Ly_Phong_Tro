using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordDashboardViewModel
    {
        // Tổng quan
        public int TotalRooms { get; set; }
        public int ViewsToday { get; set; }
        public int UpcomingSchedules { get; set; }
        public decimal RevenueThisMonth { get; set; }

        // Danh sách phòng chờ duyệt
        public List<PendingRoomItem> PendingRooms { get; set; }

        // Lịch xem hôm nay
        public List<TodayScheduleItem> TodaySchedules { get; set; }

        // Tin nhắn gần đây
        public List<RecentMessageItem> RecentMessages { get; set; }

        // Yêu cầu sửa chữa
        public List<MaintenanceRequestItem> MaintenanceRequests { get; set; }
    }

    // ----------------- CHILD MODELS -----------------

    public class PendingRoomItem
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public DateTime SubmitDate { get; set; }
    }

    public class TodayScheduleItem
    {
        public string TenantName { get; set; }
        public string RoomName { get; set; }
        public string ViewTime { get; set; }
        public string Status { get; set; }
    }

    public class RecentMessageItem
    {
        public string SenderName { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
    }

    public class MaintenanceRequestItem
    {
        public string RoomName { get; set; }
        public string Title { get; set; }
        public string ReporterName { get; set; }
        public string Priority { get; set; }
    }
}
