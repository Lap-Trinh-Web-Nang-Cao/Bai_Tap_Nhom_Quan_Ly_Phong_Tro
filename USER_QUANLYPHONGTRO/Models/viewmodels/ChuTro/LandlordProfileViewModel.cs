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

        public string HoTen 
        { 
            get 
            { 
                if (Profile != null) return Profile.HoTen;
                return null;
            } 
        }
        
        public string Email 
        { 
            get 
            { 
                if (Profile != null) return Profile.Email;
                return null;
            } 
        }
        
        public string DienThoai 
        { 
            get 
            { 
                if (Profile != null) return Profile.DienThoai;
                return null;
            } 
        }
        
        public string DiaChi 
        { 
            get 
            { 
                if (Profile != null) return Profile.DiaChi;
                return null;
            } 
        }

        public string MaSoThue { get; set; }
        public string SoTaiKhoan { get; set; }
        public string TenNganHang { get; set; }
    }
}