using System;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;

namespace USER_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface để gọi API hợp đồng từ Backend
    /// </summary>
    public interface IContractsApiService
    {
        /// <summary>
        /// Lấy hợp đồng hiệu lực của người thuê
        /// </summary>
        Task<ApiResponse<TenantContractViewModel>> GetActiveContractByTenantAsync(Guid tenantId, string bearerToken = null);
    }
}
