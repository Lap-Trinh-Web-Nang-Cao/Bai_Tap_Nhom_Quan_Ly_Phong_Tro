using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _service;

        public UsersController()
        {
            _service = new UserService();
        }

        // GET: Users/Index - Danh sách người dùng (View)
        public async Task<ActionResult> Index(int page = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                var pagedData = await _service.GetUsersAsync(page, pageSize, keyword);
                
                // Convert PagedResult<NguoiDungDto> to List<UserItemViewModel>
                var userList = pagedData?.Items?.Select(u => new UserItemViewModel
                {
                    NguoiDungId = u.NguoiDungId,
                    HoTen = u.HoTen ?? "Chưa cập nhật",
                    Email = u.Email,
                    DienThoai = u.DienThoai,
                    VaiTroId = u.VaiTroId,
                    VaiTroName = u.VaiTroName,
                    IsKhoa = u.IsKhoa,
                    IsEmailXacThuc = u.IsEmailXacThuc,
                    SoPhongDaDang = 0,
                    SoPhongDaThue = 0,
                    Avatar = "/Content/img/default-avatar.png"
                }).ToList() ?? new List<UserItemViewModel>();

                ViewBag.Keyword = keyword;
                return View(userList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.Index Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Error = "Lỗi khi tải danh sách người dùng";
                ViewBag.ErrorDetails = ex.Message;
                return View(new List<UserItemViewModel>());
            }
        }

        // GET: Users/Details - Chi tiết người dùng
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                var result = await _service.GetUserByIdAsync(id);
                if (result == null || !result.Success) 
                    return HttpNotFound();
                return View(result.Data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.Details Error: {ex.Message}");
                return HttpNotFound();
            }
        }

        // ============ STATISTICS API ============

        /// <summary>
        /// API: Lấy thống kê người dùng (cho sidebar)
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetUserStatistics()
        {
            try
            {
                var apiClient = new UserApiClient();
                var stats = await apiClient.GetUserStatisticsAsync();
                
                System.Diagnostics.Debug.WriteLine($"✅ User Statistics: Tenants={stats.TotalTenants}, Landlords={stats.TotalLandlords}, Admins={stats.TotalAdmins}");
                
                return Json(new
                {
                    success = true,
                    data = stats
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetUserStatistics Error: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // ============ DATATABLES API METHODS ============

        /// <summary>
        /// API cho DataTables: Lấy danh sách người dùng với server-side processing
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetUsers()
        {
            try
            {
                // Lấy tham số từ DataTables request
                var draw = Request.Form["draw"];
                var start = int.Parse(Request.Form["start"] ?? "0");
                var length = int.Parse(Request.Form["length"] ?? "10");
                var searchValue = Request.Form["search[value]"] ?? "";
                
                // Custom filters
                var vaiTroIdStr = Request.Form["vaiTroId"];
                var statusStr = Request.Form["status"]; // Frontend gửi "active" hoặc "locked"
                var keywordStr = Request.Form["keyword"];

                int? vaiTroId = null;
                if (!string.IsNullOrEmpty(vaiTroIdStr) && int.TryParse(vaiTroIdStr, out int vaiTro))
                {
                    vaiTroId = vaiTro;
                }

                // Combine search sources
                string keyword = string.IsNullOrEmpty(keywordStr) ? searchValue : keywordStr;

                // Convert frontend "status" filter thành "isKhoa" boolean
                bool? isKhoa = null;
                if (!string.IsNullOrEmpty(statusStr))
                {
                    if (statusStr.ToLower() == "locked")
                        isKhoa = true; // Đã khóa
                    else if (statusStr.ToLower() == "active")
                        isKhoa = false; // Không khóa
                }

                // Tính toán pageIndex từ start và length
                int pageIndex = (start / length) + 1;

                System.Diagnostics.Debug.WriteLine($"🔍 GetUsers: start={start}, length={length}, pageIndex={pageIndex}");
                System.Diagnostics.Debug.WriteLine($"🔍 GetUsers filter: vaiTroId={vaiTroId}, isKhoa={isKhoa}, keyword={keyword}");

                // Gọi service để lấy dữ liệu từ API Backend
                var pagedData = await _service.GetUsersAsync(pageIndex, length, keyword, vaiTroId, isKhoa);

                // Map dữ liệu sang object cho DataTables
                var users = new List<object>();
                if (pagedData?.Items != null)
                {
                    users = pagedData.Items.Select(u => new
                    {
                        NguoiDungId = u.NguoiDungId,
                        HoTen = u.HoTen ?? "Chưa cập nhật",
                        Email = u.Email ?? "",
                        DienThoai = u.DienThoai ?? "",
                        Avatar = "/Content/img/default-avatar.png",
                        VaiTroId = u.VaiTroId,
                        VaiTroName = u.VaiTroName ?? GetRoleName(u.VaiTroId),
                        IsKhoa = u.IsKhoa,
                        IsEmailXacThuc = u.IsEmailXacThuc,
                        CreatedAt = u.CreatedAt,
                        SoPhongDaDang = 0,
                        SoPhongDaThue = 0
                    }).Cast<object>().ToList();
                }

                var result = new
                {
                    draw = draw,
                    recordsTotal = pagedData?.TotalRecords ?? 0,
                    recordsFiltered = pagedData?.TotalRecords ?? 0,
                    data = users
                };

                System.Diagnostics.Debug.WriteLine($"✅ GetUsers result: {result.recordsTotal} records, filtered: {result.recordsFiltered}");

                return Json(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.GetUsers Error: {ex.Message}\n{ex.StackTrace}");
                return Json(new
                {
                    draw = Request.Form["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    error = ex.Message,
                    data = new List<object>()
                });
            }
        }

        /// <summary>
        /// API: Lấy chi tiết người dùng để hiển thị trong Modal
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetUserDetail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(
                        new { success = false, message = "ID không hợp lệ" },
                        JsonRequestBehavior.AllowGet
                    );

                var result = await _service.GetUserByIdAsync(id);

                if (result == null || !result.Success || result.Data == null)
                    return Json(
                        new { success = false, message = result?.Message ?? "Không tìm thấy người dùng" },
                        JsonRequestBehavior.AllowGet
                    );

                var user = result.Data;

                return Json(
                    new
                    {
                        success = true,
                        data = new
                        {
                            NguoiDungId = user.NguoiDungId,
                            HoTen = user.HoTen ?? "Chưa cập nhật",
                            Email = user.Email ?? "",
                            DienThoai = user.DienThoai ?? "",
                            SoCCCD = user.LoaiGiayTo ?? "",
                            Avatar = user.Avatar ?? "/Content/img/default-avatar.png",
                            VaiTroId = user.VaiTroId,
                            VaiTroName = user.VaiTroName ?? GetRoleName(user.VaiTroId),
                            IsKhoa = user.IsKhoa,
                            IsEmailXacThuc = user.IsEmailXacThuc,
                            NgayDangKy = user.CreatedAt,
                            SoPhongDaDang = user.SoPhongDaDang,
                            SoDatPhong = user.SoDatPhong,
                            DiaChi = "-",
                            GhiChu = user.GhiChu ?? "-",
                            CCCDMatTruoc = "/Content/img/no-image.png",
                            CCCDMatSau = "/Content/img/no-image.png",
                            Rating = 0
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.GetUserDetail Error: {ex.Message}");
                return Json(
                    new { success = false, message = ex.Message },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        /// <summary>
        /// API: Tạo người dùng mới
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CreateUser(CreateUserRequest request)
        {
            try
            {
                if (request == null)
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });

                if (string.IsNullOrWhiteSpace(request.Email))
                    return Json(new { success = false, message = "Email là bắt buộc" });

                if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                    return Json(new { success = false, message = "Mật khẩu phải từ 6 ký tự" });

                if (request.VaiTroId < 1 || request.VaiTroId > 3)
                    return Json(new { success = false, message = "Vai trò không hợp lệ" });

                var result = await _service.CreateUserAsync(request);

                if (result?.Success ?? false)
                {
                    return Json(new { 
                        success = true, 
                        message = "Tạo người dùng thành công",
                        data = result.Data
                    });
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = result?.Message ?? "Không thể tạo người dùng" 
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.CreateUser Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Khóa người dùng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> LockUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" });

                var result = await _service.LockUserAsync(id);

                if (result?.Success ?? false)
                    return Json(new { success = true, message = "Đã khóa người dùng" });
                else
                    return Json(new { success = false, message = result?.Message ?? "Thao tác thất bại" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.LockUser Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Mở khóa người dùng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UnlockUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" });

                var result = await _service.UnlockUserAsync(id);

                if (result?.Success ?? false)
                    return Json(new { success = true, message = "Đã mở khóa người dùng" });
                else
                    return Json(new { success = false, message = result?.Message ?? "Thao tác thất bại" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.UnlockUser Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper function
        private string GetRoleName(int vaiTroId)
        {
            switch (vaiTroId)
            {
                case 1: return "Admin";
                case 2: return "Chủ trọ";
                case 3: return "Người thuê";
                default: return "Unknown";
            }
        }
    }
}
