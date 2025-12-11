using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("ChuTroThongTinPhapLy")]
    public class ChuTroThongTinPhapLy
    {
        [Key]
        public Guid NguoiDungId { get; set; } // PK & FK từ bảng User

        [Required]
        [MaxLength(20)]
        public string CCCD { get; set; }

        public DateTime? NgayCapCCCD { get; set; } // Kiểu date

        [MaxLength(200)]
        public string? NoiCapCCCD { get; set; }

        [Required]
        [MaxLength(500)]
        public string DiaChiThuongTru { get; set; }

        [MaxLength(500)]
        public string? DiaChiLienHe { get; set; }

        [MaxLength(50)]
        public string? SoDienThoaiLienHe { get; set; }

        [MaxLength(50)]
        public string? MaSoThueCaNhan { get; set; }

        [MaxLength(50)]
        public string? SoTaiKhoanNganHang { get; set; }

        [MaxLength(200)]
        public string? TenNganHang { get; set; }

        [MaxLength(200)]
        public string? ChiNhanhNganHang { get; set; }

        public Guid? TapTinGiayToId { get; set; } // FK

        [Required]
        [MaxLength(50)]
        public string TrangThaiXacThuc { get; set; } // Ví dụ: "ChoDuyet", "DaDuyet"

        [MaxLength(1000)]
        public string? GhiChu { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
