using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
 [Table("PhongHinhAnh")]
 public class PhongHinhAnh
 {
 [Key]
 public Guid HinhAnhId { get; set; }

 [Required]
 public Guid PhongId { get; set; }

 [Required]
 [MaxLength(1000)]
 public string DuongDanAnh { get; set; }

 public bool LaThumbnail { get; set; } = false;

 public int ThuTu { get; set; } =0;

 public DateTimeOffset? CreatedAt { get; set; }

 // Navigation (optional)
 public virtual Phong Phong { get; set; }
 }
}
