using System;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordHoaDonViewModel
    {
        // 1. Các trường map trực tiếp từ Database (Bảng BienLai)
        public Guid BienLaiId { get; set; }
        public string SoBienLai { get; set; } // Cột bạn mới thêm trong ảnh
        public long? SoTien { get; set; }
        public bool DaXacNhan { get; set; }
        public DateTime? ThoiGianTai { get; set; } // Để hiển thị ngày nộp

        // 2. Các trường hiển thị (Lấy từ việc Join bảng DatPhong, Phong, NguoiDung, TapTin)
        public string TenPhong { get; set; }      // Từ DatPhong -> Phong
        public string NguoiTai { get; set; }      // Từ NguoiDung (HoTen)
        public string DuongDanAnh { get; set; }   // Từ TapTin (DuongDan)
    }
}