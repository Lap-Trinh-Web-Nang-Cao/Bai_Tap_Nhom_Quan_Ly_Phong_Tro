using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Services.Implementations
{
    /// <summary>
    /// Service xử lý nghiệp vụ Settings - Quản lý Tiện ích, Quận/Huyện, Phường, Loại hỗ trợ, Loại vi phạm
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly SettingsApiClient _apiClient;

        public SettingsService()
        {
            _apiClient = new SettingsApiClient();
        }

        #region Tiện Ích

        public async Task<List<TienIchDto>> GetAllTienIchAsync()
        {
            try
            {
                return await _apiClient.GetAllTienIch();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetAllTienIchAsync Error: {ex.Message}");
                return new List<TienIchDto>();
            }
        }

        public async Task<TienIchDto> GetTienIchByIdAsync(int id)
        {
            try
            {
                return await _apiClient.GetTienIchById(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetTienIchByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<TienIchDto>> CreateTienIchAsync(string ten)
        {
            try
            {
                return await _apiClient.CreateTienIch(ten);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.CreateTienIchAsync Error: {ex.Message}");
                return new ApiResponse<TienIchDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateTienIchAsync(int id, string ten)
        {
            try
            {
                return await _apiClient.UpdateTienIch(id, ten);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.UpdateTienIchAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteTienIchAsync(int id)
        {
            try
            {
                return await _apiClient.DeleteTienIch(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.DeleteTienIchAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Quận/Huyện

        public async Task<List<QuanHuyenDto>> GetAllQuanHuyenAsync()
        {
            try
            {
                return await _apiClient.GetAllQuanHuyen();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetAllQuanHuyenAsync Error: {ex.Message}");
                return new List<QuanHuyenDto>();
            }
        }

        public async Task<QuanHuyenDto> GetQuanHuyenByIdAsync(int id)
        {
            try
            {
                return await _apiClient.GetQuanHuyenById(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetQuanHuyenByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<QuanHuyenDto>> CreateQuanHuyenAsync(string ten)
        {
            try
            {
                return await _apiClient.CreateQuanHuyen(ten);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.CreateQuanHuyenAsync Error: {ex.Message}");
                return new ApiResponse<QuanHuyenDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateQuanHuyenAsync(int id, string ten)
        {
            try
            {
                return await _apiClient.UpdateQuanHuyen(id, ten);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.UpdateQuanHuyenAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteQuanHuyenAsync(int id)
        {
            try
            {
                return await _apiClient.DeleteQuanHuyen(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.DeleteQuanHuyenAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Phường

        public async Task<List<PhuongDto>> GetPhuongAsync(int? quanHuyenId = null)
        {
            try
            {
                if (quanHuyenId.HasValue)
                    return await _apiClient.GetPhuongByQuan(quanHuyenId.Value);
                else
                    return await _apiClient.GetAllPhuong();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetPhuongAsync Error: {ex.Message}");
                return new List<PhuongDto>();
            }
        }

        public async Task<PhuongDto> GetPhuongByIdAsync(int id)
        {
            try
            {
                return await _apiClient.GetPhuongById(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetPhuongByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<PhuongDto>> CreatePhuongAsync(int quanHuyenId, string ten)
        {
            try
            {
                return await _apiClient.CreatePhuong(quanHuyenId, ten);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.CreatePhuongAsync Error: {ex.Message}");
                return new ApiResponse<PhuongDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdatePhuongAsync(int id, int quanHuyenId, string ten)
        {
            try
            {
                return await _apiClient.UpdatePhuong(id, quanHuyenId, ten);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.UpdatePhuongAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeletePhuongAsync(int id)
        {
            try
            {
                return await _apiClient.DeletePhuong(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.DeletePhuongAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Loại Hỗ Trợ

        public async Task<List<LoaiHoTroDto>> GetAllLoaiHoTroAsync()
        {
            try
            {
                return await _apiClient.GetAllLoaiHoTro();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetAllLoaiHoTroAsync Error: {ex.Message}");
                return new List<LoaiHoTroDto>();
            }
        }

        public async Task<LoaiHoTroDto> GetLoaiHoTroByIdAsync(int id)
        {
            try
            {
                return await _apiClient.GetLoaiHoTroById(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetLoaiHoTroByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<LoaiHoTroDto>> CreateLoaiHoTroAsync(string tenLoai)
        {
            try
            {
                return await _apiClient.CreateLoaiHoTro(tenLoai);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.CreateLoaiHoTroAsync Error: {ex.Message}");
                return new ApiResponse<LoaiHoTroDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateLoaiHoTroAsync(int id, string tenLoai)
        {
            try
            {
                return await _apiClient.UpdateLoaiHoTro(id, tenLoai);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.UpdateLoaiHoTroAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteLoaiHoTroAsync(int id)
        {
            try
            {
                return await _apiClient.DeleteLoaiHoTro(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.DeleteLoaiHoTroAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Loại Vi Phạm

        public async Task<List<ViPhamDto>> GetAllViPhamAsync()
        {
            try
            {
                return await _apiClient.GetAllViPham();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetAllViPhamAsync Error: {ex.Message}");
                return new List<ViPhamDto>();
            }
        }

        public async Task<ViPhamDto> GetViPhamByIdAsync(int id)
        {
            try
            {
                return await _apiClient.GetViPhamById(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.GetViPhamByIdAsync Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<ViPhamDto>> CreateViPhamAsync(string tenViPham, string moTa, long? hinhPhatTien, int? soDiemTru)
        {
            try
            {
                return await _apiClient.CreateViPham(tenViPham, moTa, hinhPhatTien, soDiemTru);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.CreateViPhamAsync Error: {ex.Message}");
                return new ApiResponse<ViPhamDto> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UpdateViPhamAsync(int id, string tenViPham, string moTa, long? hinhPhatTien, int? soDiemTru)
        {
            try
            {
                return await _apiClient.UpdateViPham(id, tenViPham, moTa, hinhPhatTien, soDiemTru);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.UpdateViPhamAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> DeleteViPhamAsync(int id)
        {
            try
            {
                return await _apiClient.DeleteViPham(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SettingsService.DeleteViPhamAsync Error: {ex.Message}");
                return new ApiResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        #endregion
    }
}
