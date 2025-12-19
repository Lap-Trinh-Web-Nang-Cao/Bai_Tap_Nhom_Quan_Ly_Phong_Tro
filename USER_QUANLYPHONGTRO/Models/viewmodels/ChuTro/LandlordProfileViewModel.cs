using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Quan trọng: Cần dòng này để dùng [Display]
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Users;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordProfileViewModel
    {
        public UserProfileDto Profile { get; set; }

        // Các thuộc tính lấy từ Profile (Chỉ đọc)
        [Display(Name = "Họ và tên")]
        public string HoTen => Profile?.HoTen;

        [Display(Name = "Email")]
        public string Email => Profile?.Email;

        [Display(Name = "Số điện thoại")]
        public string DienThoai => Profile?.DienThoai;

        [Display(Name = "Địa chỉ")]
        public string DiaChi => Profile?.DiaChi;

        // --- Các thuộc tính riêng của Chủ trọ (Cho phép chỉnh sửa) ---

        [Display(Name = "Mã số thuế")]
        public string MaSoThue { get; set; }

        [Display(Name = "Số tài khoản ngân hàng")]
        public string SoTaiKhoan { get; set; }

        [Display(Name = "Tên ngân hàng")]
        public string TenNganHang { get; set; }

        // --- THÊM DÒNG NÀY ĐỂ SỬA LỖI ---
        [Display(Name = "Giấy tờ tùy thân (CCCD)")]
        public string LoaiGiayTo { get; set; }
    }
}