using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Mặc định là khách vãng lai (không đăng nhập)
            // Session.Clear();

            return View();
        }

        // Action danh sách phòng (placeholder)
        public ActionResult DanhSachPhong(string keyword, string priceRange, string areaRange, string roomType, bool? featured, int? minArea, string sort, bool? verified)
        {
            ViewBag.Message = "Tính năng đang được phát triển";
            ViewBag.Keyword = keyword;
            ViewBag.PriceRange = priceRange;
            
            // Tạm thời redirect về trang chủ
            return RedirectToAction("Index");
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
    }
}