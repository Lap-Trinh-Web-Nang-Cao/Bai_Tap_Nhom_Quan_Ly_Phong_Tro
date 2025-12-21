using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("BaoCaoViPham")]
    public class BaoCaoViPham
    {
        [Key]
        public Guid BaoCaoId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LoaiThucThe { get; set; }

        public Guid? ThucTheId { get; set; }

        [Required]
        public Guid NguoiBaoCao { get; set; } // Foreign Key

        public int? ViPhamId { get; set; } // Foreign Key

        [Required]
        [MaxLength(300)]
        public string TieuDe { get; set; }

        public string? MoTa { get; set; } // nvarchar(max)

        [Required]
        [MaxLength(50)]
        public string TrangThai { get; set; }

        [MaxLength(1000)]
        public string? KetQua { get; set; }

        public Guid? NguoiXuLy { get; set; }

        public DateTimeOffset? ThoiGianBaoCao { get; set; }

        public DateTimeOffset? ThoiGianXuLy { get; set; }

        [Required]
        public int SoBaoCao { get; set; }


    }
}
