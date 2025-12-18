using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IHostService
    {
        //Task<PagedResult<ChuTroThongTinPhapLyDto>> GetPendingHostsAsync(int pageIndex, int pageSize);
        Task<ApiResponse<bool>> ApproveHostAsync(string nguoiDungId);
        Task<ApiResponse<bool>> RejectHostAsync(string nguoiDungId, string reason);
    }
}
