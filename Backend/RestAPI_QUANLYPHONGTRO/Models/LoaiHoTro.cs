using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("LoaiHoTro")]
    public class LoaiHoTro
    {
        [Key]
        // Lưu ý: Nếu trong SQL bạn set Identity(1,1) thì EF tự hiểu là tự tăng
        public int LoaiHoTroId { get; set; }

        [Required]
        [MaxLength(200)]
        public string TenLoai { get; set; }
    }
}
