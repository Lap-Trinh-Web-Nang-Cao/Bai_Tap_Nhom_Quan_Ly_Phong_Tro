using Microsoft.EntityFrameworkCore;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Services.Interfaces;
using RestAPI_QUANLYPHONGTRO.ViewModels;

namespace RestAPI_QUANLYPHONGTRO.Services.Implements
{
    public class TinNhanService : ITinNhanService
    {
        private readonly ApplicationDbContext _context;

        public TinNhanService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region TIN NHẮN NGƯỜI-NGƯỜI

        public async Task<TinNhan> SendAsync(SendMessageRequest request, Guid fromUserId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Yêu cầu tin nhắn không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.NoiDung) && request.TapTinId == null)
                throw new Exception("Tin nhắn phải có nội dung hoặc tệp đính kèm.");

            if (request.ToUser == Guid.Empty)
                throw new Exception("ID người nhận không hợp lệ.");

            if (fromUserId == request.ToUser)
                throw new Exception("Bạn không thể gửi tin nhắn cho chính mình.");

            var receiverExists = await _context.NguoiDungs
                .AnyAsync(u => u.NguoiDungId == request.ToUser && !u.IsKhoa);

            if (!receiverExists)
                throw new Exception("Người nhận không tồn tại hoặc đã bị khóa.");

            var tinNhan = new TinNhan
            {
                TinNhanId = Guid.NewGuid(),
                FromUser = fromUserId,
                ToUser = request.ToUser,
                NoiDung = request.NoiDung?.Trim(),
                TapTinId = request.TapTinId,
                ThoiGian = DateTimeOffset.UtcNow,
                DaDoc = false
            };

            _context.TinNhans.Add(tinNhan);
            await _context.SaveChangesAsync();

            return tinNhan;
        }

        public async Task<IEnumerable<TinNhan>> GetConversationAsync(Guid userId, Guid otherUserId)
        {
            if (userId == Guid.Empty || otherUserId == Guid.Empty)
                throw new ArgumentException("ID người dùng không hợp lệ.");

            var messages = await _context.TinNhans
                .Where(m => (m.FromUser == userId && m.ToUser == otherUserId) ||
                            (m.FromUser == otherUserId && m.ToUser == userId))
                .OrderBy(m => m.ThoiGian)
                .ToListAsync();

            return messages;
        }

        public async Task<bool> MarkAsReadAsync(Guid userId, Guid otherUserId)
        {
            if (userId == Guid.Empty || otherUserId == Guid.Empty)
                throw new ArgumentException("ID người dùng không hợp lệ.");

            var unreadMessages = await _context.TinNhans
                .Where(m => m.FromUser == otherUserId && m.ToUser == userId && !m.DaDoc)
                .ToListAsync();

            if (!unreadMessages.Any())
                return false;

            foreach (var msg in unreadMessages)
            {
                msg.DaDoc = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region STUB: SUPPORT TICKET (NOT IMPLEMENTED YET)

        public async Task<TinNhan> CreateSupportTicketAsync(CreateSupportTicketRequest request, Guid userId)
        {
            throw new NotImplementedException("Support tickets not yet implemented");
        }

        public async Task<TinNhan> SendSupportMessageAsync(Guid ticketId, SendSupportMessageRequest request, Guid userId)
        {
            throw new NotImplementedException("Support tickets not yet implemented");
        }

        public async Task<IEnumerable<TinNhan>> GetSupportMessagesAsync(Guid ticketId, Guid userId)
        {
            throw new NotImplementedException("Support tickets not yet implemented");
        }

        public async Task<IEnumerable<TinNhan>> GetMySupportsAsync(Guid userId, int pageIndex = 1, int pageSize = 10)
        {
            throw new NotImplementedException("Support tickets not yet implemented");
        }

        public async Task<bool> CloseSupportTicketAsync(Guid ticketId, Guid userId)
        {
            throw new NotImplementedException("Support tickets not yet implemented");
        }

        public async Task<TinNhan> SendAIResponseAsync(Guid ticketId, string aiMessage, string? metaData = null)
        {
            throw new NotImplementedException("Support tickets not yet implemented");
        }

        public async Task<(int Total, int Pending, int Resolved)> GetSupportStatsAsync(Guid userId)
        {
            throw new NotImplementedException("Support tickets not yet implemented");
        }

        #endregion
    }
}