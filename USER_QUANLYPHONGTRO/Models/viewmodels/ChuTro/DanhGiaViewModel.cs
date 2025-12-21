using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class DanhGiaViewModel
    {
        public Guid DanhGiaId { get; set; } //

        public Guid PhongId { get; set; } //
        public string TenPhong { get; set; } // Dùng để hiển thị tên thay vì ID

        public Guid NguoiDanhGia { get; set; } //
        public string TenNguoiDanhGia { get; set; } // Dùng để hiển thị tên thay vì ID

        public int Diem { get; set; } // Số sao (1-5)

        public string NoiDung { get; set; } //

        public DateTime ThoiGian { get; set; } //
    }

    public class ThongKeDanhGiaViewModel
    {
        public double DiemTrungBinh { get; set; }
        public int TongLuotDanhGia { get; set; }
        public List<DanhGiaViewModel> DanhSachDanhGia { get; set; }
    }
}