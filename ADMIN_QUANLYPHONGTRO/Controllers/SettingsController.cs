using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;

        public SettingsController()
        {
            _settingsService = new SettingsService();
        }

        // GET: Settings
        public ActionResult Index()
        {
            return View();
        }

        #region Tiện Ích API

        /// <summary>
        /// Lấy danh sách tiện ích
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetTienIch()
        {
            try
            {
                var data = await _settingsService.GetAllTienIchAsync();
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetTienIch Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Thêm tiện ích mới
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CreateTienIch(string ten)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ten))
                    return Json(new { success = false, message = "Tên tiện ích không được để trống" });

                var result = await _settingsService.CreateTienIchAsync(ten);
                return Json(new { success = result.Success, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreateTienIch Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật tiện ích
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UpdateTienIch(int id, string ten)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ten))
                    return Json(new { success = false, message = "Tên tiện ích không được để trống" });

                var result = await _settingsService.UpdateTienIchAsync(id, ten);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UpdateTienIch Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa tiện ích
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> DeleteTienIch(int id)
        {
            try
            {
                var result = await _settingsService.DeleteTienIchAsync(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DeleteTienIch Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Quận/Huyện API

        /// <summary>
        /// Lấy danh sách quận/huyện
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetQuanHuyen()
        {
            try
            {
                var data = await _settingsService.GetAllQuanHuyenAsync();
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetQuanHuyen Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Thêm quận/huyện mới
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CreateQuanHuyen(string ten)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ten))
                    return Json(new { success = false, message = "Tên quận/huyện không được để trống" });

                var result = await _settingsService.CreateQuanHuyenAsync(ten);
                return Json(new { success = result.Success, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreateQuanHuyen Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật quận/huyện
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UpdateQuanHuyen(int id, string ten)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ten))
                    return Json(new { success = false, message = "Tên quận/huyện không được để trống" });

                var result = await _settingsService.UpdateQuanHuyenAsync(id, ten);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UpdateQuanHuyen Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa quận/huyện
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> DeleteQuanHuyen(int id)
        {
            try
            {
                var result = await _settingsService.DeleteQuanHuyenAsync(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DeleteQuanHuyen Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Phường API

        /// <summary>
        /// Lấy danh sách phường (tất cả hoặc theo quận)
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetPhuong(int? quanHuyenId = null)
        {
            try
            {
                var data = await _settingsService.GetPhuongAsync(quanHuyenId);
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetPhuong Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Thêm phường mới
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CreatePhuong(int quanHuyenId, string ten)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ten))
                    return Json(new { success = false, message = "Tên phường không được để trống" });

                var result = await _settingsService.CreatePhuongAsync(quanHuyenId, ten);
                return Json(new { success = result.Success, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreatePhuong Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật phường
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UpdatePhuong(int id, int quanHuyenId, string ten)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ten))
                    return Json(new { success = false, message = "Tên phường không được để trống" });

                var result = await _settingsService.UpdatePhuongAsync(id, quanHuyenId, ten);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UpdatePhuong Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa phường
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> DeletePhuong(int id)
        {
            try
            {
                var result = await _settingsService.DeletePhuongAsync(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DeletePhuong Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Loại Hỗ Trợ API

        /// <summary>
        /// Lấy danh sách loại hỗ trợ
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetLoaiHoTro()
        {
            try
            {
                var data = await _settingsService.GetAllLoaiHoTroAsync();
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetLoaiHoTro Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Thêm loại hỗ trợ mới
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CreateLoaiHoTro(string tenLoai)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenLoai))
                    return Json(new { success = false, message = "Tên loại hỗ trợ không được để trống" });

                var result = await _settingsService.CreateLoaiHoTroAsync(tenLoai);
                return Json(new { success = result.Success, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreateLoaiHoTro Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật loại hỗ trợ
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UpdateLoaiHoTro(int id, string tenLoai)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenLoai))
                    return Json(new { success = false, message = "Tên loại hỗ trợ không được để trống" });

                var result = await _settingsService.UpdateLoaiHoTroAsync(id, tenLoai);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UpdateLoaiHoTro Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa loại hỗ trợ
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> DeleteLoaiHoTro(int id)
        {
            try
            {
                var result = await _settingsService.DeleteLoaiHoTroAsync(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DeleteLoaiHoTro Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Loại Vi Phạm API

        /// <summary>
        /// Lấy danh sách loại vi phạm
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetViPham()
        {
            try
            {
                var data = await _settingsService.GetAllViPhamAsync();
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetViPham Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Thêm loại vi phạm mới
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CreateViPham(string tenViPham, string moTa, long? hinhPhatTien, int? soDiemTru)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenViPham))
                    return Json(new { success = false, message = "Tên loại vi phạm không được để trống" });

                var result = await _settingsService.CreateViPhamAsync(tenViPham, moTa, hinhPhatTien, soDiemTru);
                return Json(new { success = result.Success, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ CreateViPham Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật loại vi phạm
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UpdateViPham(int id, string tenViPham, string moTa, long? hinhPhatTien, int? soDiemTru)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenViPham))
                    return Json(new { success = false, message = "Tên loại vi phạm không được để trống" });

                var result = await _settingsService.UpdateViPhamAsync(id, tenViPham, moTa, hinhPhatTien, soDiemTru);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UpdateViPham Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa loại vi phạm
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> DeleteViPham(int id)
        {
            try
            {
                var result = await _settingsService.DeleteViPhamAsync(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DeleteViPham Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region System Settings API (Cấu hình hệ thống)

        /// <summary>
        /// Lấy tất cả cài đặt hệ thống
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetAllSystemSettings()
        {
            try
            {
                var apiClient = new SettingsApiClient();
                var data = await apiClient.GetAllSystemSettings();
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetAllSystemSettings Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Lấy cài đặt theo nhóm
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetSystemSettingsByGroup(string groupName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(groupName))
                    return Json(new { success = false, message = "Group name không được để trống" }, JsonRequestBehavior.AllowGet);

                var apiClient = new SettingsApiClient();
                var data = await apiClient.GetSystemSettingsByGroup(groupName);
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetSystemSettingsByGroup Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Cập nhật nhiều cài đặt một lúc
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UpdateSystemSettingsByKey(Dictionary<string, string> settings)
        {
            try
            {
                if (settings == null || settings.Count == 0)
                    return Json(new { success = false, message = "Không có dữ liệu cập nhật" });

                var apiClient = new SettingsApiClient();
                var result = await apiClient.UpdateSystemSettingsByKey(settings);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UpdateSystemSettingsByKey Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}