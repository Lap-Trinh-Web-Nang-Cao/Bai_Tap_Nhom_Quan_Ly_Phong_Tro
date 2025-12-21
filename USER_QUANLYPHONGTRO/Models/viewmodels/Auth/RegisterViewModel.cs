using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        // ===== THÔNG TIN CƠ BẢN =====
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(500)]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        // ===== THÔNG TIN CHỦ TRỌ =====
        public string UserType { get; set; } // "KhachThue" hoặc "ChuTro"

        public string LoaiGiayTo { get; set; }
        public string SoChungMinh { get; set; }
        public DateTime? NgayCapCCCD { get; set; }
        public string NoiCapCCCD { get; set; }
        public string DiaChiThuongTru { get; set; }
        public string MaSoThue { get; set; }

        // Thông tin ngân hàng
        public string SoTaiKhoan { get; set; }
        public string TenNganHang { get; set; }
        public string ChiNhanh { get; set; }

        // Upload files
        public HttpPostedFileBase[] GiayToFiles { get; set; }

        // Điều khoản
        [Display(Name = "Tôi đồng ý với điều khoản dịch vụ")]
        public bool AgreeTerms { get; set; }

        [Display(Name = "Tôi cam kết thông tin cung cấp là đúng sự thật")]
        public bool AgreeVerification { get; set; }
    }
}
