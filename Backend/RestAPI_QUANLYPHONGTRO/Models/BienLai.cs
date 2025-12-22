using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("BienLai")]
    public class BienLai
    {
        [Key]
        public Guid BienLaiId { get; set; }

        public Guid? DatPhongId { get; set; } // Foreign Key - cho phép NULL

        public Guid? NguoiTai { get; set; } // Foreign Key - cho phép NULL

        public Guid? TapTinId { get; set; } // Foreign Key - cho phép NULL

        public long? SoTien { get; set; }

        public DateTimeOffset? ThoiGianTai { get; set; }

        public bool? DaXacNhan { get; set; } // bit -> bool nullable

        public Guid? NguoiXacNhan { get; set; }

        public string? SoBienLai { get; set; } // Thay int thành string nullable
    }
}
