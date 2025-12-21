using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace USER_QUANLYPHONGTRO.Services
{
    public class ApiClient
    {
        private readonly string _baseUrl = "https://localhost:5101/";

        public ApiClient()
        {
            // Lấy URL từ Web.config. Nếu chưa có, hãy đảm bảo Web.config có key "ApiBaseUrl"
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"]?.TrimEnd('/');
        }

        // Hàm tạo HttpClient có kèm Token (nếu có)
        private HttpClient CreateClient(string token = null)
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(_baseUrl))
            {
                client.BaseAddress = new Uri(_baseUrl);
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // ================= GET =================
        public async Task<T> GetAsync<T>(string endpoint, string token = null)
        {
            using (var client = CreateClient(token))
            {
                var response = await client.GetAsync(endpoint);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }

                // Xử lý lỗi
                throw new Exception(ParseErrorMessage(json, response.StatusCode.ToString()));
            }
        }

        // ================= POST =================
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestBody, string token = null)
        {
            using (var client = CreateClient(token))
            {
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Nếu TResponse là string (API trả về text thuần), không cần Deserialize
                    if (typeof(TResponse) == typeof(string))
                    {
                        return (TResponse)(object)json;
                    }
                    return JsonConvert.DeserializeObject<TResponse>(json);
                }

                throw new Exception(ParseErrorMessage(json, response.StatusCode.ToString()));
            }
        }

        // ================= PUT =================
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest requestBody, string token = null)
        {
            using (var client = CreateClient(token))
            {
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(endpoint, content);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(TResponse) == typeof(string))
                    {
                        return (TResponse)(object)json;
                    }
                    return JsonConvert.DeserializeObject<TResponse>(json);
                }

                throw new Exception(ParseErrorMessage(json, response.StatusCode.ToString()));
            }
        }

        // ================= DELETE =================
        public async Task<TResponse> DeleteAsync<TResponse>(string endpoint, string token = null)
        {
            using (var client = CreateClient(token))
            {
                var response = await client.DeleteAsync(endpoint);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    if (typeof(TResponse) == typeof(string))
                    {
                        return (TResponse)(object)json;
                    }
                    return JsonConvert.DeserializeObject<TResponse>(json);
                }

                throw new Exception(ParseErrorMessage(json, response.StatusCode.ToString()));
            }
        }

        // Hàm phụ để trích xuất thông báo lỗi từ JSON trả về của Backend
        private string ParseErrorMessage(string jsonResponse, string statusCode)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonResponse)) return $"Lỗi API ({statusCode})"; // Check rỗng

                dynamic errorObj = JsonConvert.DeserializeObject(jsonResponse);

                // Thêm kiểm tra null cho errorObj trước khi gọi .message
                if (errorObj != null && errorObj.message != null)
                {
                    return errorObj.message.ToString();
                }

                if (errorObj != null && errorObj.title != null)
                {
                    return errorObj.title.ToString();
                }
            }
            catch
            {
                // Bỏ qua lỗi parse
            }
            return $"Lỗi API ({statusCode}): {jsonResponse}";
        }
    }
}