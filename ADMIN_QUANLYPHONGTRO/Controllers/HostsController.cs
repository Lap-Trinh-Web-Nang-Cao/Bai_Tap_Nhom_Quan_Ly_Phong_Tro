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
    public class HostsController : Controller
    {
        private readonly IHostService _service;

        public HostsController()
        {
            _service = new HostService();
        }

        // GET: Hosts/Pending - Danh sách chủ trọ chờ duyệt
        public async Task<ActionResult> Pending(int page = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                var hosts = await _service.GetPendingHostsAsync(page, pageSize, keyword);
                
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;
                
                return View(hosts ?? new List<HostPendingItemViewModel>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.Pending Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Error = "Lỗi khi tải danh sách chủ trọ";
                ViewBag.ErrorDetails = ex.Message;
                return View(new List<HostPendingItemViewModel>());
            }
        }

        // GET: Hosts/Approved - Danh sách chủ trọ đã duyệt
        public async Task<ActionResult> Approved(int page = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                // Tạm thời sử dụng cùng logic với Pending
                // TODO: Tạo method riêng GetApprovedHostsAsync() khi cần lọc khác
                var hosts = await _service.GetPendingHostsAsync(page, pageSize, keyword);
                
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = "approved";
                
                return View("Pending", hosts ?? new List<HostPendingItemViewModel>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.Approved Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Error = "Lỗi khi tải danh sách chủ trọ đã duyệt";
                ViewBag.ErrorDetails = ex.Message;
                return View("Pending", new List<HostPendingItemViewModel>());
            }
        }

        // GET: Hosts/Rejected - Danh sách chủ trọ bị từ chối
        public async Task<ActionResult> Rejected(int page = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                // Tạm thời sử dụng cùng logic với Pending
                // TODO: Tạo method riêng GetRejectedHostsAsync() khi cần lọc khác
                var hosts = await _service.GetPendingHostsAsync(page, pageSize, keyword);
                
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = "rejected";
                
                return View("Pending", hosts ?? new List<HostPendingItemViewModel>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.Rejected Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Error = "Lỗi khi tải danh sách chủ trọ bị từ chối";
                ViewBag.ErrorDetails = ex.Message;
                return View("Pending", new List<HostPendingItemViewModel>());
            }
        }

        // GET: Hosts/ApproveModal - Lấy thông tin để duyệt
        [HttpGet]
        public async Task<ActionResult> ApproveModal(string id)
        {
            try
            {
                var hostData = await _service.GetHostDetailAsync(id);
                if (hostData == null)
                    return HttpNotFound();

                return PartialView("_ApproveModal", hostData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.ApproveModal Error: {ex.Message}");
                return HttpNotFound();
            }
        }

        // POST: Hosts/Approve - Phê duyệt chủ trọ
        [HttpPost]
        public async Task<ActionResult> Approve(string id)
        {
            try
            {
                await _service.ApproveHostAsync(id);
                return RedirectToAction("Pending");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.Approve Error: {ex.Message}");
                ViewBag.Error = "Lỗi khi phê duyệt";
                return RedirectToAction("Pending");
            }
        }

        // POST: Hosts/Reject - Từ chối chủ trọ
        [HttpPost]
        public async Task<ActionResult> Reject(string id, string reason)
        {
            try
            {
                await _service.RejectHostAsync(id, reason);
                return RedirectToAction("Pending");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.Reject Error: {ex.Message}");
                ViewBag.Error = "Lỗi khi từ chối";
                return RedirectToAction("Pending");
            }
        }

        // ============ DATATABLES API METHODS ============

        /// <summary>
        /// API cho DataTables: Lấy danh sách chủ trọ chờ duyệt
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetPendingHosts(int draw, int start, int length, string search = "")
        {
            try
            {
                // Tính toán pageIndex từ start và length
                int pageIndex = (start / length) + 1;
                
                // Gọi service để lấy dữ liệu từ API Backend
                var hosts = await _service.GetPendingHostsAsync(pageIndex, length, search);

                // Trả về format DataTables
                return Json(new
                {
                    draw = draw,
                    recordsTotal = hosts?.Count ?? 0,
                    recordsFiltered = hosts?.Count ?? 0,
                    data = hosts ?? new List<HostPendingItemViewModel>()
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.GetPendingHosts Error: {ex.Message}");
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
        /// API: Lấy chi tiết chủ trọ để hiển thị trong Modal
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetHostDetail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(
                        new { success = false, message = "ID không hợp lệ" },
                        JsonRequestBehavior.AllowGet
                    );

                var host = await _service.GetHostDetailAsync(id);
                
                if (host == null)
                    return Json(
                        new { success = false, message = "Không tìm thấy chủ trọ" },
                        JsonRequestBehavior.AllowGet
                    );

                return Json(
                    new
                    {
                        success = true,
                        data = new
                        {
                            NguoiDungId = host.NguoiDungId,
                            HoTen = host.HoTen ?? "",
                            Email = host.Email ?? "",
                            DienThoai = host.DienThoai ?? "",
                            SoCCCD = host.SoCCCD ?? "",
                            NgaySinh = host.NgaySinh,
                            QueQuan = host.QueQuan ?? "",
                            Avatar = host.Avatar ?? "/Content/img/default-avatar.png",
                            CCCDMatTruoc = host.CCCDMatTruocUrl ?? "/Content/img/no-image.png",
                            CCCDMatSau = host.CCCDMatSauUrl ?? "/Content/img/no-image.png",
                            GiayPhepKinhDoanh = host.GiayPhepKinhDoanhUrl ?? "/Content/img/no-image.png",
                            TrangThaiXacThuc = host.TrangThaiXacThuc ?? ""
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.GetHostDetail Error: {ex.Message}");
                return Json(
                    new { success = false, message = ex.Message },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        /// <summary>
        /// API: Xác thực chủ trọ
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> ApproveHost(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" });

                var result = await _service.ApproveHostAsync(id);

                if (result)
                    return Json(new { success = true, message = "Đã xác thực chủ trọ" });
                else
                    return Json(new { success = false, message = "Xác thực thất bại" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.ApproveHost Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Từ chối chủ trọ
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> RejectHost(string id, string reason)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" });

                if (string.IsNullOrEmpty(reason))
                    return Json(new { success = false, message = "Lý do từ chối không được để trống" });

                var result = await _service.RejectHostAsync(id, reason);

                if (result)
                    return Json(new { success = true, message = "Đã từ chối chủ trọ" });
                else
                    return Json(new { success = false, message = "Từ chối thất bại" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.RejectHost Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
