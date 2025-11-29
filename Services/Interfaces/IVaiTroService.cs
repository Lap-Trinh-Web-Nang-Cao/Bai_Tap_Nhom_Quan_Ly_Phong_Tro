using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IVaiTroService
    {
        Task<IEnumerable<VaiTro>> GetAllAsync();
        Task<VaiTro?> GetByIdAsync(int id);
        Task<VaiTro> CreateAsync(VaiTroRequest request);
        Task<VaiTro?> UpdateAsync(int id, VaiTroRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
