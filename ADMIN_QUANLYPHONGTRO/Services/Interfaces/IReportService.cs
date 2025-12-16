using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IReportService
    {
        Task<PagedResult<BaoCaoViPhamDto>> GetReportsAsync(int pageIndex, int pageSize);
        Task<ApiResponse<bool>> ResolveReportAsync(string baoCaoId);
    }
}
