using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class NhaTroRequest
    {
        [Required(ErrorMessage = "Tên nhà trọ không được để trống")]
        [MaxLength(300)]
        public string TieuDe { get; set; }

        [MaxLength(500)]
        public string? DiaChi { get; set; }

        public int? QuanHuyenId { get; set; }

        public int? PhuongId { get; set; }

        // Mặc định khi tạo mới thường là đang hoạt động, user có thể gửi true/false
        public bool? IsHoatDong { get; set; }
    }
}
