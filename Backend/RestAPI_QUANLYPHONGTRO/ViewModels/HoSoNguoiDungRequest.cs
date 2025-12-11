using System.ComponentModel.DataAnnotations;

namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class HoSoNguoiDungRequest
    {
        [MaxLength(200)]
        public string? HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        [MaxLength(100)]
        public string? LoaiGiayTo { get; set; }

        [MaxLength(1000)]
        public string? GhiChu { get; set; }
    }
}
