using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;

namespace USER_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface cho dịch vụ xử lý dữ liệu KhachThue
    /// </summary>
    public interface IKhachThueDataService
    {
        /// <summary>
        /// Lấy danh sách lịch đã đặt của user
        /// </summary>
        Task<List<TenantScheduleViewModel>> GetUserBookingsAsync();

        /// <summary>
        /// Lấy danh sách hóa đơn của user
        /// </summary>
        Task<List<TenantInvoiceViewModel>> GetUserInvoicesAsync(Guid userId);

        /// <summary>
        /// Lấy hợp đồng của user
        /// </summary>
        Task<object> GetUserContractAsync(Guid userId);
    }
}
