using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class NguoiDungDto
    {
        public Guid NguoiDungId { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string PasswordHash { get; set; }
        public int VaiTroId { get; set; }
        public bool IsKhoa { get; set; }
        public bool IsEmailXacThuc { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
