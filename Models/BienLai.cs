using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("BienLai")]
    public class BienLai
    {
        [Key]
        public Guid BienLaiId { get; set; }

        [Required]
        public Guid DatPhongId { get; set; } // Foreign Key

        [Required]
        public Guid NguoiTai { get; set; } // Foreign Key

        [Required]
        public Guid TapTinId { get; set; } // Foreign Key

        public long? SoTien { get; set; } // bigint -> long

        public DateTimeOffset? ThoiGianTai { get; set; }

        [Required]
        public bool DaXacNhan { get; set; } // bit -> bool

        public Guid? NguoiXacNhan { get; set; }

        [Required]
        public int SoBienLai { get; set; }
    }
}
