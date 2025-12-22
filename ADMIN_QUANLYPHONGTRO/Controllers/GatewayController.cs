using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Services;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// GatewayController - Nhận token từ USER project và redirect sang AuthController
    /// URL: /Gateway?token=eyJhbGc...
    /// </summary>
    [Filters.AllowAnonymous]
    public class GatewayController : Controller
    {
        /// <summary>
        /// Endpoint nhận token từ USER project (/Auth/Login)
        /// URL: /Gateway?token=eyJhbGc...
        /// </summary>
        [HttpGet]
        public ActionResult Index(string token = "")
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("❌ Gateway: Token trống");
                    return RedirectToAction("Index", "Auth");
                }

                Debug.WriteLine("🔵 Gateway: Nhận token từ USER project");
                Debug.WriteLine(string.Format("📝 Token: {0}...", token.Length > 30 ? token.Substring(0, 30) : token));

                // Redirect sang AuthController xử lý
                return RedirectToAction("Index", "Auth", new { token = token });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("❌ Gateway Error: {0}", ex.Message));
                return RedirectToAction("Index", "Auth");
            }
        }

        /// <summary>
        /// Alternative: Nhận token từ POST
        /// </summary>
        [HttpPost]
        public ActionResult Process(string token = "")
        {
            return Index(token);
        }
    }
}
