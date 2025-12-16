using ADMIN_QUANLYPHONGTRO.Models.Common;
using Newtonsoft.Json;
using System;
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
            _client = new HttpClient
            {
                BaseAddress = new Uri(AppSettings.ApiBaseUrl)
            };
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(content);

            return JsonConvert.DeserializeObject<T>(content);
        }

        protected async Task<T> PostAsync<T>(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, body);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(content);

            return JsonConvert.DeserializeObject<T>(content);
        }

        protected async Task<T> PutAsync<T>(string url, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(url, body);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(content);

            return JsonConvert.DeserializeObject<T>(content);
        }

        protected async Task<bool> DeleteAsync(string url)
        {
            var response = await _client.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
    }
}
