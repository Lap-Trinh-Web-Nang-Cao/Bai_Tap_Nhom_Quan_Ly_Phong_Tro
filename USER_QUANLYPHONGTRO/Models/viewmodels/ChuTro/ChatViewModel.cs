using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class ChatViewModel
    {
        public List<ConversationItem> Conversations { get; set; }
        public List<MessageItem> CurrentMessages { get; set; }
        public string SelectedUserName { get; set; }
    }

    public class ConversationItem
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string LastMessage { get; set; }
        public string Avatar { get; set; }
        public DateTime Time { get; set; }
        public int UnreadCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class MessageItem
    {
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public bool IsFromMe { get; set; }
    }
}