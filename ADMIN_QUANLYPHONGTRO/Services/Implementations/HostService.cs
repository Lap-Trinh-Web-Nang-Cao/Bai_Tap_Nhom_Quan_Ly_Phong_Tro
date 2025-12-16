using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    public class HostService : IHostService
    {
        private readonly HostApiClient _apiClient;

        public HostService()
        {
            _apiClient = new HostApiClient();
        }

        public Task<PagedResult<ChuTroThongTinPhapLyDto>> GetPendingHostsAsync(int pageIndex, int pageSize)
        {
            return _apiClient.GetPendingHosts(pageIndex, pageSize);
        }

        public Task<ApiResponse<bool>> ApproveHostAsync(string nguoiDungId)
        {
            return _apiClient.ApproveHost(nguoiDungId);
        }

        public Task<ApiResponse<bool>> RejectHostAsync(string nguoiDungId, string reason)
        {
            return _apiClient.RejectHost(nguoiDungId, reason);
        }
    }
}
