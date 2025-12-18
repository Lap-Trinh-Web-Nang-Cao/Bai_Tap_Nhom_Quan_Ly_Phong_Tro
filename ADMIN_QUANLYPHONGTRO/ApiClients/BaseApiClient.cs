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
            // TODO: Lấy token từ localStorage hoặc cache
            // Tạm thời return null
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
                var json = JsonConvert.SerializeObject(data);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(url, body);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(content);

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PostAsync failed: {ex.Message}");
                throw;
            }
        }

        protected async Task<T> PutAsync<T>(string url, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync(url, body);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(content);

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ PutAsync failed: {ex.Message}");
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
