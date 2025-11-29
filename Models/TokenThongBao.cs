using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("TokenThongBao")]
    public class TokenThongBao
    {
        [Key]
        public Guid TokenId { get; set; }

        [Required]
        public Guid NguoiDungId { get; set; } // Người sở hữu thiết bị

        [Required]
        [MaxLength(1000)]
        public string Token { get; set; } // Mã FCM/APNS từ thiết bị gửi lên

        public DateTimeOffset? ThoiGianTao { get; set; }

        [Required]
        public bool IsActive { get; set; } // True: Đang nhận thông báo, False: Đã tắt/Logout
    }
}
