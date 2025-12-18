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

        public async Task<ActionResult> Index(int page = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                var pagedData = await _service.GetUsersAsync(page, pageSize, keyword);
                
                // Convert PagedResult<NguoiDungDto> to List<UserItemViewModel>
                var userList = pagedData?.Items?.Select(u => new UserItemViewModel
                {
                    NguoiDungId = u.NguoiDungId,
                    HoTen = "User", // TODO: Get from HoSoNguoiDung if available
                    Email = u.Email,
                    DienThoai = u.DienThoai,
                    VaiTroId = u.VaiTroId,
                    IsKhoa = u.IsKhoa,
                    IsEmailXacThuc = u.IsEmailXacThuc,
                    SoPhongDaDang = 0, // TODO: Get actual count from API
                    SoPhongDaThue = 0, // TODO: Get actual count from API
                    Avatar = null // TODO: Get from HoSoNguoiDung if available
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

        public async Task<ActionResult> Details(string id)
        {
            try
            {
                var result = await _service.GetUserByIdAsync(id);
                if (result == null || !result.Success) return HttpNotFound();
                return View(result.Data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.Details Error: {ex.Message}");
                return HttpNotFound();
            }
        }

        public async Task<ActionResult> ToggleLock(string id)
        {
            try
            {
                var result = await _service.ToggleLockUserAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ UsersController.ToggleLock Error: {ex.Message}");
                return RedirectToAction("Index");
            }
        }
    }
}
