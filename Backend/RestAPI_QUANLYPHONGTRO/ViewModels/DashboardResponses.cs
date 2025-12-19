namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    /// <summary>
    /// Response cho thống kê tổng quan Dashboard
    /// </summary>
    public class DashboardStatsResponse
    {
        public int TotalRooms { get; set; }
        public int PendingRooms { get; set; }
        public int ApprovedRooms { get; set; }
        public int LockedRooms { get; set; }
        public int TotalHosts { get; set; }
        public int VerifiedHosts { get; set; }
        public int PendingHosts { get; set; }
        public int TotalTenants { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int PendingReports { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }
    }

    /// <summary>
    /// Response cho thống kê phòng theo tháng
    /// </summary>
    public class MonthlyRoomStatsResponse
    {
        public string Month { get; set; } // "2025-01"
        public int NewRooms { get; set; }
        public int ApprovedRooms { get; set; }
    }

    /// <summary>
    /// Response cho phân bố trạng thái phòng
    /// </summary>
    public class RoomStatusDistributionResponse
    {
        public int Approved { get; set; }
        public int Pending { get; set; }
        public int Rejected { get; set; }
        public int Locked { get; set; }
    }

    /// <summary>
    /// Response cho phòng chờ duyệt
    /// </summary>
    public class PendingRoomResponse
    {
        public Guid PhongId { get; set; }
        public string TieuDe { get; set; }
        public decimal GiaTien { get; set; }
        public string ChuTroName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    /// <summary>
    /// Response cho báo cáo vi phạm
    /// </summary>
    public class RecentReportResponse
    {
        public Guid BaoCaoId { get; set; }
        public string TieuDe { get; set; }
        public string LoaiThucThe { get; set; }
        public Guid? PhongId { get; set; }
        public Guid? NguoiDungId { get; set; }
        public string TrangThai { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    /// <summary>
    /// Response cho lịch sử hoạt động
    /// </summary>
    public class ActivityLogResponse
    {
        // Use long to match HanhDongAdmin.HanhDongId (bigint)
        public long ActivityId { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string PerformedBy { get; set; }
        public Guid PerformedById { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } // "success", "warning", "danger", "info"
        public string TargetType { get; set; }
        public string TargetId { get; set; }
    }
}
