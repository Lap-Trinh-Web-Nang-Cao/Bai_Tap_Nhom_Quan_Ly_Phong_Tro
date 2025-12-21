using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface ITinNhanService
    {
        // Gửi tin nhắn
        Task<TinNhan> SendAsync(SendMessageRequest request, Guid fromUserId);

        // Lấy cuộc hội thoại giữa tôi (userId) và người khác (otherUserId)
        Task<IEnumerable<TinNhan>> GetConversationAsync(Guid userId, Guid otherUserId);

        // Đánh dấu đã đọc tất cả tin nhắn từ người khác gửi cho tôi
        Task<bool> MarkAsReadAsync(Guid userId, Guid otherUserId);
    }
}
