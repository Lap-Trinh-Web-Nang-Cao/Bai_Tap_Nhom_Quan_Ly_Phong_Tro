using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IRoomService
    {
        Task<PagedResult<PhongDto>> GetPendingRoomsAsync(int pageIndex, int pageSize);
        Task<ApiResponse<bool>> ApproveRoomAsync(string phongId);
        Task<ApiResponse<bool>> RejectRoomAsync(string phongId, string reason);
        Task<ApiResponse<bool>> ToggleLockRoomAsync(string phongId);
    }
}
