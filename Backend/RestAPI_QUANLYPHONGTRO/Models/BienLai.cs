using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("BienLai")]
    public class BienLai
    {
        [Key]
        public Guid BienLaiId { get; set; }

        public Guid? DatPhongId { get; set; } // Foreign Key - nullable to handle NULL in DB

        public Guid? NguoiTai { get; set; } // Foreign Key - nullable to handle NULL in DB

        public Guid? TapTinId { get; set; } // Foreign Key - nullable to handle NULL in DB

        public long? SoTien { get; set; } // bigint -> long

        public DateTimeOffset? ThoiGianTai { get; set; }

        public bool DaXacNhan { get; set; } // bit -> bool, default false

        // DB script uses NVARCHAR(100) NULL for SoBienLai in SQL, so use string to avoid SqlNullValueException
        public string? SoBienLai { get; set; }
    }
}
