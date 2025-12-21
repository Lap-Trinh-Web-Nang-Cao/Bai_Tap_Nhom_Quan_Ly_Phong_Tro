using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordHopDongViewModel
    {
        public List<HopDongItemViewModel> DanhSachHopDong { get; set; }
        public int TongHopDongHieuLuc { get; set; }
        public int HopDongSapHetHan { get; set; }
    }

    public class HopDongItemViewModel
    {
        public Guid HopDongId { get; set; }
        public string SoHopDong { get; set; }
        public string TenPhong { get; set; }
        public string TenNguoiThue { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public decimal GiaThue { get; set; }
        public decimal TienCoc { get; set; }
        public string TrangThai { get; set; } // DangHieuLuc, DaHetHan, SapHetHan
    }
}