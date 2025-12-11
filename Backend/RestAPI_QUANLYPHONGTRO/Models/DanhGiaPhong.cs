using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("DanhGiaPhong")]
    public class DanhGiaPhong
    {
        [Key]
        public Guid DanhGiaId { get; set; }

        [Required]
        public Guid PhongId { get; set; } // FK tới bảng Phong

        [Required]
        public Guid NguoiDanhGia { get; set; } // FK tới bảng NguoiDung (User ID)

        [Required]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5.")]
        public int Diem { get; set; }

        [MaxLength(1000)]
        public string? NoiDung { get; set; }

        public DateTimeOffset? ThoiGian { get; set; }
    }
}
