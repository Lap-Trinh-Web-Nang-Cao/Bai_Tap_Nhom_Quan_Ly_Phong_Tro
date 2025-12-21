using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.Chat
{
    public class ChatRoomViewModel
    {

        public Guid CurrentUserId { get; set; }
        public Guid PartnerUserId { get; set; }

        public string PartnerName { get; set; }
        public string PartnerRole { get; set; } // ChuTro / KhachThue

        public IEnumerable<ChatMessageViewModel> Messages { get; set; }
    }
}