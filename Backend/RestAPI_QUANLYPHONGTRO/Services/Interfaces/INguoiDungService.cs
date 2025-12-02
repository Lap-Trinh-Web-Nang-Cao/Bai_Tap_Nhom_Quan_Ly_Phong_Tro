using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface INguoiDungService
    {
        // Chức năng Auth
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<string> LoginAsync(LoginRequest request);

        // Chức năng quản lý User (Ví dụ cho Admin hoặc xem profile)
        Task<NguoiDung?> GetByIdAsync(Guid id);

        Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    }
}
