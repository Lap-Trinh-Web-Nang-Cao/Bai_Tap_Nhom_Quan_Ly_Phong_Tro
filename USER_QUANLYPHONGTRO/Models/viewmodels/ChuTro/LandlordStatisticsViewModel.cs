using System.Collections.Generic;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordStatisticsViewModel
    {
        public decimal TongDoanhThuNam { get; set; }
        public double TiLeLapDay { get; set; }
        public int TongSoHopDong { get; set; }
        public double TangTruongDoanhThu { get; set; }

        // Đảm bảo tên không dấu để khớp với Controller và View
        public List<string> NhanThang { get; set; }
        public List<decimal> DoanhThuTheoThang { get; set; }

        public int SoPhongTrong { get; set; }
        public int SoPhongDaThue { get; set; }
        public int SoPhongDangSua { get; set; }
    }
}