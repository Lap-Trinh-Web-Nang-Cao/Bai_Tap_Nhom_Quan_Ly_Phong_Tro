using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Chat
{
    public class TinNhanDto
    {
        public Guid TinNhanId { get; set; }
        public Guid FromUser { get; set; }
        public Guid ToUser { get; set; }

        public string NoiDung { get; set; }
        public Guid? TapTinId { get; set; }
        public DateTimeOffset ThoiGian { get; set; }
        public bool DaDoc { get; set; }
    }
}