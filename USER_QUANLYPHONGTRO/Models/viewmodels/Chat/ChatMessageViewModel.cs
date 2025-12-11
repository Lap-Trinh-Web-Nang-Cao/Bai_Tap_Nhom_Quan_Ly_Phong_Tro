using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.Chat
{
    public class ChatMessageViewModel
    {
        public Guid TinNhanId { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }

        public string FromDisplayName { get; set; }
        public string NoiDung { get; set; }
        public DateTimeOffset ThoiGian { get; set; }

        public bool LaTinNhanCuaToi { get; set; } // để CSS canh phải/trái
    }
}