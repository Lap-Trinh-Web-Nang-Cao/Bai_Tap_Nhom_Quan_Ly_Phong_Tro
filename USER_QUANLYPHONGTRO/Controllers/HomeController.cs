using System;
using System.Collections.Generic;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.Home;          // NEW
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;     // NEW

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // NEW: dùng ViewModel cho trang chủ
            var model = new HomeIndexViewModel
            {
                BannerMessage = "Nền tảng tìm phòng trọ cho sinh viên UTE",
                Keyword = null,
                PhongNoiBat = GetMockPhongNoiBat()  // dữ liệu demo
            };

            return View(model);    // NEW: trước đây là return View();
        }

        // NEW: Action danh sách phòng (mock data, sau này nối API)
        public ActionResult DanhSachPhong(
            string keyword,
            string priceRange,
            string areaRange,
            string roomType,
            bool? featured,
            int? minArea,
            string sort,
            bool? verified)
        {
            // Tạm thời dùng mock data - sau này sẽ thay bằng gọi API + filter theo tham số
            var rooms = GetMockDanhSachPhong();

            ViewBag.Keyword = keyword;
            ViewBag.PriceRange = priceRange;
            ViewBag.Message = "Danh sách phòng (dữ liệu demo, sẽ nối API sau)";

            return View(rooms);   // NEW: trả về view với model List<PhongTroListItemViewModel>
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        // ================== HELPER MOCK DATA ==================

        // NEW: Phòng nổi bật trên trang chủ
        private List<PhongTroListItemViewModel> GetMockPhongNoiBat()
        {
            return new List<PhongTroListItemViewModel>
            {
                new PhongTroListItemViewModel
                {
                    PhongId = Guid.NewGuid(),
                    TieuDe = "Phòng trọ sinh viên gần UTE",
                    TenNhaTro = "Nhà trọ Hoa Mai",
                    DiaChi = "123 Lê Văn Việt, Q.9",
                    DienTich = 18,
                    GiaTien = 1800000,
                    DiemTrungBinh = 4.5,
                    SoLuongDanhGia = 12,
                    TienIchNganGon = new[] { "Wifi", "Chỗ để xe", "Giờ giấc tự do" }
                },
                new PhongTroListItemViewModel
                {
                    PhongId = Guid.NewGuid(),
                    TieuDe = "Phòng trọ mới xây, full nội thất",
                    TenNhaTro = "Nhà trọ SUNRISE",
                    DiaChi = "456 Man Thiện, TP Thủ Đức",
                    DienTich = 22,
                    GiaTien = 2500000,
                    DiemTrungBinh = 4.8,
                    SoLuongDanhGia = 8,
                    TienIchNganGon = new[] { "Máy lạnh", "Máy nước nóng", "Camera" }
                },
                new PhongTroListItemViewModel
                {
                    PhongId = Guid.NewGuid(),
                    TieuDe = "Căn hộ mini 1PN cho sinh viên",
                    TenNhaTro = "Căn hộ Mini UTE HOME",
                    DiaChi = "Đường Hoàng Hữu Nam, Q.9",
                    DienTich = 28,
                    GiaTien = 3200000,
                    DiemTrungBinh = 4.2,
                    SoLuongDanhGia = 5,
                    TienIchNganGon = new[] { "Thang máy", "Ban công", "Bếp riêng" }
                }
            };
        }

        // NEW: Danh sách phòng tổng quát (dùng cho DanhSachPhong)
        private List<PhongTroListItemViewModel> GetMockDanhSachPhong()
        {
            // Dùng lại từ PhongNoiBat + có thể thêm vài phòng khác
            var list = GetMockPhongNoiBat();

            list.Add(new PhongTroListItemViewModel
            {
                PhongId = Guid.NewGuid(),
                TieuDe = "Phòng giá rẻ cho sinh viên",
                TenNhaTro = "Nhà trọ Bình Dân",
                DiaChi = "Khu phố 6, Linh Trung, Thủ Đức",
                DienTich = 16,
                GiaTien = 1300000,
                DiemTrungBinh = 3.9,
                SoLuongDanhGia = 20,
                TienIchNganGon = new[] { "Wifi", "Giờ giấc tự do" }
            });

            return list;
        }
    }
}
