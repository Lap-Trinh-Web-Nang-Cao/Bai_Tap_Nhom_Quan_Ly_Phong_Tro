using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.Dtos.Reports
{
    public class ViPhamDto
    {
        public int ViPhamId { get; set; }
        public string TenViPham { get; set; }
        public string MoTa { get; set; }
        public long? HinhPhatTien { get; set; }
        public int? SoDiemTru { get; set; }
    }
}