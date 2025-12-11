using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ILoaiHoTroService
    {
        Task<IEnumerable<LoaiHoTro>> GetAllAsync();
        Task<LoaiHoTro?> GetByIdAsync(int id);
        Task<LoaiHoTro> CreateAsync(LoaiHoTroRequest request);
        Task<LoaiHoTro?> UpdateAsync(int id, LoaiHoTroRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
