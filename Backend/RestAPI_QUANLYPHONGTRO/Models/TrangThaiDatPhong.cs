using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestAPI_QUANLYPHONGTRO.Models
{
    [Table("TrangThaiDatPhong")]
    public class TrangThaiDatPhong
    {
        [Key]
        // Quan trọng: Tắt tự tăng để đảm bảo ID đúng như quy định (1, 2, 3...)
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TrangThaiId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenTrangThai { get; set; }
    }
}
