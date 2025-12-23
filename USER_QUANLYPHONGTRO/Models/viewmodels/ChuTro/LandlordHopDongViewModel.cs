using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    /// <summary>
    /// ViewModel cho trang Hợp Đồng của Chủ Trọ
    /// </summary>
    public class LandlordHopDongViewModel
    {
        public int TongHopDongHieuLuc { get; set; }
        public int HopDongSapHetHan { get; set; }
        public int HopDongDaHetHan { get; set; }
        
        public List<HopDongItemViewModel> DanhSachHopDong { get; set; } = new List<HopDongItemViewModel>();
        
        // Legacy support
        public IEnumerable<LandlordHopDongItemViewModel> HopDongList { get; set; }
    }

    /// <summary>
    /// Item trong danh sách hợp đồng
    /// </summary>
    public class HopDongItemViewModel
    {
        public Guid HopDongId { get; set; }
        public string SoHopDong { get; set; }
        
        public string TenPhong { get; set; }
        public Guid PhongId { get; set; }
        
        public string TenNguoiThue { get; set; }
        public Guid NguoiThueId { get; set; }
        public string SdtNguoiThue { get; set; }
        
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        
        public decimal GiaThue { get; set; }
        public decimal TienCoc { get; set; }
        
        public string TrangThai { get; set; } // "DaCoc", "DangHieuLuc", "SapHetHan", "DaHetHan"
        
        public int SoNgayConLai => Math.Max(0, (int)(NgayKetThuc - DateTime.Now).TotalDays);
    }

    /// <summary>
    /// Legacy ViewModel
    /// </summary>
    public class LandlordHopDongItemViewModel
    {
        public Guid HopDongId { get; set; }
        public string TenPhong { get; set; }
        public string TenKhachThue { get; set; }

        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        public long GiaThue { get; set; }
    }
}
