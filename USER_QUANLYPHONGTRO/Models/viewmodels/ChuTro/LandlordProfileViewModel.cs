using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Users;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordProfileViewModel
    {
        public UserProfileDto Profile { get; set; }

        public string HoTen => Profile?.HoTen;
        public string Email => Profile?.Email;
        public string DienThoai => Profile?.DienThoai;
        public string DiaChi => Profile?.DiaChi;

        public string MaSoThue { get; set; }
        public string SoTaiKhoan { get; set; }
        public string TenNganHang { get; set; }
    }
}