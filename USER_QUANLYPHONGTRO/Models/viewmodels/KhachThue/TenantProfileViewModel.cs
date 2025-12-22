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
        public Guid? UserId { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string DienThoai { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string AvatarUrl { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }

        [Display(Name = "CMND/CCCD")]
        public string CCCD { get; set; }

        // Legacy - nếu có code cũ sử dụng
        public UserProfileDto Profile { get; set; }
    }

    // ViewModel cho Lịch sử hoạt động
    public class TenantActivityViewModel
    {
        public Guid Id { get; set; }
        public string LoaiHoatDong { get; set; } // XemPhong, DatLich, ThanhToan, etc.
        public string MoTa { get; set; }
        public DateTime ThoiGian { get; set; }
        public string Icon { get; set; }
        public string MauSac { get; set; } // CSS class hoặc color code
    }

    // ViewModel cho Thông báo
    public class TenantNotificationViewModel
    {
        public Guid Id { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public DateTime ThoiGian { get; set; }
        public bool DaDoc { get; set; }
        public string LoaiThongBao { get; set; } // LichHen, HoaDon, HopDong, HeThong
        public string Url { get; set; } // Link đến trang liên quan
    }

    // ViewModel cho Phòng yêu thích
    public class TenantFavoriteViewModel
    {
        public Guid PhongId { get; set; }
        public string TieuDe { get; set; }
        public string DiaChi { get; set; }
        public long GiaThue { get; set; }
        public double DienTich { get; set; }
        public string HinhAnhUrl { get; set; }
        public DateTime NgayYeuThich { get; set; }
        public string TrangThaiPhong { get; set; } // ConTrong, DaThue
    }
}