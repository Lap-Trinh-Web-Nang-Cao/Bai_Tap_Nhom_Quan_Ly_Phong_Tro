using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ISupportService
    {
        Task<PagedResult<YeuCauHoTroDto>> GetSupportsAsync(int pageIndex, int pageSize);
        Task<ApiResponse<bool>> MarkSolvedAsync(string hoTroId);
    }
}
