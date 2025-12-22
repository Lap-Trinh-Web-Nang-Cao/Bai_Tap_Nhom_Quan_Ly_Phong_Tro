using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Services;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Controller cho trang Dashboard Admin
    /// Hiển thị thống kê tổng quan, biểu đồ và hoạt động gần đây
    /// Token-based: check nếu có session Admin, không có thì redirect /
    /// </summary>
    [Filters.AllowAnonymous]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly AdminAuthService _authService;

        public DashboardController()
        {
            _dashboardService = new DashboardService();
            _authService = new AdminAuthService();
        }

        /// <summary>
        /// Trang Dashboard chính
        /// GET: /Dashboard
        /// Yêu cầu: Phải có token Admin hợp lệ trong session
        /// </summary>
        public async Task<ActionResult> Index()
        {
            // Kiểm tra Admin đã đăng nhập chưa
            if (!_authService.IsAdminLoggedIn())
            {
                System.Diagnostics.Debug.WriteLine("❌ Dashboard: Admin not logged in, redirecting to /");
                return Redirect("/");
            }

            try
            {
                // Lấy dữ liệu từ service (gọi API)
                var viewModel = await _dashboardService.GetDashboardDataAsync();
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Dashboard Error: {0}", ex.Message));
                
                ViewBag.Error = "Không thể tải dữ liệu dashboard. Vui lòng thử lại sau.";
                ViewBag.ErrorDetails = ex.Message;
                
                return View(new DashboardViewModel());
            }
        }

        /// <summary>
        /// API endpoint để refresh dashboard data (AJAX)
        /// GET: /Dashboard/Refresh
        /// Yêu cầu: Phải có token Admin hợp lệ
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> Refresh()
        {
            // Kiểm tra Admin
            if (!_authService.IsAdminLoggedIn())
            {
                return Json(new
                {
                    success = false,
                    message = "Admin session expired",
                    redirect = "/"
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var viewModel = await _dashboardService.GetDashboardDataAsync();
                
                return Json(new
                {
                    success = true,
                    data = viewModel
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Không thể refresh dữ liệu",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}