using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class AssignRoleRequest
    {
        [Required]
        public Guid NguoiDungId { get; set; }

        [Required]
        public int VaiTroId { get; set; }

        [MaxLength(500)]
        public string? GhiChu { get; set; }
    }
}
