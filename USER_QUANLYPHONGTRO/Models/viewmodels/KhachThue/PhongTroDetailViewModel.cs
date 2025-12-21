using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos.Reviews;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;

namespace USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue
{
    public class PhongTroDetailViewModel
    {
        // --- Thông tin định danh ---
        public Guid PhongId { get; set; }
        public Guid NhaTroId { get; set; }
        public Guid ChuTroId { get; set; }

        // --- Thông tin cơ bản ---
        public string TieuDe { get; set; }
        // Đã xóa trường MoTa

        // --- Thông số kỹ thuật ---
        public double DienTich { get; set; }
        public decimal GiaTien { get; set; }
        public long? TienCoc { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string TrangThai { get; set; }

        // --- Đánh giá ---
        public double? DiemTrungBinh { get; set; }
        public int SoLuongDanhGia { get; set; }

        // --- Vị trí ---
        public string TenNhaTro { get; set; }
        public string DiaChi { get; set; }

        // --- Hình ảnh ---
        public string AnhDaiDien { get; set; }
        public List<string> HinhAnh { get; set; }

        // --- Tiện ích ---
        public List<string> TienIchs { get; set; }
        public IEnumerable<TienIchDto> TienIchList { get; set; }

        // --- Danh sách đánh giá chi tiết ---
        public IEnumerable<DanhGiaPhongDto> DanhGiaList { get; set; }

        // --- Logic đặt phòng ---
        public bool CoTheDatLich { get; set; }
    }
}