using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<NguoiDungDto>> GetUsersAsync(int pageIndex, int pageSize, string keyword = "");
        Task<ApiResponse<NguoiDungDto>> GetUserByIdAsync(string id);
        Task<ApiResponse<bool>> ToggleLockUserAsync(string id);
    }
}
