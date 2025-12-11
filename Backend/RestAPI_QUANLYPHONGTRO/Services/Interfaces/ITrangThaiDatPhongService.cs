using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ITrangThaiDatPhongService
    {
        Task<IEnumerable<TrangThaiDatPhong>> GetAllAsync();
        Task<TrangThaiDatPhong?> GetByIdAsync(int id);

        // Admin cập nhật tên trạng thái (VD: Sửa "ChoXacNhan" thành "Đang chờ duyệt")
        Task<TrangThaiDatPhong?> UpdateAsync(int id, TrangThaiRequest request);
    }
}
