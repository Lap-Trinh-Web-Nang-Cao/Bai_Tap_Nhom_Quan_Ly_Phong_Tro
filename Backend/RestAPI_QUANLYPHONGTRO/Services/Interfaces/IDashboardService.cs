using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface cho Dashboard Service - Cung cấp dữ liệu thống kê
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Lấy thống kê tổng quan Dashboard
        /// </summary>
        Task<DashboardStatsResponse> GetDashboardStatsAsync();

        /// <summary>
        /// Lấy thống kê phòng theo tháng (12 tháng gần nhất)
        /// </summary>
        Task<List<MonthlyRoomStatsResponse>> GetMonthlyRoomStatsAsync(int months = 12);

        /// <summary>
        /// Lấy phân bố trạng thái phòng
        /// </summary>
        Task<RoomStatusDistributionResponse> GetRoomStatusDistributionAsync();

        /// <summary>
        /// Lấy danh sách phòng chờ duyệt (với pagination)
        /// </summary>
        Task<PagedResult<PendingRoomResponse>> GetPendingRoomsAsync(int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// Lấy danh sách phòng chờ duyệt Top N (dùng cho Dashboard widget)
        /// </summary>
        Task<List<PendingRoomResponse>> GetTopPendingRoomsAsync(int top = 5);

        /// <summary>
        /// Lấy danh sách báo cáo vi phạm mới (top N)
        /// </summary>
        Task<List<RecentReportResponse>> GetRecentReportsAsync(int top = 5);

        /// <summary>
        /// Lấy lịch sử hoạt động gần đây (top N)
        /// </summary>
        Task<List<ActivityLogResponse>> GetRecentActivitiesAsync(int top = 10);

        /// <summary>
        /// Lấy số người dùng mới đăng ký trong tháng này
        /// </summary>
        Task<int> GetNewUsersThisMonthAsync();
    }
}
