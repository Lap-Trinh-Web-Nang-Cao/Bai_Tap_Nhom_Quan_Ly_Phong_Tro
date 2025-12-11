using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IViPhamService
    {
        Task<IEnumerable<ViPham>> GetAllAsync();
        Task<ViPham?> GetByIdAsync(int id);

        // Admin tạo loại vi phạm mới
        Task<ViPham> CreateAsync(ViPhamRequest request);

        // Admin cập nhật quy định xử phạt
        Task<ViPham?> UpdateAsync(int id, ViPhamRequest request);

        // Admin xóa
        Task<bool> DeleteAsync(int id);
    }
}
