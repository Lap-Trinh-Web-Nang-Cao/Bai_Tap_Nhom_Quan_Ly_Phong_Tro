using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using RestAPI_QUANLYPHONGTRO.Data;
using RestAPI_QUANLYPHONGTRO.Models;
using RestAPI_QUANLYPHONGTRO.Hubs;
using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Controller cho Chat - Hỗ trợ giao tiếp giữa USER project và SignalR Hub
    /// Không yêu cầu Authorization để dễ tích hợp với SignalR
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatController> _logger;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(ApplicationDbContext context, ILogger<ChatController> logger, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Gửi tin nhắn THƯỜNG (người-người)
        /// POST: api/chat/send
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.FromUserId) || string.IsNullOrEmpty(request.ToUserId) || string.IsNullOrEmpty(request.Content))
                    return BadRequest("Dữ liệu không hợp lệ");

                if (!Guid.TryParse(request.FromUserId, out var fromUserId) || !Guid.TryParse(request.ToUserId, out var toUserId))
                    return BadRequest("ID không hợp lệ");

                // Check self-contact
                if (fromUserId == toUserId)
                    return BadRequest("Bạn không thể gửi tin nhắn cho chính mình");

                var tinNhan = new TinNhan
                {
                    TinNhanId = Guid.NewGuid(),
                    FromUser = fromUserId,
                    ToUser = toUserId,
                    NoiDung = request.Content.Trim(),
                    ThoiGian = DateTimeOffset.UtcNow,
                    DaDoc = false
                };

                _context.TinNhans.Add(tinNhan);
                await _context.SaveChangesAsync();

                // Prepare response
                var response = new ChatMessageResponse
                {
                    MessageId = tinNhan.TinNhanId.ToString(),
                    FromUserId = tinNhan.FromUser.ToString(),
                    ToUserId = tinNhan.ToUser.ToString(),
                    Content = tinNhan.NoiDung,
                    Timestamp = tinNhan.ThoiGian.HasValue ? tinNhan.ThoiGian.Value.UtcDateTime : DateTime.UtcNow,
                    IsRead = tinNhan.DaDoc
                };

                // Get sender name for realtime notification
                var senderUser = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.NguoiDungId == fromUserId);
                var fromDisplayName = senderUser?.Email ?? "Người dùng";

                // If receiver online via ChatHub, notify them in realtime
                if (ChatHub.TryGetConnectionId(toUserId.ToString(), out var connectionId))
                {
                    var payload = new
                    {
                        MessageId = response.MessageId,
                        FromUserId = response.FromUserId,
                        FromUserName = fromDisplayName,
                        ToUserId = response.ToUserId,
                        Content = response.Content,
                        MessageType = response.MessageType,
                        Timestamp = response.Timestamp,
                        IsRead = response.IsRead
                    };

                    try
                    {
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", payload);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Không gửi realtime được (người nhận có thể offline)");
                    }
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error saving message");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử tin nhắn THƯỜNG
        /// GET: api/chat/history?user1={userId1}&user2={userId2}&page=1&pageSize=50
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([Required][FromQuery] string user1, [Required][FromQuery] string user2, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                if (string.IsNullOrEmpty(user1) || string.IsNullOrEmpty(user2))
                    return BadRequest(new { error = "user1 và user2 không được để trống" });

                if (!Guid.TryParse(user1, out var userId1) || !Guid.TryParse(user2, out var userId2))
                    return BadRequest(new { error = "ID không hợp lệ" });

                if (!await _context.Database.CanConnectAsync())
                    return StatusCode(503, new { error = "Database is not accessible" });

                var messages = await _context.TinNhans
                    .Where(m => (m.FromUser == userId1 && m.ToUser == userId2) ||
                                (m.FromUser == userId2 && m.ToUser == userId1))
                    .OrderBy(m => m.ThoiGian)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new ChatMessageResponse
                    {
                        MessageId = m.TinNhanId.ToString(),
                        FromUserId = m.FromUser.ToString(),
                        ToUserId = m.ToUser.ToString(),
                        Content = m.NoiDung ?? "",
                        Timestamp = m.ThoiGian.HasValue ? m.ThoiGian.Value.UtcDateTime : DateTime.UtcNow,
                        IsRead = m.DaDoc
                    })
                    .ToListAsync();

                if (!messages.Any())
                {
                    _logger.LogWarning($"No messages found for users {user1} and {user2}");
                }

                return Ok(messages);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "❌ Database context error");
                return StatusCode(503, new { error = "Database connection error", details = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading chat history");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách liên hệ (từ tin nhắn THƯỜNG)
        /// GET: api/chat/contacts?userId={userId}
        /// </summary>
        [HttpGet("contacts")]
        public async Task<IActionResult> GetContacts([Required][FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest(new { error = "userId không được để trống" });

                if (!Guid.TryParse(userId, out var userGuid))
                    return BadRequest(new { error = "userId không hợp lệ" });

                var contactIds = await _context.TinNhans
                    .Where(m => m.FromUser == userGuid || m.ToUser == userGuid)
                    .Select(m => m.FromUser == userGuid ? m.ToUser : m.FromUser)
                    .Distinct()
                    .ToListAsync();

                var contacts = new List<ContactResponse>();

                foreach (var contactId in contactIds)
                {
                    // Get user with profile
                    var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.NguoiDungId == contactId);
                    var hoSo = await _context.HoSoNguoiDungs.FirstOrDefaultAsync(h => h.NguoiDungId == contactId);

                    if (user != null)
                    {
                        var lastMessage = await _context.TinNhans
                            .Where(m => (m.FromUser == userGuid && m.ToUser == contactId) ||
                                        (m.FromUser == contactId && m.ToUser == userGuid))
                            .OrderByDescending(m => m.ThoiGian)
                            .FirstOrDefaultAsync();

                        var unreadCount = await _context.TinNhans
                            .Where(m => m.FromUser == contactId && m.ToUser == userGuid && !m.DaDoc)
                            .CountAsync();

                        // Prioritize HoTen from HoSoNguoiDung, fallback to Email
                        var displayName = hoSo?.HoTen ?? user.Email ?? "Người dùng";

                        contacts.Add(new ContactResponse
                        {
                            UserId = user.NguoiDungId.ToString(),
                            UserName = displayName,
                            LastMessage = lastMessage?.NoiDung ?? "",
                            LastMessageTime = lastMessage?.ThoiGian?.UtcDateTime,
                            UnreadCount = unreadCount
                        });
                    }
                }

                contacts = contacts.OrderByDescending(c => c.LastMessageTime).ToList();

                _logger.LogInformation($"✅ GetContacts for {userId}: {contacts.Count} contacts found");
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting contacts");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu tin nhắn đã đọc
        /// PUT: api/chat/read/{messageId}
        /// </summary>
        [HttpPut("read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(string messageId)
        {
            try
            {
                if (!Guid.TryParse(messageId, out var msgId))
                {
                    return BadRequest("messageId không hợp lệ");
                }

                var message = await _context.TinNhans.FindAsync(msgId);
                if (message == null)
                {
                    return NotFound("Tin nhắn không tồn tại");
                }

                message.DaDoc = true;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Đã đánh dấu đã đọc" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error marking message as read");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Đánh dấu tất cả tin nhắn từ một người là đã đọc
        /// PUT: api/chat/read-all?fromUserId={fromId}&toUserId={toId}
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead([Required][FromQuery] string fromUserId, [Required][FromQuery] string toUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(fromUserId) || string.IsNullOrEmpty(toUserId))
                    return BadRequest(new { error = "fromUserId và toUserId không được để trống" });

                if (!Guid.TryParse(fromUserId, out var fromId) || !Guid.TryParse(toUserId, out var toId))
                {
                    return BadRequest(new { error = "fromUserId hoặc toUserId không hợp lệ" });
                }

                var unreadMessages = await _context.TinNhans
                    .Where(m => m.FromUser == fromId && m.ToUser == toId && !m.DaDoc)
                    .ToListAsync();

                foreach (var msg in unreadMessages)
                {
                    msg.DaDoc = true;
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, count = unreadMessages.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error marking all messages as read");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy số tin nhắn chưa đọc
        /// GET: api/chat/unread-count?userId={userId}
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount([Required][FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest(new { error = "userId không được để trống" });

                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return BadRequest(new { error = "userId không hợp lệ" });
                }

                var count = await _context.TinNhans
                    .Where(m => m.ToUser == userGuid && !m.DaDoc)
                    .CountAsync();

                return Ok(new { unreadCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting unread count");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    #region Request/Response Models

    public class SendChatMessageRequest
    {
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? MessageType { get; set; } = "text";
    }

    public class ChatMessageResponse
    {
        public string MessageId { get; set; } = string.Empty;
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MessageType { get; set; } = "text";
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }

    public class ContactResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }

    #endregion
}
