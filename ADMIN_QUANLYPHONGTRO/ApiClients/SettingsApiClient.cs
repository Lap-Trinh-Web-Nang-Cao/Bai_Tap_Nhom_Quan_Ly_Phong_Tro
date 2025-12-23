using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.ApiClients
{
    /// <summary>
    /// API Client cho Settings - Quản lý Tiện ích, Quận/Huyện, Phường, Loại hỗ trợ, Loại vi phạm
    /// </summary>
    public class SettingsApiClient : BaseApiClient
    {
        #region Tiện Ích

        public async Task<List<TienIchDto>> GetAllTienIch()
        {
            try
            {
                var token = await GetAsync<JToken>("api/tienich");
                return ParseListResponse<TienIchDto>(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetAllTienIch Error: {ex.Message}");
                return new List<TienIchDto>();
            }
        }

        public async Task<TienIchDto> GetTienIchById(int id)
        {
            try
            {
                return await GetAsync<TienIchDto>($"api/tienich/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetTienIchById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<TienIchDto>> CreateTienIch(string ten)
        {
            try
            {
                var request = new { Ten = ten };
                var result = await PostAsync<TienIchDto>("api/tienich", request);
                return new ApiResponse<TienIchDto>
                {
                    Success = result != null,
                    Message = result != null ? "Thêm tiện ích thành công" : "Không thể thêm tiện ích",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.CreateTienIch Error: {ex.Message}");
                return new ApiResponse<TienIchDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateTienIch(int id, string ten)
        {
            try
            {
                var request = new { Ten = ten };
                var result = await PutAsync<TienIchDto>($"api/tienich/{id}", request);
                return new ApiResponse<bool>
                {
                    Success = result != null,
                    Message = result != null ? "Cập nhật tiện ích thành công" : "Không thể cập nhật tiện ích"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.UpdateTienIch Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteTienIch(int id)
        {
            try
            {
                var success = await DeleteAsync($"api/tienich/{id}");
                return new ApiResponse<bool>
                {
                    Success = success,
                    Message = success ? "Xóa tiện ích thành công" : "Không thể xóa tiện ích"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.DeleteTienIch Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Quận/Huyện

        public async Task<List<QuanHuyenDto>> GetAllQuanHuyen()
        {
            try
            {
                var token = await GetAsync<JToken>("api/quanhuyen");
                return ParseListResponse<QuanHuyenDto>(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetAllQuanHuyen Error: {ex.Message}");
                return new List<QuanHuyenDto>();
            }
        }

        public async Task<QuanHuyenDto> GetQuanHuyenById(int id)
        {
            try
            {
                return await GetAsync<QuanHuyenDto>($"api/quanhuyen/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetQuanHuyenById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<QuanHuyenDto>> CreateQuanHuyen(string ten)
        {
            try
            {
                var request = new { Ten = ten };
                var result = await PostAsync<QuanHuyenDto>("api/quanhuyen", request);
                return new ApiResponse<QuanHuyenDto>
                {
                    Success = result != null,
                    Message = result != null ? "Thêm quận/huyện thành công" : "Không thể thêm quận/huyện",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.CreateQuanHuyen Error: {ex.Message}");
                return new ApiResponse<QuanHuyenDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateQuanHuyen(int id, string ten)
        {
            try
            {
                var request = new { Ten = ten };
                var result = await PutAsync<QuanHuyenDto>($"api/quanhuyen/{id}", request);
                return new ApiResponse<bool>
                {
                    Success = result != null,
                    Message = result != null ? "Cập nhật quận/huyện thành công" : "Không thể cập nhật quận/huyện"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.UpdateQuanHuyen Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteQuanHuyen(int id)
        {
            try
            {
                var success = await DeleteAsync($"api/quanhuyen/{id}");
                return new ApiResponse<bool>
                {
                    Success = success,
                    Message = success ? "Xóa quận/huyện thành công" : "Không thể xóa quận/huyện"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.DeleteQuanHuyen Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Phường

        public async Task<List<PhuongDto>> GetAllPhuong()
        {
            try
            {
                var token = await GetAsync<JToken>("api/phuong");
                return ParseListResponse<PhuongDto>(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetAllPhuong Error: {ex.Message}");
                return new List<PhuongDto>();
            }
        }

        public async Task<List<PhuongDto>> GetPhuongByQuan(int quanHuyenId)
        {
            try
            {
                var token = await GetAsync<JToken>($"api/phuong/by-quan/{quanHuyenId}");
                return ParseListResponse<PhuongDto>(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetPhuongByQuan Error: {ex.Message}");
                return new List<PhuongDto>();
            }
        }

        public async Task<PhuongDto> GetPhuongById(int id)
        {
            try
            {
                return await GetAsync<PhuongDto>($"api/phuong/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetPhuongById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<PhuongDto>> CreatePhuong(int quanHuyenId, string ten)
        {
            try
            {
                var request = new { QuanHuyenId = quanHuyenId, Ten = ten };
                var result = await PostAsync<PhuongDto>("api/phuong", request);
                return new ApiResponse<PhuongDto>
                {
                    Success = result != null,
                    Message = result != null ? "Thêm phường thành công" : "Không thể thêm phường",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.CreatePhuong Error: {ex.Message}");
                return new ApiResponse<PhuongDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdatePhuong(int id, int quanHuyenId, string ten)
        {
            try
            {
                var request = new { QuanHuyenId = quanHuyenId, Ten = ten };
                var result = await PutAsync<PhuongDto>($"api/phuong/{id}", request);
                return new ApiResponse<bool>
                {
                    Success = result != null,
                    Message = result != null ? "Cập nhật phường thành công" : "Không thể cập nhật phường"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.UpdatePhuong Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeletePhuong(int id)
        {
            try
            {
                var success = await DeleteAsync($"api/phuong/{id}");
                return new ApiResponse<bool>
                {
                    Success = success,
                    Message = success ? "Xóa phường thành công" : "Không thể xóa phường"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.DeletePhuong Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Loại Hỗ Trợ

        public async Task<List<LoaiHoTroDto>> GetAllLoaiHoTro()
        {
            try
            {
                var token = await GetAsync<JToken>("api/loaihotro");
                return ParseListResponse<LoaiHoTroDto>(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetAllLoaiHoTro Error: {ex.Message}");
                return new List<LoaiHoTroDto>();
            }
        }

        public async Task<LoaiHoTroDto> GetLoaiHoTroById(int id)
        {
            try
            {
                return await GetAsync<LoaiHoTroDto>($"api/loaihotro/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetLoaiHoTroById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<LoaiHoTroDto>> CreateLoaiHoTro(string tenLoai)
        {
            try
            {
                var request = new { TenLoai = tenLoai };
                var result = await PostAsync<LoaiHoTroDto>("api/loaihotro", request);
                return new ApiResponse<LoaiHoTroDto>
                {
                    Success = result != null,
                    Message = result != null ? "Thêm loại hỗ trợ thành công" : "Không thể thêm loại hỗ trợ",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.CreateLoaiHoTro Error: {ex.Message}");
                return new ApiResponse<LoaiHoTroDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateLoaiHoTro(int id, string tenLoai)
        {
            try
            {
                var request = new { TenLoai = tenLoai };
                var result = await PutAsync<LoaiHoTroDto>($"api/loaihotro/{id}", request);
                return new ApiResponse<bool>
                {
                    Success = result != null,
                    Message = result != null ? "Cập nhật loại hỗ trợ thành công" : "Không thể cập nhật loại hỗ trợ"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.UpdateLoaiHoTro Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteLoaiHoTro(int id)
        {
            try
            {
                var success = await DeleteAsync($"api/loaihotro/{id}");
                return new ApiResponse<bool>
                {
                    Success = success,
                    Message = success ? "Xóa loại hỗ trợ thành công" : "Không thể xóa loại hỗ trợ"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.DeleteLoaiHoTro Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Loại Vi Phạm

        public async Task<List<ViPhamDto>> GetAllViPham()
        {
            try
            {
                var token = await GetAsync<JToken>("api/vipham");
                return ParseListResponse<ViPhamDto>(token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetAllViPham Error: {ex.Message}");
                return new List<ViPhamDto>();
            }
        }

        public async Task<ViPhamDto> GetViPhamById(int id)
        {
            try
            {
                return await GetAsync<ViPhamDto>($"api/vipham/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetViPhamById Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<ViPhamDto>> CreateViPham(string tenViPham, string moTa, long? hinhPhatTien, int? soDiemTru)
        {
            try
            {
                var request = new
                {
                    TenViPham = tenViPham,
                    MoTa = moTa,
                    HinhPhatTien = hinhPhatTien,
                    SoDiemTru = soDiemTru
                };
                var result = await PostAsync<ViPhamDto>("api/vipham", request);
                return new ApiResponse<ViPhamDto>
                {
                    Success = result != null,
                    Message = result != null ? "Thêm loại vi phạm thành công" : "Không thể thêm loại vi phạm",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.CreateViPham Error: {ex.Message}");
                return new ApiResponse<ViPhamDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateViPham(int id, string tenViPham, string moTa, long? hinhPhatTien, int? soDiemTru)
        {
            try
            {
                var request = new
                {
                    TenViPham = tenViPham,
                    MoTa = moTa,
                    HinhPhatTien = hinhPhatTien,
                    SoDiemTru = soDiemTru
                };
                var result = await PutAsync<ViPhamDto>($"api/vipham/{id}", request);
                return new ApiResponse<bool>
                {
                    Success = result != null,
                    Message = result != null ? "Cập nhật loại vi phạm thành công" : "Không thể cập nhật loại vi phạm"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.UpdateViPham Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteViPham(int id)
        {
            try
            {
                var success = await DeleteAsync($"api/vipham/{id}");
                return new ApiResponse<bool>
                {
                    Success = success,
                    Message = success ? "Xóa loại vi phạm thành công" : "Không thể xóa loại vi phạm"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.DeleteViPham Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region System Settings (Cấu hình hệ thống)

        public async Task<List<dynamic>> GetAllSystemSettings()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔍 SettingsApiClient.GetAllSystemSettings called");
                var token = await GetAsync<JToken>("api/systemsettings");
                
                // Handle response format from backend
                if (token != null && token.Type == JTokenType.Object)
                {
                    // Response format: { success: true, data: [...] }
                    var dataToken = token["data"];
                    if (dataToken != null && dataToken.Type == JTokenType.Array)
                    {
                        var result = dataToken.ToObject<List<dynamic>>() ?? new List<dynamic>();
                        System.Diagnostics.Debug.WriteLine($"✅ GetAllSystemSettings: Retrieved {result.Count} settings");
                        return result;
                    }
                }
                
                // Fallback to direct array parsing
                if (token != null && token.Type == JTokenType.Array)
                {
                    var result = token.ToObject<List<dynamic>>() ?? new List<dynamic>();
                    System.Diagnostics.Debug.WriteLine($"✅ GetAllSystemSettings: Retrieved {result.Count} settings (array format)");
                    return result;
                }
                
                System.Diagnostics.Debug.WriteLine("⚠️ GetAllSystemSettings: No valid data in response");
                return new List<dynamic>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetAllSystemSettings Error: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<dynamic> GetSystemSettingByKey(string key)
        {
            try
            {
                return await GetAsync<dynamic>($"api/systemsettings/{key}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetSystemSettingByKey Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<dynamic>> GetSystemSettingsByGroup(string groupName)
        {
            try
            {
                var token = await GetAsync<JToken>($"api/systemsettings/by-group/{groupName}");
                
                // Handle response format from backend
                if (token != null && token.Type == JTokenType.Object)
                {
                    var dataToken = token["data"];
                    if (dataToken != null && dataToken.Type == JTokenType.Array)
                    {
                        return dataToken.ToObject<List<dynamic>>() ?? new List<dynamic>();
                    }
                }
                
                if (token != null && token.Type == JTokenType.Array)
                {
                    return token.ToObject<List<dynamic>>() ?? new List<dynamic>();
                }
                
                return new List<dynamic>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.GetSystemSettingsByGroup Error: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public async Task<ApiResponse<dynamic>> UpdateSystemSettingsByKey(Dictionary<string, string> settings)
        {
            try
            {
                var result = await PostAsync<dynamic>("api/systemsettings/update-by-key", settings);
                return new ApiResponse<dynamic>
                {
                    Success = result != null,
                    Message = "Cập nhật cài đặt thành công"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsApiClient.UpdateSystemSettingsByKey Error: {ex.Message}");
                return new ApiResponse<dynamic> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Parse response thành List, hỗ trợ cả array và object với items/data
        /// </summary>
        private List<T> ParseListResponse<T>(JToken token)
        {
            if (token == null)
                return new List<T>();

            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>() ?? new List<T>();
            }

            if (token.Type == JTokenType.Object)
            {
                var items = token["items"] ?? token["Items"] ?? token["data"] ?? token["Data"];
                if (items != null && items.Type == JTokenType.Array)
                {
                    return items.ToObject<List<T>>() ?? new List<T>();
                }
            }

            return new List<T>();
        }

        #endregion
    }
}
