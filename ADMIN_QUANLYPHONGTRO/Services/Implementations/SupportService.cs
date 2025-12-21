using System.Collections.Generic;
using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    public class SupportService : ISupportService
    {
        private readonly SupportApiClient _apiClient;

        public SupportService()
        {
            _apiClient = new SupportApiClient();
        }

        public Task<List<YeuCauHoTroDto>> GetAllSupportsAsync()
        {
            return _apiClient.GetAllSupportsAsync();
        }

        public Task<PagedResult<YeuCauHoTroDto>> GetSupportsPagedAsync(int pageIndex, int pageSize, string status = null)
        {
            return _apiClient.GetSupportsPagedAsync(pageIndex, pageSize, status);
        }

        public Task<YeuCauHoTroDto> GetByIdAsync(string hoTroId)
        {
            return _apiClient.GetByIdAsync(hoTroId);
        }

        public Task<ApiResponse<bool>> UpdateStatusAsync(string hoTroId, string status)
        {
            return _apiClient.UpdateStatusAsync(hoTroId, status);
        }

        public Task<List<LoaiHoTroDto>> GetLoaiHoTroAsync()
        {
            return _apiClient.GetLoaiHoTroAsync();
        }

        public Task<ADMIN_QUANLYPHONGTRO.ApiClients.SupportStatistics> GetStatisticsAsync()
        {
            return _apiClient.GetStatisticsAsync();
        }
    }
}
