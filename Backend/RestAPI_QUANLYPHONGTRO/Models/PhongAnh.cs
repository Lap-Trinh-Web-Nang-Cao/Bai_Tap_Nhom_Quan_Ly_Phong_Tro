using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
 [Table("PhongAnh")]
 public class PhongAnh
 {
 [Key]
 public Guid PhongAnhId { get; set; }

 [Required]
 public Guid PhongId { get; set; }

 [Required]
 public Guid TapTinId { get; set; }

 public int ThuTu { get; set; } =1;

 // Navigation (optional)
 public virtual Phong Phong { get; set; }
 public virtual TapTin TapTin { get; set; }
 }
}
