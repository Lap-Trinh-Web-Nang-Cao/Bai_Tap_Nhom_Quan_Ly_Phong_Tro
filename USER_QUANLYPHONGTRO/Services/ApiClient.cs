using System;
using System.Configuration;
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
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly string _baseUrl;

        public ApiClient()
        {
            // Lấy base url API từ Web.config: <appSettings><add key="ApiBaseUrl" value="https://localhost:5001" /></appSettings>
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"]?.TrimEnd('/');
        }

        private HttpClient CreateClient(string bearerToken = null)
        {
            var client = new HttpClient();

            if (!string.IsNullOrEmpty(_baseUrl))
            {
                client.BaseAddress = new Uri(_baseUrl);
            }

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
            using (var client = CreateClient(bearerToken))
            {
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<T>.ErrorResult(
                        $"Request GET {url} failed",
                        json,
                        (int)response.StatusCode);
                }

                var result = JsonConvert.DeserializeObject<ApiResponse<T>>(json);
                return result;
            }
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data, string bearerToken = null)
        {
            using (var client = CreateClient(bearerToken))
            {
                var jsonContent = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<TResponse>.ErrorResult(
                        $"Request POST {url} failed",
                        json,
                        (int)response.StatusCode);
                }

                var result = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(json);
                return result;
            }
        }

        public async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data, string bearerToken = null)
        {
            using (var client = CreateClient(bearerToken))
            {
                var jsonContent = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<TResponse>.ErrorResult(
                        $"Request PUT {url} failed",
                        json,
                        (int)response.StatusCode);
                }

                var result = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(json);
                return result;
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(string url, string bearerToken = null)
        {
            using (var client = CreateClient(bearerToken))
            {
                var response = await client.DeleteAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ApiResponse<object>.ErrorResult(
                        $"Request DELETE {url} failed",
                        json,
                        (int)response.StatusCode);
                }

                var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);
                return result;
            }
        }
    }
}
