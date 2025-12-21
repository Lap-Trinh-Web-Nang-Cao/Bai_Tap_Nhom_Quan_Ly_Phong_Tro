using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Lấy danh sách users với filter
        /// </summary>
        Task<PagedResult<NguoiDungDto>> GetUsersAsync(int pageIndex, int pageSize, string keyword = "", int? vaiTroId = null, bool? isKhoa = null);
        
        /// <summary>
        /// Lấy chi tiết user
        /// </summary>
        Task<ApiResponse<NguoiDungDetailDto>> GetUserByIdAsync(string id);
        
        /// <summary>
        /// Tạo user mới
        /// </summary>
        Task<ApiResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
        
        /// <summary>
        /// Khóa tài khoản
        /// </summary>
        Task<ApiResponse<bool>> LockUserAsync(string id);
        
        /// <summary>
        /// Mở khóa tài khoản
        /// </summary>
        Task<ApiResponse<bool>> UnlockUserAsync(string id);
    }
}
