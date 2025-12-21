using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class DonDatPhongViewModel
    {
        public Guid DatPhongId { get; set; }
        public int SoDatPhong { get; set; } // Lấy từ cột SoDatPhong tự tăng
        public string TenKhachHang { get; set; }
        public string TenPhong { get; set; }
        public string LoaiDatPhong { get; set; } // Ví dụ: Giữ chỗ, Thuê ngay
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string TrangThai { get; set; } // ChoXacNhan, DaXacNhan, DaThanhToan...
        public long GiaTien { get; set; }
        public DateTime ThoiGianTao { get; set; }
    }
}