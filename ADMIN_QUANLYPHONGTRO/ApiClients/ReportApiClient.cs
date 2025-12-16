using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class ReportApiClient : BaseApiClient
    {
        public async Task<PagedResult<BaoCaoViPhamDto>> GetReports(int pageIndex, int pageSize)
        {
            return await GetAsync<PagedResult<BaoCaoViPhamDto>>(
                $"reports?pageIndex={pageIndex}&pageSize={pageSize}"
            );
        }

        public async Task<ApiResponse<bool>> ResolveReport(string id)
        {
            return await PutAsync<ApiResponse<bool>>($"reports/{id}/resolve", null);
        }
    }
}
