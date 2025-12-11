using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Users;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class TenantProfileViewModel
    {
        public UserProfileDto Profile { get; set; }

        [Display(Name = "Họ và tên")]
        public string HoTen => Profile?.HoTen;

        [Display(Name = "Email")]
        public string Email => Profile?.Email;

        [Display(Name = "Số điện thoại")]
        public string DienThoai => Profile?.DienThoai;

        [Display(Name = "Địa chỉ")]
        public string DiaChi => Profile?.DiaChi;
    }
}