using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class TenantHopDongViewModel
    {
        public Guid HopDongId { get; set; }      // sau này từ API
        public string TenPhong { get; set; }
        public string TenChuTro { get; set; }

        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        public long GiaThue { get; set; }
        public long? TienCoc { get; set; }

        public string FileHopDongUrl { get; set; }
    }
}