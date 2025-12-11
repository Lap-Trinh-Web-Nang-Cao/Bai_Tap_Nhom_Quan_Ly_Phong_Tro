using System;
using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordHopDongViewModel
    {
        public IEnumerable<LandlordHopDongItemViewModel> HopDongList { get; set; }
    }

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
