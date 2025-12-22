using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class TenantInvoiceViewModel
    {
        public Guid HoaDonId { get; set; }
        public string ThangNam { get; set; } // "12/2025"
        
        public long TienThue { get; set; }
        public long TienDien { get; set; }
        public long TienNuoc { get; set; }
        public long PhiKhac { get; set; }
        
        public long TongTien { get; set; }
        public string TrangThai { get; set; } // Đã thanh toán / Chưa thanh toán
        public DateTime? NgayThanhToan { get; set; }
     
        public DateTime? HanThanhToan { get; set; }
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