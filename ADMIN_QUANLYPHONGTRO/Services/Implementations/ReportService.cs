using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    /// <summary>
    /// Service xử lý nghiệp vụ báo cáo vi phạm
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly ReportApiClient _apiClient;

        public ReportService()
        {
            _apiClient = new ReportApiClient();
        }

        /// <summary>
        /// Lấy tất cả báo cáo vi phạm
        /// </summary>
        public async Task<List<BaoCaoViPhamDto>> GetAllReportsAsync()
        {
            try
            {
                return await _apiClient.GetAllReports();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportService.GetAllReportsAsync Error: {ex.Message}");
                return new List<BaoCaoViPhamDto>();
            }
        }

        /// <summary>
        /// Lấy danh sách báo cáo với phân trang
        /// </summary>
        public async Task<PagedResult<BaoCaoViPhamDto>> GetReportsAsync(int pageIndex, int pageSize, string keyword = "", string status = "")
        {
            try
            {
                return await _apiClient.GetReports(pageIndex, pageSize, keyword, status);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportService.GetReportsAsync Error: {ex.Message}");
                return new PagedResult<BaoCaoViPhamDto>
                {
                    Items = new List<BaoCaoViPhamDto>(),
                    TotalRecords = 0,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
            }
        }

        /// <summary>
        /// Lấy chi tiết báo cáo theo ID
        /// </summary>
        public async Task<BaoCaoViPhamDto> GetReportByIdAsync(string baoCaoId)
        {
            try
            {
                return await _apiClient.GetReportById(baoCaoId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportService.GetReportByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Xử lý báo cáo (đánh dấu đã xử lý)
        /// </summary>
        public async Task<ApiResponse<bool>> ResolveReportAsync(string baoCaoId, string ketQua = "Đã xử lý vi phạm")
        {
            try
            {
                return await _apiClient.ResolveReport(baoCaoId, ketQua);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportService.ResolveReportAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Từ chối báo cáo
        /// </summary>
        public async Task<ApiResponse<bool>> RejectReportAsync(string baoCaoId, string lyDo)
        {
            try
            {
                return await _apiClient.RejectReport(baoCaoId, lyDo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportService.RejectReportAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Xóa báo cáo
        /// </summary>
        public async Task<bool> DeleteReportAsync(string baoCaoId)
        {
            try
            {
                return await _apiClient.DeleteReport(baoCaoId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportService.DeleteReportAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách loại vi phạm
        /// </summary>
        public async Task<List<ViPhamDto>> GetViolationTypesAsync()
        {
            try
            {
                return await _apiClient.GetViolationTypes();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportService.GetViolationTypesAsync Error: {ex.Message}");
                return new List<ViPhamDto>();
            }
        }

        /// <summary>
        /// Lấy số lượng báo cáo theo trạng thái
        /// </summary>
        public async Task<ReportStatistics> GetStatisticsAsync()
        {
            try
            {
                var allReports = await _apiClient.GetAllReports();
                
                return new ReportStatistics
                {
                    TotalReports = allReports.Count,
                    PendingReports = allReports.Count(x => x.TrangThai == "CHO_XU_LY"),
                    ProcessingReports = allReports.Count(x => x.TrangThai == "DANG_XU_LY"),
                    ResolvedReports = allReports.Count(x => x.TrangThai == "DA_XU_LY"),
                    RejectedReports = allReports.Count(x => x.TrangThai == "TU_CHOI")
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ReportService.GetStatisticsAsync Error: {ex.Message}");
                return new ReportStatistics();
            }
        }
    }
}
