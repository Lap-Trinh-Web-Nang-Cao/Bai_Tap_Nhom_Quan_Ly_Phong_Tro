using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class ChuTroController : Controller
    {
        // Kiểm tra xem user có phải Chủ Trọ không
        private bool CheckChuTroRole()
        {
            var role = Session["UserRole"]?.ToString();
            if (role != "ChuTro")
            {
                return false;
            }
            return true;
        }

        // Dashboard - Trang chủ Seller Center
        public ActionResult Dashboard()
        {
            if (!CheckChuTroRole())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Title = "Tổng quan";
            return View();
        }

        // Quản lý phòng
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

        // GET: ChuTro
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
    }
}