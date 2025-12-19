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
        public async Task<PagedResult<HostPendingItemViewModel>> GetPendingHosts(int pageIndex, int pageSize, string keyword = "")
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔍 HostApiClient.GetPendingHosts: pageIndex={pageIndex}, pageSize={pageSize}, keyword={keyword}");
                
                var result = await GetAsync<HostPendingListResponse>(
                    $"hosts/pending?pageIndex={pageIndex}&pageSize={pageSize}&keyword={keyword}"
                );

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
                    Avatar = item.avatar ?? "/Content/img/default-avatar.png",
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

                var result = await GetAsync<HostApprovalItemDto>($"hosts/{id}/detail");

                if (result == null)
                    return null;

                return new HostApprovalDetailViewModel
                {
                    NguoiDungId = result.nguoiDungId,
                    HoTen = result.hoTen ?? "Chủ trọ",
                    Email = result.email ?? "",
                    DienThoai = result.dienThoai ?? "",
                    Avatar = result.avatar ?? "/Content/img/default-avatar.png",
                    SoCCCD = result.soCCCD ?? "",
                    NgaySinh = result.ngaySinh,
                    QueQuan = result.queQuan ?? "",
                    CCCDMatTruocUrl = result.cccdMatTruocUrl ?? "/Content/img/no-image.png",
                    CCCDMatSauUrl = result.cccdMatSauUrl ?? "/Content/img/no-image.png",
                    GiayPhepKinhDoanhUrl = result.giayPhepKinhDoanhUrl ?? "/Content/img/no-image.png",
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
                return await PutAsync<ApiResponse<bool>>($"hosts/{id}/approve", null);
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
                return await PutAsync<ApiResponse<bool>>($"hosts/{id}/reject", new { reason });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostApiClient.RejectHost Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }
    }
}
