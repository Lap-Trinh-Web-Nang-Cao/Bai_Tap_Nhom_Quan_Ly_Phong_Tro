using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class TrangThaiRequest
    {
        [Required]
        [MaxLength(100)]
        public string TenTrangThai { get; set; }
    }
}
