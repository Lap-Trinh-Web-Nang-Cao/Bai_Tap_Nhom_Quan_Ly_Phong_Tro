using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    /// <summary>
    /// Service xử lý logic Dashboard
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly DashboardApiClient _dashboardApi;

        public DashboardService()
        {
            try
            {
                _dashboardApi = new DashboardApiClient();
                System.Diagnostics.Debug.WriteLine($"✅ DashboardService: DashboardApiClient initialized");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DashboardService Constructor Error: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu Dashboard từ API
        /// </summary>
        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var viewModel = new DashboardViewModel();

            try
            {
                // Gọi API song song để tăng performance
                var statsTask = _dashboardApi.GetDashboardStatsAsync();
                var monthlyRoomsTask = _dashboardApi.GetMonthlyRoomStatsAsync(12);
                var statusDistributionTask = _dashboardApi.GetRoomStatusDistributionAsync();
                var pendingRoomsTask = _dashboardApi.GetPendingRoomsAsync(5);
                var recentReportsTask = _dashboardApi.GetRecentReportsAsync(5);
                var recentActivitiesTask = _dashboardApi.GetRecentActivitiesAsync(10);

                // Đợi tất cả API calls hoàn thành
                await Task.WhenAll(
                    statsTask,
                    monthlyRoomsTask,
                    statusDistributionTask,
                    pendingRoomsTask,
                    recentReportsTask,
                    recentActivitiesTask
                );

                // Map statistics
                var stats = await statsTask;
                MapStatistics(viewModel, stats);

                // Map chart data
                var monthlyRooms = await monthlyRoomsTask;
                var statusDistribution = await statusDistributionTask;
                MapChartData(viewModel, monthlyRooms, statusDistribution);

                // Map activities
                var pendingRooms = await pendingRoomsTask;
                var recentReports = await recentReportsTask;
                var recentActivities = await recentActivitiesTask;
                MapActivities(viewModel, pendingRooms, recentReports, recentActivities);
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Dashboard Service Error: {ex.Message}");
                
                // Load dữ liệu mẫu nếu API fail
                LoadFallbackData(viewModel);
            }

            return viewModel;
        }

        /// <summary>
        /// Refresh cache (placeholder cho future caching implementation)
        /// </summary>
        public async Task RefreshDashboardCacheAsync()
        {
            await Task.CompletedTask;
        }

        #region Private Mapping Methods

        private void MapStatistics(DashboardViewModel viewModel, DashboardStatsDto stats)
        {
            viewModel.TotalRooms = stats.TotalRooms;
            viewModel.PendingRooms = stats.PendingRooms;
            viewModel.VerifiedHosts = stats.VerifiedHosts;
            viewModel.TotalTenants = stats.TotalTenants;
            viewModel.PendingReports = stats.PendingReports;
            viewModel.MonthlyRevenue = stats.MonthlyRevenue;
        }

        private void MapChartData(DashboardViewModel viewModel, 
            List<MonthlyRoomStatsDto> monthlyRooms, 
            RoomStatusDistributionDto statusDist)
        {
            // Map monthly rooms data
            viewModel.RoomsChartData = monthlyRooms.Select(m => new MonthlyRoomData
            {
                Month = ParseMonthName(m.Month),
                Count = m.NewRooms
            }).ToList();

            // Map status distribution
            viewModel.RoomStatusChartData = new RoomStatusData
            {
                Approved = statusDist.Approved,
                Pending = statusDist.Pending,
                Locked = statusDist.Locked
            };
        }

        private void MapActivities(DashboardViewModel viewModel, 
            List<PhongDto> pendingRooms, 
            List<BaoCaoViPhamDto> recentReports, 
            List<ActivityLogDto> activities)
        {
            // Map pending rooms
            viewModel.PendingRoomsList = pendingRooms.Select(r => new PendingRoomItem
            {
                RoomId = r.PhongId,
                Title = r.TieuDe ?? "Phòng trọ",
                ImageUrl = "/Content/img/room-default.jpg", // Default image
                Price = r.GiaTien,
                OwnerName = "Chủ trọ", // TODO: Get from relationship
                SubmittedDate = r.CreatedAt.DateTime
            }).ToList();

            // Map recent reports
            viewModel.PendingReportsList = recentReports.Select(r => new PendingReportItem
            {
                ReportId = r.BaoCaoId,
                ViolationType = r.TieuDe ?? "Vi phạm",
                TargetType = r.LoaiThucThe ?? "Phòng",
                TargetName = r.PhongId.HasValue ? $"#{r.PhongId}" : (r.NguoiDungId.HasValue ? $"User #{r.NguoiDungId}" : "N/A"),
                ReporterName = "User", // TODO: Get from relationship
                ReportedDate = r.CreatedAt.DateTime,
                Severity = r.TrangThai == "URGENT" ? "Cao" : (r.TrangThai == "PENDING" ? "Trung bình" : "Thấp")
            }).ToList();

            // Map activity logs
            viewModel.RecentActivities = activities.Select(a => new ActivityLog
            {
                Id = a.ActivityId.GetHashCode(),
                Action = TranslateAction(a.Action),
                Description = a.Description ?? "",
                PerformedBy = a.PerformedBy ?? "Admin",
                Timestamp = a.Timestamp,
                Type = a.Type ?? "info"
            }).ToList();
        }

        private void LoadFallbackData(DashboardViewModel viewModel)
        {
            // Dữ liệu mẫu khi API fail
            viewModel.TotalRooms = 0;
            viewModel.VerifiedHosts = 0;
            viewModel.TotalTenants = 0;
            viewModel.PendingReports = 0;
            viewModel.MonthlyRevenue = 0;
            viewModel.PendingRooms = 0;

            // Sample chart data
            viewModel.RoomsChartData = Enumerable.Range(1, 12).Select(i => new MonthlyRoomData
            {
                Month = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }[i - 1],
                Count = 0
            }).ToList();

            viewModel.RoomStatusChartData = new RoomStatusData
            {
                Approved = 0,
                Pending = 0,
                Locked = 0
            };
        }

        #endregion

        #region Helper Methods

        private string ParseMonthName(string monthStr)
        {
            // Input: "2025-01" -> Output: "Jan"
            if (string.IsNullOrEmpty(monthStr) || monthStr.Length < 7)
                return monthStr;

            var month = int.Parse(monthStr.Substring(5, 2));
            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            return month > 0 && month <= 12 ? monthNames[month - 1] : monthStr;
        }

        private string TranslateAction(string action)
        {
            // Dịch action code sang tiếng Việt
            switch (action)
            {
                case "DUYET_PHONG":
                    return "Duyệt phòng";
                case "KHOA_TAI_KHOAN":
                    return "Khóa tài khoản";
                case "DUYET_CHU_TRO":
                    return "Duyệt chủ trọ";
                case "TU_CHOI_PHONG":
                    return "Từ chối phòng";
                case "XU_LY_BAO_CAO":
                    return "Xử lý báo cáo";
                default:
                    return action;
            }
        }

        #endregion
    }
}
