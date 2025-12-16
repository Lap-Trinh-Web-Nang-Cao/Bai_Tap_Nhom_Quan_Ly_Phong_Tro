using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserApiClient _apiClient;

        public UserService()
        {
            _apiClient = new UserApiClient();
        }

        public Task<PagedResult<NguoiDungDto>> GetUsersAsync(int pageIndex, int pageSize, string keyword = "")
        {
            return _apiClient.GetUsers(pageIndex, pageSize, keyword);
        }

        public Task<ApiResponse<NguoiDungDto>> GetUserByIdAsync(string id)
        {
            return _apiClient.GetUserById(id);
        }

        public Task<ApiResponse<bool>> ToggleLockUserAsync(string id)
        {
            return _apiClient.ToggleLockUser(id);
        }
    }
}
