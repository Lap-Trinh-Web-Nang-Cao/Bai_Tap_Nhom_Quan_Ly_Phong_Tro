using System;
using System.ComponentModel.DataAnnotations;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class ChiTietHopDongViewModel
    {
        public Guid HopDongId { get; set; }

        // 1. Thông tin Phòng
        [Display(Name = "Tên phòng")]
        public string TenPhong { get; set; }

        [Display(Name = "Địa chỉ phòng")]
        public string DiaChiPhong { get; set; }

        // 2. Thông tin Khách thuê
        [Display(Name = "Họ tên khách")]
        public string TenKhach { get; set; }

        [Display(Name = "Số điện thoại")]
        public string DienThoai { get; set; }

        [Display(Name = "CCCD / CMND")]
        public string CCCD { get; set; }

        [Display(Name = "Địa chỉ thường trú")]
        public string DiaChiThuongTru { get; set; }

        // 3. Chi tiết thuê
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime? NgayKetThuc { get; set; }

        [Display(Name = "Giá thuê (VNĐ/tháng)")]
        public decimal GiaThue { get; set; }

        [Display(Name = "Tiền cọc (VNĐ)")]
        public decimal TienCoc { get; set; }

        [Display(Name = "Điều khoản")]
        public string DieuKhoan { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } // "HieuLuc", "HetHan", "DaHuy"
    }
}