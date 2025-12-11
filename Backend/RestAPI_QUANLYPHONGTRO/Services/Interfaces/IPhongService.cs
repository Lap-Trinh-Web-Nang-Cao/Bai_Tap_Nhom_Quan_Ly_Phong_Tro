using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IPhongService
    {
        // Public: Tìm kiếm phòng (Theo nhà trọ, giá, diện tích...)
        Task<(IEnumerable<Phong> Data, int TotalCount)> GetPublicRoomsAsync(
            Guid? nhaTroId,
            long? minPrice,
            long? maxPrice,
            int pageIndex,
            int pageSize);

        // Public: Xem chi tiết
        Task<Phong?> GetByIdAsync(Guid id);

        // Chủ trọ: Tạo phòng
        Task<Phong> CreateAsync(CreatePhongRequest request, Guid userId);

        // Chủ trọ: Cập nhật phòng
        Task<Phong?> UpdateAsync(Guid id, CreatePhongRequest request, Guid userId);

        // Admin: Duyệt phòng
        Task<bool> ApproveRoomAsync(Guid id, Guid adminId);

        // Admin: Khóa/Mở khóa phòng
        Task<bool> LockRoomAsync(Guid id, bool isLocked);

        // Admin: Xóa phòng
        Task<bool> DeleteAsync(Guid id);
    }
}
