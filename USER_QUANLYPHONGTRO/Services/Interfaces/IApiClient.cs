using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.Dtos;

namespace USER_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface dùng chung để gọi API backend
    /// </summary>
    public interface IApiClient
    {
        Task<ApiResponse<T>> GetAsync<T>(string url, string bearerToken = null);
        Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data, string bearerToken = null);
        Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data, string bearerToken = null);
        Task<ApiResponse<object>> DeleteAsync(string url, string bearerToken = null);
    }
}
