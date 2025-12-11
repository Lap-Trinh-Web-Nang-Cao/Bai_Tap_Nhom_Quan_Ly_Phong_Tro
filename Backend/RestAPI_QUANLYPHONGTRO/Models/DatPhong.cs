using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("DatPhong")]
    public class DatPhong
    {
        [Key]
        public Guid DatPhongId { get; set; }

        [Required]
        public Guid PhongId { get; set; } // Phòng nào

        [Required]
        public Guid NguoiThueId { get; set; } // Ai thuê (Lấy từ Token)

        [Required]
        public Guid ChuTroId { get; set; } // Chủ của phòng đó

        [Required]
        [MaxLength(30)]
        public string Loai { get; set; } // Ví dụ: "TheoNgay", "TheoThang"

        [Required]
        public DateTimeOffset BatDau { get; set; }

        public DateTimeOffset? KetThuc { get; set; }

        public DateTimeOffset? ThoiGianTao { get; set; }

        [Required]
        public int TrangThaiId { get; set; } // 1: Chờ duyệt, 2: Đã duyệt, 3: Hủy...

        public Guid? TapTinBienLaiId { get; set; } // ID file ảnh chuyển khoản (nếu có)

        [MaxLength] // nvarchar(max)
        public string? GhiChu { get; set; }

        [Required]
        public int SoDatPhong { get; set; } // Có thể là mã tự tăng hoặc random để dễ đọc
    }
}
