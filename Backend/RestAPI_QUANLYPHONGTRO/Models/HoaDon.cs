using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
 [Table("HoaDon")]
 public class HoaDon
 {
 [Key]
 public Guid HoaDonId { get; set; }

 [Required]
 public Guid HopDongId { get; set; }

 [Required]
 public int Thang { get; set; }

 [Required]
 public int Nam { get; set; }

 [Required]
 public long TienPhong { get; set; }

 public long? TienDien { get; set; }
 public long? TienNuoc { get; set; }
 public long? TienDichVu { get; set; }

 [Required]
 public long TongTien { get; set; }

 [MaxLength(50)]
 public string TrangThai { get; set; } = "ChuaThanhToan";

 public DateTimeOffset NgayLap { get; set; }
 public DateTimeOffset? NgayThanhToan { get; set; }
 }
}
