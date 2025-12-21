using System;
using System.Collections.Generic;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public string BannerMessage { get; set; }

        // Danh sách phòng để hiển thị lưới
        public List<PhongTroListItemViewModel> PhongNoiBat { get; set; }

        // --- MỚI THÊM: Dùng cho Tabs ---
        public List<Models.Dtos.Rooms.NhaTroDto> DanhSachNhaTro { get; set; }

        // ID của Tab đang chọn (null = Tất cả)
        public Guid? SelectedNhaTroId { get; set; }

        // Hỗ trợ phân trang
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
