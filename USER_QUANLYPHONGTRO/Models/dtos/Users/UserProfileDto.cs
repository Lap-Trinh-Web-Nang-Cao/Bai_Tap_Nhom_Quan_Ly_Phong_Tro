using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Users
{
    public class UserProfileDto
    {
        public Guid NguoiDungId { get; set; }

        // Tài khoản
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string TenVaiTro { get; set; }

        // Hồ sơ
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string LoaiGiayTo { get; set; }
        public string GhiChu { get; set; }
        public string DiaChi { get; set; }
    }
}