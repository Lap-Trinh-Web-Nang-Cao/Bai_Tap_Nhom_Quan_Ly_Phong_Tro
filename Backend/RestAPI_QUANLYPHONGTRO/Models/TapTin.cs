using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("TapTin")]
    public class TapTin
    {
        [Key]
        public Guid TapTinId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string DuongDan { get; set; } // URL để truy cập file (VD: /uploads/abc.jpg)

        [MaxLength(100)]
        public string? MimeType { get; set; } // VD: image/jpeg, application/pdf

        public Guid? TaiBangNguoi { get; set; } // ID người upload (UserId)

        public DateTimeOffset? ThoiGianTai { get; set; }
    }
}
