using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<PagedResult<BienLaiDto>> GetTransactionsAsync(int pageIndex, int pageSize);
        Task<ApiResponse<bool>> ConfirmPaymentAsync(string bienLaiId);
    }
}
