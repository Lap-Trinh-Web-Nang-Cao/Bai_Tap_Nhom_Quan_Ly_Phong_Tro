using System;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class DanhSachPhongViewModel
    {
        public Guid PhongId { get; set; }
        public string TieuDe { get; set; }
        public string DiaChi { get; set; }
        public decimal GiaTien { get; set; }
        public double? DienTich { get; set; }
        public string TrangThai { get; set; } // "con_trong", "da_thue"
        public bool IsDuyet { get; set; }
        public string HinhAnh { get; set; } // Đường dẫn ảnh đại diện
    }
}