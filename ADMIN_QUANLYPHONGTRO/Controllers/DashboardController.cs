using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Controller cho trang Dashboard Admin
    /// Hiển thị thống kê tổng quan, biểu đồ và hoạt động gần đây
    /// </summary>
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController()
        {
            _dashboardService = new DashboardService();
        }

        /// <summary>
        /// Trang Dashboard chính
        /// GET: /Dashboard
        /// </summary>
        public async Task<ActionResult> Index()
        {
            try
            {
                // Lấy dữ liệu từ service (gọi API)
                var viewModel = await _dashboardService.GetDashboardDataAsync();
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Dashboard Error: {ex.Message}");
                
                ViewBag.Error = "Không thể tải dữ liệu dashboard. Vui lòng thử lại sau.";
                ViewBag.ErrorDetails = ex.Message;
                
                // Trả về view với model rỗng
                return View(new DashboardViewModel());
            }
        }

        /// <summary>
        /// API endpoint để refresh dashboard data (AJAX)
        /// GET: /Dashboard/Refresh
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> Refresh()
        {
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