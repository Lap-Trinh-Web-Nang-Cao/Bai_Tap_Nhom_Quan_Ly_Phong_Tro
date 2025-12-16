using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class PhongDto
    {
        public Guid PhongId { get; set; }
        public Guid NguoiDungId { get; set; }  // chủ trọ
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public decimal GiaTien { get; set; }
        public double DienTich { get; set; }
        public string DiaChi { get; set; }
        public bool IsDuyet { get; set; }
        public bool IsBiKhoa { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int? SoDatPhongGanNhat { get; set; }
    }
}
