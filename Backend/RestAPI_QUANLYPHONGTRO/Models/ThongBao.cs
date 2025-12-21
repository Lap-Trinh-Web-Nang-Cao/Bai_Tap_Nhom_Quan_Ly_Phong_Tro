using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("ThongBao")]
    public class ThongBao
    {
        [Key]
        public Guid ThongBaoId { get; set; }

        [Required]
        public Guid NguoiDungId { get; set; } // Người nhận thông báo

        [Required]
        [MaxLength(250)]
        public string TieuDe { get; set; }

        [Required]
        public string NoiDung { get; set; }

        [MaxLength(50)]
        public string Loai { get; set; } // "success", "info", "warning", "error"

        public bool DaXem { get; set; } = false;

        public DateTimeOffset ThoiGianTao { get; set; } = DateTimeOffset.Now;

        [MaxLength(200)]
        public string? RedirectUrl { get; set; } // Link để nhảy đến khi click vào thông báo (ví dụ: chi tiết đơn hàng)
    }
}
