using System;
using System.Collections.Generic;
using USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class QuanLyPhongViewModel
    {
        public Guid PhongId { get; set; }
        public string TenPhong { get; set; }
        public string TenNhaTro { get; set; }
        public decimal GiaTien { get; set; }
        public double DienTich { get; set; }
        public string TrangThai { get; set; } // Còn trống, Đã thuê...
        public string AnhDaiDien { get; set; }
        public bool IsDuyet { get; set; } // Đã được Admin duyệt chưa
    }
}