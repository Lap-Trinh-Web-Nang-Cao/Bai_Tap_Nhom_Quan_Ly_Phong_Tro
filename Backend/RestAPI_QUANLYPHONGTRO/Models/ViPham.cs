using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("ViPham")]
    public class ViPham
    {
        [Key]
        public int ViPhamId { get; set; } // Khóa chính tự tăng

        [Required]
        [MaxLength(200)]
        public string TenViPham { get; set; } // Tên lỗi

        [MaxLength(1000)]
        public string? MoTa { get; set; }

        public long? HinhPhatTien { get; set; } // Số tiền phạt mặc định (nếu có)

        public int? SoDiemTru { get; set; } // Điểm uy tín bị trừ (nếu có)
    }
}
