using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class LandlordHoaDonViewModel
    {
        public IEnumerable<BienLaiDto> BienLaiCanThu { get; set; }
    }
}