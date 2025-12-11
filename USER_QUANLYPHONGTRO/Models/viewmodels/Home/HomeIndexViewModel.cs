using System.Collections.Generic;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        // Danh sách phòng nổi bật
        public List<PhongTroListItemViewModel> PhongNoiBat { get; set; }
            = new List<PhongTroListItemViewModel>();

        // Từ khoá đang tìm (nếu có)
        public string Keyword { get; set; }

        // Thông điệp / banner
        public string BannerMessage { get; set; }
    }
}
