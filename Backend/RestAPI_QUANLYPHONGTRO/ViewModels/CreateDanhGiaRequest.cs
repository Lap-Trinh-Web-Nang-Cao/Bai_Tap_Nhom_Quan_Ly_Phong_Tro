using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class CreateDanhGiaRequest
    {
        [Required]
        public Guid PhongId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Điểm phải từ 1 đến 5.")]
        public int Diem { get; set; }

        [MaxLength(1000)]
        public string? NoiDung { get; set; }
    }
}
