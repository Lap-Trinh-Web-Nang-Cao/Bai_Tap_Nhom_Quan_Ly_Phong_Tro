using System;
using System.Collections.Generic;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class ChuTroController : Controller
    {
        // ============================================================
        // 1. DASHBOARD (TỔNG QUAN)
        // ============================================================
        public ActionResult Dashboard()
        {
            // Sử dụng đúng các thuộc tính tiếng Việt từ LandlordDashboardViewModel.cs
            var model = new LandlordDashboardViewModel
            {
                TongSoPhong = 12,
                LuotXemHomNay = 45,
                LichHenSapToi = 3,
                DoanhThuThang = 25000000,

                // Khởi tạo các danh sách để tránh lỗi NullReferenceException trong View
                DanhSachPhongCho = new List<PhongChoDuyetItem>(),
                LichHenHomNay = new List<LichHenItem>(),
                TinNhanGanDay = new List<TinNhanItem>(),
                YeuCauSuaChua = new List<YeuCauSuaChuaItem>()
            };

            return View(model);
        }

        // ============================================================
        // 2. QUẢN LÝ PHÒNG (DANH SÁCH + CHỜ DUYỆT + NÚT THÊM)
        // ============================================================
        public ActionResult QuanLyPhong()
        {
            // Trang này tổng hợp cả danh sách phòng đang chạy và phòng đang chờ duyệt
            var model = new QuanLyPhongViewModel
            {
                // Danh sách phòng đang hoạt động
                DanhSachTatCaPhong = new List<PhongTroHienThiItem>
                {
                    new PhongTroHienThiItem { 
                        PhongId = Guid.NewGuid(), 
                        TieuDe = "Phòng Studio cao cấp Q.1", 
                        GiaTien = 5500000, 
                        DiaChi = "123 Nguyễn Trãi, P. Bến Thành", 
                        TrangThai = "Còn trống" 
                    }
                },
                // Danh sách phòng chờ duyệt
                DanhSachPhongChoDuyet = new List<PhongChoDuyetItem>()
            };

            return View(model);
        }

        public ActionResult TinNhan()
        {
            var model = new ChatViewModel
            {
                Conversations = new List<ConversationItem>
        {
            new ConversationItem { UserName = "Nguyễn Văn A", LastMessage = "Phòng này còn trống không ạ?", Time = DateTime.Now, UnreadCount = 2, IsActive = true },
            new ConversationItem { UserName = "Trần Thị B", LastMessage = "Cảm ơn chủ trọ nhiều nhé!", Time = DateTime.Now.AddHours(-2), UnreadCount = 0 }
        },
                CurrentMessages = new List<MessageItem>
        {
            new MessageItem { Content = "Chào bạn, mình muốn hỏi về phòng 101", Time = DateTime.Now.AddMinutes(-30), IsFromMe = false },
            new MessageItem { Content = "Chào bạn, phòng 101 vẫn còn trống bạn nhé.", Time = DateTime.Now.AddMinutes(-25), IsFromMe = true },
            new MessageItem { Content = "Phòng này còn trống không ạ?", Time = DateTime.Now.AddMinutes(-10), IsFromMe = false }
        },
                SelectedUserName = "Nguyễn Văn A"
            };
            return View(model);
        }
        public ActionResult DonDatPhong()
        {
            // Trong thực tế, bạn sẽ truy vấn từ bảng DatPhong
            // Hiện tại khởi tạo danh sách rỗng để hiển thị giao diện mặc định
            var model = new List<DonDatPhongViewModel>();

            return View(model);
        }
        // GET: ChuTro/YeuCauHoTro
        public ActionResult YeuCauHoTro()
        {
            // Giả lập dữ liệu dựa trên các trường SQL mới
            var model = new List<YeuCauHoTroViewModel>
    {
        new YeuCauHoTroViewModel {
            HoTroId = Guid.NewGuid(),
            PhongId = Guid.NewGuid(),
            TenPhong = "Phòng 201",
            NguoiYeuCau = Guid.NewGuid(),
            TenNguoiYeuCau = "Lê Văn C",
            LoaiHoTroId = 1,
            TenLoaiHoTro = "Sửa chữa",
            TieuDe = "Hỏng khóa cửa",
            MoTa = "Khóa cửa chính bị kẹt không mở được từ bên ngoài.",
            TrangThai = "Moi",
            ThoiGianTao = DateTime.Now
        }
    };

            return View(model);
        }
        public ActionResult TaoPhong()
        {
            // Sử dụng PhongTroEditViewModel.cs cho trang đăng tin
            var model = new PhongTroEditViewModel
            {
                AvailableAmenities = new List<SelectListItem>() // Khởi tạo danh sách tiện ích rỗng
            };
            return View(model);
        }

        // POST: Xử lý dữ liệu khi nhấn "Lưu" trên form tạo phòng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoPhong(PhongTroEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic xử lý upload ảnh từ ImageFiles và lưu vào Database tại đây
                // ...
                return RedirectToAction("QuanLyPhong");
            }
            return View(model);
        }

        // ============================================================
        // 3. THỐNG KÊ CHI TIẾT
        // ============================================================
        public ActionResult ThongKe()
        {
            var model = new LandlordStatisticsViewModel
            {
                TongDoanhThuNam = 150000000,
                TiLeLapDay = 85.5,
                TongSoHopDong = 12,
                SoPhongTrong = 3,
                SoPhongDaThue = 15,
                SoPhongDangSua = 2,
                NhanThang = new List<string> { "Thg 8", "Thg 9", "Thg 10", "Thg 11", "Thg 12" },
                DoanhThuTheoThang = new List<decimal> { 18000000, 22000000, 25000000, 23000000, 30000000 }
            };

            return View(model);
        }

        // ============================================================
        // 4. CÁC CHỨC NĂNG KHÁC (HỢP ĐỒNG, LỊCH HẸN, HÓA ĐƠN)
        // ============================================================
        public ActionResult LichHen()
        {
            // Trong thực tế, bạn sẽ lấy dữ liệu từ Database tại đây
            var model = new List<LichHenViewModel>
    {
        new LichHenViewModel {
            TenKhachHang = "Nguyễn Văn A",
            SoDienThoai = "0987 654 321",
            TenPhong = "Phòng 102 - Studio cao cấp",
            NgayXem = DateTime.Now.AddDays(1),
            GioXem = "14:30",
            TrangThai = "ChoXacNhan",
            GhiChu = "Muốn xem phòng có ban công hướng Đông."
        },
        new LichHenViewModel {
            TenKhachHang = "Trần Thị B",
            SoDienThoai = "0905 123 456",
            TenPhong = "Phòng 305 - Căn hộ 2PN",
            NgayXem = DateTime.Now,
            GioXem = "16:00",
            TrangThai = "DaXacNhan"
        }
    };

            return View(model);
        }

        public ActionResult HopDong()
        {
            return View(new LandlordHopDongViewModel { HopDongList = new List<LandlordHopDongItemViewModel>() });
        }

        public ActionResult ChiTietHopDong(Guid? id)
        {
            var model = new ChiTietHopDongViewModel
            {
                HopDongId = id ?? Guid.NewGuid(),
                TenPhong = "P.101 - Phòng Studio Ban Công",
                DiaChiPhong = "123 Võ Văn Tần, Q.3, TP.HCM",
                TenKhach = "Nguyễn Văn A",
                GiaThue = 5500000,
                TrangThai = "HieuLuc"
            };
            return View(model);
        }
        public ActionResult DanhGia()
        {
            // Giả lập dữ liệu khớp với cấu trúc SQL
            var model = new ThongKeDanhGiaViewModel
            {
                DiemTrungBinh = 4.5,
                TongLuotDanhGia = 2,
                DanhSachDanhGia = new List<DanhGiaViewModel>
        {
            new DanhGiaViewModel {
                DanhGiaId = Guid.NewGuid(),
                TenNguoiDanhGia = "Nguyễn Văn A",
                TenPhong = "Phòng 102",
                Diem = 5,
                NoiDung = "Phòng sạch sẽ, rất hài lòng.",
                ThoiGian = DateTime.Now.AddDays(-1)
            },
            new DanhGiaViewModel {
                DanhGiaId = Guid.NewGuid(),
                TenNguoiDanhGia = "Trần Thị B",
                TenPhong = "Phòng 305",
                Diem = 4,
                NoiDung = "Chủ nhà nhiệt tình, phòng hơi cũ tí.",
                ThoiGian = DateTime.Now.AddDays(-3)
            }
        }
            };
            return View(model);
        }

        public ActionResult HoaDon()
        {
            return View(new List<LandlordHoaDonViewModel>());
        }

        public ActionResult ThongTinCaNhan()
        {
            return View(new LandlordProfileViewModel());
        }
    }
}