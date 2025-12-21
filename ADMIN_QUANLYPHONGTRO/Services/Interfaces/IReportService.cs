using System.Collections.Generic;
using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface cho service quản lý báo cáo vi phạm
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Lấy tất cả báo cáo vi phạm
        /// </summary>
        Task<List<BaoCaoViPhamDto>> GetAllReportsAsync();

        /// <summary>
        /// Lấy danh sách báo cáo với phân trang
        /// </summary>
        Task<PagedResult<BaoCaoViPhamDto>> GetReportsAsync(int pageIndex, int pageSize, string keyword = "", string status = "");

        /// <summary>
        /// Lấy chi tiết báo cáo theo ID
        /// </summary>
        Task<BaoCaoViPhamDto> GetReportByIdAsync(string baoCaoId);

        /// <summary>
        /// Xử lý báo cáo (đánh dấu đã xử lý)
        /// </summary>
        Task<ApiResponse<bool>> ResolveReportAsync(string baoCaoId, string ketQua = "Đã xử lý vi phạm");

        /// <summary>
        /// Từ chối báo cáo
        /// </summary>
        Task<ApiResponse<bool>> RejectReportAsync(string baoCaoId, string lyDo);

        /// <summary>
        /// Xóa báo cáo
        /// </summary>
        Task<bool> DeleteReportAsync(string baoCaoId);

        /// <summary>
        /// Cập nhật trạng thái báo cáo
        /// </summary>
        Task<ApiResponse<bool>> UpdateStatusAsync(string baoCaoId, string trangThai, string chiTiet = "");
        /// <summary>
        /// Lấy danh sách loại vi phạm
        /// </summary>
        Task<List<ViPhamDto>> GetViolationTypesAsync();

        /// <summary>
        /// Lấy số lượng báo cáo theo trạng thái
        /// </summary>
        Task<ReportStatistics> GetStatisticsAsync();
    }

    /// <summary>
    /// Model thống kê báo cáo
    /// </summary>
    public class ReportStatistics
    {
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int ProcessingReports { get; set; }
        public int ResolvedReports { get; set; }
        public int RejectedReports { get; set; }
    }
}
