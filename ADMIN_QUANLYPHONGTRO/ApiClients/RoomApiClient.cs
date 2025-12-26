using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    public class RoomApiClient : BaseApiClient
    {
        /// <summary>
        /// Gọi API để lấy danh sách phòng chờ duyệt
        /// </summary>
        public async Task<PagedResult<PhongDto>> GetPendingRooms(int pageIndex, int pageSize, string keyword = "")
        {
            try
            {
                var response = await GetAsync<JObject>(
                    $"phong/pending?pageIndex={pageIndex}&pageSize={pageSize}&keyword={Uri.EscapeDataString(keyword ?? "")}"
                );

                if (response == null)
                    return new PagedResult<PhongDto> { Items = new System.Collections.Generic.List<PhongDto>(), TotalRecords = 0 };

                var items = response["data"]?.ToObject<System.Collections.Generic.List<PhongDto>>() 
                    ?? new System.Collections.Generic.List<PhongDto>();
                
                return new PagedResult<PhongDto>
                {
                    Items = items,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = response["totalCount"]?.Value<int>() ?? 0
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetPendingRooms Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gọi API để lấy danh sách tất cả phòng (cho List page)
        /// </summary>
        public async Task<PagedResult<PhongDto>> GetAllRooms(int pageIndex, int pageSize, string keyword = "")
        {
            try
            {
                var response = await GetAsync<JObject>(
                    $"phong?pageIndex={pageIndex}&pageSize={pageSize}&keyword={Uri.EscapeDataString(keyword ?? "")}"
                );

                if (response == null)
                    return new PagedResult<PhongDto> { Items = new System.Collections.Generic.List<PhongDto>(), TotalRecords = 0 };

                var items = response["data"]?.ToObject<System.Collections.Generic.List<PhongDto>>()
                    ?? new System.Collections.Generic.List<PhongDto>();
                
                return new PagedResult<PhongDto>
                {
                    Items = items,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = response["totalCount"]?.Value<int>() ?? 0
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetAllRooms Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gọi API để lấy chi tiết phòng
        /// </summary>
        public async Task<PhongDto> GetRoomDetail(string id)
        {
            try
            {
                return await GetAsync<PhongDto>($"phong/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetRoomDetail Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gọi API để duyệt phòng
        /// </summary>
        public async Task<ApiResponse<bool>> ApproveRoom(string id)
        {
            try
            {
                return await PutAsync<ApiResponse<bool>>($"phong/approve/{id}", null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ApproveRoom Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gọi API để từ chối phòng
        /// </summary>
        public async Task<ApiResponse<bool>> RejectRoom(string id, string reason)
        {
            try
            {
                return await PutAsync<ApiResponse<bool>>($"phong/{id}/reject", new { reason });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RejectRoom Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gọi API để khóa/mở khóa phòng
        /// </summary>
        public async Task<ApiResponse<bool>> ToggleLockRoom(string id, bool isLocked = true)
        {
            try
            {
                return await PutAsync<ApiResponse<bool>>($"phong/lock/{id}?isLocked={isLocked}", null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ToggleLockRoom Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gọi API để lấy thống kê phòng
        /// </summary>
        public async Task<RoomStatsDto> GetRoomStats()
        {
            try
            {
                var response = await GetAsync<JObject>("phong/stats");
                
                if (response == null)
                    return new RoomStatsDto { Total = 0, Pending = 0, Approved = 0, Locked = 0 };

                return new RoomStatsDto
                {
                    Total = response["total"]?.Value<int>() ?? 0,
                    Pending = response["pending"]?.Value<int>() ?? 0,
                    Approved = response["approved"]?.Value<int>() ?? 0,
                    Locked = response["locked"]?.Value<int>() ?? 0
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetRoomStats Error: {ex.Message}");
                return new RoomStatsDto { Total = 0, Pending = 0, Approved = 0, Locked = 0 };
            }
        }
    }

    public class RoomStatsDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Locked { get; set; }
    }
}
