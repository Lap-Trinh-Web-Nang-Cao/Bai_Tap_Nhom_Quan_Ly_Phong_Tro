using System;
using System.Collections.Generic;

namespace ADMIN_QUANLYPHONGTRO.Models.ViewModels
{
    /// <summary>
    /// ViewModel cho trang Dashboard
    /// </summary>
    public class DashboardViewModel
    {
        // Statistics Cards
        public int TotalRooms { get; set; }
        public int VerifiedHosts { get; set; }
        public int TotalTenants { get; set; }
        public int PendingReports { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int PendingRooms { get; set; }

        // Chart Data
        public List<MonthlyRoomData> RoomsChartData { get; set; }
        public RoomStatusData RoomStatusChartData { get; set; }

        // Recent Activities
        public List<ActivityLog> RecentActivities { get; set; }

        // Pending Items
        public List<PendingRoomItem> PendingRoomsList { get; set; }
        public List<PendingReportItem> PendingReportsList { get; set; }

        public DashboardViewModel()
        {
            RoomsChartData = new List<MonthlyRoomData>();
            RoomStatusChartData = new RoomStatusData();
            RecentActivities = new List<ActivityLog>();
            PendingRoomsList = new List<PendingRoomItem>();
            PendingReportsList = new List<PendingReportItem>();
        }
    }

    /// <summary>
    /// Dữ liệu phòng theo tháng (cho biểu đồ đường)
    /// </summary>
    public class MonthlyRoomData
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Dữ liệu trạng thái phòng (cho biểu đồ tròn)
    /// </summary>
    public class RoomStatusData
    {
        public int Approved { get; set; }
        public int Pending { get; set; }
        public int Locked { get; set; }
    }

    /// <summary>
    /// Log hoạt động trong hệ thống
    /// </summary>
    public class ActivityLog
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string PerformedBy { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } // success, info, warning, danger
    }

    /// <summary>
    /// Phòng chờ duyệt
    /// </summary>
    public class PendingRoomItem
    {
        public Guid RoomId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string OwnerName { get; set; }
        public DateTime SubmittedDate { get; set; }
    }

    /// <summary>
    /// Báo cáo vi phạm chờ xử lý
    /// </summary>
    public class PendingReportItem
    {
        public Guid ReportId { get; set; }
        public string ViolationType { get; set; }
        public string TargetType { get; set; }
        public string TargetName { get; set; }
        public string ReporterName { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Severity { get; set; } // Cao, Trung bình, Thấp
    }
}
