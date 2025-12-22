using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class CapNhatPhongViewModel : TaoPhongViewModel
    {
        public Guid PhongId { get; set; }

        // Đường dẫn ảnh đang có trong DB (để hiện lên cho user biết)
        public string AnhHienTai { get; set; }
    }
}