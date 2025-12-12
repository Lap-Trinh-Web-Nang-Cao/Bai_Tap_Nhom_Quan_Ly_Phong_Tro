using System;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class ChiTietHopDongViewModel
    {
        public Guid DatPhongId { get; set; }

        // --- 1. Thông tin bên cho thuê (Lấy từ bảng NguoiDung + HoSoNguoiDung của ChuTroId) ---
        public string TenChuTro { get; set; }
        public string SdtChuTro { get; set; }

        // --- 2. Thông tin bên thuê (Lấy từ bảng NguoiDung + HoSoNguoiDung của NguoiThueId) ---
        public string TenNguoiThue { get; set; }
        public string CCCD { get; set; }        // Lấy từ cột LoaiGiayTo trong bảng HoSoNguoiDung
        public string SdtNguoiThue { get; set; }

        // --- 3. Thông tin phòng & Hợp đồng (Lấy từ bảng Phong, NhaTro, DatPhong) ---
        public string TenPhong { get; set; }    // TieuDe của Phong
        public string DiaChi { get; set; }      // DiaChi của NhaTro
        public decimal GiaTien { get; set; }    // GiaTien của Phong
        public DateTimeOffset NgayBatDau { get; set; } // BatDau của DatPhong
        public DateTimeOffset? NgayKetThuc { get; set; } // KetThuc của DatPhong (nếu có)
    }
}