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

        #region CŨ: TIN NHẮN NGƯỜI-NGƯỜI

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
                DaDoc = false,
                LoaiTinNhan = "Normal"
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
                .Where(m => m.LoaiTinNhan == "Normal")
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
                .Where(m => m.LoaiTinNhan == "Normal")
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

        #region MỚI: HỖ TRỢ TICKET + AI READY

        /// <summary>
        /// Tạo ticket hỗ trợ mới - sử dụng TinNhan làm message đầu tiên
        /// </summary>
        public async Task<TinNhan> CreateSupportTicketAsync(CreateSupportTicketRequest request, Guid userId)
        {
            // Validate user
            var user = await _context.NguoiDungs.FindAsync(userId);
            if (user == null || user.IsKhoa)
                throw new Exception("Người dùng không hợp lệ hoặc đã bị khóa.");

            if (string.IsNullOrWhiteSpace(request.TieuDe) || string.IsNullOrWhiteSpace(request.MoTa))
                throw new ArgumentException("Tiêu đề và mô tả không được để trống.");

            // Lấy ID admin để gửi cho (có thể config sau)
            var adminId = await _context.NguoiDungs
                .Where(u => u.VaiTroId == 1) // VaiTroId 1 = Admin
                .Select(u => u.NguoiDungId)
                .FirstOrDefaultAsync();

            if (adminId == Guid.Empty)
                throw new Exception("Không tìm thấy admin hỗ trợ.");

            // Tạo message đầu tiên của ticket
            var ticketMessage = new TinNhan
            {
                TinNhanId = Guid.NewGuid(),
                FromUser = userId,
                ToUser = adminId,
                NoiDung = $"[TICKET] {request.TieuDe}\n\n{request.MoTa}",
                ThoiGian = DateTimeOffset.UtcNow,
                DaDoc = false,
                LoaiTinNhan = "Support",
                UuTien = request.UuTien,
                LoaiVanDe = request.LoaiVanDe,
                TrangThaiHoTro = "Pending", // Chờ xử lý
                IsAIResponse = false
            };

            _context.TinNhans.Add(ticketMessage);
            await _context.SaveChangesAsync();

            return ticketMessage;
        }

        /// <summary>
        /// Gửi tin nhắn trong ticket hỗ trợ
        /// </summary>
        public async Task<TinNhan> SendSupportMessageAsync(Guid ticketId, SendSupportMessageRequest request, Guid userId)
        {
            if (ticketId == Guid.Empty)
                throw new ArgumentException("ID ticket không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.NoiDung))
                throw new ArgumentException("Nội dung tin nhắn không được để trống.");

            // Tìm ticket gốc
            var ticket = await _context.TinNhans.FindAsync(ticketId);
            if (ticket == null || ticket.LoaiTinNhan != "Support")
                throw new Exception("Ticket không tồn tại hoặc không phải ticket hỗ trợ.");

            // Kiểm tra quyền (user phải là người tạo hoặc admin)
            var user = await _context.NguoiDungs.FindAsync(userId);
            if (user == null)
                throw new Exception("Người dùng không hợp lệ.");

            bool isCreator = ticket.FromUser == userId;
            bool isAdmin = user.VaiTroId == 1;

            if (!isCreator && !isAdmin)
                throw new Exception("Bạn không có quyền trả lời ticket này.");

            // Tạo tin nhắn mới
            var newMessage = new TinNhan
            {
                TinNhanId = Guid.NewGuid(),
                FromUser = userId,
                ToUser = isCreator ? ticket.ToUser : ticket.FromUser, // Gửi cho người còn lại
                NoiDung = request.NoiDung.Trim(),
                TapTinId = request.TapTinId,
                ThoiGian = DateTimeOffset.UtcNow,
                DaDoc = false,
                LoaiTinNhan = "Support",
                YeuCauHoTroId = ticketId,
                UuTien = ticket.UuTien,
                LoaiVanDe = ticket.LoaiVanDe,
                MetaData = request.MetaData,
                TrangThaiHoTro = isAdmin ? "Answered" : ticket.TrangThaiHoTro,
                IsAIResponse = false
            };

            _context.TinNhans.Add(newMessage);

            // Update trạng thái ticket
            ticket.TrangThaiHoTro = isAdmin ? "Answered" : ticket.TrangThaiHoTro;

            await _context.SaveChangesAsync();

            return newMessage;
        }

        /// <summary>
        /// Lấy lịch sử tin nhắn của ticket
        /// </summary>
        public async Task<IEnumerable<TinNhan>> GetSupportMessagesAsync(Guid ticketId, Guid userId)
        {
            if (ticketId == Guid.Empty)
                throw new ArgumentException("ID ticket không hợp lệ.");

            // Tìm ticket gốc
            var ticket = await _context.TinNhans.FindAsync(ticketId);
            if (ticket == null)
                throw new Exception("Ticket không tồn tại.");

            // Kiểm tra quyền
            var user = await _context.NguoiDungs.FindAsync(userId);
            bool isCreator = ticket.FromUser == userId;
            bool isAdmin = user?.VaiTroId == 1;

            if (!isCreator && !isAdmin)
                throw new Exception("Bạn không có quyền xem ticket này.");

            // Lấy tất cả tin nhắn liên kết
            var messages = await _context.TinNhans
                .Where(m => m.YeuCauHoTroId == ticketId || m.TinNhanId == ticketId)
                .Where(m => m.LoaiTinNhan == "Support")
                .OrderBy(m => m.ThoiGian)
                .ToListAsync();

            // Đánh dấu đã đọc
            var unread = messages.Where(m => m.ToUser == userId && !m.DaDoc).ToList();
            foreach (var msg in unread)
                msg.DaDoc = true;

            if (unread.Any())
                await _context.SaveChangesAsync();

            return messages;
        }

        /// <summary>
        /// Lấy danh sách ticket hỗ trợ của user
        /// </summary>
        public async Task<IEnumerable<TinNhan>> GetMySupportsAsync(Guid userId, int pageIndex = 1, int pageSize = 10)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("ID người dùng không hợp lệ.");

            var skip = (pageIndex - 1) * pageSize;

            var tickets = await _context.TinNhans
                .Where(m => m.LoaiTinNhan == "Support")
                .Where(m => m.FromUser == userId || m.ToUser == userId)
                .Where(m => m.YeuCauHoTroId == null) // Chỉ lấy ticket gốc
                .OrderByDescending(m => m.ThoiGian)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return tickets;
        }

        /// <summary>
        /// Đóng ticket hỗ trợ
        /// </summary>
        public async Task<bool> CloseSupportTicketAsync(Guid ticketId, Guid userId)
        {
            if (ticketId == Guid.Empty)
                throw new ArgumentException("ID ticket không hợp lệ.");

            var ticket = await _context.TinNhans.FindAsync(ticketId);
            if (ticket == null || ticket.LoaiTinNhan != "Support")
                throw new Exception("Ticket không tồn tại.");

            // Chỉ creator hoặc admin mới được đóng
            var user = await _context.NguoiDungs.FindAsync(userId);
            bool isCreator = ticket.FromUser == userId;
            bool isAdmin = user?.VaiTroId == 1;

            if (!isCreator && !isAdmin)
                throw new Exception("Bạn không có quyền đóng ticket này.");

            ticket.TrangThaiHoTro = "Resolved";
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Gửi tin nhắn từ AI - dùng cho tương lai
        /// </summary>
        public async Task<TinNhan> SendAIResponseAsync(Guid ticketId, string aiMessage, string? metaData = null)
        {
            if (ticketId == Guid.Empty)
                throw new ArgumentException("ID ticket không hợp lệ.");

            var ticket = await _context.TinNhans.FindAsync(ticketId);
            if (ticket == null)
                throw new Exception("Ticket không tồn tại.");

            // Lấy AI bot user (hoặc tạo nếu chưa có)
            var aiUser = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.Email == "ai-bot@system.local");

            if (aiUser == null)
            {
                // Tạo user AI nếu chưa có
                aiUser = new NguoiDung
                {
                    NguoiDungId = Guid.NewGuid(),
                    Email = "ai-bot@system.local",
                    DienThoai = "0000000000",
                    PasswordHash = "SYSTEM_AI_BOT",
                    VaiTroId = 1, // Admin role
                    IsKhoa = false,
                    IsEmailXacThuc = true,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                _context.NguoiDungs.Add(aiUser);
                await _context.SaveChangesAsync();
            }

            var aiResponse = new TinNhan
            {
                TinNhanId = Guid.NewGuid(),
                FromUser = aiUser.NguoiDungId,
                ToUser = ticket.FromUser,
                NoiDung = aiMessage,
                ThoiGian = DateTimeOffset.UtcNow,
                DaDoc = false,
                LoaiTinNhan = "Support",
                YeuCauHoTroId = ticketId,
                UuTien = ticket.UuTien,
                LoaiVanDe = ticket.LoaiVanDe,
                MetaData = metaData,
                TrangThaiHoTro = "Answered",
                IsAIResponse = true
            };

            _context.TinNhans.Add(aiResponse);
            ticket.TrangThaiHoTro = "Answered";
            await _context.SaveChangesAsync();

            return aiResponse;
        }

        /// <summary>
        /// Lấy thống kê hỗ trợ của user
        /// </summary>
        public async Task<(int Total, int Pending, int Resolved)> GetSupportStatsAsync(Guid userId)
        {
            var tickets = await _context.TinNhans
                .Where(m => m.LoaiTinNhan == "Support")
                .Where(m => m.FromUser == userId)
                .Where(m => m.YeuCauHoTroId == null)
                .ToListAsync();

            int total = tickets.Count;
            int pending = tickets.Count(t => t.TrangThaiHoTro == "Pending");
            int resolved = tickets.Count(t => t.TrangThaiHoTro == "Resolved");

            return (total, pending, resolved);
        }

        #endregion
    }
}