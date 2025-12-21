using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class ChiTietHopDongViewModel
    {
        // Thông tin chung
        public Guid HopDongId { get; set; }
        public string SoHopDong { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayLap { get; set; }

        // Thông tin người thuê
        public string TenNguoiThue { get; set; }
        public string SoDienThoai { get; set; }
        public string CCCD { get; set; }
        public string QueQuan { get; set; }

        // Thông tin phòng
        public string TenPhong { get; set; }
        public double DienTich { get; set; }
        public decimal GiaThue { get; set; }
        public decimal TienCoc { get; set; }

        // Thời hạn và điều khoản
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int KyThanhToan { get; set; } // Ví dụ: 1 tháng/lần
        public string GhiChu { get; set; }
    }
}