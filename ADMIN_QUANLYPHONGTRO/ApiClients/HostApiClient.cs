using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
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
                var result = await GetAsync<dynamic>(
                    $"hosts/pending?pageIndex={pageIndex}&pageSize={pageSize}&keyword={keyword}"
                );

                if (result == null || result.Items == null)
                    return new PagedResult<HostPendingItemViewModel> { Items = new List<HostPendingItemViewModel>(), TotalRecords = 0 };

                // Map Backend DTOs to ViewModel
                var items = new List<HostPendingItemViewModel>();
                
                try
                {
                    foreach (var item in result.Items)
                    {
                        // Parse Guid safely
                        Guid nguoiDungId;
                        if (item.NguoiDungId == null)
                        {
                            System.Diagnostics.Debug.WriteLine("⚠️ NguoiDungId is null, skipping this item");
                            continue;
                        }

                        // Try to parse the Guid
                        if (item.NguoiDungId is Guid guidValue)
                        {
                            nguoiDungId = guidValue;
                        }
                        else if (item.NguoiDungId is string guidString && Guid.TryParse(guidString, out var parsedGuid))
                        {
                            nguoiDungId = parsedGuid;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ Could not parse Guid: {item.NguoiDungId}");
                            continue;
                        }

                        items.Add(new HostPendingItemViewModel
                        {
                            NguoiDungId = nguoiDungId,
                            HoTen = item.HoTen ?? "Chủ trọ",
                            Email = item.Email ?? "",
                            DienThoai = item.DienThoai ?? "",
                            Avatar = item.Avatar ?? "/Content/img/default-avatar.png",
                            SoCCCD = item.SoCCCD ?? "",
                            DaTaiGiayTo = item.DaTaiGiayTo ?? false,
                            SoTapTinDinhKem = item.SoTapTinDinhKem ?? 0,
                            NgayDangKy = item.NgayDangKy ?? DateTime.Now,
                            TrangThaiXacThuc = item.TrangThaiXacThuc ?? "Chờ duyệt",
                            LoaiGiayTo = item.LoaiGiayTo ?? ""
                        });
                    }
                }
                catch (Exception mapEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error mapping items: {mapEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {mapEx.StackTrace}");
                }

                return new PagedResult<HostPendingItemViewModel>
                {
                    Items = items,
                    PageIndex = result.PageIndex ?? pageIndex,
                    PageSize = result.PageSize ?? pageSize,
                    TotalRecords = result.TotalCount ?? result.TotalRecords ?? 0
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

                var result = await GetAsync<dynamic>($"hosts/{id}/detail");

                if (result == null)
                    return null;

                return new HostApprovalDetailViewModel
                {
                    NguoiDungId = result.NguoiDungId,
                    HoTen = result.HoTen ?? "Chủ trọ",
                    Email = result.Email ?? "",
                    DienThoai = result.DienThoai ?? "",
                    Avatar = result.Avatar ?? "/Content/img/default-avatar.png",
                    SoCCCD = result.SoCCCD ?? "",
                    NgaySinh = result.NgaySinh ?? DateTime.Now,
                    QueQuan = result.QueQuan ?? "",
                    CCCDMatTruocUrl = result.CCCDMatTruocUrl ?? "/Content/img/no-image.png",
                    CCCDMatSauUrl = result.CCCDMatSauUrl ?? "/Content/img/no-image.png",
                    GiayPhepKinhDoanhUrl = result.GiayPhepKinhDoanhUrl ?? "/Content/img/no-image.png",
                    TrangThaiXacThuc = result.TrangThaiXacThuc ?? "Chờ duyệt"
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
