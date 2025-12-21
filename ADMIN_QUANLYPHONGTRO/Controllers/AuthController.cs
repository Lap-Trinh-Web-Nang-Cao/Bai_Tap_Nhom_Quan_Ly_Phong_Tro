using System;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Services;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly AdminAuthService _authService = new AdminAuthService();

        // ===== TRANG ĐĂNG NHẬP ADMIN =====
        [HttpGet]
        public ActionResult Login(string returnUrl = "")
        {
            // Nếu đã đăng nhập rồi
            if (_authService.IsAdminLoggedIn())
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Login(string email, string password, string returnUrl = "")
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Email và mật khẩu không được để trống";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            try
            {
                // Gọi API để đăng nhập
                var loginResult = await _authService.LoginAsync(email, password);

                if (!loginResult.Success)
                {
                    ViewBag.ErrorMessage = loginResult.Message;
                    ViewBag.ReturnUrl = returnUrl;
                    return View();
                }

                // Lưu session
                var userInfo = _authService.ExtractUserInfoFromToken(loginResult.Token);
                _authService.SaveAdminSessionFromToken(loginResult.Token, userInfo);

                // Redirect
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi đăng nhập: {ex.Message}";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }

        // ===== ĐĂNG XUẤT ADMIN =====
        [HttpGet]
        public ActionResult Logout()
        {
            _authService.LogoutAdmin();
            return RedirectToAction("Login");
        }
    }

    /// <summary>
    /// Attribute cho phép bỏ qua xác thực
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}
