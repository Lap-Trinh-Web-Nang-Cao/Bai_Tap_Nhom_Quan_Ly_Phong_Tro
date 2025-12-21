using System.Collections.Generic;
using USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro
{
    public class QuanLyPhongViewModel
    {
        // Danh sách phòng đã được duyệt và đang hoạt động
        public List<PhongTroHienThiItem> DanhSachTatCaPhong { get; set; }

        // Danh sách phòng đang đợi admin phê duyệt (Sử dụng class bạn đã có)
        public List<PhongChoDuyetItem> DanhSachPhongChoDuyet { get; set; }
    }

    public class PhongTroHienThiItem
    {
        public System.Guid PhongId { get; set; }
        public string TieuDe { get; set; }
        public long GiaTien { get; set; }
        public string DiaChi { get; set; }
        public string HinhAnh { get; set; }
        public string TrangThai { get; set; }
    }
}