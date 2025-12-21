using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    /// <summary>
    /// Request để Admin tạo người dùng mới
    /// </summary>
    public class AdminCreateUserRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự")]
        public string Password { get; set; }

        public string? DienThoai { get; set; }

        public string? HoTen { get; set; }

        /// <summary>
        /// Vai trò: 1 = Admin, 2 = Chủ trọ, 3 = Người thuê
        /// </summary>
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [Range(1, 3, ErrorMessage = "Vai trò phải là 1 (Admin), 2 (Chủ trọ), hoặc 3 (Người thuê)")]
        public int VaiTroId { get; set; } = 3;

        /// <summary>
        /// Trạng thái email đã xác thực (Admin có thể set true ngay)
        /// </summary>
        public bool IsEmailXacThuc { get; set; } = false;
    }
}
