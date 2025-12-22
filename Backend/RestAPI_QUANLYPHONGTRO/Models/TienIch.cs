using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("TienIch")]
    public class TienIch
    {
        [Key]
        public int TienIchId { get; set; } // Khóa chính kiểu int (thường là tự tăng)

        [Required]
        [MaxLength(200)]
        public string Ten { get; set; } // Tên tiện ích
    }
}
