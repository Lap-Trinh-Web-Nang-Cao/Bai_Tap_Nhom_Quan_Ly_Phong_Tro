using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    public class HostService : IHostService
    {
        private readonly HostApiClient _apiClient;

        public HostService()
        {
            _apiClient = new HostApiClient();
        }

        /// <summary>
        /// Lấy danh sách chủ trọ chờ duyệt
        /// </summary>
        public async Task<PagedResult<HostPendingItemViewModel>> GetPendingHostsAsync(int pageIndex, int pageSize, string keyword = "")
        {
            try
            {
                var result = await _apiClient.GetPendingHosts(pageIndex, pageSize, keyword);
                return result;  // ✅ Trả về PagedResult hoàn chỉnh
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.GetPendingHostsAsync Error: {ex.Message}");
                return new PagedResult<HostPendingItemViewModel> 
                { 
                    Items = new List<HostPendingItemViewModel>(), 
                    TotalRecords = 0 
                };
            }
        }

        /// <summary>
        /// Lấy chi tiết chủ trọ để duyệt
        /// </summary>
        public async Task<HostApprovalDetailViewModel> GetHostDetailAsync(string hostId)
        {
            try
            {
                return await _apiClient.GetHostDetail(hostId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.GetHostDetailAsync Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Phê duyệt chủ trọ
        /// </summary>
        public async Task<bool> ApproveHostAsync(string hostId)
        {
            try
            {
                var result = await _apiClient.ApproveHost(hostId);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.ApproveHostAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Từ chối chủ trọ
        /// </summary>
        public async Task<bool> RejectHostAsync(string hostId, string reason)
        {
            try
            {
                var result = await _apiClient.RejectHost(hostId, reason);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostService.RejectHostAsync Error: {ex.Message}");
                return false;
            }
        }
    }
}
