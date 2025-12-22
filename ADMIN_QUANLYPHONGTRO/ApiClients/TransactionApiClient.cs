using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class TransactionApiClient : BaseApiClient
    {
        public async Task<PagedResult<BienLaiDto>> GetTransactions(int pageIndex, int pageSize)
        {
            return await GetAsync<PagedResult<BienLaiDto>>(
                $"transactions?pageIndex={pageIndex}&pageSize={pageSize}"
            );
        }

        public async Task<ApiResponse<bool>> ConfirmPayment(string id)
        {
            return await PutAsync<ApiResponse<bool>>($"transactions/{id}/confirm", null);
        }
    }
}
