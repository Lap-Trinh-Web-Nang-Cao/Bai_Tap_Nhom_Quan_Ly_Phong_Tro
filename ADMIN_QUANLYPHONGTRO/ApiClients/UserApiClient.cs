using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class UserApiClient : BaseApiClient
    {
        public async Task<PagedResult<NguoiDungDto>> GetUsers(int pageIndex, int pageSize, string keyword = "")
        {
            string url = $"nguoidung?pageIndex={pageIndex}&pageSize={pageSize}&keyword={keyword}";
            return await GetAsync<PagedResult<NguoiDungDto>>(url);
        }

        public async Task<ApiResponse<NguoiDungDto>> GetUserById(string id)
        {
            return await GetAsync<ApiResponse<NguoiDungDto>>($"nguoidung/{id}");
        }

        public async Task<ApiResponse<bool>> ToggleLockUser(string id)
        {
            return await PutAsync<ApiResponse<bool>>($"nguoidung/{id}/toggle-lock", null);
        }
    }
}
