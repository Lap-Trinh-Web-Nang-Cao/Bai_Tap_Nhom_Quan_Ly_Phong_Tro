using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    /// <summary>
    /// API Client cho Dashboard - Gọi các endpoint thống kê
    /// </summary>
    public class DashboardApiClient : BaseApiClient
    {
        private const string BASE_PATH = "api/dashboard";

        /// <summary>
        /// Lấy tổng quan thống kê
        /// </summary>
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            return await GetAsync<DashboardStatsDto>($"{BASE_PATH}/stats");
        }

        /// <summary>
        /// Lấy dữ liệu biểu đồ phòng theo tháng (6 tháng gần nhất)
        /// </summary>
        public async Task<List<MonthlyRoomStatsDto>> GetMonthlyRoomStatsAsync(int months = 12)
        {
            return await GetAsync<List<MonthlyRoomStatsDto>>($"{BASE_PATH}/rooms/monthly?months={months}");
        }

        /// <summary>
        /// Lấy dữ liệu phân bố phòng theo trạng thái
        /// </summary>
        public async Task<RoomStatusDistributionDto> GetRoomStatusDistributionAsync()
        {
            return await GetAsync<RoomStatusDistributionDto>($"{BASE_PATH}/rooms/status-distribution");
        }

        /// <summary>
        /// Lấy danh sách phòng chờ duyệt (top 5)
        /// </summary>
        public async Task<List<PhongDto>> GetPendingRoomsAsync(int top = 5)
        {
            return await GetAsync<List<PhongDto>>($"{BASE_PATH}/rooms/pending?top={top}");
        }

        /// <summary>
        /// Lấy danh sách báo cáo vi phạm mới (top 5)
        /// </summary>
        public async Task<List<BaoCaoViPhamDto>> GetRecentReportsAsync(int top = 5)
        {
            return await GetAsync<List<BaoCaoViPhamDto>>($"{BASE_PATH}/reports/recent?top={top}");
        }

        /// <summary>
        /// Lấy lịch sử hoạt động gần đây (top 10)
        /// </summary>
        public async Task<List<ActivityLogDto>> GetRecentActivitiesAsync(int top = 10)
        {
            return await GetAsync<List<ActivityLogDto>>($"{BASE_PATH}/activities/recent?top={top}");
        }

        /// <summary>
        /// Lấy dữ liệu doanh thu theo tháng
        /// </summary>
        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 6)
        {
            return await GetAsync<List<MonthlyRevenueDto>>($"{BASE_PATH}/revenue/monthly?months={months}");
        }

        /// <summary>
        /// Lấy số lượng người dùng mới trong tháng
        /// </summary>
        public async Task<int> GetNewUsersThisMonthAsync()
        {
            return await GetAsync<int>($"{BASE_PATH}/users/new-this-month");
        }
    }
}
