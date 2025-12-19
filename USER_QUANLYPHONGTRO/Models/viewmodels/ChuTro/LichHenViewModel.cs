using System;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LichHenViewModel
    {
        // ID để xử lý nút Duyệt/Từ chối
        public Guid DatPhongId { get; set; }

        // Thông tin hiển thị ra bảng
        public string TenNguoiDat { get; set; }
        public string TenPhong { get; set; }
        public string DiaChiPhong { get; set; }
        public string DienThoai { get; set; }

        // Thời gian hẹn
        public DateTimeOffset NgayBatDau { get; set; }

        // Lời nhắn của khách
        public string GhiChu { get; set; }

        // Trạng thái
        public string TrangThai { get; set; }
    }
}