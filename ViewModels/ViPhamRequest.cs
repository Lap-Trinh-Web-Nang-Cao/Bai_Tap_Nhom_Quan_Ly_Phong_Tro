using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class ViPhamRequest
    {
        [Required(ErrorMessage = "Tên vi phạm không được để trống")]
        [MaxLength(200)]
        public string TenViPham { get; set; }

        [MaxLength(1000)]
        public string? MoTa { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "Tiền phạt không hợp lệ")]
        public long? HinhPhatTien { get; set; }

        public int? SoDiemTru { get; set; }
    }
}
