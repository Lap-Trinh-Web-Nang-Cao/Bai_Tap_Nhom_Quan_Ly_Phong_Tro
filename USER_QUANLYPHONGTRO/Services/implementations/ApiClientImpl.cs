using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Services.Interfaces;

namespace USER_QUANLYPHONGTRO.Services.Implementations
{
    /// <summary>
    /// HttpClient wrapper để gọi API backend
    /// </summary>
    public class ApiClientImpl : IApiClient
    {
        private readonly string _baseUrl;
        private const int RequestTimeoutSeconds = 30;

        public ApiClientImpl()
        {
            // Lấy base url API từ Web.config: <appSettings><add key="ApiBaseUrl" value="https://localhost:5001" /></appSettings>
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"]?.TrimEnd('/');
            
            // Fallback to default if not configured
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                _baseUrl = "https://localhost:7039"; // Backend API URL
            }

            Debug.WriteLine($"✅ ApiClientImpl initialized with BaseUrl: {_baseUrl}");
        }

        private HttpClient CreateClient(string bearerToken = null)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(RequestTimeoutSeconds)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(bearerToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            return client;
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string url, string bearerToken = null)
        {
            try
            {
                using (var client = CreateClient(bearerToken))
                {
                    Debug.WriteLine($"📡 GET {_baseUrl}{url}");
                    var response = await client.GetAsync(url);
                    var json = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"❌ GET {url} returned {(int)response.StatusCode}");
                        return ApiResponse<T>.ErrorResult(
                            $"Request GET {url} failed with status {(int)response.StatusCode}",
                            json,
                            (int)response.StatusCode);
                    }

                    var result = JsonConvert.DeserializeObject<ApiResponse<T>>(json);
                    Debug.WriteLine($"✅ GET {url} success");
                    return result;
                }
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"⏱️ GET {url} timeout: {ex.Message}");
                return ApiResponse<T>.ErrorResult($"Request timeout: {ex.Message}", null, 408);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"❌ GET {url} http error: {ex.Message}");
                return ApiResponse<T>.ErrorResult($"Request error: {ex.Message}", null, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ GET {url} error: {ex.GetType().Name} - {ex.Message}");
                return ApiResponse<T>.ErrorResult($"Error: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data, string bearerToken = null)
        {
            try
            {
                using (var client = CreateClient(bearerToken))
                {
                    var jsonContent = JsonConvert.SerializeObject(data);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    Debug.WriteLine($"📡 POST {_baseUrl}{url} - Data: {jsonContent}");
                    var response = await client.PostAsync(url, content);
                    var json = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"❌ POST {url} returned {(int)response.StatusCode}");
                        return ApiResponse<TResponse>.ErrorResult(
                            $"Request POST {url} failed with status {(int)response.StatusCode}",
                            json,
                            (int)response.StatusCode);
                    }

                    var result = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(json);
                    Debug.WriteLine($"✅ POST {url} success");
                    return result;
                }
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"⏱️ POST {url} timeout: {ex.Message}");
                return ApiResponse<TResponse>.ErrorResult($"Request timeout: {ex.Message}", null, 408);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"❌ POST {url} http error: {ex.Message}");
                return ApiResponse<TResponse>.ErrorResult($"Request error: {ex.Message}", null, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ POST {url} error: {ex.GetType().Name} - {ex.Message}");
                return ApiResponse<TResponse>.ErrorResult($"Error: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data, string bearerToken = null)
        {
            try
            {
                using (var client = CreateClient(bearerToken))
                {
                    var jsonContent = JsonConvert.SerializeObject(data);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    Debug.WriteLine($"📡 PUT {_baseUrl}{url} - Data: {jsonContent}");
                    var response = await client.PutAsync(url, content);
                    var json = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"❌ PUT {url} returned {(int)response.StatusCode}");
                        return ApiResponse<TResponse>.ErrorResult(
                            $"Request PUT {url} failed with status {(int)response.StatusCode}",
                            json,
                            (int)response.StatusCode);
                    }

                    var result = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(json);
                    Debug.WriteLine($"✅ PUT {url} success");
                    return result;
                }
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"⏱️ PUT {url} timeout: {ex.Message}");
                return ApiResponse<TResponse>.ErrorResult($"Request timeout: {ex.Message}", null, 408);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"❌ PUT {url} http error: {ex.Message}");
                return ApiResponse<TResponse>.ErrorResult($"Request error: {ex.Message}", null, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ PUT {url} error: {ex.GetType().Name} - {ex.Message}");
                return ApiResponse<TResponse>.ErrorResult($"Error: {ex.Message}", null, 500);
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(string url, string bearerToken = null)
        {
            try
            {
                using (var client = CreateClient(bearerToken))
                {
                    Debug.WriteLine($"📡 DELETE {_baseUrl}{url}");
                    var response = await client.DeleteAsync(url);
                    var json = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"❌ DELETE {url} returned {(int)response.StatusCode}");
                        return ApiResponse<object>.ErrorResult(
                            $"Request DELETE {url} failed with status {(int)response.StatusCode}",
                            json,
                            (int)response.StatusCode);
                    }

                    var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);
                    Debug.WriteLine($"✅ DELETE {url} success");
                    return result;
                }
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"⏱️ DELETE {url} timeout: {ex.Message}");
                return ApiResponse<object>.ErrorResult($"Request timeout: {ex.Message}", null, 408);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"❌ DELETE {url} http error: {ex.Message}");
                return ApiResponse<object>.ErrorResult($"Request error: {ex.Message}", null, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ DELETE {url} error: {ex.GetType().Name} - {ex.Message}");
                return ApiResponse<object>.ErrorResult($"Error: {ex.Message}", null, 500);
            }
        }
    }
}
