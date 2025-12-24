using System;
using System.Configuration;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.Auth;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService = new AuthService();

        // ===== TRANG ĐĂNG NHẬP (THỐNG NHẤT) =====
        [HttpGet]
        public ActionResult Login(string returnUrl = "")
        {
            // Nếu đã đăng nhập rồi thì redirect về trang chủ tương ứng
            if (_authService.IsUserLoggedIn())
            {
                return RedirectToHome();
            }

            ViewBag.ReturnUrl = returnUrl;
            // Use explicit view path to avoid view resolution issues
            return View("~/Views/auth/Login.cshtml", new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Vui lòng kiểm tra lại thông tin";
                ViewBag.ReturnUrl = returnUrl;
                return View("~/Views/auth/Login.cshtml", model);
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 Login attempt for: {model.Email}");
                
                // Gọi API để đăng nhập
                var loginResult = await _authService.LoginAsync(model.Email, model.Password);

                if (!loginResult.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Login failed: {loginResult.Message}");
                    ViewBag.ErrorMessage = loginResult.Message;
                    ViewBag.ReturnUrl = returnUrl;
                    return View("~/Views/auth/Login.cshtml", model);
                }

                System.Diagnostics.Debug.WriteLine($"✅ Login API success, saving session...");
                
                // Lưu thông tin vào Session
                var userInfo = new AuthService().GetUserInfoFromToken(loginResult.Token);
                _authService.SaveUserSessionFromToken(loginResult.Token, userInfo);

                System.Diagnostics.Debug.WriteLine($"✅ Session saved for user: {userInfo.Email}, Role: {userInfo.VaiTroId}");
                
                // ✅ Force client to reload with session by using TempData
                TempData["LoginSuccess"] = true;
                TempData["UserName"] = userInfo.Email;
                
                // Điều hướng dựa trên vai trò
                var redirectResult = RedirectToHome();
                
                System.Diagnostics.Debug.WriteLine($"✅ Redirecting to home...");
                
                return redirectResult;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Login exception: {ex.Message}\n{ex.StackTrace}");
                ViewBag.ErrorMessage = $"Lỗi đăng nhập: {ex.Message}";
                ViewBag.ReturnUrl = returnUrl;
                return View("~/Views/auth/Login.cshtml", model);
            }
        }

        // ===== TRANG ĐĂNG KÝ NGƯỜI THUÊ =====
        [HttpGet]
        public ActionResult RegisterNguoiThue()
        {
            return View(new RegisterViewModel { UserType = "NguoiThue" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> RegisterNguoiThue(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Vui lòng kiểm tra lại thông tin";
                return View(model);
            }

            try
            {
                // TODO: Gọi API /nguoidung/register để đăng ký
                // Tạm thời hiển thị thông báo thành công
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", new { type = "nguoithue" });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi đăng ký: {ex.Message}";
                return View(model);
            }
        }

        // ===== TRANG ĐĂNG KÝ CHỦ TRỌ =====
        [HttpGet]
        public ActionResult RegisterChuTro()
        {
            return View(new RegisterViewModel { UserType = "ChuTro" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> RegisterChuTro(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Vui lòng kiểm tra lại thông tin";
                return View(model);
            }

            try
            {
                // TODO: Gọi API /nguoidung/register để đăng ký chủ trọ
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đợi xác minh danh tính.";
                return RedirectToAction("Login", new { type = "chutro" });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi đăng ký: {ex.Message}";
                return View(model);
            }
        }

        // ===== ĐĂNG XUẤT =====
        [HttpGet]
        public ActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("Index", "Home");
        }

        // ===== QUÊN MẬT KHẨU =====
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // ===== HÀM HELPER: ĐIỀU HƯỚNG VỀ TRANG CHỦ PHÙ HỢP =====
        private ActionResult RedirectToHome()
        {
            var userSession = _authService.GetCurrentUserSession();

            if (userSession == null)
            {
                return RedirectToAction("Login");
            }

            // Điều hướng dựa trên vai trò
            switch (userSession.VaiTroId)
            {
                case 1: // Admin → Redirect sang ADMIN project Gateway
                    // ✅ Redirect đến Admin Gateway (auto-login)
                    var adminGatewayUrl = ConfigurationManager.AppSettings["AdminGatewayUrl"]
                        ?? "https://18.140.64.80:5000/Gateway";
                    var redirectUrl = $"{adminGatewayUrl}?token="
                        + System.Web.HttpUtility.UrlEncode(userSession.AuthToken);

                    System.Diagnostics.Debug.WriteLine($"🔵 Admin redirect → {adminGatewayUrl}");
                    return Redirect(redirectUrl);

                case 2: // Chủ Trọ
                    return RedirectToAction("Dashboard", "ChuTro");

                case 3: // Người Thuê (mặc định)
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
    }
}