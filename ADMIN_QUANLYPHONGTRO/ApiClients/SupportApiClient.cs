using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class SupportApiClient : BaseApiClient
    {
        public async Task<PagedResult<YeuCauHoTroDto>> GetSupports(int pageIndex, int pageSize)
        {
            return await GetAsync<PagedResult<YeuCauHoTroDto>>(
                $"support?pageIndex={pageIndex}&pageSize={pageSize}"
            );
        }

        public async Task<ApiResponse<bool>> MarkSolved(string id)
        {
            return await PutAsync<ApiResponse<bool>>($"support/{id}/solve", null);
        }
    }
}
