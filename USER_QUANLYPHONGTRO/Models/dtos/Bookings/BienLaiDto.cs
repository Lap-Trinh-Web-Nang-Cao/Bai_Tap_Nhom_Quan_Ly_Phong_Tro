using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Bookings
{
    public class BienLaiDto
    {
        public Guid BienLaiId { get; set; }
        public Guid DatPhongId { get; set; }
        public Guid NguoiTai { get; set; }
        public Guid TapTinId { get; set; }

        public long? SoTien { get; set; }
        public DateTimeOffset ThoiGianTai { get; set; }
        public bool DaXacNhan { get; set; }
        public Guid? NguoiXacNhan { get; set; }
    }
}