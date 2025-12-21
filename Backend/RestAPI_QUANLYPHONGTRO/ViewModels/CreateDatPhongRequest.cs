using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class CreateDatPhongRequest
    {
        [Required]
        public Guid PhongId { get; set; }

        // Trong thực tế, Backend nên tự tìm ID chủ trọ dựa vào PhongId
        // Nhưng ở đây tạm thời cho Client gửi lên, hoặc bạn query từ bảng Phong
        [Required]
        public Guid ChuTroId { get; set; }

        [Required]
        public string Loai { get; set; } // "Ngay" hoặc "Thang"

        [Required]
        public DateTimeOffset BatDau { get; set; }

        public DateTimeOffset? KetThuc { get; set; }

        public string? GhiChu { get; set; }
    }
}
