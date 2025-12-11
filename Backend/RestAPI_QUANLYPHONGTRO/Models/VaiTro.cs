using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("VaiTro")]
    public class VaiTro
    {
        [Key]
        public int VaiTroId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenVaiTro { get; set; }
    }
}
