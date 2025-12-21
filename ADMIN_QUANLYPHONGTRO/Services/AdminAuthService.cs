using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADMIN_QUANLYPHONGTRO.Services
{
    /// <summary>
    /// Admin Authentication Service
    /// Xử lý đăng nhập, token, session cho Admin
    /// </summary>
    public class AdminAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AdminAuthService()
        {
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7039/api/";
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Đăng nhập Admin với Email & Password
        /// </summary>
        public async Task<AdminLoginResponse> LoginAsync(string email, string password)
        {
            try
            {
                var loginRequest = new
                {
                    Email = email,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}nguoidung/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Admin Login API Error: {errorContent}");
                    return new AdminLoginResponse { Success = false, Message = "Email hoặc mật khẩu không đúng" };
                }

                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(resultJson);

                if (result == null || result.Token == null)
                {
                    return new AdminLoginResponse { Success = false, Message = "Phản hồi API không hợp lệ" };
                }

                var token = (string)result.Token;
                var userInfo = ExtractUserInfoFromToken(token);

                // Chỉ cho phép Admin (VaiTroId = 1) đăng nhập
                if (userInfo.VaiTroId != 1)
                {
                    return new AdminLoginResponse { Success = false, Message = "Bạn không có quyền truy cập Admin" };
                }

                return new AdminLoginResponse
                {
                    Success = true,
                    Token = token,
                    UserId = userInfo.UserId,
                    Email = userInfo.Email,
                    VaiTroId = userInfo.VaiTroId,
                    Message = "Đăng nhập Admin thành công"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ AdminAuthService.LoginAsync Error: {ex.Message}");
                return new AdminLoginResponse { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        /// <summary>
        /// Giải mã JWT Token (PUBLIC)
        /// Sử dụng manual decode vì .NET Framework 4.7.2 không có JwtSecurityTokenHandler
        /// </summary>
        public AdminUserTokenInfo ExtractUserInfoFromToken(string token)
        {
            try
            {
                // JWT token có 3 phần: header.payload.signature, cách nhau bởi dấu .
                var parts = token.Split('.');
                if (parts.Length != 3)
                {
                    throw new Exception("Token JWT không hợp lệ");
                }

                // Lấy phần payload (phần thứ 2)
                var payload = parts[1];

                // Thêm padding nếu cần
                var paddingNeeded = 4 - (payload.Length % 4);
                if (paddingNeeded < 4)
                {
                    payload += new string('=', paddingNeeded);
                }

                // Decode Base64
                var decodedPayload = System.Convert.FromBase64String(payload);
                var payloadJson = System.Text.Encoding.UTF8.GetString(decodedPayload);

                // Parse JSON
                var payloadObject = JObject.Parse(payloadJson);

                // Lấy các claim cần thiết
                var userId = payloadObject["sub"]?.ToString() ?? payloadObject["oid"]?.ToString() ?? 
                            payloadObject["nameid"]?.ToString();
                var email = payloadObject["email"]?.ToString();
                var vaiTroIdStr = payloadObject["VaiTroId"]?.ToString() ?? payloadObject["vaitroid"]?.ToString();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(vaiTroIdStr))
                {
                    Debug.WriteLine($"⚠️ Token payload: {payloadJson}");
                    throw new Exception("Token không chứa thông tin UserId hoặc VaiTroId");
                }

                return new AdminUserTokenInfo
                {
                    UserId = Guid.Parse(userId),
                    Email = email ?? "unknown",
                    VaiTroId = int.Parse(vaiTroIdStr)
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ExtractUserInfoFromToken Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lưu Admin Session
        /// </summary>
        public void SaveAdminSessionFromToken(string token, AdminUserTokenInfo userInfo)
        {
            HttpContext.Current.Session["AdminAuthToken"] = token;
            HttpContext.Current.Session["AdminUserId"] = userInfo.UserId.ToString();
            HttpContext.Current.Session["AdminUserEmail"] = userInfo.Email;
            HttpContext.Current.Session["AdminVaiTroId"] = userInfo.VaiTroId;
            HttpContext.Current.Session["AdminUserName"] = userInfo.Email;
            HttpContext.Current.Session["AdminUserRole"] = "Admin";
            HttpContext.Current.Session.Timeout = 1440; // 24 giờ

            Debug.WriteLine($"✅ Admin Session saved for: {userInfo.Email}");
        }

        /// <summary>
        /// Kiểm tra Admin đã đăng nhập chưa
        /// </summary>
        public bool IsAdminLoggedIn()
        {
            return HttpContext.Current.Session != null &&
                   HttpContext.Current.Session["AdminAuthToken"] != null;
        }

        /// <summary>
        /// Lấy Admin Session
        /// </summary>
        public AdminSessionInfo GetCurrentAdminSession()
        {
            if (!IsAdminLoggedIn())
                return null;

            return new AdminSessionInfo
            {
                UserId = Guid.Parse(HttpContext.Current.Session["AdminUserId"].ToString()),
                UserEmail = HttpContext.Current.Session["AdminUserEmail"].ToString(),
                VaiTroId = (int)HttpContext.Current.Session["AdminVaiTroId"],
                UserRole = HttpContext.Current.Session["AdminUserRole"].ToString(),
                AuthToken = HttpContext.Current.Session["AdminAuthToken"].ToString()
            };
        }

        /// <summary>
        /// Đăng xuất Admin
        /// </summary>
        public void LogoutAdmin()
        {
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }
        }
    }

    /// <summary>
    /// Response từ Admin Login
    /// </summary>
    public class AdminLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int VaiTroId { get; set; }
    }

    /// <summary>
    /// Thông tin từ Token
    /// </summary>
    public class AdminUserTokenInfo
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int VaiTroId { get; set; }
    }

    /// <summary>
    /// Thông tin Admin trong Session
    /// </summary>
    public class AdminSessionInfo
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public int VaiTroId { get; set; }
        public string UserRole { get; set; }
        public string AuthToken { get; set; }

        public bool IsAdmin => VaiTroId == 1;
    }
}
