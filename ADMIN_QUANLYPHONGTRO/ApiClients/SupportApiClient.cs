using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class SupportApiClient : BaseApiClient
    {
        /// <summary>
        /// Lấy danh sách tất cả yêu cầu hỗ trợ (cho Admin)
        /// </summary>
        public async Task<List<YeuCauHoTroDto>> GetAllSupportsAsync()
        {
            try
            {
                return await GetAsync<List<YeuCauHoTroDto>>("yeucauhotro/all");
            }
            catch
            {
                return new List<YeuCauHoTroDto>();
            }
        }
        
        /// <summary>
        /// Lấy danh sách yêu cầu hỗ trợ có phân trang
        /// </summary>
        public async Task<PagedResult<YeuCauHoTroDto>> GetSupportsPagedAsync(int pageIndex, int pageSize, string status = null)
        {
            try
            {
                var url = string.Format("yeucauhotro?pageIndex={0}&pageSize={1}", pageIndex, pageSize);
                if (!string.IsNullOrEmpty(status))
                {
                    url += "&status=" + status;
                }
                return await GetAsync<PagedResult<YeuCauHoTroDto>>(url);
            }
            catch
            {
                return new PagedResult<YeuCauHoTroDto>
                {
                    Items = new List<YeuCauHoTroDto>(),
                    TotalRecords = 0,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
            }
        }
        
        /// <summary>
        /// Lấy chi tiết yêu cầu hỗ trợ
        /// </summary>
        public async Task<YeuCauHoTroDto> GetByIdAsync(string id)
        {
            try
            {
                return await GetAsync<YeuCauHoTroDto>("yeucauhotro/" + id);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Cập nhật trạng thái yêu cầu hỗ trợ (Admin)
        /// </summary>
        public async Task<ApiResponse<bool>> UpdateStatusAsync(string id, string status)
        {
            try
            {
                return await PutAsync<ApiResponse<bool>>(
                    string.Format("yeucauhotro/admin-status/{0}?status={1}", id, status), 
                    null
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        
        /// <summary>
        /// Lấy danh sách loại hỗ trợ
        /// </summary>
        public async Task<List<LoaiHoTroDto>> GetLoaiHoTroAsync()
        {
            try
            {
                return await GetAsync<List<LoaiHoTroDto>>("loaihotro");
            }
            catch
            {
                return new List<LoaiHoTroDto>();
            }
        }
        
        /// <summary>
        /// Lấy thống kê yêu cầu hỗ trợ
        /// </summary>
        public async Task<SupportStatistics> GetStatisticsAsync()
        {
            try
            {
                return await GetAsync<SupportStatistics>("yeucauhotro/statistics");
            }
            catch
            {
                return new SupportStatistics();
            }
        }
    }
    
    /// <summary>
    /// Thống kê yêu cầu hỗ trợ
    /// </summary>
    public class SupportStatistics
    {
        public int TotalRequests { get; set; }
        public int NewRequests { get; set; }
        public int ProcessingRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int RejectedRequests { get; set; }
    }
}
