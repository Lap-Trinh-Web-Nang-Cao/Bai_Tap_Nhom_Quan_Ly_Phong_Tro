using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class TokenThongBaoDto
    {
        public Guid TokenId { get; set; }
        public Guid NguoiDungId { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
