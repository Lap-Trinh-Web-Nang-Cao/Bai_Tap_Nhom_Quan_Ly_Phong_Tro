using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class ChuTroThongTinPhapLyDto
    {
        public Guid NguoiDungId { get; set; }
        public string CCCD { get; set; }
        public DateTime? NgayCapCCCD { get; set; }
        public string NoiCapCCCD { get; set; }
        public string DiaChiThuongTru { get; set; }
        public string DiaChiLienHe { get; set; }
        public string SoDienThoaiLienHe { get; set; }
        public string MaSoThueCaNhan { get; set; }
        public string SoTaiKhoanNganHang { get; set; }
        public string TenNganHang { get; set; }
        public string ChiNhanhNganHang { get; set; }
        public Guid? TapTinGiayToId { get; set; }
        public string TrangThaiXacThuc { get; set; }
        public string GhiChu { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
