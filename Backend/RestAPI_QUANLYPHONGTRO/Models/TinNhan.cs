using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("TinNhan")]
    public class TinNhan
    {
        [Key]
        public Guid TinNhanId { get; set; }

        [Required]
        public Guid FromUser { get; set; } // Người gửi

        [Required]
        public Guid ToUser { get; set; } // Người nhận

        [MaxLength] // nvarchar(max)
        public string? NoiDung { get; set; }

        public Guid? TapTinId { get; set; } // File đính kèm (nếu có)

        public DateTimeOffset? ThoiGian { get; set; }

        [Required]
        public bool DaDoc { get; set; } // Trạng thái đã xem

        // Navigation Properties (Để join lấy tên người gửi/nhận nếu cần)
        // public virtual NguoiDung Sender { get; set; }
        // public virtual NguoiDung Receiver { get; set; }
    }
}
