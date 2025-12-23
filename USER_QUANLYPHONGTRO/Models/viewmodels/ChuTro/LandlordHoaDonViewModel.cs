using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    /// <summary>
    /// ViewModel cho trang Hóa Đơn của Chủ Trọ
    /// </summary>
    public class LandlordHoaDonViewModel
    {
        public Guid HoaDonId { get; set; }
        public string SoHoaDon { get; set; }
        
        public Guid PhongId { get; set; }
        public string TenPhong { get; set; }
        
        public Guid NguoiThueId { get; set; }
        public string TenNguoiThue { get; set; }
        
        public string ThangNam { get; set; } // "12/2025"
        public DateTime ThoiGianTao { get; set; }
        public DateTime HanThanhToan { get; set; }
        
        public decimal TienThue { get; set; }
        public decimal TienDien { get; set; }
        public decimal TienNuoc { get; set; }
        public decimal PhiKhac { get; set; }
        public decimal TongTien { get; set; }
        
        public decimal DaThanhToan { get; set; }
        public decimal ConLai => TongTien - DaThanhToan;
        
        public string TrangThai { get; set; } // "ChuaThanhToan", "DaThanhToan", "QuaHan"
        public bool DaThuTien => DaThanhToan >= TongTien;
        
        public string GhiChu { get; set; }
        
        // Legacy
        public IEnumerable<BienLaiDto> BienLaiCanThu { get; set; }
    }

    /// <summary>
    /// ViewModel tổng hợp cho trang danh sách hóa đơn
    /// </summary>
    public class LandlordHoaDonListViewModel
    {
        public int TongHoaDon { get; set; }
        public int DaThanhToan { get; set; }
        public int ChuaThanhToan { get; set; }
        public decimal TongTienCanThu { get; set; }
        
        public List<LandlordHoaDonViewModel> DanhSachHoaDon { get; set; } = new List<LandlordHoaDonViewModel>();
    }
}