using System;
using System.ComponentModel.DataAnnotations;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class YeuCauHoTroViewModel
    {
        public Guid HoTroId { get; set; } //

        public Guid PhongId { get; set; } //

        // Dùng để hiển thị tên phòng trên giao diện thay vì chỉ hiện ID
        public string TenPhong { get; set; }

        public Guid NguoiYeuCau { get; set; } // ID người gửi

        // Dùng để hiển thị tên người gửi trên giao diện
        public string TenNguoiYeuCau { get; set; }

        public int LoaiHoTroId { get; set; } //

        // Tên loại hỗ trợ (Sửa chữa, Điện nước, ...)
        public string TenLoaiHoTro { get; set; }

        [Required]
        public string TieuDe { get; set; } //

        public string MoTa { get; set; } //

        public string TrangThai { get; set; } //

        public DateTime ThoiGianTao { get; set; } //
    }
}