using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("HanhDongAdmin")]
    public class HanhDongAdmin
    {
        [Key]
        public long HanhDongId { get; set; } // bigint -> long

        [Required]
        public Guid AdminId { get; set; } // Người thực hiện

        [Required]
        [MaxLength(200)]
        public string HanhDong { get; set; } // Ví dụ: "KHOA_TAI_KHOAN"

        [MaxLength(200)]
        public string? MucTieuBang { get; set; } // Ví dụ: "NguoiDung"

        [MaxLength(200)]
        public string? BanGhiId { get; set; } // ID của đối tượng bị tác động (Lưu string vì có thể là Guid hoặc Int)

        public string? ChiTiet { get; set; } // JSON hoặc text mô tả thay đổi

        public DateTimeOffset? ThoiGian { get; set; }
    }
}
