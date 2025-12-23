using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class HostApiClient : BaseApiClient
    {
        /// <summary>
        /// Lấy danh sách chủ trọ chờ duyệt từ Backend API
        /// </summary>
        public async Task<PagedResult<HostPendingItemViewModel>> GetPendingHosts(int pageIndex, int pageSize, string keyword = "", string status = "")
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔍 HostApiClient.GetPendingHosts: pageIndex={pageIndex}, pageSize={pageSize}, keyword={keyword}, status={status}");
                
                // Ensure pageSize is within backend allowed range (1..100)
                if (pageSize < 1)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ HostApiClient.GetPendingHosts: pageSize {pageSize} too small, clamping to 1");
                    pageSize = 1;
                }
                else if (pageSize > 100)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ HostApiClient.GetPendingHosts: pageSize {pageSize} too large, clamping to 100");
                    pageSize = 100;
                }

                // Build query string
                var url = $"api/hosts/pending?pageIndex={pageIndex}&pageSize={pageSize}";
                if (!string.IsNullOrEmpty(keyword))
                    url += $"&keyword={Uri.EscapeDataString(keyword)}";
                if (!string.IsNullOrEmpty(status))
                    url += $"&status={Uri.EscapeDataString(status)}";

                var result = await GetAsync<HostPendingListResponse>(url);

                System.Diagnostics.Debug.WriteLine($"📦 Raw API Response: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");

                if (result == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Result is null");
                    return new PagedResult<HostPendingItemViewModel> { Items = new List<HostPendingItemViewModel>(), TotalRecords = 0 };
                }

                var items = result.items;
                System.Diagnostics.Debug.WriteLine($"📋 Items count: {items?.Count ?? 0}");

                if (items == null || items.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Items is null or empty");
                    return new PagedResult<HostPendingItemViewModel> 
                    { 
                        Items = new List<HostPendingItemViewModel>(), 
                        TotalRecords = result.totalCount,
                        PageIndex = result.pageIndex,
                        PageSize = result.pageSize
                    };
                }

                // Map Backend DTOs to ViewModel
                var mappedItems = items.Select(item => new HostPendingItemViewModel
                {
                    NguoiDungId = item.nguoiDungId,
                    HoTen = item.hoTen ?? "Chủ trọ",
                    Email = item.email ?? "",
                    DienThoai = item.dienThoai ?? "",
                    Avatar = !string.IsNullOrEmpty(item.avatar) ? item.avatar : "/Content/img/default-avatar.png",
                    SoCCCD = item.soCCCD ?? "",
                    LoaiGiayTo = item.loaiGiayTo ?? "",
                    DaTaiGiayTo = item.daTaiGiayTo,
                    SoTapTinDinhKem = item.soTapTinDinhKem,
                    NgayDangKy = item.ngayDangKy,
                    TrangThaiXacThuc = item.trangThaiXacThuc ?? "Chờ duyệt"
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"✅ Mapped {mappedItems.Count} DTOs");

                if (mappedItems.Any())
                {
                    var sample = mappedItems.First();
                    System.Diagnostics.Debug.WriteLine($"📌 Sample Item:");
                    System.Diagnostics.Debug.WriteLine($"   - NguoiDungId: {sample.NguoiDungId}");
                    System.Diagnostics.Debug.WriteLine($"   - HoTen: {sample.HoTen}");
                    System.Diagnostics.Debug.WriteLine($"   - Email: {sample.Email}");
                    System.Diagnostics.Debug.WriteLine($"   - TrangThaiXacThuc: {sample.TrangThaiXacThuc}");
                }

                return new PagedResult<HostPendingItemViewModel>
                {
                    Items = mappedItems,
                    PageIndex = result.pageIndex,
                    PageSize = result.pageSize,
                    TotalRecords = result.totalCount
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostApiClient.GetPendingHosts Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new PagedResult<HostPendingItemViewModel> { Items = new List<HostPendingItemViewModel>(), TotalRecords = 0 };
            }
        }

        /// <summary>
        /// Lấy chi tiết chủ trọ từ Backend API
        /// </summary>
        public async Task<HostApprovalDetailViewModel> GetHostDetail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;

                var result = await GetAsync<HostApprovalItemDto>($"api/hosts/{id}/detail");

                if (result == null)
                    return null;

                return new HostApprovalDetailViewModel
                {
                    NguoiDungId = result.nguoiDungId,
                    HoTen = result.hoTen ?? "Chủ trọ",
                    Email = result.email ?? "",
                    DienThoai = result.dienThoai ?? "",
                    Avatar = !string.IsNullOrEmpty(result.avatar) ? result.avatar : "/Content/img/default-avatar.png",
                    SoCCCD = result.soCCCD ?? "",
                    NgaySinh = result.ngaySinh,
                    QueQuan = result.queQuan ?? "",
                    CCCDMatTruocUrl = !string.IsNullOrEmpty(result.cccdMatTruocUrl) ? result.cccdMatTruocUrl : "/Content/img/no-image.png",
                    CCCDMatSauUrl = !string.IsNullOrEmpty(result.cccdMatSauUrl) ? result.cccdMatSauUrl : "/Content/img/no-image.png",
                    GiayPhepKinhDoanhUrl = !string.IsNullOrEmpty(result.giayPhepKinhDoanhUrl) ? result.giayPhepKinhDoanhUrl : "/Content/img/no-image.png",
                    TrangThaiXacThuc = result.trangThaiXacThuc ?? "Chờ duyệt"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostApiClient.GetHostDetail Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Xác thực chủ trọ
        /// </summary>
        public async Task<ApiResponse<bool>> ApproveHost(string id)
        {
            try
            {
                // Send an empty JSON object instead of null to avoid sending a literal "null" body.
                var result = await PutAsync<dynamic>($"api/hosts/{id}/approve", new { });

                System.Diagnostics.Debug.WriteLine($"🔁 HostApiClient.ApproveHost response: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");

                // Parse response từ Backend
                if (result != null)
                {
                    bool success = false;
                    string message = "Đã xác thực chủ trọ";
                    try
                    {
                        success = result.success ?? false;
                        message = result.message ?? message;
                    }
                    catch
                    {
                        // fallback in case dynamic shape different
                    }

                    return new ApiResponse<bool> { Success = success, Message = message, Data = success };
                }

                return new ApiResponse<bool> { Success = true, Message = "Đã xác thực chủ trọ", Data = true };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostApiClient.ApproveHost Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Từ chối chủ trọ
        /// </summary>
        public async Task<ApiResponse<bool>> RejectHost(string id, string reason)
        {
            try
            {
                var result = await PutAsync<dynamic>($"api/hosts/{id}/reject", new { Reason = reason });
                
                // Parse response từ Backend
                if (result != null)
                {
                    bool success = result.success ?? false;
                    string message = result.message ?? "Đã từ chối chủ trọ";
                    return new ApiResponse<bool> { Success = success, Message = message, Data = success };
                }
                
                return new ApiResponse<bool> { Success = true, Message = "Đã từ chối chủ trọ" };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostApiClient.RejectHost Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        /// <summary>
        /// Lấy thống kê chủ trọ theo trạng thái
        /// </summary>
        public async Task<HostStatsResponse> GetHostStats()
        {
            try
            {
                var result = await GetAsync<HostStatsResponse>("api/hosts/stats");
                return result ?? new HostStatsResponse();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostApiClient.GetHostStats Error: {ex.Message}");
                return new HostStatsResponse();
            }
        }
    }

    /// <summary>
    /// Response cho thống kê chủ trọ
    /// </summary>
    public class HostStatsResponse
    {
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Total { get; set; }
    }
}
