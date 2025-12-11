using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Support
{
    public class YeuCauHoTroDto
    {
        public Guid HoTroId { get; set; }
        public Guid? PhongId { get; set; }
        public Guid? NguoiYeuCau { get; set; }

        public int LoaiHoTroId { get; set; }
        public string TenLoaiHoTro { get; set; }

        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public string TrangThai { get; set; }  // Moi, DangXuLy, HoanThanh...

        public DateTimeOffset ThoiGianTao { get; set; }
    }
}