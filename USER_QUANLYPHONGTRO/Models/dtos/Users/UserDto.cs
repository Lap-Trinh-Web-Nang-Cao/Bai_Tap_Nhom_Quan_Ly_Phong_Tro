using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Users
{
    public class UserDto
    {
        public Guid NguoiDungId { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public int VaiTroId { get; set; }
        public string TenVaiTro { get; set; }

        public bool IsKhoa { get; set; }
        public bool IsEmailXacThuc { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}