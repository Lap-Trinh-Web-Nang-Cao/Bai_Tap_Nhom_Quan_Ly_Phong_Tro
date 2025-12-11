using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Bookings
{
    public class DatPhongDto
    {
        public Guid DatPhongId { get; set; }
        public Guid PhongId { get; set; }
        public Guid NguoiThueId { get; set; }
        public Guid ChuTroId { get; set; }

        public string Loai { get; set; }  // có thể là "XemPhong" / "ThueThang"...
        public DateTimeOffset BatDau { get; set; }
        public DateTimeOffset? KetThuc { get; set; }
        public DateTimeOffset ThoiGianTao { get; set; }

        public int TrangThaiId { get; set; }
        public string TenTrangThai { get; set; }

        public Guid? TapTinBienLaiId { get; set; }
        public string GhiChu { get; set; }
    }
}