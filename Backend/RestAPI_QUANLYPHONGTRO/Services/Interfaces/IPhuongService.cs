using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IPhuongService
    {
        Task<IEnumerable<Phuong>> GetAllAsync();

        // Hàm quan trọng nhất: Lấy danh sách phường của 1 quận cụ thể
        Task<IEnumerable<Phuong>> GetByQuanIdAsync(int quanId);

        Task<Phuong?> GetByIdAsync(int id);
        Task<Phuong> CreateAsync(PhuongRequest request);
        Task<Phuong?> UpdateAsync(int id, PhuongRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
