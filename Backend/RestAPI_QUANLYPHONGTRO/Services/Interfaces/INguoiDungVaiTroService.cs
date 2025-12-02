using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface INguoiDungVaiTroService
    {
        // Gán vai trò cho user
        Task<bool> AddRoleToUserAsync(AssignRoleRequest request);

        // Gỡ vai trò khỏi user
        Task<bool> RemoveRoleFromUserAsync(Guid userId, int roleId);

        // Lấy danh sách các vai trò của 1 user
        Task<IEnumerable<NguoiDungVaiTro>> GetRolesByUserIdAsync(Guid userId);
    }
}
