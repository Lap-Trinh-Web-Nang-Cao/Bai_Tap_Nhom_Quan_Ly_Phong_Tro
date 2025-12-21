using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class ChangePasswordRequest
    {
        [Required]
        public string MatKhauCu { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải từ 6 ký tự")]
        public string MatKhauMoi { get; set; }

        [Compare("MatKhauMoi", ErrorMessage = "Nhập lại mật khẩu không khớp")]
        public string XacNhanMatKhau { get; set; }
    }
}
