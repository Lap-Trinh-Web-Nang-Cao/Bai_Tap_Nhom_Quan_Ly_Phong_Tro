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

        public decimal? DienTich { get; set; }
        public long GiaTien { get; set; }

        public double? DiemTrungBinh { get; set; }
        public int SoLuongDanhGia { get; set; }

        public bool IsDuyet { get; set; }
        public bool IsBiKhoa { get; set; }

        // Hiển thị vài tiện ích ngắn gọn
        public string[] TienIchNganGon { get; set; }
    }
}