using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.Home
{
    public class PhongSearchViewModel
    {
        // Danh sách nhà trọ để hiển thị Tabs
        public List<NhaTroDto> DanhSachNhaTro { get; set; }

        // Danh sách phòng trọ đang hiển thị
        public List<PhongTroListItemViewModel> DanhSachPhong { get; set; }

        // Các tham số để giữ trạng thái khi phân trang/lọc
        public Guid? SelectedNhaTroId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}