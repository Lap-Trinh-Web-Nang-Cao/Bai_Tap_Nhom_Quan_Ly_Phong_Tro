using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Rooms
{
    public class NhaTroDto
    {
        public Guid NhaTroId { get; set; }
        public Guid ChuTroId { get; set; }

        public string TieuDe { get; set; }
        public string DiaChi { get; set; }
        public int? QuanHuyenId { get; set; }
        public int? PhuongId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public bool IsHoatDong { get; set; }
    }
}