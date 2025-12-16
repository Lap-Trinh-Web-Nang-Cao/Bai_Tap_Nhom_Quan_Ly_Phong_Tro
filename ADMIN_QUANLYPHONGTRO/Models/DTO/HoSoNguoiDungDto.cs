using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class HoSoNguoiDungDto
    {
        public Guid NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string LoaiGiayTo { get; set; }
        public string GhiChu { get; set; }
    }
}