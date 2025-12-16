using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly RoomApiClient _apiClient;

        public RoomService()
        {
            _apiClient = new RoomApiClient();
        }

        public Task<PagedResult<PhongDto>> GetPendingRoomsAsync(int pageIndex, int pageSize)
        {
            return _apiClient.GetPendingRooms(pageIndex, pageSize);
        }

        public Task<ApiResponse<bool>> ApproveRoomAsync(string phongId)
        {
            return _apiClient.ApproveRoom(phongId);
        }

        public Task<ApiResponse<bool>> RejectRoomAsync(string phongId, string reason)
        {
            return _apiClient.RejectRoom(phongId, reason);
        }

        public Task<ApiResponse<bool>> ToggleLockRoomAsync(string phongId)
        {
            return _apiClient.ToggleLockRoom(phongId);
        }
    }
}
