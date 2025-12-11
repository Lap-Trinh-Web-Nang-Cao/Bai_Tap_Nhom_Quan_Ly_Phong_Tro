using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Reviews;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class PhongTroDetailViewModel
    {
        public Guid PhongId { get; set; }
        public string TieuDe { get; set; }

        public decimal? DienTich { get; set; }
        public long GiaTien { get; set; }
        public long? TienCoc { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string TrangThai { get; set; }

        public double? DiemTrungBinh { get; set; }
        public int SoLuongDanhGia { get; set; }

        public string TenNhaTro { get; set; }
        public string DiaChiNhaTro { get; set; }

        public IEnumerable<TienIchDto> TienIchList { get; set; }
        public IEnumerable<DanhGiaPhongDto> DanhGiaList { get; set; }

        public bool CoTheDatLich { get; set; }
    }
}