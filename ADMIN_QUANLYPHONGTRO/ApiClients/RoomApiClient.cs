using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class RoomApiClient : BaseApiClient
    {
        public async Task<PagedResult<PhongDto>> GetPendingRooms(int pageIndex, int pageSize)
        {
            return await GetAsync<PagedResult<PhongDto>>(
                $"rooms/pending?pageIndex={pageIndex}&pageSize={pageSize}"
            );
        }

        public async Task<ApiResponse<bool>> ApproveRoom(string id)
        {
            return await PutAsync<ApiResponse<bool>>($"rooms/{id}/approve", null);
        }

        public async Task<ApiResponse<bool>> RejectRoom(string id, string reason)
        {
            return await PutAsync<ApiResponse<bool>>($"rooms/{id}/reject", new { reason });
        }

        public async Task<ApiResponse<bool>> ToggleLockRoom(string id)
        {
            return await PutAsync<ApiResponse<bool>>($"rooms/{id}/toggle-lock", null);
        }
    }
}
