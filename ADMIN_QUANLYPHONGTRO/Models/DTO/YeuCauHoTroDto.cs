using System;

namespace ADMIN_QUANLYPHONGTRO.Models.DTO
{
    public class YeuCauHoTroDto
    {
        public Guid HoTroId { get; set; }
        public Guid NguoiDungId { get; set; }
        public string ChuDe { get; set; }
        public string NoiDung { get; set; }
        public string TrangThai { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
