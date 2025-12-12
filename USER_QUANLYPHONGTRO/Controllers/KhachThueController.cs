using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Controller cho NGƯỜI THUÊ ĐÃ ĐĂNG NHẬP
    /// Yêu cầu: UserRole = "KhachThue"
    /// Khách vãng lai xem phòng → dùng GuestController
    /// </summary>
    //[Authorize] // Tất cả actions đều yêu cầu login
    public class KhachThueController : Controller
    {
        private readonly ApiClient _apiClient;

        public KhachThueController()
        {
            _apiClient = new ApiClient();
        }

        // Kiểm tra có đúng role Người thuê không
        private bool CheckKhachThueRole()
        {
            var role = Session["UserRole"]?.ToString();
            return role == "KhachThue";
        }

        // GET: /KhachThue → Welcome page
        public async Task<ActionResult> Index()
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            try
            {
                // Get featured rooms (top rated, limit 6)
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6&sortBy=rating");

                if (response.Success && response.Data != null)
                {
                    ViewBag.FeaturedRooms = response.Data.Data ?? new List<PhongDto>();
                }
                else
                {
                    ViewBag.FeaturedRooms = new List<PhongDto>();
                }
            }
            catch
            {
                ViewBag.FeaturedRooms = new List<PhongDto>();
            }

            // Show welcome page
            ViewBag.Title = "Chào mừng";
            return View();
        }

        // Dashboard người thuê
        public ActionResult Dashboard()
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            // TODO: Lấy dữ liệu thực từ API khi có endpoints
            var model = new TenantDashboardViewModel
            {
                TenNguoiThue = Session["HoTen"] as string ?? "Người thuê",
                Email = Session["UserName"] as string ?? "unknown@example.com",

                // Tạm thời để 0, chờ API
                SoPhongDaXem = 0,
                SoLichHenSapToi = 0,
                SoHopDongDangHieuLuc = 0,
                SoHoaDonChuaThanhToan = 0,

                // Empty lists - chờ API endpoints
                LichHenSapToi = new List<TenantScheduleItem>(),
                HopDongHieuLuc = null,
                HoaDonGanDay = new List<TenantInvoiceItem>()
            };

            ViewBag.Title = "Trang chủ người thuê";
            return View(model);
        }

        // Danh sách phòng (lấy từ API - số ít)
        public async Task<ActionResult> DanhSachPhong()
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            try
            {
                // Lấy 4 phòng demo
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=4");

                if (response.Success && response.Data != null)
                {
                    var rooms = response.Data.Data ?? new List<PhongDto>();
                    ViewBag.Title = "Danh sách phòng";
                    return View(rooms);
                }

                return View(new List<PhongDto>());
            }
            catch
            {
                return View(new List<PhongDto>());
            }
        }

        // Chi tiết phòng
        public async Task<ActionResult> ChiTietPhong(Guid? id)
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            if (!id.HasValue)
            {
                return RedirectToAction("DanhSachPhong");
            }

            try
            {
                var response = await _apiClient.GetAsync<PhongDto>($"/api/phong/{id.Value}");

                if (response.Success && response.Data != null)
                {
                    ViewBag.Title = "Chi tiết phòng";
                    return View(response.Data);
                }

                return RedirectToAction("DanhSachPhong");
            }
            catch
            {
                return RedirectToAction("DanhSachPhong");
            }
        }

        // Đặt phòng (Form)
        public ActionResult DatPhong(Guid? roomId)
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            if (!roomId.HasValue)
            {
                return RedirectToAction("DanhSachPhong");
            }

            ViewBag.RoomId = roomId;
            ViewBag.Title = "Đặt lịch xem phòng";
            return View();
        }

        // Xử lý đặt phòng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatPhong(FormCollection form)
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            try
            {
                // TODO: Gọi API POST /api/datphong
                TempData["SuccessMessage"] = "Đặt lịch xem phòng thành công! Chủ trọ sẽ liên hệ với bạn sớm.";
                return RedirectToAction("LichDaDat");
            }
            catch
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại.";
                return RedirectToAction("DanhSachPhong");
            }
        }

        // Lịch đã đặt
        public ActionResult LichDaDat()
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            // TODO: Lấy từ API /api/datphong khi có
            var list = new List<TenantScheduleItem>();
            return View(list);
        }

        // Hợp đồng
        public ActionResult HopDong()
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            // TODO: Lấy từ API /api/hopdong khi có
            var list = new List<TenantContractItem>();
            return View(list);
        }

        // Hóa đơn
        public ActionResult HoaDon()
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            // TODO: Lấy từ API /api/hoadon khi có
            var list = new List<TenantInvoiceItem>();
            return View(list);
        }

        // Thông tin cá nhân
        public ActionResult ThongTinCaNhan()
        {
            //if (!CheckKhachThueRole())
            //{
            //    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            //}

            ViewBag.Message = "Trang thông tin cá nhân đang được phát triển.";
            return View();
        }

        // ================== HELPER METHODS (if needed) ==================
        // Mock data removed - Use real API calls instead
    }
}
