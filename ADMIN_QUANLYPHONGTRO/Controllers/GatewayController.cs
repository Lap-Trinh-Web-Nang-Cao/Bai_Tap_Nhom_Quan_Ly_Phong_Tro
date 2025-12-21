using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Services;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Gateway Controller - Nhận token từ USER project và auto-login
    /// </summary>
    [AllowAnonymous] // ✅ Gateway không cần auth
    public class GatewayController : Controller
    {
        private readonly AdminAuthService _authService = new AdminAuthService();

        /// <summary>
        /// Endpoint nhận token từ USER project (/Auth/Login)
        /// URL: /Gateway?token=eyJhbGc...
        /// </summary>
        [HttpGet]
        public ActionResult Index(string token = "")
        {
            try
            {
                // ✅ Step 1: Validate token không trống
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("❌ Gateway: Token trống");
                    return RedirectToAction("Login", "Auth");
                }

                Debug.WriteLine($"🔵 Gateway: Nhận token từ USER project");
                Debug.WriteLine($"📝 Token: {token?.Substring(0, 30)}...");

                // ✅ Step 2: Giải mã token để lấy thông tin user
                var userInfo = _authService.ExtractUserInfoFromToken(token);

                Debug.WriteLine($"📝 UserId: {userInfo.UserId}");
                Debug.WriteLine($"📝 Email: {userInfo.Email}");
                Debug.WriteLine($"📝 VaiTroId: {userInfo.VaiTroId}");

                // ✅ Step 3: Kiểm tra có phải Admin không (VaiTroId = 1)
                if (userInfo.VaiTroId != 1)
                {
                    Debug.WriteLine($"❌ Gateway: User không phải Admin (VaiTroId={userInfo.VaiTroId})");
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập Admin!";
                    return RedirectToAction("Login", "Auth");
                }

                Debug.WriteLine($"✅ Gateway: VaiTroId=1 (Admin) → Auto-login");

                // ✅ Step 4: Lưu vào Admin Session
                _authService.SaveAdminSessionFromToken(token, userInfo);

                Debug.WriteLine($"✅ Gateway: Session saved for {userInfo.Email}");

                // ✅ Step 5: Redirect đến Admin Dashboard
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Gateway Error: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi xác thực: {ex.Message}";
                return RedirectToAction("Login", "Auth");
            }
        }

        /// <summary>
        /// Alternative: Nhận token từ Cookie (nếu URL quá dài)
        /// </summary>
        [HttpPost]
        public ActionResult ProcessToken(string token = "")
        {
            // Giống Index nhưng dùng POST
            return Index(token);
        }
    }
}
