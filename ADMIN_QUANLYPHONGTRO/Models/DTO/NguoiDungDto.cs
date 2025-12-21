using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    /// <summary>
    /// DTO cơ bản cho người dùng (danh sách)
    /// </summary>
    public class NguoiDungDto
    {
        public Guid NguoiDungId { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string HoTen { get; set; }
        public int VaiTroId { get; set; }
        public string VaiTroName { get; set; }
        public bool IsKhoa { get; set; }
        public bool IsEmailXacThuc { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO chi tiết người dùng
    /// </summary>
    public class NguoiDungDetailDto
    {
        public Guid NguoiDungId { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GhiChu { get; set; }
        public string LoaiGiayTo { get; set; }
        public int VaiTroId { get; set; }
        public string VaiTroName { get; set; }
        public bool IsKhoa { get; set; }
        public bool IsEmailXacThuc { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SoPhongDaDang { get; set; }
        public int SoDatPhong { get; set; }
        public string Avatar { get; set; }
    }
}
