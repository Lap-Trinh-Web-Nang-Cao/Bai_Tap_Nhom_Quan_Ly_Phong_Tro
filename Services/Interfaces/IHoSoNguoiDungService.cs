using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IHoSoNguoiDungService
    {
        // Lấy hồ sơ của một user
        Task<HoSoNguoiDung?> GetProfileAsync(Guid userId);

        // Tạo mới hoặc Cập nhật hồ sơ (Upsert)
        Task<HoSoNguoiDung> UpsertProfileAsync(Guid userId, HoSoNguoiDungRequest request);
    }
}
