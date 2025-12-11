using RestAPI_QUANLYPHONGTRO.Models;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IChuTroThongTinPhapLyService
    {
        Task<IEnumerable<ChuTroThongTinPhapLy>> GetAllAsync();
        Task<ChuTroThongTinPhapLy?> GetByIdAsync(Guid nguoiDungId);
        Task<ChuTroThongTinPhapLy> CreateOrUpdateAsync(ChuTroThongTinPhapLy model);
        Task<bool> DeleteAsync(Guid nguoiDungId);

        // Nghiệp vụ phê duyệt hồ sơ chủ trọ
        Task<bool> ApproveAsync(Guid nguoiDungId, string trangThai);
    }
}
