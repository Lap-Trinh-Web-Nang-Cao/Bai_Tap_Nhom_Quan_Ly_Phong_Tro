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
                    HoTen = "Người dùng",
                    Email = u.Email,
                    DienThoai = u.DienThoai,
                    VaiTroId = u.VaiTroId,
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

        // ============ DATATABLES API METHODS ============

        /// <summary>
        /// API cho DataTables: Lấy danh sách người dùng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetUsers(int draw, int start, int length, string search = "")
        {
            try
            {
                // Tính toán pageIndex từ start và length
                int pageIndex = (start / length) + 1;

                // Gọi service để lấy dữ liệu từ API Backend
                var pagedData = await _service.GetUsersAsync(pageIndex, length, search);

                // Map dữ liệu sang object cho DataTables
                var users = new List<object>();
                if (pagedData?.Items != null)
                {
                    users = pagedData.Items.Select(u => new
                    {
                        NguoiDungId = u.NguoiDungId,
                        HoTen = "Người dùng",
                        Email = u.Email ?? "",
                        DienThoai = u.DienThoai ?? "",
                        Avatar = "/Content/img/default-avatar.png",
                        VaiTroId = u.VaiTroId,
                        IsKhoa = u.IsKhoa,
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
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.GetUsers Error: {ex.Message}");
                return Json(new
                {
                    draw = draw,
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
                        new { success = false, message = "Không tìm thấy người dùng" },
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
                            HoTen = "Người dùng",
                            Email = user.Email ?? "",
                            DienThoai = user.DienThoai ?? "",
                            SoCCCD = "",
                            Avatar = "/Content/img/default-avatar.png",
                            VaiTroId = user.VaiTroId,
                            IsKhoa = user.IsKhoa,
                            NgayDangKy = user.CreatedAt,
                            SoPhongDaDang = 0,
                            DiaChi = "-",
                            GhiChu = "-",
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
        /// API: Khóa/Mở khóa người dùng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> LockUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" });

                var result = await _service.ToggleLockUserAsync(id);

                if (result?.Success ?? false)
                    return Json(new { success = true, message = "Đã khóa người dùng" });
                else
                    return Json(new { success = false, message = "Thao tác thất bại" });
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

                var result = await _service.ToggleLockUserAsync(id);

                if (result?.Success ?? false)
                    return Json(new { success = true, message = "Đã mở khóa người dùng" });
                else
                    return Json(new { success = false, message = "Thao tác thất bại" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.UnlockUser Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
