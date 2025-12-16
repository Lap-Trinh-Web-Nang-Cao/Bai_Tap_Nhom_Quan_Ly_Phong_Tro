using ADMIN_QUANLYPHONGTRO.Models.ViewModels;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface cho Dashboard Service
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Lấy toàn bộ dữ liệu cho Dashboard
        /// </summary>
        Task<DashboardViewModel> GetDashboardDataAsync();

        /// <summary>
        /// Refresh cache dashboard (nếu có)
        /// </summary>
        Task RefreshDashboardCacheAsync();
    }
}
