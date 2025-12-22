using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;

namespace USER_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface để gọi API hóa đơn từ Backend
    /// </summary>
    public interface IInvoicesApiService
    {
        /// <summary>
        /// Lấy danh sách hóa đơn của người thuê
        /// </summary>
        Task<ApiResponse<List<TenantInvoiceViewModel>>> GetInvoicesByTenantAsync(Guid tenantId, string bearerToken = null);

        /// <summary>
        /// Thanh toán hóa đơn
        /// </summary>
        Task<ApiResponse<object>> PayInvoiceAsync(Guid invoiceId, string bearerToken = null);
    }
}
