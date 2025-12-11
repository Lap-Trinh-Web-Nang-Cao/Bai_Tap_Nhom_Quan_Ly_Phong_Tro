using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        public Guid NguoiDungId { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; } // Dùng làm tên đăng nhập

        [MaxLength(50)]
        public string? DienThoai { get; set; }

        [MaxLength(512)]
        public string? PasswordHash { get; set; } // Lưu chuỗi đã mã hóa

        [Required]
        public int VaiTroId { get; set; } // 1: Admin, 2: Chu tro, 3: Nguoi thue

        [Required]
        public bool IsKhoa { get; set; }

        [Required]
        public bool IsEmailXacThuc { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
