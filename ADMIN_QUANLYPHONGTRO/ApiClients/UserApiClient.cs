using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class UserApiClient : BaseApiClient
    {
        public class LoginResponse
        {
            public string Token { get; set; }
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        public async Task<LoginResponse> LoginAsync(dynamic request)
        {
            try
            {
                var response = await PostAsync<LoginResponse>("nguoidung/login", request);
                Debug.WriteLine($"✅ Login response: Token = {response?.Token?.Substring(0, 20)}...");
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserApiClient.LoginAsync Error: {ex.Message}");
                throw;
            }
        }

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

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        public async Task<ApiResponse<bool>> LockUserAsync(Guid userId)
        {
            try
            {
                var url = $"nguoidung/{userId}/lock";
                var result = await PutAsync<ApiResponse<bool>>(url, new { });
                Debug.WriteLine($"✅ LockUser success");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserApiClient.LockUserAsync Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        public async Task<ApiResponse<bool>> UnlockUserAsync(Guid userId)
        {
            try
            {
                var url = $"nguoidung/{userId}/unlock";
                var result = await PutAsync<ApiResponse<bool>>(url, new { });
                Debug.WriteLine($"✅ UnlockUser success");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserApiClient.UnlockUserAsync Error: {ex.Message}");
                throw;
            }
        }
    }
}
