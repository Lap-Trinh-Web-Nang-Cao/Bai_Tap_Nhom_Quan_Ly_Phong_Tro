using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("Phong")]
    public class Phong
    {
        [Key]
        public Guid PhongId { get; set; }

        [Required]
        public Guid NhaTroId { get; set; } // FK tới bảng NhaTro

        [MaxLength(250)]
        public string? TieuDe { get; set; } // Ví dụ: Phòng 101, Phòng VIP...

        [Column(TypeName = "decimal(8,2)")] // Định dạng số thập phân trong SQL
        public decimal? DienTich { get; set; }

        [Required]
        public long GiaTien { get; set; }

        public long? TienCoc { get; set; }

        public int? SoNguoiToiDa { get; set; }

        [Required]
        [MaxLength(50)]
        public string TrangThai { get; set; } // "Trong", "DaThue", "DangSuaChua"

        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        // --- Các trường Cache (Tính toán từ bảng Đánh giá) ---
        public double? DiemTrungBinh { get; set; } // float -> double
        public int? SoLuongDanhGia { get; set; }

        // --- Các trường quản lý (Admin) ---
        [Required]
        public bool IsDuyet { get; set; } // Đã được admin duyệt chưa

        public Guid? NguoiDuyet { get; set; } // Admin ID

        public DateTimeOffset? ThoiGianDuyet { get; set; }

        [Required]
        public bool IsBiKhoa { get; set; } // Có bị khóa do vi phạm không

        public virtual NhaTro NhaTro { get; set; }


        public bool IsDeleted { get; set; } = false;
    }
}
