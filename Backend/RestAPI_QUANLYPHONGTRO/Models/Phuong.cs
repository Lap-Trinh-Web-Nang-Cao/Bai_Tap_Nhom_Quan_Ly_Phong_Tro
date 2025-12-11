using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("Phuong")]
    public class Phuong
    {
        [Key]
        public int PhuongId { get; set; }

        [Required]
        public int QuanHuyenId { get; set; } // FK trỏ tới QuanHuyen

        [Required]
        [MaxLength(200)]
        public string Ten { get; set; }

        // Navigation Property (Tuỳ chọn: Để tiện truy xuất thông tin quận cha)
        // [ForeignKey("QuanHuyenId")]
        // public virtual QuanHuyen QuanHuyen { get; set; }
    }
}
