using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface INhaTroService
    {
        // Public: Lấy danh sách hiển thị trang chủ (có thể lọc theo khu vực sau này)
        Task<IEnumerable<NhaTro>> GetAllActiveAsync();

        // Public: Xem chi tiết
        Task<NhaTro?> GetByIdAsync(Guid id);

        // Chủ trọ: Xem danh sách nhà trọ CỦA MÌNH
        Task<IEnumerable<NhaTro>> GetByOwnerIdAsync(Guid chuTroId);

        // Chủ trọ: Tạo mới
        Task<NhaTro> CreateAsync(NhaTroRequest request, Guid chuTroId);

        // Chủ trọ: Cập nhật
        Task<NhaTro?> UpdateAsync(Guid id, NhaTroRequest request, Guid chuTroId);

        // Chủ trọ/Admin: Xóa
        Task<bool> DeleteAsync(Guid id, Guid currentUserId, bool isAdmin);
    }
}
