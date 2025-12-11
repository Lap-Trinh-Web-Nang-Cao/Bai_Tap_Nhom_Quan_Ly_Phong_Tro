using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IDanhGiaPhongService
    {
        // Tạo đánh giá mới (Cần userId của người đang đăng nhập)
        Task<DanhGiaPhong> CreateAsync(CreateDanhGiaRequest request, Guid userId);

        // Lấy tất cả đánh giá của 1 phòng cụ thể (Để hiển thị trang chi tiết phòng)
        Task<IEnumerable<DanhGiaPhong>> GetByPhongIdAsync(Guid phongId);

        // Xóa đánh giá (Dành cho Admin hoặc chính chủ)
        Task<bool> DeleteAsync(Guid danhGiaId, Guid userId, bool isAdmin);
    }
}
