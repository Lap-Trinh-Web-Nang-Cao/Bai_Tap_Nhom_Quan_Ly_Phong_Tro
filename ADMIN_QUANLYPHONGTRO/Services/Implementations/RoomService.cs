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
    public class RoomService : IRoomService
    {
        private readonly RoomApiClient _apiClient;

        public RoomService()
        {
            _apiClient = new RoomApiClient();
        }

        /// <summary>
        /// Lấy danh sách phòng chờ duyệt
        /// </summary>
        public async Task<PagedResult<RoomPendingItemViewModel>> GetPendingRoomsAsync(int pageIndex, int pageSize, string keyword = "")
        {
            try
            {
                var result = await _apiClient.GetPendingRooms(pageIndex, pageSize, keyword);
                
                if (result == null || result.Items == null || result.Items.Count == 0)
                {
                    return new PagedResult<RoomPendingItemViewModel>
                    {
                        Items = new List<RoomPendingItemViewModel>(),
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalRecords = 0
                    };
                }

                // Chuyển đổi PhongDto sang RoomPendingItemViewModel
                var viewModels = result.Items.Select(dto => new RoomPendingItemViewModel
                {
                    PhongId = dto.PhongId,
                    TieuDe = dto.TieuDe ?? "N/A",
                    DienTich = (decimal?)dto.DienTich,
                    GiaTien = (long)dto.GiaTien,
                    TienCoc = null,
                    SoNguoiToiDa = null,
                    TrangThai = "Chờ duyệt",
                    CreatedAt = dto.CreatedAt.LocalDateTime,
                    UpdatedAt = null,
                    DiemTrungBinh = 0,
                    SoLuongDanhGia = 0,
                    IsDuyet = dto.IsDuyet,
                    IsBiKhoa = dto.IsBiKhoa,
                    ThoiGianDuyet = null,
                    NhaTroName = "Nhà trọ",
                    ChuTroName = "Chủ trọ",
                    ImageUrl = "/Content/img/room-placeholder.jpg"
                }).ToList();

                return new PagedResult<RoomPendingItemViewModel>
                {
                    Items = viewModels,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = result.TotalRecords
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomService.GetPendingRoomsAsync Error: {ex.Message}");
                return new PagedResult<RoomPendingItemViewModel> 
                { 
                    Items = new List<RoomPendingItemViewModel>(), 
                    TotalRecords = 0 
                };
            }
        }

        /// <summary>
        /// Lấy chi tiết phòng để duyệt
        /// </summary>
        public async Task<RoomPendingItemViewModel> GetRoomDetailAsync(string roomId)
        {
            try
            {
                var dto = await _apiClient.GetRoomDetail(roomId);
                
                if (dto == null)
                    return null;

                return new RoomPendingItemViewModel
                {
                    PhongId = dto.PhongId,
                    TieuDe = dto.TieuDe ?? "N/A",
                    DienTich = (decimal?)dto.DienTich,
                    GiaTien = (long)dto.GiaTien,
                    TienCoc = null,
                    SoNguoiToiDa = null,
                    TrangThai = "Chờ duyệt",
                    CreatedAt = dto.CreatedAt.LocalDateTime,
                    UpdatedAt = null,
                    DiemTrungBinh = 0,
                    SoLuongDanhGia = 0,
                    IsDuyet = dto.IsDuyet,
                    IsBiKhoa = dto.IsBiKhoa,
                    ThoiGianDuyet = null,
                    NhaTroName = "Nhà trọ",
                    ChuTroName = "Chủ trọ",
                    ImageUrl = "/Content/img/room-placeholder.jpg"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomService.GetRoomDetailAsync Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Duyệt phòng
        /// </summary>
        public async Task<bool> ApproveRoomAsync(string roomId)
        {
            try
            {
                var result = await _apiClient.ApproveRoom(roomId);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomService.ApproveRoomAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Từ chối phòng
        /// </summary>
        public async Task<bool> RejectRoomAsync(string roomId, string reason)
        {
            try
            {
                var result = await _apiClient.RejectRoom(roomId, reason);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomService.RejectRoomAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Khóa/Mở khóa phòng
        /// </summary>
        public async Task<bool> ToggleLockRoomAsync(string roomId, bool isLocked = true)
        {
            try
            {
                var result = await _apiClient.ToggleLockRoom(roomId, isLocked);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomService.ToggleLockRoomAsync Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả phòng
        /// </summary>
        public async Task<PagedResult<RoomPendingItemViewModel>> GetAllRoomsAsync(int pageIndex, int pageSize, string keyword = "")
        {
            try
            {
                var result = await _apiClient.GetAllRooms(pageIndex, pageSize, keyword);
                
                if (result == null || result.Items == null || result.Items.Count == 0)
                {
                    return new PagedResult<RoomPendingItemViewModel>
                    {
                        Items = new List<RoomPendingItemViewModel>(),
                        PageIndex = pageIndex,
                        PageSize = pageSize,
                        TotalRecords = 0
                    };
                }

                // Chuyển đổi PhongDto sang RoomPendingItemViewModel
                var viewModels = result.Items.Select(dto => new RoomPendingItemViewModel
                {
                    PhongId = dto.PhongId,
                    TieuDe = dto.TieuDe ?? "N/A",
                    DienTich = (decimal?)dto.DienTich,
                    GiaTien = (long)dto.GiaTien,
                    TienCoc = null,
                    SoNguoiToiDa = null,
                    TrangThai = "Trống",
                    CreatedAt = dto.CreatedAt.LocalDateTime,
                    UpdatedAt = null,
                    DiemTrungBinh = 0,
                    SoLuongDanhGia = 0,
                    IsDuyet = dto.IsDuyet,
                    IsBiKhoa = dto.IsBiKhoa,
                    ThoiGianDuyet = null,
                    NhaTroName = "Nhà trọ",
                    ChuTroName = "Chủ trọ",
                    ImageUrl = "/Content/img/room-placeholder.jpg"
                }).ToList();

                return new PagedResult<RoomPendingItemViewModel>
                {
                    Items = viewModels,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = result.TotalRecords
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomService.GetAllRoomsAsync Error: {ex.Message}");
                return new PagedResult<RoomPendingItemViewModel> 
                { 
                    Items = new List<RoomPendingItemViewModel>(), 
                    TotalRecords = 0 
                };
            }
        }

        /// <summary>
        /// Lấy thống kê phòng
        /// </summary>
        public async Task<RoomStatsViewModel> GetRoomStatsAsync()
        {
            try
            {
                var stats = await _apiClient.GetRoomStats();
                
                if (stats == null)
                {
                    return new RoomStatsViewModel
                    {
                        Total = 0,
                        Pending = 0,
                        Approved = 0,
                        Locked = 0
                    };
                }

                return new RoomStatsViewModel
                {
                    Total = stats.Total,
                    Pending = stats.Pending,
                    Approved = stats.Approved,
                    Locked = stats.Locked
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomService.GetRoomStatsAsync Error: {ex.Message}");
                return new RoomStatsViewModel
                {
                    Total = 0,
                    Pending = 0,
                    Approved = 0,
                    Locked = 0
                };
            }
        }
    }
}
