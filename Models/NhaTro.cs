using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("NhaTro")]
    public class NhaTro
    {
        [Key]
        public Guid NhaTroId { get; set; }

        [Required]
        public Guid ChuTroId { get; set; } // Người sở hữu nhà trọ này (Lấy từ Token)

        [Required]
        [MaxLength(300)]
        public string TieuDe { get; set; } // Tên nhà trọ (VD: Trọ cô Bảy)

        [MaxLength(500)]
        public string? DiaChi { get; set; } // Địa chỉ cụ thể (Số nhà, đường...)

        public int? QuanHuyenId { get; set; } // ID Quận/Huyện (Liên kết bảng danh mục địa chính)

        public int? PhuongId { get; set; } // ID Phường/Xã

        public DateTimeOffset? CreatedAt { get; set; }

        public bool? IsHoatDong { get; set; } // Trạng thái: Đang cho thuê hoặc tạm đóng
    }
}
