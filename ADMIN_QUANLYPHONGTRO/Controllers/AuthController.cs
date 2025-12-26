using System;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Services;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// AuthController - Xử lý token từ USER project (Gateway pattern)
    /// Admin không có login riêng, đăng nhập qua USER project rồi nhận token
    /// </summary>
    [Filters.AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly AdminAuthService _authService = new AdminAuthService();

        /// <summary>
        /// Endpoint nhận token từ USER project hoặc URL
        /// Nếu có token hợp lệ → lưu vào session → redirect Dashboard
        /// Nếu không có token hoặc invalid → redirect về guest (/)
        /// </summary>
        [HttpGet]
        public ActionResult Index(string token = "")
        {
            System.Diagnostics.Debug.WriteLine($"🔵 AuthController.Index - token received: {(string.IsNullOrEmpty(token) ? "EMPTY" : token.Substring(0, Math.Min(20, token.Length)) + "...")}");

            // Nếu có token trong URL
            if (!string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine($"✅ Processing token from URL...");
                return ProcessToken(token);
            }

            // Nếu đã có token trong session, redirect Dashboard
            if (_authService.IsAdminLoggedIn())
            {
                System.Diagnostics.Debug.WriteLine($"✅ Token already in session, redirecting to Dashboard...");
                return RedirectToAction("Index", "Dashboard");
            }

            // Không có token, redirect về guest area
            System.Diagnostics.Debug.WriteLine($"❌ No token found, redirecting to home...");
            return Redirect("/");
        }

        /// <summary>
        /// POST: Xử lý token từ request body (JSON)
        /// </summary>
        [HttpPost]
        public ActionResult Process(string token = "")
        {
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Token không được để trống" });
            }

            return ProcessToken(token);
        }

        /// <summary>
        /// Hàm xử lý token chung
        /// </summary>
        private ActionResult ProcessToken(string token)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("🔵 AuthController: Processing token {0}...", token.Substring(0, Math.Min(30, token.Length))));

                // Giải mã token
                var userInfo = _authService.ExtractUserInfoFromToken(token);

                System.Diagnostics.Debug.WriteLine(string.Format("📝 User: {0}, VaiTroId: {1}", userInfo.UserEmail, userInfo.VaiTroId));

                // Kiểm tra có phải Admin không (VaiTroId = 1)
                if (userInfo.VaiTroId != 1)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("❌ AuthController: User không phải Admin (VaiTroId={0})", userInfo.VaiTroId));
                    
                    // Không phải Admin, không lưu session, redirect về guest
                    TempData["WarningMessage"] = "Bạn không có quyền truy cập Admin panel";
                    return Redirect("/");
                }

                // Là Admin, lưu vào session
                _authService.SaveAdminSessionFromToken(token, userInfo);

                System.Diagnostics.Debug.WriteLine(string.Format("✅ AuthController: Admin session saved for {0}", userInfo.UserEmail));

                // Redirect tới Dashboard
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ AuthController Error: {0}", ex.Message));
                TempData["ErrorMessage"] = "Token không hợp lệ hoặc đã hết hạn";
                return Redirect("/");
            }
        }

        /// <summary>
        /// Đăng xuất - Xóa session Admin
        /// </summary>
        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔴 AuthController: Starting logout process");
                
                var currentSession = _authService.GetCurrentAdminSession();
                if (currentSession != null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("📝 Logging out: {0}", currentSession.UserEmail));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No active session found");
                }
                
                _authService.LogoutAdmin();
                
                System.Diagnostics.Debug.WriteLine("✅ Logout completed, redirecting to home");
                
                // Clear any temp data
                TempData.Clear();
                
                // Redirect to home page (USER project)
                return Redirect("~/");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ Logout Error: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace));
                // Even if error, try to redirect
                return Redirect("~/");
            }
        }

        /// <summary>
        /// API endpoint để check token hợp lệ hay không
        /// GET: /Auth/ValidateToken?token=...
        /// </summary>
        [HttpGet]
        public ActionResult ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { valid = false, message = "Token không được để trống" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var userInfo = _authService.ExtractUserInfoFromToken(token);
                var isAdmin = userInfo.VaiTroId == 1;
                var isExpired = userInfo.ExpiresAt <= DateTime.UtcNow;

                return Json(new
                {
                    valid = isAdmin && !isExpired,
                    isAdmin = isAdmin,
                    isExpired = isExpired,
                    userEmail = userInfo.UserEmail,
                    vaiTroId = userInfo.VaiTroId,
                    expiresAt = userInfo.ExpiresAt
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { valid = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
