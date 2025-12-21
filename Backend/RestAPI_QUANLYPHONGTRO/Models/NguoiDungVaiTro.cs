using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("NguoiDungVaiTro")]
    public class NguoiDungVaiTro
    {
        // Vì là Composite Key nên ta sẽ cấu hình Key trong DbContext, 
        // ở đây chỉ cần đánh dấu ForeignKey

        [ForeignKey("NguoiDung")]
        public Guid NguoiDungId { get; set; }

        [ForeignKey("VaiTro")]
        public int VaiTroId { get; set; }

        [Required]
        public DateTimeOffset NgayBatDau { get; set; }

        public DateTimeOffset? NgayKetThuc { get; set; }

        [MaxLength(500)]
        public string? GhiChu { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }
        public virtual VaiTro VaiTro { get; set; }
    }
}
