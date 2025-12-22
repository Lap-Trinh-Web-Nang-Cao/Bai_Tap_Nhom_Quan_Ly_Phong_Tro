using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class DatPhongDto
    {
        public Guid DatPhongId { get; set; }
        public int SoDatPhong { get; set; }
        public Guid PhongId { get; set; }
        public Guid NguoiThueId { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; }
    }
}
