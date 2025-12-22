using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Reviews
{
    public class DanhGiaPhongDto
    {
        public Guid DanhGiaId { get; set; }
        public Guid PhongId { get; set; }
        public Guid NguoiDanhGia { get; set; }

        public int Diem { get; set; } // 1..5
        public string NoiDung { get; set; }
        public DateTimeOffset ThoiGian { get; set; }

    }
}