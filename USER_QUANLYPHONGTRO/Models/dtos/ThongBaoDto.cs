using System;

namespace USER_QUANLYPHONGTRO.Models.Dtos
{
    public class ThongBaoDto
    {
        public Guid ThongBaoId { get; set; }
        public Guid NguoiDungId { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string Loai { get; set; } // success, info, warning, error
        public bool DaXem { get; set; }
        public DateTimeOffset ThoiGianTao { get; set; }
        public string RedirectUrl { get; set; }
    }
}
