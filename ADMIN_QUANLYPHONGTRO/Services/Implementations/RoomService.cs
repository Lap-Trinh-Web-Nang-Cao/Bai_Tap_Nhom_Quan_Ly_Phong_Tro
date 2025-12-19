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
                // TODO: Gọi API thực tế
                // var result = await _apiClient.GetPendingRooms(pageIndex, pageSize, keyword);
                // return result;

                // STUB DATA cho testing UI
                await Task.Delay(100); // Simulate API call

                var mockData = new List<RoomPendingItemViewModel>
                {
                    new RoomPendingItemViewModel
                    {
                        PhongId = Guid.NewGuid(),
                        TieuDe = "Phòng trọ cao cấp gần ĐH Đà Nẵng",
                        DienTich = 25m,
                        GiaTien = 3500000,
                        TienCoc = 3500000,
                        SoNguoiToiDa = 2,
                        TrangThai = "Trống",
                        CreatedAt = DateTime.Now.AddDays(-2),
                        IsDuyet = false,
                        IsBiKhoa = false,
                        NhaTroName = "Nhà trọ ABC",
                        ChuTroName = "Nguyễn Văn A",
                        DiemTrungBinh = 0,
                        SoLuongDanhGia = 0,
                        ImageUrl = "/Content/img/room-placeholder.jpg"
                    },
                    new RoomPendingItemViewModel
                    {
                        PhongId = Guid.NewGuid(),
                        TieuDe = "Phòng đơn có gác lửng",
                        DienTich = 18m,
                        GiaTien = 2500000,
                        TienCoc = 2500000,
                        SoNguoiToiDa = 1,
                        TrangThai = "Trống",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        IsDuyet = false,
                        IsBiKhoa = false,
                        NhaTroName = "Nhà trọ XYZ",
                        ChuTroName = "Trần Thị B",
                        DiemTrungBinh = 4.5,
                        SoLuongDanhGia = 12,
                        ImageUrl = "/Content/img/room-placeholder.jpg"
                    }
                };

                return new PagedResult<RoomPendingItemViewModel>
                {
                    Items = mockData,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = mockData.Count
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
                // TODO: Gọi API thực tế
                await Task.Delay(100);

                return new RoomPendingItemViewModel
                {
                    PhongId = Guid.Parse(roomId),
                    TieuDe = "Phòng trọ cao cấp gần ĐH Đà Nẵng",
                    DienTich = 25m,
                    GiaTien = 3500000,
                    TienCoc = 3500000,
                    SoNguoiToiDa = 2,
                    TrangThai = "Trống",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    IsDuyet = false,
                    IsBiKhoa = false,
                    NhaTroName = "Nhà trọ ABC",
                    ChuTroName = "Nguyễn Văn A",
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
                // TODO: Gọi API thực tế
                await Task.Delay(100);
                System.Diagnostics.Debug.WriteLine($"✅ Approved room: {roomId}");
                return true;
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
                // TODO: Gọi API thực tế
                await Task.Delay(100);
                System.Diagnostics.Debug.WriteLine($"❌ Rejected room: {roomId}, Reason: {reason}");
                return true;
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
                // TODO: Gọi API thực tế
                await Task.Delay(100);
                System.Diagnostics.Debug.WriteLine($"🔒 {(isLocked ? "Locked" : "Unlocked")} room: {roomId}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomService.ToggleLockRoomAsync Error: {ex.Message}");
                return false;
            }
        }
    }
}
