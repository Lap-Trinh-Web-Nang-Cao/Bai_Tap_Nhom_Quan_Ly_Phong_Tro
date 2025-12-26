using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Services.Interfaces;

namespace USER_QUANLYPHONGTRO.Services
{
    /// <summary>
    /// HttpClient wrapper để gọi API backend
    /// Tự động lấy token từ Session và thêm vào Authorization header
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            // Lấy base url API từ Web.config
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"]?.TrimEnd('/');
            
            // Fallback to default if not configured
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                _baseUrl = "http://18.140.64.80:5000"; // Backend API URL
            }

            // ===== SSL/TLS Configuration =====
            // Cho phép TLS 1.2 (required cho .NET Framework 4.7.2)
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // Bỏ qua SSL certificate validation (CHỈ CHO DEVELOPMENT)
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) =>
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ SSL Certificate Check - Errors: {sslPolicyErrors}");
                    return true; // Accept all certificates for dev
                };

            // ===== HttpClient Configuration =====
            var handler = new HttpClientHandler
            {
                // Bỏ qua SSL validation ở handler level
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Handler SSL Check - Errors: {errors}");
                    return true;
                }
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            System.Diagnostics.Debug.WriteLine($"✅ ApiClient initialized");
            System.Diagnostics.Debug.WriteLine($"   ApiBaseUrl: {_baseUrl}");
        }

        /// <summary>
        /// Lấy token từ Session hoặc trả về null nếu chưa đăng nhập
        /// </summary>
        private string GetStoredToken()
        {
            try
            {
                if (System.Web.HttpContext.Current?.Session != null)
                {
                    var sessionToken = System.Web.HttpContext.Current.Session["AccessToken"]?.ToString();
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

            System.Diagnostics.Debug.WriteLine($"⚠️ No token found in Session");
            return null;
        }

        /// <summary>
        /// Tạo HttpClient với Authorization header nếu có token
        /// </summary>
        private HttpClient CreateClient(string bearerToken = null)
        {
            var client = new HttpClient();

            if (!string.IsNullOrEmpty(_baseUrl))
            {
                client.BaseAddress = new Uri(_baseUrl + "/"); // Ensure trailing slash for base address
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // Ưu tiên token truyền vào, nếu không có thì lấy từ Session
            var token = bearerToken ?? GetStoredToken();
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                System.Diagnostics.Debug.WriteLine($"✅ Authorization header added with token");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ No token available - request will be anonymous");
            }

            return client;
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string url, string bearerToken = null)
        {
            using (var client = CreateClient(bearerToken))
            {
                try
                {
                    var fullUrl = $"{_baseUrl}{url}";
                    System.Diagnostics.Debug.WriteLine($"🔗 GET Request:");
                    System.Diagnostics.Debug.WriteLine($"   URL: {fullUrl}");
                    System.Diagnostics.Debug.WriteLine($"   Has Token: {!string.IsNullOrEmpty(bearerToken ?? GetStoredToken())}");
                    
                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"📡 Response Status: {(int)response.StatusCode} {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"📥 Response Length: {content?.Length ?? 0} bytes");
                    
                    if (content != null && content.Length > 0)
                    {
                        var preview = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
                        System.Diagnostics.Debug.WriteLine($"📥 Response Preview: {preview}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Response body is empty!");
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ API Error {(int)response.StatusCode}: {content}");
                        return ApiResponse<T>.ErrorResult(
                            $"Request GET {url} failed with status {response.StatusCode}",
                            content,
                            (int)response.StatusCode);
                    }

                    // Check if content is empty
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Empty response body - returning empty data");
                        return new ApiResponse<T>
                        {
                            Success = true,
                            Data = default(T),
                            Message = "Empty response"
                        };
                    }

                    // ✅ TRY TO DESERIALIZE AS ApiResponse<T> FIRST
                    try
                    {
                        var result = JsonConvert.DeserializeObject<ApiResponse<T>>(content);
                        if (result != null && (result.Success || result.Data != null))
                        {
                            System.Diagnostics.Debug.WriteLine($"✅ GetAsync success (ApiResponse<T> format)");
                            System.Diagnostics.Debug.WriteLine($"   Success: {result.Success}");
                            System.Diagnostics.Debug.WriteLine($"   Message: {result.Message}");
                            System.Diagnostics.Debug.WriteLine($"   Data is null: {result.Data == null}");
                            return result;
                        }
                        // else fallthrough to try raw data
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Failed to deserialize as ApiResponse<T>: {ex.Message}");
                    }

                    // ✅ IF FAILS, DESERIALIZE AS RAW T AND WRAP IN ApiResponse
                    try
                    {
                        var rawData = JsonConvert.DeserializeObject<T>(content);
                        System.Diagnostics.Debug.WriteLine($"✅ GetAsync success (raw data format, wrapping in ApiResponse)");
                        System.Diagnostics.Debug.WriteLine($"   Raw data is null: {rawData == null}");
                        return new ApiResponse<T>
                        {
                            Success = true,
                            Data = rawData,
                            Message = "Success"
                        };
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Failed to deserialize as raw T: {ex.Message}");
                        return ApiResponse<T>.ErrorResult($"Failed to parse response: {ex.Message}", content, (int)response.StatusCode);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ HttpRequestException: {httpEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"   InnerException: {httpEx.InnerException?.Message}");
                    return ApiResponse<T>.ErrorResult($"HTTP Error: {httpEx.Message}", null, 0);
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Request timeout");
                    return ApiResponse<T>.ErrorResult("Request timeout", null, 0);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ GetAsync error: {ex.Message}\n{ex.StackTrace}");
                    return ApiResponse<T>.ErrorResult($"Error: {ex.Message}", null, 0);
                }
            }
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data, string bearerToken = null)
        {
            using (var client = CreateClient(bearerToken))
            {
                try
                {
                    var jsonContent = JsonConvert.SerializeObject(data);
                    System.Diagnostics.Debug.WriteLine($"🔗 POST: {_baseUrl}{url}");
                    System.Diagnostics.Debug.WriteLine($"📤 Body: {(jsonContent.Length > 200 ? jsonContent.Substring(0, 200) + "..." : jsonContent)}");
                    
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"📡 Response Status: {(int)response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"📥 Response Body: {(responseContent.Length > 300 ? responseContent.Substring(0, 300) + "..." : responseContent)}");

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ API Error {(int)response.StatusCode}: {responseContent}");
                        return ApiResponse<TResponse>.ErrorResult(
                            $"Request POST {url} failed with status {response.StatusCode}",
                            responseContent,
                            (int)response.StatusCode);
                    }

                    // ✅ TRY TO DESERIALIZE AS ApiResponse<TResponse> FIRST
                    try
                    {
                        var result = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(responseContent);
                        if (result != null && (result.Success || result.Data != null))
                        {
                            System.Diagnostics.Debug.WriteLine($"✅ PostAsync success (ApiResponse<TResponse> format)");
                            return result;
                        }
                        // else fallthrough
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Failed to deserialize as ApiResponse<TResponse>: {ex.Message}");
                    }

                    // ✅ IF FAILS, DESERIALIZE AS RAW TResponse AND WRAP IN ApiResponse
                    try
                    {
                        var rawData = JsonConvert.DeserializeObject<TResponse>(responseContent);
                        System.Diagnostics.Debug.WriteLine($"✅ PostAsync success (raw data format, wrapping in ApiResponse)");
                        return new ApiResponse<TResponse>
                        {
                            Success = true,
                            Data = rawData,
                            Message = "Success"
                        };
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Failed to deserialize as raw TResponse: {ex.Message}");
                        return ApiResponse<TResponse>.ErrorResult($"Failed to parse response: {ex.Message}", responseContent, (int)response.StatusCode);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ HttpRequestException: {httpEx.Message}");
                    return ApiResponse<TResponse>.ErrorResult($"HTTP Error: {httpEx.Message}", null, 0);
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Request timeout");
                    return ApiResponse<TResponse>.ErrorResult("Request timeout", null, 0);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ PostAsync error: {ex.Message}\n{ex.StackTrace}");
                    return ApiResponse<TResponse>.ErrorResult($"Error: {ex.Message}", null, 0);
                }
            }
        }

        public async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data, string bearerToken = null)
        {
            using (var client = CreateClient(bearerToken))
            {
                try
                {
                    var jsonContent = JsonConvert.SerializeObject(data);
                    System.Diagnostics.Debug.WriteLine($"🔗 PUT: {_baseUrl}{url}");
                    System.Diagnostics.Debug.WriteLine($"📤 Body: {(jsonContent.Length > 200 ? jsonContent.Substring(0, 200) + "..." : jsonContent)}");
                    
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(url, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"📡 Response Status: {(int)response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"📥 Response Body: {(responseContent.Length > 300 ? responseContent.Substring(0, 300) + "..." : responseContent)}");

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ API Error {(int)response.StatusCode}: {responseContent}");
                        return ApiResponse<TResponse>.ErrorResult(
                            $"Request PUT {url} failed with status {response.StatusCode}",
                            responseContent,
                            (int)response.StatusCode);
                    }

                    // ✅ TRY TO DESERIALIZE AS ApiResponse<TResponse> FIRST
                    try
                    {
                        var result = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(responseContent);
                        if (result != null && (result.Success || result.Data != null))
                        {
                            System.Diagnostics.Debug.WriteLine($"✅ PutAsync success (ApiResponse<TResponse> format)");
                            return result;
                        }
                        // else fallthrough
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Failed to deserialize as ApiResponse<TResponse>: {ex.Message}");
                    }

                    // ✅ IF FAILS, DESERIALIZE AS RAW TResponse AND WRAP IN ApiResponse
                    try
                    {
                        var rawData = JsonConvert.DeserializeObject<TResponse>(responseContent);
                        System.Diagnostics.Debug.WriteLine($"✅ PutAsync success (raw data format, wrapping in ApiResponse)");
                        return new ApiResponse<TResponse>
                        {
                            Success = true,
                            Data = rawData,
                            Message = "Success"
                        };
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Failed to deserialize as raw TResponse: {ex.Message}");
                        return ApiResponse<TResponse>.ErrorResult($"Failed to parse response: {ex.Message}", responseContent, (int)response.StatusCode);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ HttpRequestException: {httpEx.Message}");
                    return ApiResponse<TResponse>.ErrorResult($"HTTP Error: {httpEx.Message}", null, 0);
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Request timeout");
                    return ApiResponse<TResponse>.ErrorResult("Request timeout", null, 0);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ PutAsync error: {ex.Message}\n{ex.StackTrace}");
                    return ApiResponse<TResponse>.ErrorResult($"Error: {ex.Message}", null, 0);
                }
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(string url, string bearerToken = null)
        {
            using (var client = CreateClient(bearerToken))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"🔗 DELETE: {_baseUrl}{url}");
                    
                    var response = await client.DeleteAsync(url);
                    var json = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"📡 Response Status: {(int)response.StatusCode}");

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ API Error {(int)response.StatusCode}: {json}");
                        return ApiResponse<object>.ErrorResult(
                            $"Request DELETE {url} failed with status {response.StatusCode}",
                            json,
                            (int)response.StatusCode);
                    }

                    // Try ApiResponse<object>
                    try
                    {
                        var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);
                        if (result != null && (result.Success || result.Data != null))
                        {
                            System.Diagnostics.Debug.WriteLine($"✅ DeleteAsync success (ApiResponse<object> format)");
                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ DeleteAsync failed to parse as ApiResponse<object>: {ex.Message}");
                    }

                    // Fallback: return wrapped raw object
                    try
                    {
                        var raw = JsonConvert.DeserializeObject<object>(json);
                        return new ApiResponse<object> { Success = true, Data = raw, Message = "Success" };
                    }
                    catch (Exception ex)
                    {
                        return ApiResponse<object>.ErrorResult($"Failed to parse response: {ex.Message}", json, (int)response.StatusCode);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ DeleteAsync failed: {httpEx.Message}");
                    return ApiResponse<object>.ErrorResult($"HTTP Error: {httpEx.Message}", null, 0);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ DeleteAsync error: {ex.Message}\n{ex.StackTrace}");
                    return ApiResponse<object>.ErrorResult($"Error: {ex.Message}", null, 0);
                }
            }
        }
    }
}
