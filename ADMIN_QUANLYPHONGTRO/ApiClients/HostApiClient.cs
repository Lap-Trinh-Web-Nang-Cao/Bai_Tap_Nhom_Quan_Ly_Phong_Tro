using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class HostApiClient : BaseApiClient
    {
        public async Task<PagedResult<ChuTroThongTinPhapLyDto>> GetPendingHosts(int pageIndex, int pageSize)
        {
            return await GetAsync<PagedResult<ChuTroThongTinPhapLyDto>>(
                $"hosts/pending?pageIndex={pageIndex}&pageSize={pageSize}"
            );
        }

        public async Task<ApiResponse<bool>> ApproveHost(string id)
        {
            return await PutAsync<ApiResponse<bool>>($"hosts/{id}/approve", null);
        }

        public async Task<ApiResponse<bool>> RejectHost(string id, string reason)
        {
            return await PutAsync<ApiResponse<bool>>($"hosts/{id}/reject", new { reason });
        }
    }
}
