using ADMIN_QUANLYPHONGTRO.Models.Common;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class BaseApiClient
    {
        private readonly HttpClient _client;

        public BaseApiClient()
        {
            // Bỏ qua SSL Certificate validation (dev mode)
            ServicePointManager.ServerCertificateValidationCallback = 
                (sender, certificate, chain, sslPolicyErrors) => true;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(AppSettings.ApiBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)  // 30 giây timeout
            };
            
            System.Diagnostics.Debug.WriteLine($"✅ BaseApiClient: ApiBaseUrl = {AppSettings.ApiBaseUrl}");

            // Thêm Authorization Header nếu có token
            var token = GetStoredToken();
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                System.Diagnostics.Debug.WriteLine($"✅ BaseApiClient: Token added");
            }
        }

        private string GetStoredToken()
        {
            // Lấy token từ Session (được lưu sau khi login)
            try
            {
                if (System.Web.HttpContext.Current?.Session != null)
                {
                    var sessionToken = System.Web.HttpContext.Current.Session["AuthToken"]?.ToString();
                    if (!string.IsNullOrEmpty(sessionToken))
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Token loaded from Session");
                        return sessionToken;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error loading token from Session: {ex.Message}");
            }

            // Fallback: Lấy từ cookie nếu Session không có
            try
            {
                var cookie = System.Web.HttpContext.Current?.Request.Cookies["AuthToken"];
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Token loaded from Cookie");
                    return cookie.Value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error loading token from Cookie: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine($"⚠️ No token found in Session or Cookie");
            return null;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var fullUrl = _client.BaseAddress + url;
                System.Diagnostics.Debug.WriteLine($"🔍 Calling GET: {fullUrl}");
                
                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"✅ Response Status: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ API Error {(int)response.StatusCode}: {content}");
                    throw new Exception($"API Error {(int)response.StatusCode}: {content}");
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HttpRequestException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ InnerException: {ex.InnerException?.Message}");
                throw new Exception($"Không thể kết nối tới API ({url}): {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ TaskCanceledException (Timeout): {ex.Message}");
                throw new Exception($"API timeout ({url}): {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetAsync failed: {ex.Message}");
                throw;
            }
        }

        protected async Task<T> PostAsync<T>(string url, object data)
        {
            try
            {
                var fullUrl = _client.BaseAddress + url;
                System.Diagnostics.Debug.WriteLine($"🔍 Calling POST: {fullUrl}");
                
                HttpContent body = null;
                
                if (data != null)
                {
                    var json = JsonConvert.SerializeObject(data);
                    System.Diagnostics.Debug.WriteLine($"📤 Request Body: {json}");
                    body = new StringContent(json, Encoding.UTF8, "application/json");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"📤 Request Body: (empty)");
                    body = new StringContent("", Encoding.UTF8, "application/json");
                }

                var response = await _client.PostAsync(url, body);
                var content = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"✅ Response Status: {(int)response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"📥 Response Body: {(string.IsNullOrEmpty(content) ? "(empty)" : content)}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = string.IsNullOrEmpty(content) ? $"HTTP {(int)response.StatusCode}" : content;
                    System.Diagnostics.Debug.WriteLine($"❌ API Error {(int)response.StatusCode}: {errorMsg}");
                    throw new Exception($"API Error {(int)response.StatusCode}: {errorMsg}");
                }

                // Nếu response body trống, tạo một response mặc định
                if (string.IsNullOrEmpty(content))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Response body is empty, attempting to deserialize as default");
                    // Tạo một instance mặc định - điều này chỉ hoạt động cho types có parameterless constructor
                    var defaultInstance = Activator.CreateInstance<T>();
                    return defaultInstance;
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HttpRequestException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ InnerException: {ex.InnerException?.Message}");
                throw new Exception($"Không thể kết nối tới API ({url}): {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ TaskCanceledException (Timeout): {ex.Message}");
                throw new Exception($"API timeout ({url}): {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ JsonException: {ex.Message}");
                throw new Exception($"Invalid JSON response from API ({url}): {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PostAsync failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected async Task<T> PutAsync<T>(string url, object data)
        {
            try
            {
                var fullUrl = _client.BaseAddress + url;
                System.Diagnostics.Debug.WriteLine($"🔍 Calling PUT: {fullUrl}");
                
                HttpContent body = null;
                
                if (data != null)
                {
                    var json = JsonConvert.SerializeObject(data);
                    System.Diagnostics.Debug.WriteLine($"📤 Request Body: {json}");
                    body = new StringContent(json, Encoding.UTF8, "application/json");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"📤 Request Body: (empty)");
                    body = new StringContent("", Encoding.UTF8, "application/json");
                }

                var response = await _client.PutAsync(url, body);
                var content = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"✅ Response Status: {(int)response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"📥 Response Body: {(string.IsNullOrEmpty(content) ? "(empty)" : content)}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = string.IsNullOrEmpty(content) ? $"HTTP {(int)response.StatusCode}" : content;
                    System.Diagnostics.Debug.WriteLine($"❌ API Error {(int)response.StatusCode}: {errorMsg}");
                    throw new Exception($"API Error {(int)response.StatusCode}: {errorMsg}");
                }

                // Nếu response body trống, tạo một response mặc định
                if (string.IsNullOrEmpty(content))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Response body is empty, attempting to deserialize as default");
                    // Tạo một instance mặc định - điều này chỉ hoạt động cho types có parameterless constructor
                    var defaultInstance = Activator.CreateInstance<T>();
                    return defaultInstance;
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HttpRequestException: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ InnerException: {ex.InnerException?.Message}");
                throw new Exception($"Không thể kết nối tới API ({url}): {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ TaskCanceledException (Timeout): {ex.Message}");
                throw new Exception($"API timeout ({url}): {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ JsonException: {ex.Message}");
                throw new Exception($"Invalid JSON response from API ({url}): {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PutAsync failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected async Task<bool> DeleteAsync(string url)
        {
            try
            {
                var response = await _client.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DeleteAsync failed: {ex.Message}");
                throw;
            }
        }
    }
}
