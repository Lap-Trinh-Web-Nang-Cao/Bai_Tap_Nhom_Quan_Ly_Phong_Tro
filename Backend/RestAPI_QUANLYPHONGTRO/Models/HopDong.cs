using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
 [Table("HopDong")]
 public class HopDong
 {
 [Key]
 public Guid HopDongId { get; set; }

 [Required]
 public Guid DatPhongId { get; set; }

 [Required]
 public Guid PhongId { get; set; }

 [Required]
 public Guid ChuTroId { get; set; }

 [Required]
 public Guid NguoiThueId { get; set; }

 [Required]
 public DateTime NgayBatDau { get; set; }

 public DateTime? NgayKetThuc { get; set; }

 [Required]
 public long TienThue { get; set; }

 public long? TienCoc { get; set; }

 [MaxLength(50)]
 public string TrangThai { get; set; } = "ConHieuLuc";

 public DateTimeOffset CreatedAt { get; set; }
 }
}
