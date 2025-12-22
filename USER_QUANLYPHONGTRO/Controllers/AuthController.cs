using System;
using System.IdentityModel.Tokens.Jwt; // Cần thiết để đọc Role từ Token
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Auth;
using USER_QUANLYPHONGTRO.Models.ViewModels.Auth;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiClient _apiClient;

        public AuthController()
        {
            _apiClient = new ApiClient();
        }

        [HttpGet]
        public ActionResult Login(string type = "nguoithue")
        {
            // Nếu đã đăng nhập, tự động chuyển hướng
            if (Session["UserId"] != null)
            {
                return RedirectBasedOnRole(Session["UserRole"]?.ToString());
            }
            ViewBag.UserType = type;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string userType = "nguoithue")
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var loginPayload = new { Email = model.Email, Password = model.Password };

                // 1. Gọi API
                var user = await _apiClient.PostAsync<object, LoginResponseDto>("api/nguoidung/login", loginPayload);

                System.Diagnostics.Debug.WriteLine($"API Trả về VaiTroId: {user?.VaiTroId}");

                // 2. Kiểm tra dữ liệu trả về
                if (user != null && user.NguoiDungId != Guid.Empty)
                {
                    // 3. LƯU SESSION
                    Session["UserId"] = user.NguoiDungId;
                    Session["UserName"] = user.Email;
                    Session["UserRole"] = user.VaiTroId;
                    Session["HoTen"] = user.HoTen;

                    // --- SỬA QUAN TRỌNG TẠI ĐÂY ---
                    // Lưu Token thật từ API vào Session
                    Session["AccessToken"] = user.Token; 
                    // ------------------------------

                    System.Diagnostics.Debug.WriteLine($"---> [LOGIN SUCCESS] Token saved: {user.Token?.Substring(0, 10)}...");

                    // 4. ĐIỀU HƯỚNG
                    if (user.VaiTroId == 2) // Chủ trọ
                    {
                        return RedirectToAction("Dashboard", "ChuTro");
                    }
                    else if (user.VaiTroId == 1) // Admin
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else // Khách thuê
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi đăng nhập: " + ex.Message;
            }

            ViewBag.UserType = userType;
            return View(model);
        }

        private ActionResult RedirectBasedOnRole(string roleId)
        {
            // Chuyển về string để so sánh an toàn
            string r = roleId?.ToString() ?? "";

            // Kiểm tra Role Chủ Trọ (Là số "2")
            if (r == "2")
            {
                return RedirectToAction("Dashboard", "ChuTro");
            }

            // Kiểm tra Admin (Là số "1")
            if (r == "1")
            {
                // return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Home");
            }

            // Mặc định là Khách thuê
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}