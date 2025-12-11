using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class TienIchRequest
    {
        [Required(ErrorMessage = "Tên tiện ích không được để trống")]
        [MaxLength(200)]
        public string Ten { get; set; }
    }
}
