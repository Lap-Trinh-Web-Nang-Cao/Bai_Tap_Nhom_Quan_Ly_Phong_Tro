using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class TinNhanDto
    {
        public Guid TinNhanId { get; set; }
        public Guid GuiTuId { get; set; }
        public Guid GuiDenId { get; set; }
        public string NoiDung { get; set; }
        public bool DaDoc { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
