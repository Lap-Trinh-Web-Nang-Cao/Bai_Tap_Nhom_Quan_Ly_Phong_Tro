using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("PhongTienIch")]
    public class PhongTienIch
    {
        [Key]
        public int PId { get; set; } // Khóa chính tự tăng

        [Required]
        public Guid PhongId { get; set; } // FK tới Phong

        [Required]
        public int TienIchId { get; set; } // FK tới TienIch

        // Navigation properties (Tuỳ chọn để join bảng)
        // public virtual Phong Phong { get; set; }
        // public virtual TienIch TienIch { get; set; }
    }
}
