using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LichHenViewModel
    {
        public DatPhongDto DatPhong { get; set; }

        public string TenKhachThue { get; set; }
        public string TenPhong { get; set; }
        public string DiaChiPhong { get; set; }

        public DateTimeOffset ThoiGianHen => DatPhong.BatDau;
    }

}