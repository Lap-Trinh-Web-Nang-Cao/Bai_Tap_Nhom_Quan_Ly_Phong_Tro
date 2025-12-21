using System.ComponentModel.DataAnnotations;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
