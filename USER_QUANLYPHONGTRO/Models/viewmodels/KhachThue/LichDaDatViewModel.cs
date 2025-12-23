using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class LichDaDatViewModel
    {
        public DatPhongDto DatPhong { get; set; }

        public string TieuDePhong { get; set; }
        public string DiaChiPhong { get; set; }
        public string TenChuTro { get; set; }
    }

    /// <summary>
    /// ViewModel cho lịch hẹn xem phòng (Danh sách lịch đặt)
    /// </summary>
    public class TenantScheduleViewModel
    {
        public Guid BookingId { get; set; }
        public Guid PhongId { get; set; }
        
        public string TieuDePhong { get; set; }
        public string DiaChi { get; set; }
        public string TrangThai { get; set; }
        public int TrangThaiId { get; set; }  // 1=Chờ xác nhận, 2=Đã xác nhận, 5=Đã hủy
        
        public DateTime ThoiGianHen { get; set; }
        public string SdtChuTro { get; set; }
        public string TenChuTro { get; set; }
        public string GhiChu { get; set; }
        public string LoaiDatPhong { get; set; } // "XemPhong" hoặc "ThuePhong"
    }
}