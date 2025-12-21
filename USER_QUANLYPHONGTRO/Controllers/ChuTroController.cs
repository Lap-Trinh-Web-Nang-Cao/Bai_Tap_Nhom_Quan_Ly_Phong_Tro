using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class ChuTroController : Controller
    {
        private readonly ApiClient _apiClient;

        public ChuTroController()
        {
            _apiClient = new ApiClient();
        }

        // --- MIDDLEWARE KIỂM TRA QUYỀN (QUAN TRỌNG) ---
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            // 1. Kiểm tra đăng nhập
            if (session["UserId"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Auth");
                return;
            }

            // 2. Kiểm tra Role
            // LƯU Ý QUAN TRỌNG: Session["UserRole"] đang lưu số nguyên (int) là 2
            // Nếu so sánh với chuỗi "CHUTRO" hoặc "ChuTro" sẽ bị SAI -> Dẫn đến bị đá về Home

            var role = session["UserRole"];
            string roleStr = role?.ToString(); // Chuyển sang chuỗi để dễ so sánh: "2"

            // Kiểm tra: Nếu KHÔNG PHẢI là "2" VÀ KHÔNG PHẢI "CHUTRO" thì mới đá ra
            if (roleStr != "2" && roleStr?.ToUpper() != "CHUTRO")
            {
                // Đây chính là nguyên nhân bạn bị đá về Home
                filterContext.Result = RedirectToAction("Index", "Home");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        // GET: Dashboard
        public async Task<ActionResult> Dashboard()
        {
            string token = Session["AccessToken"] as string;

            try
            {
                // Gọi API lấy thống kê (nếu chưa có API thì dùng dữ liệu mẫu)
                // var stats = await _apiClient.GetAsync<LandlordDashboardViewModel>("api/dashboard/stats", token);

                // --- DỮ LIỆU MẪU ĐỂ TEST GIAO DIỆN ---
                var model = new LandlordDashboardViewModel
                {
                    TongSoPhong = 15,
                    
                    DoanhThuThang = 25000000,
                };
                // -------------------------------------

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải Dashboard: " + ex.Message;
                return View(new LandlordDashboardViewModel());
            }
        }


        // ============================================================
        // 2. QUẢN LÝ PHÒNG (DANH SÁCH + CHỜ DUYỆT + NÚT THÊM)
        // ============================================================
        public async Task<ActionResult> QuanLyPhong()
        {
            // 1. Kiểm tra đăng nhập
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");

            // Parse an toàn hơn
            if (!Guid.TryParse(Session["UserId"].ToString(), out Guid userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // 2. Gọi API và Deserialize thẳng sang ViewModel
                // Cách này tự động khớp "phongId" (API) vào "PhongId" (C#) -> Không bị lỗi null
                var listPhong = await _apiClient.GetAsync<List<QuanLyPhongViewModel>>($"api/phong/landlord/{userId}");

                // Nếu API trả về null hoặc rỗng thì khởi tạo list mới
                return View(listPhong ?? new List<QuanLyPhongViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải dữ liệu: " + ex.Message;
                return View(new List<QuanLyPhongViewModel>());
            }
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



        public ActionResult ChiTietHopDong(Guid id)
        {
            // Giả lập lấy dữ liệu chi tiết từ Database theo ID
            var model = new ChiTietHopDongViewModel
            {
                HopDongId = id,
                SoHopDong = "HD-2025-001",
                TrangThai = "DangHieuLuc",
                NgayLap = new DateTime(2025, 1, 1),
                TenNguoiThue = "Nguyễn Văn A",
                SoDienThoai = "0987 654 321",
                CCCD = "012345678901",
                QueQuan = "Hà Nội",
                TenPhong = "Phòng 101 - Studio",
                DienTich = 25.5,
                GiaThue = 3500000,
                TienCoc = 3500000,
                NgayBatDau = new DateTime(2025, 1, 1),
                NgayKetThuc = new DateTime(2025, 12, 31),
                KyThanhToan = 1,
                GhiChu = "Khách thuê cam kết ở ít nhất 6 tháng."
            };

            return View(model);
        }
        // GET: ChuTro/TaoHopDong
        public ActionResult TaoHopDong()
        {
            // Bạn có thể lấy danh sách phòng trống và khách hàng từ DB để đổ vào DropdownList tại đây
            return View();
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
        public ActionResult HopDong()
        {
            var model = new LandlordHopDongViewModel
            {
                TongHopDongHieuLuc = 2,
                HopDongSapHetHan = 1,
                DanhSachHopDong = new List<HopDongItemViewModel>
        {
            new HopDongItemViewModel {
                HopDongId = Guid.NewGuid(),
                SoHopDong = "HD-2025-001",
                TenPhong = "Phòng 101",
                TenNguoiThue = "Nguyễn Văn A",
                NgayBatDau = new DateTime(2025, 1, 1),
                NgayKetThuc = new DateTime(2025, 12, 31),
                GiaThue = 3500000,
                TienCoc = 3500000,
                TrangThai = "DangHieuLuc"
            },
            new HopDongItemViewModel {
                HopDongId = Guid.NewGuid(),
                SoHopDong = "HD-2024-015",
                TenPhong = "Phòng 202",
                TenNguoiThue = "Trần Thị B",
                NgayBatDau = new DateTime(2024, 6, 15),
                NgayKetThuc = new DateTime(2025, 2, 15), // Sắp hết hạn
                GiaThue = 4000000,
                TienCoc = 4000000,
                TrangThai = "SapHetHan"
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