using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Viewmodels.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public DateTime ExpiredAt { get; set; }

        public Guid NguoiDungId { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string HoTen { get; set; }
        public string TenVaiTro { get; set; }   // Admin / ChuTro / NguoiThue
        public bool IsKhoa { get; set; }
        public bool IsEmailXacThuc { get; set; }
    }
}