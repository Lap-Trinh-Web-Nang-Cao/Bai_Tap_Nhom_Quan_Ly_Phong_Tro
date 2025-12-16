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

        public Task<PagedResult<YeuCauHoTroDto>> GetSupportsAsync(int pageIndex, int pageSize)
        {
            return _apiClient.GetSupports(pageIndex, pageSize);
        }

        public Task<ApiResponse<bool>> MarkSolvedAsync(string hoTroId)
        {
            return _apiClient.MarkSolved(hoTroId);
        }
    }
}
