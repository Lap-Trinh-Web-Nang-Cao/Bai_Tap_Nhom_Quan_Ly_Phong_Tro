using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADMIN_QUANLYPHONGTRO.Services
{
    /// <summary>
    /// Session info cho Admin đã đăng nhập
    /// </summary>
    public class AdminSessionInfo
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int VaiTroId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        
        public bool IsAdmin { get { return VaiTroId == 1; } }
        public bool IsExpired { get { return DateTime.UtcNow >= ExpiresAt; } }
    }

    /// <summary>
    /// Service xử lý xác thực Admin
    /// </summary>
    public class AdminAuthService
    {
        private const string SESSION_KEY = "AdminSession";
        private const string TOKEN_COOKIE_KEY = "AuthToken";
        private readonly string _apiBaseUrl;

        public AdminAuthService()
        {
            // Lấy API Base URL từ Web.config hoặc dùng mặc định
            _apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] 
                ?? "https://localhost:7149";
        }

        /// <summary>
        /// Đăng nhập Admin - gọi API và lưu session
        /// </summary>
        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    
                    var loginData = new { Email = email, Password = password };
                    var json = JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("/api/NguoiDung/login", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine(string.Format("🔵 AdminAuthService.LoginAsync - Response: {0}", responseContent));

                    if (!response.IsSuccessStatusCode)
                    {
                        return new LoginResult
                        {
                            Success = false,
                            Message = "Email hoặc mật khẩu không đúng"
                        };
                    }

                    var result = JObject.Parse(responseContent);
                    
                    var success = result["success"]?.Value<bool>() ?? result["Success"]?.Value<bool>() ?? false;
                    if (!success)
                    {
                        var message = result["message"]?.ToString() ?? result["Message"]?.ToString() ?? "Đăng nhập thất bại";
                        return new LoginResult
                        {
                            Success = false,
                            Message = message
                        };
                    }

                    string token = result["token"]?.ToString() 
                        ?? result["Token"]?.ToString() 
                        ?? result["data"]?["token"]?.ToString()
                        ?? result["Data"]?["Token"]?.ToString();
                    
                    if (string.IsNullOrEmpty(token))
                    {
                        return new LoginResult
                        {
                            Success = false,
                            Message = "Không nhận được token từ server"
                        };
                    }

                    return new LoginResult
                    {
                        Success = true,
                        Token = token,
                        Message = "Đăng nhập thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ AdminAuthService.LoginAsync Error: {0}", ex.Message));
                return new LoginResult
                {
                    Success = false,
                    Message = string.Format("Lỗi kết nối server: {0}", ex.Message)
                };
            }
        }

        /// <summary>
        /// Trích xuất thông tin từ JWT token (parse thủ công, không cần thư viện JWT)
        /// </summary>
        public AdminSessionInfo ExtractUserInfoFromToken(string token)
        {
            try
            {
                // JWT có 3 phần: header.payload.signature
                var parts = token.Split('.');
                if (parts.Length != 3)
                {
                    return new AdminSessionInfo();
                }

                // Decode payload (phần thứ 2)
                var payload = parts[1];
                // Thêm padding nếu cần
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                // Replace các ký tự URL-safe
                payload = payload.Replace('-', '+').Replace('_', '/');
                
                var jsonBytes = Convert.FromBase64String(payload);
                var jsonString = Encoding.UTF8.GetString(jsonBytes);
                var claims = JObject.Parse(jsonString);

                // Lấy các claims
                var userId = claims["nameid"]?.ToString() ?? claims["sub"]?.ToString() ?? claims["NameIdentifier"]?.ToString();
                var email = claims["email"]?.ToString() ?? claims["Email"]?.ToString();
                var name = claims["unique_name"]?.ToString() ?? claims["name"]?.ToString() ?? claims["Name"]?.ToString();
                var roleIdStr = claims["VaiTroId"]?.ToString() ?? claims["role"]?.ToString() ?? claims["Role"]?.ToString();
                var expStr = claims["exp"]?.ToString();

                int vaiTroId = 0;
                int.TryParse(roleIdStr, out vaiTroId);

                DateTime expiresAt = DateTime.UtcNow.AddHours(24); // Default 24h
                if (!string.IsNullOrEmpty(expStr))
                {
                    long expSeconds;
                    if (long.TryParse(expStr, out expSeconds))
                    {
                        expiresAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expSeconds);
                    }
                }

                Guid uid = Guid.Empty;
                if (!string.IsNullOrEmpty(userId))
                {
                    Guid.TryParse(userId, out uid);
                }

                return new AdminSessionInfo
                {
                    UserId = uid,
                    UserEmail = email ?? string.Empty,
                    UserName = name ?? email ?? string.Empty,
                    VaiTroId = vaiTroId,
                    Token = token,
                    ExpiresAt = expiresAt
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ ExtractUserInfoFromToken Error: {0}", ex.Message));
                return new AdminSessionInfo();
            }
        }

        /// <summary>
        /// Lưu session admin vào HttpContext + Cookie (để API Client lấy được token)
        /// </summary>
        public void SaveAdminSessionFromToken(string token, AdminSessionInfo userInfo)
        {
            try
            {
                if (HttpContext.Current == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ SaveAdminSession: HttpContext is null");
                    return;
                }

                userInfo.Token = token;
                
                // Lưu vào Session
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SESSION_KEY] = userInfo;
                    System.Diagnostics.Debug.WriteLine(string.Format("✅ SaveAdminSession: Saved to Session for {0}", userInfo.UserEmail));
                }
                
                // Lưu vào Cookie (để BaseApiClient lấy được)
                try
                {
                    var tokenCookie = new HttpCookie(TOKEN_COOKIE_KEY)
                    {
                        Value = token,
                        Expires = userInfo.ExpiresAt,
                        HttpOnly = true,
                        Secure = false // Đặt true nếu dùng HTTPS
                    };
                    HttpContext.Current.Response.Cookies.Add(tokenCookie);
                    System.Diagnostics.Debug.WriteLine(string.Format("✅ SaveAdminSession: Saved to Cookie for {0}", userInfo.UserEmail));
                }
                catch (Exception cookieEx)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("⚠️ SaveAdminSession Cookie Error: {0}", cookieEx.Message));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ SaveAdminSession Error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Lấy session admin hiện tại
        /// </summary>
        public AdminSessionInfo GetCurrentAdminSession()
        {
            try
            {
                if (HttpContext.Current == null || HttpContext.Current.Session == null)
                    return null;

                var session = HttpContext.Current.Session[SESSION_KEY] as AdminSessionInfo;
                
                // Kiểm tra token hết hạn
                if (session != null && session.IsExpired)
                {
                    LogoutAdmin();
                    return null;
                }

                return session;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ GetCurrentAdminSession Error: {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra admin đã đăng nhập chưa
        /// </summary>
        public bool IsAdminLoggedIn()
        {
            var session = GetCurrentAdminSession();
            return session != null && session.VaiTroId == 1 && !session.IsExpired;
        }

        /// <summary>
        /// Đăng xuất admin - xóa Session và Cookie
        /// </summary>
        public void LogoutAdmin()
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    // Xóa Session trước
                    if (HttpContext.Current.Session != null)
                    {
                        try
                        {
                            HttpContext.Current.Session.Remove(SESSION_KEY);
                            HttpContext.Current.Session.Clear();
                            HttpContext.Current.Session.Abandon();
                            System.Diagnostics.Debug.WriteLine("✅ LogoutAdmin: Session cleared and abandoned");
                        }
                        catch (Exception sessionEx)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("⚠️ LogoutAdmin Session Error: {0}", sessionEx.Message));
                        }
                    }
                    
                    // Xóa Cookie sau
                    try
                    {
                        // Xóa AuthToken cookie
                        var tokenCookie = new HttpCookie(TOKEN_COOKIE_KEY)
                        {
                            Expires = DateTime.Now.AddDays(-1),
                            HttpOnly = true,
                            Secure = false
                        };
                        HttpContext.Current.Response.Cookies.Add(tokenCookie);
                        System.Diagnostics.Debug.WriteLine("✅ LogoutAdmin: Deleted AuthToken cookie");
                        
                        // Xóa tất cả cookies có thể
                        if (HttpContext.Current.Request.Cookies.Count > 0)
                        {
                            foreach (string cookieName in HttpContext.Current.Request.Cookies)
                            {
                                var cookie = new HttpCookie(cookieName)
                                {
                                    Expires = DateTime.Now.AddDays(-1)
                                };
                                HttpContext.Current.Response.Cookies.Add(cookie);
                            }
                            System.Diagnostics.Debug.WriteLine(string.Format("✅ LogoutAdmin: Cleared {0} cookies", HttpContext.Current.Request.Cookies.Count));
                        }
                    }
                    catch (Exception cookieEx)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("⚠️ LogoutAdmin Cookie Error: {0}", cookieEx.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ LogoutAdmin Error: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Lấy token của admin hiện tại
        /// </summary>
        public string GetCurrentToken()
        {
            var session = GetCurrentAdminSession();
            return session != null ? session.Token : null;
        }
    }

    /// <summary>
    /// Kết quả đăng nhập
    /// </summary>
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
