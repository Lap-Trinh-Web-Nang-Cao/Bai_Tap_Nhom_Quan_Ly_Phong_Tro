using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.Chat
{
    /// <summary>
    /// ViewModel cho trang Chat
    /// </summary>
    public class ChatPageViewModel
    {
        public string CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public string CurrentUserRole { get; set; } // "ChuTro" hoặc "KhachThue"
        public string CurrentUserAvatar { get; set; }
        public string SignalRHubUrl { get; set; }
        
        public List<ContactViewModel> Contacts { get; set; }
        
        public ChatPageViewModel()
        {
            Contacts = new List<ContactViewModel>();
        }
    }

    /// <summary>
    /// ViewModel cho liên hệ
    /// </summary>
    public class ContactViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; } // "ChuTro" hoặc "KhachThue"
        public string AvatarUrl { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsOnline { get; set; }
        
        public string RoleDisplay
        {
            get
            {
                if (Role == "ChuTro") return "Chủ trọ";
                if (Role == "KhachThue") return "Khách thuê";
                return Role;
            }
        }
        
        public string LastMessageTimeDisplay
        {
            get
            {
                var diff = DateTime.Now - LastMessageTime;
                if (diff.TotalMinutes < 1) return "Vừa xong";
                if (diff.TotalMinutes < 60) return string.Format("{0} phút trước", (int)diff.TotalMinutes);
                if (diff.TotalHours < 24) return string.Format("{0} giờ trước", (int)diff.TotalHours);
                if (diff.TotalDays < 7) return string.Format("{0} ngày trước", (int)diff.TotalDays);
                return LastMessageTime.ToString("dd/MM/yyyy");
            }
        }
    }

    /// <summary>
    /// ViewModel cho tin nhắn
    /// </summary>
    public class MessageViewModel
    {
        public string MessageId { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; }
        public string FromAvatarUrl { get; set; }
        public string ToUserId { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; } // "text", "image", "file"
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsFromMe { get; set; }
        
        public string TimeDisplay
        {
            get
            {
                return Timestamp.ToString("HH:mm");
            }
        }
        
        public string DateDisplay
        {
            get
            {
                var today = DateTime.Today;
                if (Timestamp.Date == today)
                    return "Hôm nay";
                if (Timestamp.Date == today.AddDays(-1))
                    return "Hôm qua";
                return Timestamp.ToString("dd/MM/yyyy");
            }
        }
    }
}
