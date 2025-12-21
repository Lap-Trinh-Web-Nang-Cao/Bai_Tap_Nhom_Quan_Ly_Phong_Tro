using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class BienLaiDto
    {
        public Guid BienLaiId { get; set; }
        public int SoBienLai { get; set; }
        public Guid DatPhongId { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public bool DaXacNhan { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
