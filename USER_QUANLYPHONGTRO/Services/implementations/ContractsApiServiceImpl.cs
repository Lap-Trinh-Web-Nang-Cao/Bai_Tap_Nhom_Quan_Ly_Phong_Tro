using System;
using System.Threading.Tasks;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;
using USER_QUANLYPHONGTRO.Services.Interfaces;

namespace USER_QUANLYPHONGTRO.Services.Implementations
{
    /// <summary>
    /// Thực hiện các cuộc gọi API hợp đồng
    /// </summary>
    public class ContractsApiServiceImpl : IContractsApiService
    {
        private readonly IApiClient _apiClient;

        public ContractsApiServiceImpl() : this(new ApiClientImpl()) { }

        public ContractsApiServiceImpl(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Lấy hợp đồng hiệu lực của người thuê
        /// </summary>
        public Task<ApiResponse<TenantContractViewModel>> GetActiveContractByTenantAsync(Guid tenantId, string bearerToken = null)
        {
            // Lấy UserId từ session nếu không truyền vào
            var userIdStr = HttpContext.Current?.Session?["UserId"]?.ToString();
            if (!Guid.TryParse(userIdStr, out var nguoiThueId))
            {
                return Task.FromResult(
                    ApiResponse<TenantContractViewModel>.ErrorResult("Missing UserId in session"));
            }

            return _apiClient.GetAsync<TenantContractViewModel>($"/api/hopdong/nguoithue/{nguoiThueId}/hieuluc", bearerToken);
        }
    }
}
