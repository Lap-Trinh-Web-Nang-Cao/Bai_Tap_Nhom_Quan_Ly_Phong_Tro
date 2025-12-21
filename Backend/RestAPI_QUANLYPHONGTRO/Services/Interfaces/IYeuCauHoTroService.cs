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
        
        // === ADMIN METHODS ===
        
        /// <summary>
        /// Admin: Lấy tất cả yêu cầu hỗ trợ
        /// </summary>
        Task<IEnumerable<YeuCauHoTro>> GetAllAsync();
        
        /// <summary>
        /// Admin: Lấy chi tiết yêu cầu theo ID
        /// </summary>
        Task<YeuCauHoTro?> GetByIdAsync(Guid hoTroId);
        
        /// <summary>
        /// Admin: Cập nhật trạng thái (không cần check quyền chủ trọ)
        /// </summary>
        Task<bool> AdminUpdateStatusAsync(Guid hoTroId, string trangThaiMoi);
        
        /// <summary>
        /// Admin: Lấy thống kê yêu cầu hỗ trợ
        /// </summary>
        Task<SupportStatisticsDto> GetStatisticsAsync();
    }
    
    /// <summary>
    /// DTO cho thống kê yêu cầu hỗ trợ
    /// </summary>
    public class SupportStatisticsDto
    {
        public int TotalRequests { get; set; }
        public int NewRequests { get; set; }
        public int ProcessingRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int RejectedRequests { get; set; }
    }
}
