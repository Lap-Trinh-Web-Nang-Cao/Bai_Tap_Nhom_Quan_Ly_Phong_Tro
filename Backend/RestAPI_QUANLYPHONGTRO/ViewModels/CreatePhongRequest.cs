using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class CreatePhongRequest
    {
        [Required]
        public Guid NhaTroId { get; set; }

        [MaxLength(250)]
        public string? TieuDe { get; set; }

        public decimal? DienTich { get; set; }

        [Required]
        [Range(0, long.MaxValue, ErrorMessage = "Giá tiền không hợp lệ")]
        public long GiaTien { get; set; }

        public long? TienCoc { get; set; }

        public int? SoNguoiToiDa { get; set; }

        // Trạng thái mặc định khi tạo thường là "Trong" (Trống)
    }
}
