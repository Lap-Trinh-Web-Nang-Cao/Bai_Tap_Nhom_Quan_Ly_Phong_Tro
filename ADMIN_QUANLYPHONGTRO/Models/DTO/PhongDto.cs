using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class PhongDto
    {
        public Guid PhongId { get; set; }
        public Guid NhaTroId { get; set; }  // FK tới nhà trọ
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public decimal? DienTich { get; set; }
        public long GiaTien { get; set; }
        public long? TienCoc { get; set; }
        public int? SoNguoiToiDa { get; set; }
        public string TrangThai { get; set; }
        public bool IsDuyet { get; set; }
        public bool IsBiKhoa { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public double? DiemTrungBinh { get; set; }
        public int? SoLuongDanhGia { get; set; }
    }
}
