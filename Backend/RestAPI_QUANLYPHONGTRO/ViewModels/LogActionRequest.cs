using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class LogActionRequest
    {
        [Required]
        public string HanhDong { get; set; }

        public string? MucTieuBang { get; set; }

        public string? BanGhiId { get; set; }

        public string? ChiTiet { get; set; }
    }
}
