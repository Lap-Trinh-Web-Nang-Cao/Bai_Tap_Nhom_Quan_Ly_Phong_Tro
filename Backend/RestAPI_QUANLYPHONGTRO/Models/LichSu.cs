using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("LichSu")]
    public class LichSu
    {
        [Key]
        public long LichSuId { get; set; } // bigint -> long

        public Guid? NguoiDungId { get; set; } // Người thực hiện (Có thể null nếu là Guest)

        [Required]
        [MaxLength(200)]
        public string HanhDong { get; set; } // Ví dụ: "XEM_PHONG", "TIM_KIEM"

        [MaxLength(200)]
        public string? TenBang { get; set; } // Ví dụ: "PhongTro"

        [MaxLength(200)]
        public string? BanGhiId { get; set; } // ID của phòng trọ hoặc đối tượng tương tác

        public string? ChiTiet { get; set; } // JSON mô tả thêm (nếu cần)

        public DateTimeOffset? ThoiGian { get; set; }
    }
}
