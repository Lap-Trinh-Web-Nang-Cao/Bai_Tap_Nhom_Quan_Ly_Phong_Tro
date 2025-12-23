using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace RestAPI_QUANLYPHONGTRO.Hubs
{
    /// <summary>
    /// SignalR Hub cho Chat Realtime giữa Chủ Trọ và Người Thuê
    /// </summary>
    public class ChatHub : Hub
    {
        // Dictionary lưu mapping: UserId -> ConnectionId
        private static ConcurrentDictionary<string, string> UserConnections = new();
        
        // Dictionary lưu thông tin user online
        private static ConcurrentDictionary<string, OnlineUser> OnlineUsers = new();

        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        #region Connection Events

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("🟢 Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            
            var userEntry = UserConnections.FirstOrDefault(x => x.Value == connectionId);
            if (!string.IsNullOrEmpty(userEntry.Key))
            {
                UserConnections.TryRemove(userEntry.Key, out _);
                OnlineUsers.TryRemove(userEntry.Key, out _);
                
                await Clients.All.SendAsync("UserOffline", userEntry.Key);
                
                _logger.LogInformation("🔴 User offline: {UserId}", userEntry.Key);
            }

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region User Registration

        /// <summary>
        /// Đăng ký user khi vào trang chat
        /// </summary>
        public async Task RegisterUser(string userId, string userName, string userRole, string? avatarUrl)
        {
            if (string.IsNullOrEmpty(userId)) return;

            var connectionId = Context.ConnectionId;

            UserConnections.AddOrUpdate(userId, connectionId, (key, old) => connectionId);

            var userInfo = new OnlineUser
            {
                UserId = userId,
                UserName = userName,
                Role = userRole,
                AvatarUrl = avatarUrl ?? "/Content/images/default-avatar.png",
                ConnectionId = connectionId,
                LastSeen = DateTime.UtcNow
            };
            OnlineUsers.AddOrUpdate(userId, userInfo, (key, old) => userInfo);

            // Thông báo cho tất cả về user mới online
            await Clients.All.SendAsync("UserOnline", userInfo);

            // Gửi danh sách online cho client mới
            await Clients.Caller.SendAsync("OnlineUsers", OnlineUsers.Values.ToList());

            _logger.LogInformation("✅ User registered: {UserName} ({Role})", userName, userRole);
        }

        /// <summary>
        /// Lấy danh sách user đang online
        /// </summary>
        public Task<List<OnlineUser>> GetOnlineUsers()
        {
            return Task.FromResult(OnlineUsers.Values.ToList());
        }

        #endregion

        #region Messaging

        /// <summary>
        /// Gửi tin nhắn riêng đến một user
        /// </summary>
        public async Task SendPrivateMessage(string toUserId, string message, string messageType)
        {
            if (string.IsNullOrEmpty(toUserId) || string.IsNullOrEmpty(message)) return;

            var senderEntry = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (string.IsNullOrEmpty(senderEntry.Key)) return;

            if (!OnlineUsers.TryGetValue(senderEntry.Key, out var sender)) return;

            var chatMessage = new ChatMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                FromUserId = sender.UserId,
                FromUserName = sender.UserName,
                FromAvatarUrl = sender.AvatarUrl,
                ToUserId = toUserId,
                Content = message,
                MessageType = messageType ?? "text",
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            // Gửi cho người nhận
            if (UserConnections.TryGetValue(toUserId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", chatMessage);
            }

            // Gửi xác nhận cho người gửi
            await Clients.Caller.SendAsync("MessageSent", chatMessage);

            _logger.LogInformation("💬 Message from {From} to {To}", sender.UserName, toUserId);
        }

        /// <summary>
        /// Gửi tin nhắn nhóm (broadcast)
        /// </summary>
        public async Task SendGroupMessage(string groupName, string message)
        {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(message)) return;

            var senderEntry = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (string.IsNullOrEmpty(senderEntry.Key)) return;

            if (!OnlineUsers.TryGetValue(senderEntry.Key, out var sender)) return;

            var chatMessage = new ChatMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                FromUserId = sender.UserId,
                FromUserName = sender.UserName,
                FromAvatarUrl = sender.AvatarUrl,
                ToUserId = groupName,
                Content = message,
                MessageType = "text",
                Timestamp = DateTime.UtcNow,
                IsGroup = true
            };

            await Clients.All.SendAsync("ReceiveGroupMessage", chatMessage);
        }

        /// <summary>
        /// Đánh dấu tin nhắn đã đọc
        /// </summary>
        public async Task MarkAsRead(string messageId, string fromUserId)
        {
            if (UserConnections.TryGetValue(fromUserId, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId).SendAsync("MessageRead", messageId);
            }
        }

        #endregion

        #region Typing Indicator

        public async Task StartTyping(string toUserId)
        {
            var senderEntry = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (string.IsNullOrEmpty(senderEntry.Key)) return;

            if (!OnlineUsers.TryGetValue(senderEntry.Key, out var sender)) return;

            if (UserConnections.TryGetValue(toUserId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("UserTyping", sender.UserId, sender.UserName);
            }
        }

        public async Task StopTyping(string toUserId)
        {
            var senderEntry = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (string.IsNullOrEmpty(senderEntry.Key)) return;

            if (UserConnections.TryGetValue(toUserId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("UserStoppedTyping", senderEntry.Key);
            }
        }

        #endregion
    }

    #region Models

    public class OnlineUser
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; }
    }

    public class ChatMessage
    {
        public string MessageId { get; set; } = string.Empty;
        public string FromUserId { get; set; } = string.Empty;
        public string FromUserName { get; set; } = string.Empty;
        public string? FromAvatarUrl { get; set; }
        public string ToUserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MessageType { get; set; } = "text";
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsGroup { get; set; }
    }

    #endregion
}
