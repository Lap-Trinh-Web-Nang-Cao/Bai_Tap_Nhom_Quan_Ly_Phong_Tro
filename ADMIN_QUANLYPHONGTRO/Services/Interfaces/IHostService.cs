using System.Collections.Generic;
using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IHostService
    {
        // Lấy danh sách chủ trọ chờ duyệt (với phân trang)
        Task<PagedResult<HostPendingItemViewModel>> GetPendingHostsAsync(int pageIndex, int pageSize, string keyword = "");
        
        // Lấy chi tiết chủ trọ để duyệt
        Task<HostApprovalDetailViewModel> GetHostDetailAsync(string hostId);
        
        // Phê duyệt chủ trọ
        Task<bool> ApproveHostAsync(string hostId);
        
        // Từ chối chủ trọ
        Task<bool> RejectHostAsync(string hostId, string reason);
    }
}
