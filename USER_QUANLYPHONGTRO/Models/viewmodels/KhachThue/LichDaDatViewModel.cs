using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class LichDaDatViewModel
    {
        public Guid DatPhongId { get; set; }
        public string TenPhong { get; set; }
        public string DiaChi { get; set; }
        public string AnhDaiDien { get; set; }
        public decimal GiaTien { get; set; }
        public DateTime NgayHen { get; set; }
        public DateTime NgayTao { get; set; }
        public int TrangThaiId { get; set; }
        public string TenTrangThai { get; set; }
        public string GhiChu { get; set; }
    }
}