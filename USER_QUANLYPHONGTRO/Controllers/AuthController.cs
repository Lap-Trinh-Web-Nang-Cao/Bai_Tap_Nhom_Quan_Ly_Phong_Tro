using System;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.Auth;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class AuthController : Controller
    {
        // ===== TRANG ĐĂNG NHẬP =====
        [HttpGet]
        public ActionResult Login(string type = "nguoithue")
        {
            ViewBag.UserType = type;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string userType = "nguoithue")
        {
            if (ModelState.IsValid)
            {
                bool isChuTro = userType.ToLower() == "chutro";

                // FAKE LOGIN - Dùng để test layout
                if (isChuTro || model.Email.ToLower().Contains("chutro"))
                {
                    // Giả lập đăng nhập Chủ Trọ
                    Session["UserName"] = model.Email;
                    Session["HoTen"] = "Nguyễn Văn A (Chủ Trọ)";
                    Session["UserRole"] = "ChuTro";
                    Session["IsVerified"] = true;
                    Session["NotificationCount"] = 5;
                    Session["MessageCount"] = 3;

                    // REDIRECT ĐẾN TRANG CHỦ TRỌ (SELLER CENTER)
                    return RedirectToAction("Dashboard", "ChuTro");
                }
                else
                {
                    // Giả lập đăng nhập Người Thuê
                    Session["UserName"] = model.Email;
                    Session["HoTen"] = "Trần Thị B (Người Thuê)";
                    Session["UserRole"] = "KhachThue";
                    Session["IsVerified"] = true;
                    Session["NotificationCount"] = 2;
                    Session["MessageCount"] = 1;

                    // REDIRECT VỀ TRANG CHỦ NGƯỜI THUÊ
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ErrorMessage = "Email hoặc mật khẩu không chính xác";
            ViewBag.UserType = userType;
            return View(model);
        }

        // ===== TRANG ĐĂNG KÝ NGƯỜI THUÊ =====
        [HttpGet]
        public ActionResult NguoiThue_Register()
        {
            return View(new RegisterViewModel { UserType = "KhachThue" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterNguoiThue(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.SuccessMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", new { type = "nguoithue" });
            }

            ViewBag.ErrorMessage = "Vui lòng kiểm tra lại thông tin";
            return View("NguoiThue_Register", model);
        }

        // ===== TRANG ĐĂNG KÝ CHỦ TRỌ =====
        [HttpGet]
        public ActionResult ChuTro_Register()
        {
            return View(new RegisterViewModel { UserType = "ChuTro" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterChuTro(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.SuccessMessage = "Đăng ký thành công! Vui lòng đợi xác minh danh tính.";
                return RedirectToAction("Login", new { type = "chutro" });
            }

            ViewBag.ErrorMessage = "Vui lòng kiểm tra lại thông tin";
            return View("ChuTro_Register", model);
        }

        // ===== ĐĂNG XUẤT =====
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ===== QUÊN MẬT KHẨU =====
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}