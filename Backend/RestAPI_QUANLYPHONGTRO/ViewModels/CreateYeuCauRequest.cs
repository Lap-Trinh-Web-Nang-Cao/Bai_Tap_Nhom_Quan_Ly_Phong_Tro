using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class CreateYeuCauRequest
    {
        [Required]
        public Guid PhongId { get; set; }

        [Required]
        public int LoaiHoTroId { get; set; }

        [Required]
        [MaxLength(300)]
        public string TieuDe { get; set; }

        public string? MoTa { get; set; }
    }
}
