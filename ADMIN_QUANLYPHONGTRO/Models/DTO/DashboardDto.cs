using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    /// <summary>
    /// DTO cho thống kê tổng quan Dashboard
    /// </summary>
    public class DashboardStatsDto
    {
        /// <summary>
        /// Tổng số phòng trong hệ thống
        /// </summary>
        public int TotalRooms { get; set; }

        /// <summary>
        /// Số phòng chờ duyệt
        /// </summary>
        public int PendingRooms { get; set; }

        /// <summary>
        /// Số phòng đã duyệt
        /// </summary>
        public int ApprovedRooms { get; set; }

        /// <summary>
        /// Số phòng bị khóa
        /// </summary>
        public int LockedRooms { get; set; }

        /// <summary>
        /// Tổng số chủ trọ
        /// </summary>
        public int TotalHosts { get; set; }

        /// <summary>
        /// Số chủ trọ đã xác minh
        /// </summary>
        public int VerifiedHosts { get; set; }

        /// <summary>
        /// Số chủ trọ chờ xác minh
        /// </summary>
        public int PendingHosts { get; set; }

        /// <summary>
        /// Tổng số người thuê
        /// </summary>
        public int TotalTenants { get; set; }

        /// <summary>
        /// Số người dùng mới trong tháng
        /// </summary>
        public int NewUsersThisMonth { get; set; }

        /// <summary>
        /// Số báo cáo vi phạm chờ xử lý
        /// </summary>
        public int PendingReports { get; set; }

        /// <summary>
        /// Doanh thu tháng này (VND)
        /// </summary>
        public decimal MonthlyRevenue { get; set; }

        /// <summary>
        /// Tăng trưởng doanh thu so với tháng trước (%)
        /// </summary>
        public decimal RevenueGrowth { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê phòng theo tháng
    /// </summary>
    public class MonthlyRoomStatsDto
    {
        public string Month { get; set; }  // "2025-01"
        public int NewRooms { get; set; }  // Phòng đăng mới
        public int ApprovedRooms { get; set; }  // Phòng được duyệt
    }

    /// <summary>
    /// DTO cho phân bố trạng thái phòng
    /// </summary>
    public class RoomStatusDistributionDto
    {
        public int Approved { get; set; }  // Đã duyệt
        public int Pending { get; set; }   // Chờ duyệt
        public int Rejected { get; set; }  // Từ chối
        public int Locked { get; set; }    // Bị khóa
    }

    /// <summary>
    /// DTO cho doanh thu theo tháng
    /// </summary>
    public class MonthlyRevenueDto
    {
        public string Month { get; set; }  // "2025-01"
        public decimal Revenue { get; set; }  // Doanh thu (VND)
        public int TransactionCount { get; set; }  // Số giao dịch
    }

    /// <summary>
    /// DTO cho lịch sử hoạt động
    /// </summary>
    public class ActivityLogDto
    {
        public Guid ActivityId { get; set; }
        public string Action { get; set; }  // "DUYET_PHONG", "KHOA_TAI_KHOAN"...
        public string Description { get; set; }  // Mô tả chi tiết
        public string PerformedBy { get; set; }  // Tên admin thực hiện
        public Guid PerformedById { get; set; }  // ID admin
        public DateTime Timestamp { get; set; }  // Thời gian
        public string Type { get; set; }  // "success", "warning", "danger", "info"
        public string TargetType { get; set; }  // "Phong", "NguoiDung", "ChuTro"
        public string TargetId { get; set; }  // ID đối tượng
    }
}
