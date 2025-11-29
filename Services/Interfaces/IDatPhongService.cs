using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IDatPhongService
    {
        // Đặt phòng mới
        Task<DatPhong> CreateBookingAsync(CreateDatPhongRequest request, Guid userId);

        // Lấy lịch sử đặt phòng của người dùng hiện tại
        Task<IEnumerable<DatPhong>> GetMyBookingsAsync(Guid userId);

        // Lấy danh sách yêu cầu đặt phòng (Dành cho Chủ trọ xem ai đặt phòng mình)
        Task<IEnumerable<DatPhong>> GetRequestsForLandlordAsync(Guid chuTroId);

        // Chủ trọ duyệt hoặc từ chối
        Task<bool> UpdateStatusAsync(Guid datPhongId, int trangThaiId, Guid chuTroId);
    }
}
