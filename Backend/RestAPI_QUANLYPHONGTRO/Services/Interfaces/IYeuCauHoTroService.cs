using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IYeuCauHoTroService
    {
        // Người thuê: Tạo yêu cầu mới
        Task<YeuCauHoTro> CreateAsync(CreateYeuCauRequest request, Guid userId);

        // Người thuê: Xem danh sách yêu cầu mình đã gửi
        Task<IEnumerable<YeuCauHoTro>> GetMyRequestsAsync(Guid userId);

        // Chủ trọ: Xem danh sách yêu cầu gửi đến các phòng CỦA MÌNH
        Task<IEnumerable<YeuCauHoTro>> GetRequestsForLandlordAsync(Guid chuTroId);

        // Chủ trọ: Cập nhật trạng thái (VD: Moi -> DangXuLy -> HoanThanh)
        Task<bool> UpdateStatusAsync(Guid hoTroId, string trangThaiMoi, Guid chuTroId);
    }
}
