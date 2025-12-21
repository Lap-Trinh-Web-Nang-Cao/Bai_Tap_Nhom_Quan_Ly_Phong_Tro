using System.Collections.Generic;
using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ISupportService
    {
        /// <summary>
        /// Lấy tất cả yêu cầu hỗ trợ
        /// </summary>
        Task<List<YeuCauHoTroDto>> GetAllSupportsAsync();
        
        /// <summary>
        /// Lấy danh sách yêu cầu có phân trang
        /// </summary>
        Task<PagedResult<YeuCauHoTroDto>> GetSupportsPagedAsync(int pageIndex, int pageSize, string status = null);
        
        /// <summary>
        /// Lấy chi tiết yêu cầu
        /// </summary>
        Task<YeuCauHoTroDto> GetByIdAsync(string hoTroId);
        
        /// <summary>
        /// Cập nhật trạng thái yêu cầu
        /// </summary>
        Task<ApiResponse<bool>> UpdateStatusAsync(string hoTroId, string status);
        
        /// <summary>
        /// Lấy danh sách loại hỗ trợ
        /// </summary>
        Task<List<LoaiHoTroDto>> GetLoaiHoTroAsync();
        
        /// <summary>
        /// Lấy thống kê
        /// </summary>
        Task<SupportStatistics> GetStatisticsAsync();
    }
}
