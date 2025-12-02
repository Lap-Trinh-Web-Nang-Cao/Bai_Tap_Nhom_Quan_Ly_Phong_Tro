using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class PhuongRequest
    {
        [Required]
        public int QuanHuyenId { get; set; }

        [Required(ErrorMessage = "Tên phường không được để trống")]
        [MaxLength(200)]
        public string Ten { get; set; }
    }
}