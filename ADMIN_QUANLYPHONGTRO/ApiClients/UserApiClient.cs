using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using Newtonsoft.Json.Linq;
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

        /// <summary>
        /// Lấy danh sách users với filter
        /// NOTE: Backend returns "TotalCount" / "items" (or "data"), while admin PagedResult expects TotalRecords.
        /// Parse raw JObject to support both shapes and map into admin PagedResult correctly.
        /// </summary>
        public async Task<PagedResult<NguoiDungDto>> GetUsers(int pageIndex, int pageSize, string keyword = "", int? vaiTroId = null, bool? isKhoa = null)
        {
            try
            {
                string url = $"nguoidung?pageIndex={pageIndex}&pageSize={pageSize}";

                if (!string.IsNullOrWhiteSpace(keyword))
                    url += $"&keyword={Uri.EscapeDataString(keyword)}";

                if (vaiTroId.HasValue && vaiTroId.Value > 0)
                    url += $"&vaiTroId={vaiTroId.Value}";

                if (isKhoa.HasValue)
                    url += $"&isKhoa={isKhoa.Value.ToString().ToLower()}";
                Debug.WriteLine($"📡 GetUsers: {url}");

                // Parse response as JObject to be resilient to different JSON field names
                var response = await GetAsync<JObject>(url);

                if (response == null)
                {
                    Debug.WriteLine("⚠️ UserApiClient.GetUsers: response is null");
                    return new PagedResult<NguoiDungDto>
                    {
                        Items = new System.Collections.Generic.List<NguoiDungDto>(),
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalRecords = 0
                    };
                }

                // Accept multiple possible field names for items/data
                JToken itemsToken = response["items"] ?? response["Items"] ?? response["data"] ?? response["Data"] ?? response["Items"] ?? response["items"];
                var items = itemsToken != null
                    ? itemsToken.ToObject<System.Collections.Generic.List<NguoiDungDto>>()
                    : new System.Collections.Generic.List<NguoiDungDto>();

                // Accept multiple possible field names for total count
                int total = response["totalCount"]?.Value<int>() 
                            ?? response["TotalCount"]?.Value<int>() 
                            ?? response["totalRecords"]?.Value<int>() 
                            ?? response["TotalRecords"]?.Value<int>() 
                            ?? 0;

                Debug.WriteLine($"📦 GetUsers parsed: items={items?.Count ?? 0}, total={total}");

                return new PagedResult<NguoiDungDto>
                {
                    Items = items,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = total
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserApiClient.GetUsers Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lấy chi tiết user
        /// </summary>
        public async Task<ApiResponse<NguoiDungDetailDto>> GetUserById(string id)
        {
            try
            {
                Debug.WriteLine($"📡 GetUserById: {id}");
                return await GetAsync<ApiResponse<NguoiDungDetailDto>>($"nguoidung/{id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserApiClient.GetUserById Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Admin tạo user mới
        /// </summary>
        public async Task<ApiResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                Debug.WriteLine($"📡 CreateUser: {request.Email}, VaiTro: {request.VaiTroId}");
                return await PostAsync<ApiResponse<CreateUserResponse>>("nguoidung/admin-create", request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserApiClient.CreateUserAsync Error: {ex.Message}");
                throw;
            }
        }

        public async Task<ApiResponse<bool>> ToggleLockUser(string id)
        {
            return await PutAsync<ApiResponse<bool>>($"nguoidung/{id}/toggle-lock", null);
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        public async Task<ApiResponse<bool>> LockUserAsync(string userId)
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
        public async Task<ApiResponse<bool>> UnlockUserAsync(string userId)
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

        /// <summary>
        /// Lấy thống kê người dùng (Admin)
        /// </summary>
        public async Task<UserStatisticsResponse> GetUserStatisticsAsync()
        {
            try
            {
                Debug.WriteLine($"📡 GetUserStatistics");
                return await GetAsync<UserStatisticsResponse>("nguoidung/statistics");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserApiClient.GetUserStatistics Error: {ex.Message}");
                // Return default if API not implemented
                return new UserStatisticsResponse { };
            }
        }

        /// <summary>
        /// Response cho thống kê người dùng
        /// </summary>
        public class UserStatisticsResponse
        {
            public int TotalUsers { get; set; }
            public int TotalTenants { get; set; }      // Role = 3
            public int TotalLandlords { get; set; }    // Role = 2
            public int TotalAdmins { get; set; }       // Role = 1
            public int LockedUsers { get; set; }
            public int NewUsersThisMonth { get; set; }
            public int VerifiedEmails { get; set; }
        }
    }

    /// <summary>
    /// Request tạo user mới
    /// </summary>
    public class CreateUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DienThoai { get; set; }
        public string HoTen { get; set; }
        public int VaiTroId { get; set; }
        public bool IsEmailXacThuc { get; set; }
    }

    /// <summary>
    /// Response khi tạo user thành công
    /// </summary>
    public class CreateUserResponse
    {
        public Guid NguoiDungId { get; set; }
    }
}
