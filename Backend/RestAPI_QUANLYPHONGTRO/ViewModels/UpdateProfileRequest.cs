using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class UpdateProfileRequest
    {
        [Required]
        [MaxLength(50)]
        public string DienThoai { get; set; }
    }
}
