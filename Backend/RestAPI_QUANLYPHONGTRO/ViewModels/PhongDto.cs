namespace RestAPI_QUANLYPHONGTRO.ViewModels
{
    public class PhongDto
    {
        public Guid PhongId { get; set; }
        public Guid NhaTroId { get; set; }

        public string TieuDe { get; set; }
        public string? MoTa { get; set; }
        public decimal? DienTich { get; set; }
        public long GiaTien { get; set; }
        public long? TienCoc { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string TrangThai { get; set; }

        public double? DiemTrungBinh { get; set; }
        public int SoLuongDanhGia { get; set; }

        public bool IsDuyet { get; set; }
        public bool IsBiKhoa { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        // Hình ảnh
        public string? HinhAnhDaiDien { get; set; }
        public List<string>? DanhSachHinhAnh { get; set; }

        // Thông tin nhà trọ (nested)
        public NhaTroSimpleDto? NhaTro { get; set; }

        // Danh sách tiện ích
        public List<string>? TienIchList { get; set; }
    }

    public class NhaTroSimpleDto
    {
        public Guid NhaTroId { get; set; }
        public string TieuDe { get; set; }
        public string? DiaChi { get; set; }
    }
}
