using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class PhongTienIchRequest
    {
        [Required]
        public Guid PhongId { get; set; }

        [Required]
        public int TienIchId { get; set; }
    }
}
