using System;
using System.Diagnostics;
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

        /// <summary>
        /// Lấy danh sách users với filter
        /// </summary>
        public async Task<PagedResult<NguoiDungDto>> GetUsersAsync(int pageIndex, int pageSize, string keyword = "", int? vaiTroId = null, bool? isKhoa = null)
        {
            try
            {
                return await _apiClient.GetUsers(pageIndex, pageSize, keyword, vaiTroId, isKhoa);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserService.GetUsersAsync Error: {ex.Message}");
                return new PagedResult<NguoiDungDto>
                {
                    Items = new System.Collections.Generic.List<NguoiDungDto>(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = 0
                };
            }
        }

        /// <summary>
        /// Lấy chi tiết user
        /// </summary>
        public async Task<ApiResponse<NguoiDungDetailDto>> GetUserByIdAsync(string id)
        {
            try
            {
                return await _apiClient.GetUserById(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserService.GetUserByIdAsync Error: {ex.Message}");
                return new ApiResponse<NguoiDungDetailDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Tạo user mới
        /// </summary>
        public async Task<ApiResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                return await _apiClient.CreateUserAsync(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserService.CreateUserAsync Error: {ex.Message}");
                return new ApiResponse<CreateUserResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        public async Task<ApiResponse<bool>> LockUserAsync(string id)
        {
            try
            {
                return await _apiClient.LockUserAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserService.LockUserAsync Error: {ex.Message}");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        public async Task<ApiResponse<bool>> UnlockUserAsync(string id)
        {
            try
            {
                return await _apiClient.UnlockUserAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UserService.UnlockUserAsync Error: {ex.Message}");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
