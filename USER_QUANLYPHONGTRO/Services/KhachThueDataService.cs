using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;

namespace USER_QUANLYPHONGTRO.Services
{
    /// <summary>
    /// Service xử lý dữ liệu cho KhachThue - cách ly business logic
    /// </summary>
    public class KhachThueDataService
    {
        private readonly ApiClient _apiClient;
        private readonly IPhongApiService _phongApiService;

        public KhachThueDataService()
        {
            _apiClient = new ApiClient();
            _phongApiService = new PhongApiService();
        }

        #region Booking Methods

        /// <summary>
        /// Lấy danh sách lịch đã đặt của user
        /// </summary>
        public async Task<List<TenantScheduleViewModel>> GetUserBookingsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 GetUserBookingsAsync - Starting");

                var response = await _apiClient.GetAsync<dynamic>("/api/datphong/my-bookings");

                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}, StatusCode: {response?.StatusCode}");

                if (response == null || !response.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ API failed with status {response?.StatusCode}");
                    return new List<TenantScheduleViewModel>();
                }

                var bookings = new List<TenantScheduleViewModel>();

                if (response.Data != null)
                {
                    var dataArray = response.Data as JArray;
                    if (dataArray != null && dataArray.Count > 0)
                    {
                        foreach (var item in dataArray)
                        {
                            try
                            {
                                var booking = MapToTenantScheduleViewModel(item);
                                if (booking != null)
                                {
                                    bookings.Add(booking);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ Error mapping booking item: {ex.Message}");
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ Mapped {bookings.Count} bookings");
                    }
                }

                return bookings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetUserBookingsAsync Error: {ex.Message}");
                return new List<TenantScheduleViewModel>();
            }
        }

        /// <summary>
        /// Map JToken to TenantScheduleViewModel
        /// </summary>
        private TenantScheduleViewModel MapToTenantScheduleViewModel(JToken item)
        {
            if (item == null) return null;

            var phongIdStr = GetValue<string>(item["PhongId"] ?? item["phongId"], null);
            string tieuDePhong = "Phòng trọ";
            string diaChi = "Đang cập nhật";

            // Lấy thông tin phòng nếu có PhongId
            if (!string.IsNullOrEmpty(phongIdStr) && Guid.TryParse(phongIdStr, out var phongId))
            {
                try
                {
                    var phongDetail = _phongApiService.GetRoomDetailAsync(phongId).Result;
                    if (phongDetail != null)
                    {
                        tieuDePhong = phongDetail.TieuDe;
                        if (phongDetail.NhaTro != null)
                        {
                            diaChi = phongDetail.NhaTro.DiaChi;
                        }
                    }
                }
                catch { /* Use default values */ }
            }

            var trangThaiId = GetValue<int>(item["TrangThaiId"] ?? item["trangThaiId"], 1);

            return new TenantScheduleViewModel
            {
                BookingId = GetValue<Guid>(item["DatPhongId"] ?? item["datPhongId"], Guid.Empty),
                PhongId = GetValue<Guid>(item["PhongId"] ?? item["phongId"], Guid.Empty),
                TieuDePhong = tieuDePhong,
                DiaChi = diaChi,
                TrangThaiId = trangThaiId,
                TrangThai = GetTrangThaiText(trangThaiId),
                ThoiGianHen = GetValue<DateTime>(item["BatDau"] ?? item["batDau"], DateTime.Now),
                SdtChuTro = GetValue<string>(item["SdtChuTro"] ?? item["sdtChuTro"], "N/A"),
                GhiChu = GetValue<string>(item["GhiChu"] ?? item["ghiChu"], ""),
                LoaiDatPhong = GetValue<string>(item["Loai"] ?? item["loai"], "XemPhong")
            };
        }

        #endregion

        #region Invoice Methods

        /// <summary>
        /// Lấy danh sách hóa đơn của user
        /// </summary>
        public async Task<List<TenantInvoiceViewModel>> GetUserInvoicesAsync(Guid userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 GetUserInvoicesAsync - Starting for UserId: {userId}");

                var invoicesService = new InvoicesApiService();
                var response = await invoicesService.GetInvoicesByTenantAsync(userId, null);

                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}, StatusCode: {response?.StatusCode}");

                if (response == null || !response.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ API failed with status {response?.StatusCode}");
                    return new List<TenantInvoiceViewModel>();
                }

                return response.Data ?? new List<TenantInvoiceViewModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetUserInvoicesAsync Error: {ex.Message}");
                return new List<TenantInvoiceViewModel>();
            }
        }

        #endregion

        #region Contract Methods

        /// <summary>
        /// Lấy hợp đồng của user
        /// </summary>
        public async Task<object> GetUserContractAsync(Guid userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 GetUserContractAsync - Starting for UserId: {userId}");

                var contractsService = new ContractsApiService();
                var response = await contractsService.GetActiveContractByTenantAsync(userId, null);

                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}, StatusCode: {response?.StatusCode}");

                if (response == null || !response.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ API failed with status {response?.StatusCode}");
                    return null;
                }

                return response.Data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetUserContractAsync Error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Helper Methods

        private T GetValue<T>(JToken token, T defaultValue = default)
        {
            if (token == null || token.Type == JTokenType.Null)
                return defaultValue;

            try
            {
                if (token.Type == JTokenType.String && string.IsNullOrWhiteSpace(token.ToString()))
                {
                    return defaultValue;
                }
                return token.ToObject<T>();
            }
            catch
            {
                return defaultValue;
            }
        }

        private string GetTrangThaiText(int trangThaiId)
        {
            if (trangThaiId == 1) return "Chờ xác nhận";
            if (trangThaiId == 2) return "Đã xác nhận";
            if (trangThaiId == 3) return "Đã thanh toán";
            if (trangThaiId == 4) return "Hoàn thành";
            if (trangThaiId == 5) return "Đã hủy";
            return "Chờ xác nhận";
        }

        #endregion
    }
}
