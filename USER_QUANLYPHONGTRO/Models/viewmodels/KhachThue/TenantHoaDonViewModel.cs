using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class TenantHoaDonViewModel
    {
        public IEnumerable<BienLaiDto> BienLaiList { get; set; }
    }
}