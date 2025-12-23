using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    /// <summary>
    /// ViewModel cho hóa đơn
    /// </summary>
    public class TenantInvoiceViewModel
    {
        public Guid HoaDonId { get; set; }
        public Guid PhongId { get; set; }
        public Guid HopDongId { get; set; }
        
        public string SoHoaDon { get; set; }
        public string ThangNam { get; set; } // "12/2025"
        public string TieuDePhong { get; set; }
        public string DiaChi { get; set; }
        
        public DateTime ThoiGianTao { get; set; }
        public DateTime ThoiGianDaoHan { get; set; }
        
        public decimal SoTienTrongKy { get; set; } // Tiền phòng + tiền nước/điện
        
        public decimal TienThue { get; set; }
        public decimal TienDien { get; set; }
        public decimal TienNuoc { get; set; }
        public decimal PhiKhac { get; set; }
        
        public decimal TongTien { get; set; }
        public decimal TienDaThanhToan { get; set; }
        public decimal SoTienConLai => TongTien - TienDaThanhToan;
        
        public string TrangThai { get; set; } // "Chưa thanh toán", "Đã thanh toán"
        public bool DaThanhToan => TienDaThanhToan >= TongTien;
        
        public DateTime? NgayThanhToan { get; set; }
        public DateTime? HanThanhToan { get; set; }
        
        public string GhiChu { get; set; }
        
        public int SoNgayConLai
        {
            get
            {
                if (!HanThanhToan.HasValue)
                    return 0;
                var remaining = (int)(HanThanhToan.Value - DateTime.Now).TotalDays;
                return remaining > 0 ? remaining : 0;
            }
        }
    }

    // Legacy - để tương thích code cũ
    public class TenantHoaDonViewModel
    {
        public IEnumerable<BienLaiDto> BienLaiList { get; set; }
    }
}