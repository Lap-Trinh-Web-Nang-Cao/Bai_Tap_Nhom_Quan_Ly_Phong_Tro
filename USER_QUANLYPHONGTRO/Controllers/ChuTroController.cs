using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Controller cho CHỦ TRỌ ĐÃ ĐĂNG NHẬP  
    /// Yêu cầu: UserRole = "ChuTro"
    /// Sử dụng API thật từ backend
    /// </summary>
    [Authorize]
    public class ChuTroController : Controller
    {
        private readonly ApiClient _apiClient;

        public ChuTroController()
        {
            _apiClient = new ApiClient();
        }

        #region Helper Methods

        private bool CheckChuTroRole()
        {
            var role = Session["UserRole"]?.ToString();
            return role == "ChuTro";
        }

        private Guid? GetCurrentUserId()
        {
            var userIdStr = Session["UserId"]?.ToString();
            if (Guid.TryParse(userIdStr, out Guid userId))
            {
                return userId;
            }
            return null;
        }

        #endregion

        #region Main Actions

        public ActionResult Index()
        {
            if (!CheckChuTroRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "chutro" });
            }

            return RedirectToAction("Dashboard");
        }

        public async Task<ActionResult> Dashboard()
        {
            if (!CheckChuTroRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "chutro" });
            }

            try
            {
                var roomsResponse = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=100");
                var totalRooms = 0;

                if (roomsResponse.Success && roomsResponse.Data != null)
                {
                    totalRooms = roomsResponse.Data.TotalCount ?? 0;
                }

                var model = new LandlordDashboardViewModel
                {
                    TotalRooms = totalRooms,
                    ViewsToday = 156,
                    UpcomingSchedules = 8,
                    RevenueThisMonth = 45200000,
                    PendingRooms = new List<PendingRoomItem>(),
                    TodaySchedules = new List<TodayScheduleItem>(),
                    RecentMessages = new List<RecentMessageItem>(),
                    MaintenanceRequests = new List<MaintenanceRequestItem>()
                };

                ViewBag.Title = "Tổng quan";
                ViewBag.ChuTroName = Session["HoTen"] ?? "Chủ trọ";
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi: {ex.Message}";
                return View(new LandlordDashboardViewModel());
            }
        }

        #endregion

        #region Quản Lý Phòng

        public async Task<ActionResult> QuanLyPhong(int page = 1, int pageSize = 20)
        {
            if (!CheckChuTroRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "chutro" });
            }

            try
            {
                var response = await _apiClient.GetAsync<dynamic>($"/api/phong?page={page}&pageSize={pageSize}");

                if (response.Success && response.Data != null)
                {
                    var rooms = response.Data.Data ?? new List<PhongDto>();
                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = response.Data.TotalPages ?? 1;
                    ViewBag.TotalItems = response.Data.TotalCount ?? 0;
                    ViewBag.Title = "Quản lý phòng";

                    return View(rooms);
                }
                else
                {
                    ViewBag.ErrorMessage = response.Message ?? "Không thể tải danh sách phòng";
                    return View(new List<PhongDto>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi: {ex.Message}";
                return View(new List<PhongDto>());
            }
        }

        public async Task<ActionResult> ChiTietPhong(Guid? id)
        {
            if (!CheckChuTroRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "chutro" });
            }

            if (!id.HasValue)
            {
                return RedirectToAction("QuanLyPhong");
            }

            try
            {
                var response = await _apiClient.GetAsync<PhongDto>($"/api/phong/{id.Value}");

                if (response.Success && response.Data != null)
                {
                    ViewBag.Title = "Chi tiết phòng";
                    return View(response.Data);
                }
                else
                {
                    return RedirectToAction("QuanLyPhong");
                }
            }
            catch
            {
                return RedirectToAction("QuanLyPhong");
            }
        }

        #endregion

        #region AJAX

        [HttpGet]
        public JsonResult GetRevenueChart(string filter = "30days")
        {
            if (!CheckChuTroRole())
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            var data = new
            {
                success = true,
                labels = new[] { "01/12", "05/12", "10/12", "15/12", "20/12", "25/12", "30/12" },
                values = new[] { 5200000, 6800000, 5500000, 7200000, 6100000, 8300000, 7500000 }
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetViewsChart()
        {
            if (!CheckChuTroRole())
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            var data = new
            {
                success = true,
                labels = new[] { "Phòng 101", "Phòng 205", "Phòng 303", "Phòng 402", "Phòng 501" },
                values = new[] { 45, 38, 32, 28, 25 }
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> RefreshData(string filter = "30days")
        {
            if (!CheckChuTroRole())
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var roomsResponse = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=1");
                var totalRooms = 0;

                if (roomsResponse.Success && roomsResponse.Data != null)
                {
                    totalRooms = roomsResponse.Data.TotalCount ?? 0;
                }

                var data = new
                {
                    success = true,
                    totalRooms = totalRooms,
                    viewsToday = 156,
                    upcomingSchedules = 8,
                    revenueThisMonth = 45200000
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
