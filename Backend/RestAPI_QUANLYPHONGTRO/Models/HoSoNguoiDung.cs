using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("HoSoNguoiDung")]
    public class HoSoNguoiDung
    {
        [Key]
        [ForeignKey("NguoiDung")] // Đánh dấu đây là FK trỏ về bảng NguoiDung
        public Guid NguoiDungId { get; set; }

        [MaxLength(200)]
        public string? HoTen { get; set; }

        public DateTime? NgaySinh { get; set; } // SQL date -> DateTime?

        [MaxLength(100)]
        public string? LoaiGiayTo { get; set; } // Ví dụ: "CCCD", "HoChieu"

        [MaxLength(1000)]
        public string? GhiChu { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        // Navigation Property (Tuỳ chọn, để join bảng dễ hơn)
        // public virtual NguoiDung NguoiDung { get; set; }
    }
}
