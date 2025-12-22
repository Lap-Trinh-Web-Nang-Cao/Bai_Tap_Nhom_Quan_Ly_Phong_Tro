using System;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class BookingDetailDto
    {
        public Guid DatPhongId { get; set; }
        public Guid PhongId { get; set; }
        public string TieuDePhong { get; set; }
        public string DiaChi { get; set; }
        public string Loai { get; set; }
        public DateTimeOffset BatDau { get; set; }
        public DateTimeOffset? KetThuc { get; set; }
        public DateTimeOffset ThoiGianTao { get; set; }
        public int TrangThaiId { get; set; }
        public string TenTrangThai { get; set; }
        public string SdtChuTro { get; set; }
        public string GhiChu { get; set; }
        public string HoTenNguoiThue { get; set; }
        public string SdtNguoiThue { get; set; }
    }
}
