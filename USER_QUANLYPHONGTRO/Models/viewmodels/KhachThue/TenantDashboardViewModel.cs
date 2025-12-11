using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class TenantDashboardViewModel
    {
        // Thông tin chào mừng
        public string TenNguoiThue { get; set; }
        public string Email { get; set; }

        // Thống kê nhanh
        public int SoPhongDaXem { get; set; }
        public int SoLichHenSapToi { get; set; }
        public int SoHopDongDangHieuLuc { get; set; }
        public int SoHoaDonChuaThanhToan { get; set; }

        // Lịch hẹn sắp tới
        public List<TenantScheduleItem> LichHenSapToi { get; set; } = new List<TenantScheduleItem>();

        // Hợp đồng đang hiệu lực
        public List<TenantContractItem> HopDongHieuLuc { get; set; } = new List<TenantContractItem>();

        // Hoá đơn gần đây
        public List<TenantInvoiceItem> HoaDonGanDay { get; set; } = new List<TenantInvoiceItem>();
    }

    public class TenantScheduleItem
    {
        public Guid DatPhongId { get; set; }
        public string TenPhong { get; set; }
        public string TenChuTro { get; set; }
        public DateTime ThoiGianXem { get; set; }
        public string TrangThai { get; set; } // Chờ xác nhận / Đã xác nhận / Đã hủy
    }

    public class TenantContractItem
    {
        public Guid HopDongId { get; set; }
        public string TenPhong { get; set; }
        public string DiaChi { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public long GiaThue { get; set; }
        public string TrangThai { get; set; } // Đang hiệu lực / Sắp hết hạn
    }

    public class TenantInvoiceItem
    {
        public Guid HoaDonId { get; set; }
        public string ThangNam { get; set; } // "12/2025"
        public long TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; } // Đã thanh toán / Chưa thanh toán
        public DateTime? NgayThanhToan { get; set; }
    }
}
