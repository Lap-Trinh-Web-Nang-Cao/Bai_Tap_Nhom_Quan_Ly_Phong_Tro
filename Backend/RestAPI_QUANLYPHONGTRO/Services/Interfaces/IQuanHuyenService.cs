using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IQuanHuyenService
    {
        Task<IEnumerable<QuanHuyen>> GetAllAsync();
        Task<QuanHuyen?> GetByIdAsync(int id);
        Task<QuanHuyen> CreateAsync(QuanHuyenRequest request);
        Task<QuanHuyen?> UpdateAsync(int id, QuanHuyenRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
