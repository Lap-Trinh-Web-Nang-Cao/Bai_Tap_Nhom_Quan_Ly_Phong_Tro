using RestAPI_QUANLYPHONGTRO.Models;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ILichSuService
    {
        // Hàm ghi log (Internal use)
        Task AddLogAsync(Guid? userId, string hanhDong, string? tenBang, string? banGhiId, string? chiTiet);

        // Lấy lịch sử của 1 User (Phân trang hoặc lấy limit)
        Task<IEnumerable<LichSu>> GetUserHistoryAsync(Guid userId, int limit = 20);

        // Xóa toàn bộ lịch sử của 1 User
        Task<bool> ClearUserHistoryAsync(Guid userId);
    }
}
