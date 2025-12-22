using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Reports
{
    public class BaoCaoViPhamDto
    {
        public Guid BaoCaoId { get; set; }

        public string LoaiThucThe { get; set; } // "NguoiDung" / "Phong"...
        public Guid? ThucTheId { get; set; }

        public Guid NguoiBaoCao { get; set; }
        public int? ViPhamId { get; set; }

        public string TieuDe { get; set; }
        public string MoTa { get; set; }

        public string TrangThai { get; set; } // ChoXuLy, DaXuLy...
        public string KetQua { get; set; }

        public Guid? NguoiXuLy { get; set; }
        public DateTimeOffset ThoiGianBaoCao { get; set; }
        public DateTimeOffset? ThoiGianXuLy { get; set; }
    }
}