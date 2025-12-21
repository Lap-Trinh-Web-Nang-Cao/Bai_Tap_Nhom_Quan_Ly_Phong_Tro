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
    public class TenantScheduleViewModel
    {
        public string TieuDePhong { get; set; }
        public string DiaChi { get; set; }
        public string TrangThai { get; set; }
        public DateTime ThoiGianHen { get; set; }
        public string SdtChuTro { get; set; }
        public string GhiChu { get; set; }
    }
}