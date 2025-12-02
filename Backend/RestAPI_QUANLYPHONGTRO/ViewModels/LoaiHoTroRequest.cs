using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class LoaiHoTroRequest
    {
        [Required(ErrorMessage = "Tên loại không được để trống")]
        [MaxLength(200)]
        public string TenLoai { get; set; }
    }
}
