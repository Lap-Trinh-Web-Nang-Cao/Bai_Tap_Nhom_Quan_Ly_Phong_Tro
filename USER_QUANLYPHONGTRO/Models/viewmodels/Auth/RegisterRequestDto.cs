using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Viewmodels.Auth
{
    public class RegisterRequestDto
    {
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }

        public string Password { get; set; }

        public string UserType { get; set; } // "KhachThue" / "ChuTro"

        public string LoaiGiayTo { get; set; }
        public string SoChungMinh { get; set; }
        public DateTime? NgayCapCCCD { get; set; }
        public string NoiCapCCCD { get; set; }
        public string DiaChiThuongTru { get; set; }
        public string MaSoThue { get; set; }

        public string SoTaiKhoan { get; set; }
        public string TenNganHang { get; set; }
        public string ChiNhanh { get; set; }
    }
}