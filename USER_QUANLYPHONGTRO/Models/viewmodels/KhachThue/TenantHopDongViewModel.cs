using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    // Hợp đồng đơn (single contract)
    public class TenantContractViewModel
    {
        public Guid HopDongId { get; set; }
        public string TieuDePhong { get; set; }
        public string DiaChi { get; set; }
        public string TenChuTro { get; set; }
        public string SdtChuTro { get; set; }

        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        public long GiaThue { get; set; }
        public long TienCoc { get; set; }

        public string FileHopDongUrl { get; set; }
        public string TrangThai { get; set; } // Đang hiệu lực / Sắp hết hạn / Đã kết thúc

        public int SoThangConLai
        {
            get
            {
                var now = DateTime.Now;
                if (now >= NgayKetThuc)
                    return 0;
                return (int)((NgayKetThuc - now).TotalDays / 30);
            }
        }
    }

    // Legacy - giữ để tương thích (nếu có code cũ sử dụng)
    public class TenantHopDongViewModel
    {
        public Guid HopDongId { get; set; }
        public string TenPhong { get; set; }
        public string TenChuTro { get; set; }

        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        public long GiaThue { get; set; }
        public long? TienCoc { get; set; }

        public string FileHopDongUrl { get; set; }
    }
}