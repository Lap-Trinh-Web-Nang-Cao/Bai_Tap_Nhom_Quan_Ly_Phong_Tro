using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ITienIchService
    {
        Task<IEnumerable<TienIch>> GetAllAsync();
        Task<TienIch?> GetByIdAsync(int id);
        Task<TienIch> CreateAsync(TienIchRequest request);
        Task<TienIch?> UpdateAsync(int id, TienIchRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
