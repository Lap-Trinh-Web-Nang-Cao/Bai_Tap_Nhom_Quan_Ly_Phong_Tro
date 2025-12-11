using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("YeuCauHoTro")]
    public class YeuCauHoTro
    {
        [Key]
        public Guid HoTroId { get; set; }

        [Required]
        public Guid PhongId { get; set; } // Phòng xảy ra sự cố

        [Required]
        public int LoaiHoTroId { get; set; } // Loại (Điện, Nước...)

        [Required]
        [MaxLength(300)]
        public string TieuDe { get; set; }

        public string? MoTa { get; set; } // nvarchar(max)

        [Required]
        [MaxLength(50)]
        public string TrangThai { get; set; } // Mặc định: "Moi"

        public DateTimeOffset? ThoiGianTao { get; set; }

        [Required]
        public Guid NguoiYeuCau { get; set; } // Người gửi yêu cầu (Người thuê)
    }
}
