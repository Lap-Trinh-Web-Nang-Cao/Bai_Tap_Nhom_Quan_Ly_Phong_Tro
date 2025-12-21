using System;
using System.ComponentModel.DataAnnotations;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LichHenViewModel
    {
        public Guid LichHenId { get; set; }

        [Display(Name = "Khách hàng")]
        public string TenKhachHang { get; set; }

        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Phòng")]
        public string TenPhong { get; set; }

        [Display(Name = "Thời gian xem")]
        public DateTime NgayXem { get; set; }
        public string GioXem { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } // ChoXacNhan, DaXacNhan, HoanThanh, DaHuy

        public string GhiChu { get; set; }
    }
}