using RestAPI_QUANLYPHONGTRO.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestAPI_QUANLYPHONGTRO.Services.Interfaces
{
    public interface IHostService
    {
        // Lấy danh sách chủ trọ chờ duyệt
        Task<PagedResult<HostPendingDto>> GetPendingHostsAsync(int pageIndex, int pageSize, string keyword = "");
        
        // Lấy chi tiết chủ trọ để duyệt
        Task<HostApprovalDto> GetHostDetailAsync(string hostId);
        
        // Phê duyệt chủ trọ
        Task<bool> ApproveHostAsync(string hostId);
        
        // Từ chối chủ trọ
        Task<bool> RejectHostAsync(string hostId, string reason);
    }
}
