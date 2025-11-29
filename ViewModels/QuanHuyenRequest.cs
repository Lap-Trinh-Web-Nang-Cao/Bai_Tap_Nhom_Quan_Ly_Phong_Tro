using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class QuanHuyenRequest
    {
        [Required(ErrorMessage = "Tên quận huyện không được để trống")]
        [MaxLength(200)]
        public string Ten { get; set; }
    }
}
