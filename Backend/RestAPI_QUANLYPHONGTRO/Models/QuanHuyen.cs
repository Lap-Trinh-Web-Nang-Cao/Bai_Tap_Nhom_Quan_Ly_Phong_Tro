using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("QuanHuyen")]
    public class QuanHuyen
    {
        [Key]
        public int QuanHuyenId { get; set; } // Khóa chính int

        [Required]
        [MaxLength(200)]
        public string Ten { get; set; } // Tên Quận/Huyện (VD: Quận 1, Quận Bình Thạnh...)

        // Nếu muốn Relationship ngược để biết quận này có những nhà trọ nào
        // public virtual ICollection<NhaTro> NhaTros { get; set; }
    }
}
