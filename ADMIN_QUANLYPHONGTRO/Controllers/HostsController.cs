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
                
                return View(hosts != null ? hosts.Items : new List<HostPendingItemViewModel>());
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
                var hosts = await _service.GetPendingHostsAsync(page, pageSize, keyword);
                
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = "approved";
                
                return View("Pending", hosts != null ? hosts.Items : new List<HostPendingItemViewModel>());
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
                var hosts = await _service.GetPendingHostsAsync(page, pageSize, keyword);
                
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = "rejected";
                
                return View("Pending", hosts != null ? hosts.Items : new List<HostPendingItemViewModel>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.Rejected Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Error = "Lỗi khi tải danh sách chủ trọ bị từ chối";
                ViewBag.ErrorDetails = ex.Message;
                return View("Pending", new List<HostPendingItemViewModel>());
            }
        }

        // ============ DATATABLES API METHODS ============

        /// <summary>
        /// API cho DataTables: Lấy danh sách chủ trọ với filter
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetPendingHosts(int draw, int start, int length, string status = "", string keyword = "")
        {
            try
            {
                // Tính toán pageIndex từ start và length
                int pageIndex = (start / length) + 1;
                
                // Lấy search value từ DataTables nếu không có keyword
                if (string.IsNullOrEmpty(keyword) && Request.Form["search[value]"] != null)
                {
                    keyword = Request.Form["search[value]"];
                }

                // Gọi service để lấy dữ liệu từ API Backend
                var pagedResult = await _service.GetPendingHostsAsync(pageIndex, length, keyword);

                // Lọc theo trạng thái nếu có
                var filteredItems = pagedResult?.Items ?? new List<HostPendingItemViewModel>();
                var totalFiltered = pagedResult?.TotalRecords ?? 0;

                if (!string.IsNullOrEmpty(status))
                {
                    switch (status.ToLower())
                    {
                        case "pending":
                            filteredItems = filteredItems.Where(x => 
                                x.TrangThaiXacThuc == "Chờ duyệt" || 
                                string.IsNullOrEmpty(x.TrangThaiXacThuc)).ToList();
                            break;
                        case "approved":
                            filteredItems = filteredItems.Where(x => 
                                x.TrangThaiXacThuc == "Đã xác minh").ToList();
                            break;
                        case "rejected":
                            // Accept both "Từ chối" and "Đã từ chối" (backend uses both variants in places)
                            filteredItems = filteredItems.Where(x => 
                                x.TrangThaiXacThuc == "Từ chối" || 
                                x.TrangThaiXacThuc == "Đã từ chối").ToList();
                            break;
                    }
                    totalFiltered = filteredItems.Count;
                }

                // Trả về format DataTables
                return Json(new
                {
                    draw = draw,
                    recordsTotal = pagedResult?.TotalRecords ?? 0,
                    recordsFiltered = totalFiltered,
                    data = filteredItems
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
        /// API: Lấy thống kê số lượng chủ trọ theo trạng thái
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetHostStats()
        {
            try
            {
                // Lấy tất cả host để đếm theo trạng thái
                var allHosts = await _service.GetPendingHostsAsync(1, 1000, "");
                
                var items = allHosts?.Items ?? new List<HostPendingItemViewModel>();

                var pending = items.Count(x => 
                    x.TrangThaiXacThuc == "Chờ duyệt" || 
                    string.IsNullOrEmpty(x.TrangThaiXacThuc));
                var approved = items.Count(x => x.TrangThaiXacThuc == "Đã xác minh");
                var rejected = items.Count(x => x.TrangThaiXacThuc == "Từ chối");

                return Json(
                    new
                    {
                        success = true,
                        data = new
                        {
                            pending = pending,
                            approved = approved,
                            rejected = rejected,
                            total = items.Count
                        }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.GetHostStats Error: {ex.Message}");
                return Json(
                    new
                    {
                        success = false,
                        message = ex.Message,
                        data = new { pending = 0, approved = 0, rejected = 0, total = 0 }
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        /// <summary>
        /// API: Lấy thống kê số lượng chủ trọ theo trạng thái (cho sidebar)
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetHostStatistics()
        {
            try
            {
                // Cách 1: Thử lấy từ API Backend trước
                System.Diagnostics.Debug.WriteLine("📊 GetHostStatistics - Attempting to fetch from Backend API...");
                
                try
                {
                    var allHosts = await _service.GetPendingHostsAsync(1, 1000, "");
                    
                    if (allHosts?.Items != null && allHosts.Items.Count > 0)
                    {
                        var items = allHosts.Items;
                        
                        var pending = items.Count(x => 
                            x.TrangThaiXacThuc == "Chờ duyệt" || 
                            string.IsNullOrEmpty(x.TrangThaiXacThuc));
                        var approved = items.Count(x => x.TrangThaiXacThuc == "Đã xác minh");
                        var rejected = items.Count(x => 
                            x.TrangThaiXacThuc == "Từ chối" || 
                            x.TrangThaiXacThuc == "Đã từ chối");

                        System.Diagnostics.Debug.WriteLine($"✅ Got data from Backend API: Pending={pending}, Approved={approved}, Rejected={rejected}");

                        return Json(new
                        {
                            success = true,
                            data = new
                            {
                                PendingCount = pending,
                                ApprovedCount = approved,
                                RejectedCount = rejected,
                                TotalCount = items.Count
                            }
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("⚠️ Backend API returned empty data");
                    }
                }
                catch (Exception apiEx)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Backend API call failed: {apiEx.Message}. Returning empty stats.");
                }

                // Cách 2: Nếu API Backend lỗi, trả về dữ liệu mặc định
                System.Diagnostics.Debug.WriteLine("📊 Returning default empty stats");
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        PendingCount = 0,
                        ApprovedCount = 0,
                        RejectedCount = 0,
                        TotalCount = 0
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetHostStatistics Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    data = new { PendingCount = 0, ApprovedCount = 0, RejectedCount = 0, TotalCount = 0 }
                }, JsonRequestBehavior.AllowGet);
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
                    return Json(new { success = true, message = "Đã xác thực chủ trọ thành công" });
                else
                    return Json(new { success = false, message = "Xác thực thất bại. Vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.ApproveHost Error: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
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
                    return Json(new { success = false, message = "Vui lòng nhập lý do từ chối" });

                var result = await _service.RejectHostAsync(id, reason);

                if (result)
                    return Json(new { success = true, message = "Đã từ chối chủ trọ" });
                else
                    return Json(new { success = false, message = "Từ chối thất bại. Vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HostsController.RejectHost Error: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // ============ LEGACY METHODS (for backward compatibility) ============

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
    }
}
