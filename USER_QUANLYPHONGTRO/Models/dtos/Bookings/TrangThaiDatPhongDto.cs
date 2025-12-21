using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Bookings
{
    public class TrangThaiDatPhongDto
    {
        public int TrangThaiId { get; set; }
        public string TenTrangThai { get; set; } // ChoXacNhan, DaXacNhan, ...
    }
}