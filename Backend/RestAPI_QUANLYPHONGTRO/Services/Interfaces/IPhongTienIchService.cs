using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IPhongTienIchService
    {
        // Lấy danh sách (Trả về list int ID hoặc list object TienIch tùy nhu cầu)
        // Ở đây mình trả về list Object TienIch để frontend dễ hiển thị tên
        Task<IEnumerable<TienIch>> GetAmenitiesByRoomIdAsync(Guid phongId);

        // Thêm tiện ích (Cần check quyền chủ phòng)
        Task<bool> AddAsync(PhongTienIchRequest request, Guid userId);

        // Xóa tiện ích (Cần check quyền chủ phòng)
        Task<bool> RemoveAsync(Guid phongId, int tienIchId, Guid userId);
    }
}
