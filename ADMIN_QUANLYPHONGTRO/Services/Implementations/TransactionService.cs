using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionApiClient _apiClient;

        public TransactionService()
        {
            _apiClient = new TransactionApiClient();
        }

        public Task<PagedResult<BienLaiDto>> GetTransactionsAsync(int pageIndex, int pageSize)
        {
            return _apiClient.GetTransactions(pageIndex, pageSize);
        }

        public Task<ApiResponse<bool>> ConfirmPaymentAsync(string bienLaiId)
        {
            return _apiClient.ConfirmPayment(bienLaiId);
        }
    }
}
