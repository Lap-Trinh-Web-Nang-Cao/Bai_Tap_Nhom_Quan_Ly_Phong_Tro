using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    /// <summary>
    /// ViewModel cho Lịch Hẹn Xem Phòng (Chủ Trọ)
    /// </summary>
    public class LichHenViewModel
    {
        public Guid LichHenId { get; set; }
        public Guid PhongId { get; set; }
        
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        
        public string TenPhong { get; set; }
        public string DiaChiPhong { get; set; }
        
        public DateTime NgayXem { get; set; }
        public string GioXem { get; set; }
        public string GhiChu { get; set; }
        
        public string TrangThai { get; set; } // "ChoXacNhan", "DaXacNhan", "DaHuy"
        public int TrangThaiId { get; set; }
        
        // Legacy support
        public DatPhongDto DatPhong { get; set; }
        public string TenKhachThue { get; set; }
        
        public DateTimeOffset ThoiGianHen
        {
            get
            {
                if (DatPhong != null)
                    return DatPhong.BatDau;
                return DateTimeOffset.Now;
            }
        }
    }

    /// <summary>
    /// ViewModel cho Đơn Đặt Phòng
    /// </summary>
    public class DonDatPhongViewModel
    {
        public Guid DatPhongId { get; set; }
        public int SoDatPhong { get; set; }
        
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        
        public string TenPhong { get; set; }
        public Guid PhongId { get; set; }
        
        public string LoaiDatPhong { get; set; } // "Ngay" hoặc "Thang"
        
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        
        public long GiaTien { get; set; }
        public long TienCoc { get; set; }
        
        public string TrangThai { get; set; } // "ChoXacNhan", "DaXacNhan", "DaThanhToan", "DaHuy"
        public int TrangThaiId { get; set; }
        
        public DateTime ThoiGianTao { get; set; }
        public string GhiChu { get; set; }
    }

    /// <summary>
    /// ViewModel cho Quản Lý Phòng
    /// </summary>
    public class QuanLyPhongViewModel
    {
        public Guid PhongId { get; set; }
        public Guid NhaTroId { get; set; }
        
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        
        public long GiaTien { get; set; }
        public decimal? DienTich { get; set; }
        public long? TienCoc { get; set; }
        public int? SoNguoiToiDa { get; set; }
        
        public string HinhAnhDaiDien { get; set; }
        
        // Alias cho View cũ
        public string HinhAnh 
        { 
            get { return HinhAnhDaiDien; } 
            set { HinhAnhDaiDien = value; } 
        }
        
        public string TrangThai { get; set; } // "ConTrong", "DaThue", "DangSuaChua"
        
        public bool IsDuyet { get; set; }
        public bool IsBiKhoa { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string TenNhaTro { get; set; }
        
        // Computed properties for View
        public string HienThiTrangThai
        {
            get
            {
                if (IsBiKhoa) return "Bị khóa";
                if (!IsDuyet) return "Chờ duyệt";
                
                switch (TrangThai)
                {
                    case "DaThue": return "Đã thuê";
                    case "DangSuaChua": return "Đang sửa";
                    default: return "Còn trống";
                }
            }
        }
        
        public string BadgeClass
        {
            get
            {
                if (IsBiKhoa) return "badge-danger";
                if (!IsDuyet) return "badge-warning";
                
                switch (TrangThai)
                {
                    case "DaThue": return "badge-secondary";
                    case "DangSuaChua": return "badge-warning";
                    default: return "badge-success";
                }
            }
        }
    }

    /// <summary>
    /// ViewModel cho Tạo Phòng Mới
    /// </summary>
    public class TaoPhongViewModel
    {
        public Guid NhaTroId { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        
        public long GiaTien { get; set; }
        public decimal? DienTich { get; set; }
        public long? TienCoc { get; set; }
        public int? SoNguoiToiDa { get; set; }
        
        public HttpPostedFileBase HinhAnhUpload { get; set; }
        
        public List<SelectListItem> DanhSachNhaTro { get; set; } = new List<SelectListItem>();
    }

    /// <summary>
    /// ViewModel cho Cập Nhật Phòng
    /// </summary>
    public class CapNhatPhongViewModel
    {
        public Guid PhongId { get; set; }
        public Guid NhaTroId { get; set; }
        
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        
        public long GiaTien { get; set; }
        public decimal? DienTich { get; set; }
        public long? TienCoc { get; set; }
        public int? SoNguoiToiDa { get; set; }
        
        public HttpPostedFileBase HinhAnhUpload { get; set; }
        public string AnhHienTai { get; set; }
        
        public List<SelectListItem> DanhSachNhaTro { get; set; } = new List<SelectListItem>();
    }

    /// <summary>
    /// ViewModel cho Yêu Cầu Hỗ Trợ
    /// </summary>
    public class YeuCauHoTroViewModel
    {
        public Guid HoTroId { get; set; }
        public Guid PhongId { get; set; }
        public string TenPhong { get; set; }
        
        public Guid NguoiYeuCau { get; set; }
        public string TenNguoiYeuCau { get; set; }
        
        public int LoaiHoTroId { get; set; }
        public string TenLoaiHoTro { get; set; }
        
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public string TrangThai { get; set; }
        
        public DateTime ThoiGianTao { get; set; }
    }

    /// <summary>
    /// ViewModel cho Thống Kê Đánh Giá
    /// </summary>
    public class ThongKeDanhGiaViewModel
    {
        public double DiemTrungBinh { get; set; }
        public int TongLuotDanhGia { get; set; }
        public List<DanhGiaViewModel> DanhSachDanhGia { get; set; } = new List<DanhGiaViewModel>();
    }

    public class DanhGiaViewModel
    {
        public Guid DanhGiaId { get; set; }
        public string TenNguoiDanhGia { get; set; }
        public string TenPhong { get; set; }
        public int Diem { get; set; }
        public string NoiDung { get; set; }
        public DateTime ThoiGian { get; set; }
    }

    /// <summary>
    /// ViewModel cho Chi Tiết Hợp Đồng
    /// </summary>
    public class ChiTietHopDongViewModel
    {
        public Guid HopDongId { get; set; }
        public string SoHopDong { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayLap { get; set; }
        
        public string TenNguoiThue { get; set; }
        public string SoDienThoai { get; set; }
        public string CCCD { get; set; }
        public string QueQuan { get; set; }
        
        public string TenPhong { get; set; }
        public double DienTich { get; set; }
        public decimal GiaThue { get; set; }
        public decimal TienCoc { get; set; }
        
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int KyThanhToan { get; set; }
        public string GhiChu { get; set; }
    }

    /// <summary>
    /// ViewModel cho Thống Kê
    /// </summary>
    public class LandlordStatisticsViewModel
    {
        public decimal TongDoanhThuNam { get; set; }
        public double TiLeLapDay { get; set; }
        public int TongSoHopDong { get; set; }
        public int SoPhongTrong { get; set; }
        public int SoPhongDaThue { get; set; }
        public int SoPhongDangSua { get; set; }
        
        public List<string> NhanThang { get; set; } = new List<string>();
        public List<decimal> DoanhThuTheoThang { get; set; } = new List<decimal>();
    }

    /// <summary>
    /// ViewModel cho Chat
    /// </summary>
    public class ChatViewModel
    {
        public List<ConversationItem> Conversations { get; set; } = new List<ConversationItem>();
        public List<MessageItem> CurrentMessages { get; set; } = new List<MessageItem>();
        public string SelectedUserName { get; set; }
    }

    public class ConversationItem
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string LastMessage { get; set; }
        public DateTime Time { get; set; }
        public int UnreadCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class MessageItem
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public bool IsFromMe { get; set; }
    }
}