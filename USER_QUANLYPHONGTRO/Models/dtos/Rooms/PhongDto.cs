using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Rooms
{
    public class PhongDto
    {
        public Guid PhongId { get; set; }
        public Guid NhaTroId { get; set; }

        public string TieuDe { get; set; }
        public decimal? DienTich { get; set; }
        public long GiaTien { get; set; }
        public long? TienCoc { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string TrangThai { get; set; } // "con_trong", ...

        public double? DiemTrungBinh { get; set; }
        public int SoLuongDanhGia { get; set; }

        public bool IsDuyet { get; set; }
        public bool IsBiKhoa { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        // Thông tin liên quan
        public NhaTroDto NhaTro { get; set; }
        public IEnumerable<TienIchDto> TienIchList { get; set; }
    }
}