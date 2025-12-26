using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace USER_QUANLYPHONGTRO.Services
{
    /// <summary>
    /// Service xác thực - Xử lý đăng nhập, token, session
    /// Dùng chung cho cả User & Admin
    /// </summary>
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AuthService()
        {
            _apiBaseUrl = (ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://18.140.64.80:5000").TrimEnd('/');
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Đăng nhập với Email & Password
        /// Gọi API /api/nguoidung/login để lấy JWT Token
        /// </summary>
        public async Task<LoginResponse> LoginAsync(string email, string password)
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

                var apiBase = (ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://18.140.64.80:5000").TrimEnd('/');
                var url = $"{apiBase}/api/nguoidung/login";
                Debug.WriteLine($"📡 AuthService: POST {url}");

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Login API returned {(int)response.StatusCode} {response.ReasonPhrase}: {errorContent}");
                    return new LoginResponse { Success = false, Message = "Email hoặc mật khẩu không đúng" };
                }

                var resultJson = await response.Content.ReadAsStringAsync();

                // Robust parsing: support different casing / shapes (Token, token, data.token, etc.)
                JObject payload;
                try
                {
                    payload = JObject.Parse(resultJson);
                }
                catch (Exception parseEx)
                {
                    Debug.WriteLine($"⚠️ Failed to parse login response JSON: {parseEx}");
                    Debug.WriteLine($"⚠️ Response content: {resultJson}");
                    return new LoginResponse { Success = false, Message = "Phản hồi API không hợp lệ" };
                }

                // --- after parsing payload JObject ---
                Debug.WriteLine("🔎 Login response properties:");
                foreach (var p in payload.Properties())
                {
                    Debug.WriteLine($"  - {p.Name} : {p.Value.Type}");
                }

                // Recursive search for property named "token" (case-insensitive)
                string FindTokenRec(JToken node)
                {
                    if (node == null) return null;

                    if (node.Type == JTokenType.Object)
                    {
                        foreach (var prop in ((JObject)node).Properties())
                        {
                            if (string.Equals(prop.Name, "token", StringComparison.OrdinalIgnoreCase))
                                return prop.Value?.ToString();

                            var r = FindTokenRec(prop.Value);
                            if (!string.IsNullOrEmpty(r)) return r;
                        }
                    }
                    else if (node.Type == JTokenType.Array)
                    {
                        foreach (var item in node.Children())
                        {
                            var r = FindTokenRec(item);
                            if (!string.IsNullOrEmpty(r)) return r;
                        }
                    }

                    return null;
                }

                string token = FindTokenRec(payload);

                // Fixed: properly formatted interpolated string without escaped quotes or extra characters
                Debug.WriteLine($"🔑 Extracted token: {(string.IsNullOrEmpty(token) ? "<null>" : token.Substring(0, Math.Min(20, token.Length)) + "...")}");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine($"⚠️ Login API returned unexpected payload: {resultJson}");
                    return new LoginResponse { Success = false, Message = "Phản hồi API không hợp lệ" };
                }

                // Giải mã JWT Token để lấy thông tin
                var userInfo = ExtractUserInfoFromToken(token);

                return new LoginResponse
                {
                    Success = true,
                    Token = token,
                    UserId = userInfo.UserId,
                    Email = userInfo.Email,
                    VaiTroId = userInfo.VaiTroId,
                    Message = "Đăng nhập thành công"
                };
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"❌ AuthService.HttpRequestException when calling login: {httpEx}");
                return new LoginResponse { Success = false, Message = "Lỗi kết nối tới API: " + httpEx.Message };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ AuthService.LoginAsync Error (detailed): {ex}");
                return new LoginResponse { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        /// <summary>
        /// Giải mã JWT Token để lấy thông tin người dùng
        /// Sử dụng phương pháp manual decode vì .NET Framework 4.7.2 không có JwtSecurityTokenHandler
        /// </summary>
        private UserTokenInfo ExtractUserInfoFromToken(string token)
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
                            payloadObject["nameid"]?.ToString(); // Thử nhiều tên khác nhau
                var email = payloadObject["email"]?.ToString();
                var vaiTroIdStr = payloadObject["VaiTroId"]?.ToString() ?? payloadObject["vaitroid"]?.ToString();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(vaiTroIdStr))
                {
                    Debug.WriteLine($"⚠️ Token payload: {payloadJson}");
                    throw new Exception("Token không chứa thông tin UserId hoặc VaiTroId");
                }

                return new UserTokenInfo
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
        /// Lấy thông tin người dùng từ Token
        /// </summary>
        public UserTokenInfo GetUserInfoFromToken(string token)
        {
            return ExtractUserInfoFromToken(token);
        }

        /// <summary>
        /// Lưu Token & Thông tin vào Session
        /// </summary>
        public void SaveUserSessionFromToken(string token, UserTokenInfo userInfo)
        {
            HttpContext.Current.Session["AuthToken"] = token;
            HttpContext.Current.Session["UserId"] = userInfo.UserId.ToString();
            HttpContext.Current.Session["UserEmail"] = userInfo.Email;
            HttpContext.Current.Session["VaiTroId"] = userInfo.VaiTroId;
            HttpContext.Current.Session["UserName"] = userInfo.Email; // Tạm dùng email làm username

            // Xác định loại người dùng để hiển thị đúng UI
            HttpContext.Current.Session["UserRole"] = userInfo.VaiTroId == 1 ? "Admin" :
                                                       userInfo.VaiTroId == 2 ? "ChuTro" :
                                                       "NguoiThue";

            // Thời gian hết hạn (mặc định 1 ngày)
            HttpContext.Current.Session.Timeout = 1440; // phút

            Debug.WriteLine($"✅ Session saved for user: {userInfo.Email}, Role: {userInfo.VaiTroId}");
        }

        /// <summary>
        /// Kiểm tra người dùng đã đăng nhập chưa
        /// </summary>
        public bool IsUserLoggedIn()
        {
            return HttpContext.Current?.Session != null &&
                   HttpContext.Current.Session["AuthToken"] != null;
        }

        /// <summary>
        /// Lấy Token từ Session
        /// </summary>
        public string GetAuthToken()
        {
            return HttpContext.Current?.Session?["AuthToken"]?.ToString();
        }

        /// <summary>
        /// Lấy thông tin người dùng từ Session
        /// </summary>
        public UserSessionInfo GetCurrentUserSession()
        {
            if (!IsUserLoggedIn())
                return null;

            return new UserSessionInfo
            {
                UserId = Guid.Parse(HttpContext.Current.Session["UserId"].ToString()),
                UserEmail = HttpContext.Current.Session["UserEmail"].ToString(),
                VaiTroId = Convert.ToInt32(HttpContext.Current.Session["VaiTroId"]),
                UserRole = HttpContext.Current.Session["UserRole"].ToString(),
                AuthToken = GetAuthToken()
            };
        }

        /// <summary>
        /// Đăng xuất - Xóa Session
        /// </summary>
        public void Logout()
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }
        }
    }

    /// <summary>
    /// Response từ API đăng nhập
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int VaiTroId { get; set; }
    }

    /// <summary>
    /// Thông tin giải mã từ JWT Token
    /// </summary>
    public class UserTokenInfo
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int VaiTroId { get; set; }
    }

    /// <summary>
    /// Thông tin lưu trong Session
    /// </summary>
    public class UserSessionInfo
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public int VaiTroId { get; set; }
        public string UserRole { get; set; }
        public string AuthToken { get; set; }

        public bool IsAdmin => VaiTroId == 1;
        public bool IsChuTro => VaiTroId == 2;
        public bool IsNguoiThue => VaiTroId == 3;
    }
}