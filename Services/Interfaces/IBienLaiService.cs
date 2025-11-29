using RestAPI_QUANLYPHONGTRO.Models;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IBienLaiService
    {
        Task<IEnumerable<BienLai>> GetAllAsync();
        Task<BienLai?> GetByIdAsync(Guid id);
        Task<BienLai> CreateAsync(BienLai bienLai);
        Task<BienLai?> UpdateAsync(Guid id, BienLai bienLai);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ConfirmBienLaiAsync(Guid id, Guid nguoiXacNhanId);
    }
}
