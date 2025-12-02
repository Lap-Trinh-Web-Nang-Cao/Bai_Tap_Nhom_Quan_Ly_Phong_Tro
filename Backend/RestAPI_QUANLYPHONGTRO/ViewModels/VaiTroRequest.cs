using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class VaiTroRequest
    {
        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [MaxLength(100)]
        public string TenVaiTro { get; set; }
    }
}
