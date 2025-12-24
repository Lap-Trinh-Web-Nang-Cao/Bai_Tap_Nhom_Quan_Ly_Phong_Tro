using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ITinNhanService
    {
        // ===== CŨ: TIN NHẮN NGƯỜI-NGƯỜI =====

        /// <summary>
        /// Gửi tin nhắn thường (người-người)
        /// </summary>
        Task<TinNhan> SendAsync(SendMessageRequest request, Guid fromUserId);

        /// <summary>
        /// Lấy cuộc hội thoại giữa 2 người
        /// </summary>
        Task<IEnumerable<TinNhan>> GetConversationAsync(Guid userId, Guid otherUserId);

        /// <summary>
        /// Đánh dấu đã đọc tin nhắn
        /// </summary>
        Task<bool> MarkAsReadAsync(Guid userId, Guid otherUserId);

        // ===== MỚI: HỖ TRỢ TICKET + AI READY =====

        /// <summary>
        /// Tạo ticket hỗ trợ mới
        /// </summary>
        Task<TinNhan> CreateSupportTicketAsync(CreateSupportTicketRequest request, Guid userId);

        /// <summary>
        /// Gửi tin nhắn trong ticket hỗ trợ
        /// </summary>
        Task<TinNhan> SendSupportMessageAsync(Guid ticketId, SendSupportMessageRequest request, Guid userId);

        /// <summary>
        /// Lấy lịch sử tin nhắn của ticket
        /// </summary>
        Task<IEnumerable<TinNhan>> GetSupportMessagesAsync(Guid ticketId, Guid userId);

        /// <summary>
        /// Lấy tất cả ticket hỗ trợ của user
        /// </summary>
        Task<IEnumerable<TinNhan>> GetMySupportsAsync(Guid userId, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// Đóng ticket hỗ trợ
        /// </summary>
        Task<bool> CloseSupportTicketAsync(Guid ticketId, Guid userId);

        /// <summary>
        /// Gửi tin nhắn từ AI (internal use)
        /// </summary>
        Task<TinNhan> SendAIResponseAsync(Guid ticketId, string aiMessage, string? metaData = null);

        /// <summary>
        /// Lấy thống kê hỗ trợ của user
        /// </summary>
        Task<(int Total, int Pending, int Resolved)> GetSupportStatsAsync(Guid userId);
    }
}