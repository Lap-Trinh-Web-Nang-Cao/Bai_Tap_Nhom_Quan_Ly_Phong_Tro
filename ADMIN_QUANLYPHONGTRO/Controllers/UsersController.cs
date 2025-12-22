using ADMIN_QUANLYPHONGTRO.ApiClients;
using ADMIN_QUANLYPHONGTRO.Services;
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
    /// <summary>
    /// UsersController - Quản lý người dùng
    /// Token-based: check nếu có session Admin, không có thì redirect /
    /// </summary>
    [Filters.AllowAnonymous]
    public class UsersController : Controller
    {
        private readonly IUserService _service;
        private readonly AdminAuthService _authService;

        public UsersController()
        {
            _service = new UserService();
            _authService = new AdminAuthService();
        }

        /// <summary>
        /// Kiểm tra Admin đã đăng nhập
        /// </summary>
        private bool CheckAdminSession()
        {
            if (!_authService.IsAdminLoggedIn())
            {
                System.Diagnostics.Debug.WriteLine("❌ UsersController: Admin not logged in, redirecting to /");
                return false;
            }
            return true;
        }

        // GET: Users/Index - Danh sách người dùng (View)
        public async Task<ActionResult> Index(int page = 1, int pageSize = 10, string keyword = "")
        {
            if (!CheckAdminSession()) return Redirect("/");

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
                System.Diagnostics.Debug.WriteLine(string.Format("❌ UsersController.Index Error: {0}\n{1}", ex.Message, ex.StackTrace));
                ViewBag.Error = "Lỗi khi tải danh sách người dùng";
                ViewBag.ErrorDetails = ex.Message;
                return View(new List<UserItemViewModel>());
            }
        }

        // GET: Users/Details - Chi tiết người dùng
        public async Task<ActionResult> Details(string id)
        {
            if (!CheckAdminSession()) return Redirect("/");

            try
            {
                var result = await _service.GetUserByIdAsync(id);
                if (result == null || !result.Success) 
                    return HttpNotFound();
                return View(result.Data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ UsersController.Details Error: {0}", ex.Message));
                return HttpNotFound();
            }
        }

        // ============ API METHODS (Token-based check) ============

        /// <summary>
        /// API: Lấy thống kê người dùng
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetUserStatistics()
        {
            if (!CheckAdminSession())
            {
                return Json(new { success = false, message = "Admin session expired", redirect = "/" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var apiClient = new UserApiClient();
                var stats = await apiClient.GetUserStatisticsAsync();
                
                System.Diagnostics.Debug.WriteLine(string.Format("✅ User Statistics: Tenants={0}, Landlords={1}, Admins={2}", stats.TotalTenants, stats.TotalLandlords, stats.TotalAdmins));
                
                return Json(new
                {
                    success = true,
                    data = stats
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ GetUserStatistics Error: {0}", ex.Message));
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// API cho DataTables: Lấy danh sách người dùng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetUsers()
        {
            if (!CheckAdminSession())
            {
                return Json(new
                {
                    draw = Request.Form["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    error = "Admin session expired",
                    redirect = "/",
                    data = new List<object>()
                });
            }

            try
            {
                var draw = Request.Form["draw"];
                var start = int.Parse(Request.Form["start"] ?? "0");
                var length = int.Parse(Request.Form["length"] ?? "10");
                var searchValue = Request.Form["search[value]"] ?? "";
                
                var vaiTroIdStr = Request.Form["vaiTroId"];
                var statusStr = Request.Form["status"];
                var keywordStr = Request.Form["keyword"];

                int? vaiTroId = null;
                if (!string.IsNullOrEmpty(vaiTroIdStr) && int.TryParse(vaiTroIdStr, out int vaiTro))
                {
                    vaiTroId = vaiTro;
                }

                string keyword = string.IsNullOrEmpty(keywordStr) ? searchValue : keywordStr;

                bool? isKhoa = null;
                if (!string.IsNullOrEmpty(statusStr))
                {
                    if (statusStr.ToLower() == "locked")
                        isKhoa = true;
                    else if (statusStr.ToLower() == "active")
                        isKhoa = false;
                }

                int pageIndex = (start / length) + 1;

                var pagedData = await _service.GetUsersAsync(pageIndex, length, keyword, vaiTroId, isKhoa);

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

                return Json(new
                {
                    draw = draw,
                    recordsTotal = pagedData?.TotalRecords ?? 0,
                    recordsFiltered = pagedData?.TotalRecords ?? 0,
                    data = users
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ UsersController.GetUsers Error: {0}\n{1}", ex.Message, ex.StackTrace));
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
        /// API: Lấy chi tiết người dùng
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetUserDetail(string id)
        {
            if (!CheckAdminSession())
            {
                return Json(new { success = false, message = "Admin session expired", redirect = "/" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" }, JsonRequestBehavior.AllowGet);

                var result = await _service.GetUserByIdAsync(id);

                if (result == null || !result.Success || result.Data == null)
                    return Json(new { success = false, message = result?.Message ?? "Không tìm thấy người dùng" }, JsonRequestBehavior.AllowGet);

                var user = result.Data;

                return Json(new
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
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("❌ UsersController.GetUserDetail Error: {0}", ex.Message));
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// API: Tạo người dùng mới
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> CreateUser(CreateUserRequest request)
        {
            if (!CheckAdminSession())
            {
                return Json(new { success = false, message = "Admin session expired", redirect = "/" });
            }

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
                System.Diagnostics.Debug.WriteLine(string.Format("❌ UsersController.CreateUser Error: {0}", ex.Message));
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Khóa người dùng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> LockUser(string id)
        {
            if (!CheckAdminSession())
            {
                return Json(new { success = false, message = "Admin session expired", redirect = "/" });
            }

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
                System.Diagnostics.Debug.WriteLine(string.Format("❌ UsersController.LockUser Error: {0}", ex.Message));
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Mở khóa người dùng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UnlockUser(string id)
        {
            if (!CheckAdminSession())
            {
                return Json(new { success = false, message = "Admin session expired", redirect = "/" });
            }

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
                System.Diagnostics.Debug.WriteLine(string.Format("❌ UsersController.UnlockUser Error: {0}", ex.Message));
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
