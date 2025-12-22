using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;
using USER_QUANLYPHONGTRO.Services.Interfaces;

namespace USER_QUANLYPHONGTRO.Services.Implementations
{
    /// <summary>
    /// Thực hiện các cuộc gọi API hóa đơn
    /// </summary>
    public class InvoicesApiServiceImpl : IInvoicesApiService
    {
        private readonly IApiClient _apiClient;

        public InvoicesApiServiceImpl() : this(new ApiClientImpl()) { }

        public InvoicesApiServiceImpl(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Lấy danh sách hóa đơn của người thuê
        /// </summary>
        public Task<ApiResponse<List<TenantInvoiceViewModel>>> GetInvoicesByTenantAsync(Guid tenantId, string bearerToken = null)
        {
            // Lấy UserId từ session nếu không truyền vào
            var userIdStr = HttpContext.Current?.Session?["UserId"]?.ToString();
            if (!Guid.TryParse(userIdStr, out var nguoiThueId))
            {
                return Task.FromResult(
                    ApiResponse<List<TenantInvoiceViewModel>>.ErrorResult("Missing UserId in session"));
            }

            return _apiClient.GetAsync<List<TenantInvoiceViewModel>>($"/api/hoadon/nguoithue/{nguoiThueId}", bearerToken);
        }

        /// <summary>
        /// Thanh toán hóa đơn
        /// </summary>
        public Task<ApiResponse<object>> PayInvoiceAsync(Guid invoiceId, string bearerToken = null)
        {
            if (invoiceId == Guid.Empty)
            {
                return Task.FromResult(
                    ApiResponse<object>.ErrorResult("Invalid invoice ID"));
            }

            return _apiClient.PostAsync<object, object>($"/api/hoadon/{invoiceId}/thanhtoan", new { }, bearerToken);
        }
    }
}
