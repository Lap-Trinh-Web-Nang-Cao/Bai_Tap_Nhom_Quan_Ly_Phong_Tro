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

        public async Task<TinNhan> SendAsync(SendMessageRequest request, Guid fromUserId)
        {
            // Kiểm tra: Nội dung và File không được cùng null (phải có ít nhất 1 cái)
            if (string.IsNullOrEmpty(request.NoiDung) && request.TapTinId == null)
            {
                throw new Exception("Nội dung tin nhắn không được để trống.");
            }

            var tinNhan = new TinNhan
            {
                TinNhanId = Guid.NewGuid(),
                FromUser = fromUserId,
                ToUser = request.ToUser,
                NoiDung = request.NoiDung,
                TapTinId = request.TapTinId,
                ThoiGian = DateTimeOffset.Now,
                DaDoc = false // Mới gửi thì chưa đọc
            };

            _context.TinNhans.Add(tinNhan);
            await _context.SaveChangesAsync();
            return tinNhan;
        }

        public async Task<IEnumerable<TinNhan>> GetConversationAsync(Guid userId, Guid otherUserId)
        {
            // Lấy tin nhắn 2 chiều
            var messages = await _context.TinNhans
                .Where(m => (m.FromUser == userId && m.ToUser == otherUserId) ||
                            (m.FromUser == otherUserId && m.ToUser == userId))
                .OrderBy(m => m.ThoiGian) // Xếp từ cũ đến mới
                .ToListAsync();

            return messages;
        }

        public async Task<bool> MarkAsReadAsync(Guid userId, Guid otherUserId)
        {
            // Tìm các tin nhắn mà "Người khác" gửi cho "Tôi" và chưa đọc
            var unreadMessages = await _context.TinNhans
                .Where(m => m.FromUser == otherUserId && m.ToUser == userId && !m.DaDoc)
                .ToListAsync();

            if (!unreadMessages.Any()) return false;

            foreach (var msg in unreadMessages)
            {
                msg.DaDoc = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<object>> GetMyConversationsAsync(Guid userId)
        {
            // Lấy tất cả tin nhắn liên quan đến user
            var allMessages = await _context.TinNhans
                .Where(m => m.FromUser == userId || m.ToUser == userId)
                .OrderByDescending(m => m.ThoiGian)
                .ToListAsync();

            // Nhóm theo người đối thoại
            var conversations = allMessages
                .GroupBy(m => m.FromUser == userId ? m.ToUser : m.FromUser)
                .Select(g => new
                {
                    OtherUserId = g.Key,
                    LastMessage = g.First().NoiDung,
                    LastTime = g.First().ThoiGian,
                    UnreadCount = g.Count(m => m.ToUser == userId && !m.DaDoc)
                })
                .ToList();

            var result = new List<object>();
            foreach (var conv in conversations)
            {
                var otherUser = await _context.HoSoNguoiDungs
                    .FirstOrDefaultAsync(h => h.NguoiDungId == conv.OtherUserId);

                result.Add(new
                {
                    conv.OtherUserId,
                    conv.LastMessage,
                    conv.LastTime,
                    conv.UnreadCount,
                    OtherUserName = otherUser?.HoTen ?? "Người dùng",
                    //OtherUserAvatar = otherUser?.HinhAnh ?? "/images/default-avatar.png"
                });
            }

            return result;
        }
    }
}
