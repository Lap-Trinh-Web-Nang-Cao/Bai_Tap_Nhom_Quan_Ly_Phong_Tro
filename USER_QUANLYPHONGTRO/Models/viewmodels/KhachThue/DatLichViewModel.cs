using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class DatLichViewModel
    {
        // Thông tin hiển thị
        public Guid PhongId { get; set; }
        public Guid ChuTroId { get; set; } // <-- MỚI THÊM
        public string TieuDe { get; set; }
        public string AnhDaiDien { get; set; }
        public decimal GiaTien { get; set; }
        public string DiaChi { get; set; }

        // Thông tin nhập liệu
        [Required(ErrorMessage = "Vui lòng chọn ngày giờ hẹn")]
        [Display(Name = "Ngày giờ hẹn")]
        public DateTime NgayHen { get; set; } = DateTime.Now.AddDays(1);

        [Display(Name = "Lời nhắn")]
        public string GhiChu { get; set; }
    }
}