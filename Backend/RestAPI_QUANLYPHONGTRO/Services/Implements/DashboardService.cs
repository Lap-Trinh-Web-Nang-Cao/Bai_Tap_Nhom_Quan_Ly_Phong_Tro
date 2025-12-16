using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    /// <summary>
    /// Service xử lý logic Dashboard - Cung cấp dữ liệu thống kê
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy thống kê tổng quan
        /// </summary>
        public async Task<DashboardStatsResponse> GetDashboardStatsAsync()
        {
            var now = DateTimeOffset.Now;
            var firstDayOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
            var firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);

            // VaiTroId: 2 = Chủ trọ, 3 = Người thuê
            var stats = new DashboardStatsResponse
            {
                // Thống kê phòng
                TotalRooms = await _context.Phongs.CountAsync(p => !p.IsDeleted),
                PendingRooms = await _context.Phongs.CountAsync(p => !p.IsDeleted && !p.IsDuyet),
                ApprovedRooms = await _context.Phongs.CountAsync(p => !p.IsDeleted && p.IsDuyet && !p.IsBiKhoa),
                LockedRooms = await _context.Phongs.CountAsync(p => !p.IsDeleted && p.IsBiKhoa),

                // Thống kê chủ trọ (VaiTroId = 2)
                TotalHosts = await _context.NguoiDungs.CountAsync(u => u.VaiTroId == 2 && !u.IsKhoa),
                VerifiedHosts = await _context.ChuTroThongTinPhapLys
                    .CountAsync(c => c.TrangThaiXacThuc == "DaDuyet"),
                PendingHosts = await _context.ChuTroThongTinPhapLys
                    .CountAsync(c => c.TrangThaiXacThuc == "ChoDuyet"),

                // Thống kê người thuê (VaiTroId = 3)
                TotalTenants = await _context.NguoiDungs.CountAsync(u => u.VaiTroId == 3 && !u.IsKhoa),

                // Người dùng mới trong tháng
                NewUsersThisMonth = await _context.NguoiDungs
                    .CountAsync(u => u.CreatedAt >= firstDayOfMonth),

                // Báo cáo chờ xử lý
                PendingReports = await _context.BaoCaoViPhams
                    .CountAsync(b => b.TrangThai == "CHO_XU_LY" || b.TrangThai == "DANG_XU_LY"),

                // Doanh thu tháng này (từ BienLai - những biên lai đã xác nhận)
                MonthlyRevenue = await _context.BienLais
                    .Where(b => b.ThoiGianTai >= firstDayOfMonth && b.DaXacNhan)
                    .SumAsync(b => (decimal?)(b.SoTien ?? 0)) ?? 0,
            };

            // Tính tăng trưởng doanh thu
            var lastMonthRevenue = await _context.BienLais
                .Where(b => b.ThoiGianTai >= firstDayOfLastMonth && 
                           b.ThoiGianTai < firstDayOfMonth && 
                           b.DaXacNhan)
                .SumAsync(b => (decimal?)(b.SoTien ?? 0)) ?? 0;

            stats.RevenueGrowth = lastMonthRevenue > 0
                ? Math.Round(((stats.MonthlyRevenue - lastMonthRevenue) / lastMonthRevenue) * 100, 2)
                : 0;

            return stats;
        }

        /// <summary>
        /// Thống kê phòng theo tháng (12 tháng gần nhất)
        /// </summary>
        public async Task<List<MonthlyRoomStatsResponse>> GetMonthlyRoomStatsAsync(int months = 12)
        {
            var now = DateTimeOffset.Now;
            var startDate = now.AddMonths(-months);

            var rooms = await _context.Phongs
                .Where(p => p.CreatedAt >= startDate && !p.IsDeleted)
                .ToListAsync();

            var monthlyStats = Enumerable.Range(0, months)
                .Select(i =>
                {
                    var month = now.AddMonths(-months + i + 1);
                    var monthStart = new DateTimeOffset(month.Year, month.Month, 1, 0, 0, 0, month.Offset);
                    var monthEnd = monthStart.AddMonths(1);

                    var roomsInMonth = rooms.Where(r => r.CreatedAt >= monthStart && r.CreatedAt < monthEnd).ToList();

                    return new MonthlyRoomStatsResponse
                    {
                        Month = monthStart.ToString("yyyy-MM"),
                        NewRooms = roomsInMonth.Count,
                        ApprovedRooms = roomsInMonth.Count(r => r.IsDuyet)
                    };
                })
                .ToList();

            return monthlyStats;
        }

        /// <summary>
        /// Phân bố trạng thái phòng
        /// </summary>
        public async Task<RoomStatusDistributionResponse> GetRoomStatusDistributionAsync()
        {
            return new RoomStatusDistributionResponse
            {
                Approved = await _context.Phongs.CountAsync(p => !p.IsDeleted && p.IsDuyet && !p.IsBiKhoa),
                Pending = await _context.Phongs.CountAsync(p => !p.IsDeleted && !p.IsDuyet),
                Rejected = 0, // TODO: Implement rejected status nếu cần
                Locked = await _context.Phongs.CountAsync(p => !p.IsDeleted && p.IsBiKhoa)
            };
        }

        /// <summary>
        /// Danh sách phòng chờ duyệt (mới nhất)
        /// </summary>
        public async Task<List<PendingRoomResponse>> GetPendingRoomsAsync(int top = 5)
        {
            var pendingRooms = await _context.Phongs
                .Where(p => !p.IsDeleted && !p.IsDuyet)
                .OrderByDescending(p => p.CreatedAt)
                .Take(top)
                .Select(p => new PendingRoomResponse
                {
                    PhongId = p.PhongId,
                    TieuDe = p.TieuDe ?? "Phòng trọ",
                    GiaTien = p.GiaTien,
                    ChuTroName = "Chủ trọ", // TODO: Join với NhaTro -> NguoiDung để lấy tên
                    CreatedAt = p.CreatedAt ?? DateTimeOffset.Now
                })
                .ToListAsync();

            return pendingRooms;
        }

        /// <summary>
        /// Danh sách báo cáo vi phạm mới
        /// </summary>
        public async Task<List<RecentReportResponse>> GetRecentReportsAsync(int top = 5)
        {
            var reports = await _context.BaoCaoViPhams
                .Where(b => b.TrangThai == "CHO_XU_LY" || b.TrangThai == "DANG_XU_LY")
                .OrderByDescending(b => b.ThoiGianBaoCao)
                .Take(top)
                .Select(b => new RecentReportResponse
                {
                    BaoCaoId = b.BaoCaoId,
                    TieuDe = b.TieuDe ?? "Báo cáo vi phạm",
                    LoaiThucThe = b.LoaiThucThe ?? "KHAC",
                    PhongId = b.LoaiThucThe == "PHONG" ? b.ThucTheId : null,
                    NguoiDungId = b.LoaiThucThe == "NGUOIDUNG" ? b.ThucTheId : null,
                    TrangThai = b.TrangThai ?? "CHO_XU_LY",
                    CreatedAt = b.ThoiGianBaoCao ?? DateTimeOffset.Now
                })
                .ToListAsync();

            return reports;
        }

        /// <summary>
        /// Lịch sử hoạt động admin gần đây
        /// </summary>
        public async Task<List<ActivityLogResponse>> GetRecentActivitiesAsync(int top = 10)
        {
            var activities = await _context.HanhDongAdmins
                .Where(h => h.ThoiGian != null)
                .OrderByDescending(h => h.ThoiGian)
                .Take(top)
                .ToListAsync();

            return activities.Select(h => new ActivityLogResponse
            {
                ActivityId = Guid.NewGuid(), // Temporary ID
                Action = h.HanhDong ?? "UNKNOWN",
                Description = h.ChiTiet ?? "",
                PerformedBy = "Admin", // TODO: Join với NguoiDung để lấy tên
                PerformedById = h.AdminId,
                Timestamp = h.ThoiGian.Value.DateTime,
                Type = GetActivityType(h.HanhDong),
                TargetType = h.MucTieuBang ?? "",
                TargetId = h.BanGhiId ?? ""
            }).ToList();
        }

        /// <summary>
        /// Số người dùng mới trong tháng
        /// </summary>
        public async Task<int> GetNewUsersThisMonthAsync()
        {
            var now = DateTimeOffset.Now;
            var firstDayOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);

            return await _context.NguoiDungs
                .CountAsync(u => u.CreatedAt >= firstDayOfMonth);
        }

        #region Helper Methods

        private string GetActivityType(string action)
        {
            return action switch
            {
                "DUYET_PHONG" => "success",
                "DUYET_CHU_TRO" => "success",
                "KHOA_TAI_KHOAN" => "danger",
                "TU_CHOI_PHONG" => "warning",
                "XU_LY_BAO_CAO" => "info",
                _ => "info"
            };
        }

        #endregion
    }
}
