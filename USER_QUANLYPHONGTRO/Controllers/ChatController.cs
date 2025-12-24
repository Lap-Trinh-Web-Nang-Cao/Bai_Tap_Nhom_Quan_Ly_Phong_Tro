using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat/PhongChat
        public ActionResult PhongChat()
        {
            // Kiểm tra user đã đăng nhập
            var userId = Session["UserId"];
            var userName = Session["HoTen"] as string ?? Session["UserName"] as string;
            var userRole = Session["VaiTroId"] as string ?? "3"; // Default: Người thuê
            
            if (string.IsNullOrEmpty(userId?.ToString()))
            {
                // Redirect to login if not authenticated
                return RedirectToAction("Login", "Auth", new { returnUrl = Request.Url.PathAndQuery });
            }

            // Get API base URL from config
            var apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7039";
            
            ViewBag.UserId = userId.ToString();
            ViewBag.UserName = userName ?? "Người dùng";
            ViewBag.UserRole = userRole;
            ViewBag.ApiBaseUrl = apiBaseUrl;

            System.Diagnostics.Debug.WriteLine($"✅ Chat.PhongChat - UserId: {userId}, UserName: {userName}");

            return View();
        }

        // AJAX: Get contact list
        [HttpGet]
        public ActionResult GetContacts(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return Json(new { success = false, message = "UserId required" }, JsonRequestBehavior.AllowGet);

                System.Diagnostics.Debug.WriteLine($"📡 Chat.GetContacts - UserId: {userId}");
                
                // NOTE: This should call the API endpoint /api/chat/contacts
                // For now, returning empty for testing
                var contacts = new List<dynamic>();
                
                return Json(new { success = true, data = contacts }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Chat.GetContacts Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Index redirect to PhongChat
        public ActionResult Index()
        {
            return RedirectToAction("PhongChat");
        }
    }
}