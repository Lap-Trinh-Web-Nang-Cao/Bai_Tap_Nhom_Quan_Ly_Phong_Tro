using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class PhongTroListItemViewModel
    {
        public Guid PhongId { get; set; }
        public string TieuDe { get; set; }
        public string TenNhaTro { get; set; }
        public string DiaChi { get; set; }
        public double DienTich { get; set; }
        public decimal GiaTien { get; set; }
        public double DiemTrungBinh { get; set; }
        public int SoLuongDanhGia { get; set; }
        public string[] TienIchNganGon { get; set; }

        // --- MỚI THÊM ---
        public string AnhDaiDien { get; set; }
    }
}