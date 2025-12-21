using RestAPI_QUANLYPHONGTRO.Models;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IBaoCaoViPhamService
    {
        Task<IEnumerable<BaoCaoViPham>> GetAllBaoCaoAsync();
        Task<BaoCaoViPham?> GetBaoCaoByIdAsync(Guid id);
        Task<BaoCaoViPham> CreateBaoCaoAsync(BaoCaoViPham baoCao);
        Task<BaoCaoViPham?> UpdateBaoCaoAsync(Guid id, BaoCaoViPham baoCao);
        Task<bool> DeleteBaoCaoAsync(Guid id);
    }
}
