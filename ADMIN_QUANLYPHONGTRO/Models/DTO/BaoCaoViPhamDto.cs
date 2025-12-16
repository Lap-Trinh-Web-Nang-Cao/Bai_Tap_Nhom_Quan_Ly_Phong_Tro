using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class BaoCaoViPhamDto
    {
        public Guid BaoCaoId { get; set; }
        public int SoBaoCao { get; set; }
        public string LoaiThucThe { get; set; } // PHONG / NGUOIDUNG
        public Guid? PhongId { get; set; }
        public Guid? NguoiDungId { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string TrangThai { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
