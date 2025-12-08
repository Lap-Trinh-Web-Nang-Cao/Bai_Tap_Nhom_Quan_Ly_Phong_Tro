using System;
using System.Collections.Generic;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class ChuTroController : Controller
    {
        /// <summary>
        /// Kiểm tra xem user có phải Chủ Trọ không
        /// </summary>
        private bool CheckChuTroRole()
        {
            var role = Session["UserRole"]?.ToString();
            return role == "ChuTro";
        }

        /// <summary>
        /// GET: ChuTro - Chuyển hướng tới Dashboard
        /// </summary>
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        /// <summary>
        /// GET: ChuTro/Dashboard - Trang tổng quan chủ trọ
        /// </summary>
        public ActionResult Dashboard()
        {
            if (!CheckChuTroRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "chutro" });
            }

            var model = new LandlordDashboardViewModel
            {
                TotalRooms = 24,
                ViewsToday = 156,
                UpcomingSchedules = 8,
                RevenueThisMonth = 45200000,
                PendingRooms = GetPendingRooms(),
                TodaySchedules = GetTodaySchedules(),
                RecentMessages = GetRecentMessages(),
                MaintenanceRequests = GetMaintenanceRequests()
            };

            ViewBag.Title = "Tổng quan";
            return View(model);
        }

        /// <summary>
        /// GET: ChuTro/QuanLyPhong - Trang quản lý phòng
        /// </summary>
        public ActionResult QuanLyPhong()
        {
            if (!CheckChuTroRole())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Title = "Quản lý phòng";
            ViewBag.Message = "Tính năng đang phát triển";
            return View();
        }

        /// <summary>
        /// Lấy danh sách phòng chờ duyệt (Mock Data)
        /// </summary>
        private List<PendingRoomItem> GetPendingRooms()
        {
            return new List<PendingRoomItem>
            {
                new PendingRoomItem
                {
                    Name = "Phòng trọ ABC - Quận 1",
                    ImageUrl = "/images/logo.png",
                    SubmitDate = DateTime.Now.AddHours(-2)
                },
                new PendingRoomItem
                {
                    Name = "Phòng VIP Quận 3",
                    ImageUrl = "/images/logo.png",
                    SubmitDate = DateTime.Now.AddHours(-5)
                },
                new PendingRoomItem
                {
                    Name = "Căn hộ mini Quận 7",
                    ImageUrl = "/images/logo.png",
                    SubmitDate = DateTime.Now.AddDays(-1)
                }
            };
        }

        /// <summary>
        /// Lấy danh sách lịch xem phòng hôm nay (Mock Data)
        /// </summary>
        private List<TodayScheduleItem> GetTodaySchedules()
        {
            return new List<TodayScheduleItem>
            {
                new TodayScheduleItem
                {
                    TenantName = "Nguyễn Văn A",
                    RoomName = "Phòng 101",
                    ViewTime = "09:00",
                    Status = "Chờ xác nhận"
                },
                new TodayScheduleItem
                {
                    TenantName = "Trần Thị B",
                    RoomName = "Phòng 205",
                    ViewTime = "14:00",
                    Status = "Đã xác nhận"
                },
                new TodayScheduleItem
                {
                    TenantName = "Lê Văn C",
                    RoomName = "Phòng 303",
                    ViewTime = "16:30",
                    Status = "Chờ xác nhận"
                }
            };
        }

        /// <summary>
        /// Lấy danh sách tin nhắn gần đây (Mock Data)
        /// </summary>
        private List<RecentMessageItem> GetRecentMessages()
        {
            return new List<RecentMessageItem>
            {
                new RecentMessageItem
                {
                    SenderName = "Nguyễn Văn A",
                    Avatar = "/images/logo.png",
                    Content = "Cho mình hỏi phòng còn trống không ạ?",
                    Time = "5 phút trước"
                },
                new RecentMessageItem
                {
                    SenderName = "Trần Thị B",
                    Avatar = "/images/logo.png",
                    Content = "Khi nào có thể xem phòng được ạ?",
                    Time = "15 phút trước"
                },
                new RecentMessageItem
                {
                    SenderName = "Lê Văn C",
                    Avatar = "/images/logo.png",
                    Content = "Phòng có gửi xe không anh?",
                    Time = "1 giờ trước"
                }
            };
        }

        /// <summary>
        /// Lấy danh sách yêu cầu sửa chữa (Mock Data)
        /// </summary>
        private List<MaintenanceRequestItem> GetMaintenanceRequests()
        {
            return new List<MaintenanceRequestItem>
            {
                new MaintenanceRequestItem
                {
                    RoomName = "Phòng 101",
                    ReporterName = "Nguyễn Văn A",
                    Title = "Điều hòa hỏng",
                    Priority = "Cao"
                },
                new MaintenanceRequestItem
                {
                    RoomName = "Phòng 205",
                    ReporterName = "Trần Thị B",
                    Title = "Bồn cầu bị tắc",
                    Priority = "Trung bình"
                },
                new MaintenanceRequestItem
                {
                    RoomName = "Phòng 303",
                    ReporterName = "Lê Văn C",
                    Title = "Thay bóng đèn",
                    Priority = "Thấp"
                }
            };
        }

        /// <summary>
        /// AJAX: Lấy dữ liệu biểu đồ doanh thu
        /// </summary>
        [HttpGet]
        public JsonResult GetRevenueChart(string filter = "30days")
        {
            var data = new
            {
                labels = new[] { "01/12", "05/12", "10/12", "15/12", "20/12", "25/12", "30/12" },
                values = new[] { 5200000, 6800000, 5500000, 7200000, 6100000, 8300000, 7500000 }
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AJAX: Lấy dữ liệu biểu đồ lượt xem
        /// </summary>
        [HttpGet]
        public JsonResult GetViewsChart()
        {
            var data = new
            {
                labels = new[] { "Phòng 101", "Phòng 205", "Phòng 303", "Phòng 402", "Phòng 501" },
                values = new[] { 45, 38, 32, 28, 25 }
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AJAX: Làm mới dữ liệu dashboard
        /// </summary>
        [HttpGet]
        public JsonResult RefreshData(string filter = "30days")
        {
            if (!CheckChuTroRole())
            {
                return Json(new { success = false, message = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }

            var data = new
            {
                success = true,
                totalRooms = 24,
                viewsToday = 156,
                upcomingSchedules = 8,
                revenueThisMonth = 45200000
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}