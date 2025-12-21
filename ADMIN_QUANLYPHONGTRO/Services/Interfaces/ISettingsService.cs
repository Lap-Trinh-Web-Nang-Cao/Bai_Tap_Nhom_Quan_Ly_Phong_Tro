using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADMIN_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface cho Settings Service - Quản lý Tiện ích, Quận/Huyện, Phường, Loại hỗ trợ, Loại vi phạm
    /// </summary>
    public interface ISettingsService
    {
        #region Tiện Ích

        /// <summary>
        /// Lấy danh sách tất cả tiện ích
        /// </summary>
        Task<List<TienIchDto>> GetAllTienIchAsync();

        /// <summary>
        /// Lấy tiện ích theo ID
        /// </summary>
        Task<TienIchDto> GetTienIchByIdAsync(int id);

        /// <summary>
        /// Tạo tiện ích mới
        /// </summary>
        Task<ApiResponse<TienIchDto>> CreateTienIchAsync(string ten);

        /// <summary>
        /// Cập nhật tiện ích
        /// </summary>
        Task<ApiResponse<bool>> UpdateTienIchAsync(int id, string ten);

        /// <summary>
        /// Xóa tiện ích
        /// </summary>
        Task<ApiResponse<bool>> DeleteTienIchAsync(int id);

        #endregion

        #region Quận/Huyện

        /// <summary>
        /// Lấy danh sách tất cả quận/huyện
        /// </summary>
        Task<List<QuanHuyenDto>> GetAllQuanHuyenAsync();

        /// <summary>
        /// Lấy quận/huyện theo ID
        /// </summary>
        Task<QuanHuyenDto> GetQuanHuyenByIdAsync(int id);

        /// <summary>
        /// Tạo quận/huyện mới
        /// </summary>
        Task<ApiResponse<QuanHuyenDto>> CreateQuanHuyenAsync(string ten);

        /// <summary>
        /// Cập nhật quận/huyện
        /// </summary>
        Task<ApiResponse<bool>> UpdateQuanHuyenAsync(int id, string ten);

        /// <summary>
        /// Xóa quận/huyện
        /// </summary>
        Task<ApiResponse<bool>> DeleteQuanHuyenAsync(int id);

        #endregion

        #region Phường

        /// <summary>
        /// Lấy danh sách phường (tất cả hoặc theo quận)
        /// </summary>
        Task<List<PhuongDto>> GetPhuongAsync(int? quanHuyenId = null);

        /// <summary>
        /// Lấy phường theo ID
        /// </summary>
        Task<PhuongDto> GetPhuongByIdAsync(int id);

        /// <summary>
        /// Tạo phường mới
        /// </summary>
        Task<ApiResponse<PhuongDto>> CreatePhuongAsync(int quanHuyenId, string ten);

        /// <summary>
        /// Cập nhật phường
        /// </summary>
        Task<ApiResponse<bool>> UpdatePhuongAsync(int id, int quanHuyenId, string ten);

        /// <summary>
        /// Xóa phường
        /// </summary>
        Task<ApiResponse<bool>> DeletePhuongAsync(int id);

        #endregion

        #region Loại Hỗ Trợ

        /// <summary>
        /// Lấy danh sách tất cả loại hỗ trợ
        /// </summary>
        Task<List<LoaiHoTroDto>> GetAllLoaiHoTroAsync();

        /// <summary>
        /// Lấy loại hỗ trợ theo ID
        /// </summary>
        Task<LoaiHoTroDto> GetLoaiHoTroByIdAsync(int id);

        /// <summary>
        /// Tạo loại hỗ trợ mới
        /// </summary>
        Task<ApiResponse<LoaiHoTroDto>> CreateLoaiHoTroAsync(string tenLoai);

        /// <summary>
        /// Cập nhật loại hỗ trợ
        /// </summary>
        Task<ApiResponse<bool>> UpdateLoaiHoTroAsync(int id, string tenLoai);

        /// <summary>
        /// Xóa loại hỗ trợ
        /// </summary>
        Task<ApiResponse<bool>> DeleteLoaiHoTroAsync(int id);

        #endregion

        #region Loại Vi Phạm

        /// <summary>
        /// Lấy danh sách tất cả loại vi phạm
        /// </summary>
        Task<List<ViPhamDto>> GetAllViPhamAsync();

        /// <summary>
        /// Lấy loại vi phạm theo ID
        /// </summary>
        Task<ViPhamDto> GetViPhamByIdAsync(int id);

        /// <summary>
        /// Tạo loại vi phạm mới
        /// </summary>
        Task<ApiResponse<ViPhamDto>> CreateViPhamAsync(string tenViPham, string moTa, long? hinhPhatTien, int? soDiemTru);

        /// <summary>
        /// Cập nhật loại vi phạm
        /// </summary>
        Task<ApiResponse<bool>> UpdateViPhamAsync(int id, string tenViPham, string moTa, long? hinhPhatTien, int? soDiemTru);

        /// <summary>
        /// Xóa loại vi phạm
        /// </summary>
        Task<ApiResponse<bool>> DeleteViPhamAsync(int id);

        #endregion
    }
}
