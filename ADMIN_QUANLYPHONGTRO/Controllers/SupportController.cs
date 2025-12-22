using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class SupportController : Controller
    {
        private readonly ISupportService _service;

        public SupportController()
        {
            _service = new SupportService();
        }

        /// <summary>
        /// Danh sách tất cả yêu cầu hỗ trợ
        /// </summary>
        public async Task<ActionResult> Index(string status = null)
        {
            try
            {
                var supports = await _service.GetAllSupportsAsync();
                var loaiHoTros = await _service.GetLoaiHoTroAsync();
                var statistics = await _service.GetStatisticsAsync();
                
                ViewBag.LoaiHoTros = loaiHoTros;
                ViewBag.Statistics = statistics;
                ViewBag.CurrentStatus = status;
                
                return View(supports);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách yêu cầu hỗ trợ";
                ViewBag.ErrorDetails = ex.Message;
                return View(new System.Collections.Generic.List<ADMIN_QUANLYPHONGTRO.Models.DTO.YeuCauHoTroDto>());
            }
        }

        /// <summary>
        /// Chi tiết yêu cầu hỗ trợ
        /// </summary>
        public async Task<ActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            
            try
            {
                var support = await _service.GetByIdAsync(id);
                if (support == null)
                {
                    ViewBag.Error = "Không tìm thấy yêu cầu hỗ trợ";
                    return View();
                }
                
                var loaiHoTros = await _service.GetLoaiHoTroAsync();
                ViewBag.LoaiHoTros = loaiHoTros;
                
                return View(support);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi tải chi tiết yêu cầu";
                ViewBag.ErrorDetails = ex.Message;
                return View();
            }
        }

        /// <summary>
        /// Lấy chi tiết yêu cầu (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetSupportDetail(string id)
        {
            try
            {
                var support = await _service.GetByIdAsync(id);
                if (support == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy yêu cầu hỗ trợ" }, JsonRequestBehavior.AllowGet);
                }
                
                return Json(new { success = true, data = support }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Bắt đầu xử lý yêu cầu
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Process(string id)
        {
            try
            {
                var result = await _service.UpdateStatusAsync(id, "DangXuLy");
                if (result.Success)
                {
                    return Json(new { success = true, message = "Đã bắt đầu xử lý yêu cầu" });
                }
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Hoàn thành xử lý yêu cầu
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Complete(string id)
        {
            try
            {
                var result = await _service.UpdateStatusAsync(id, "HoanThanh");
                if (result.Success)
                {
                    return Json(new { success = true, message = "Đã hoàn thành xử lý yêu cầu" });
                }
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Từ chối yêu cầu
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Reject(string id, string lyDo = "")
        {
            try
            {
                var result = await _service.UpdateStatusAsync(id, "TuChoi");
                if (result.Success)
                {
                    return Json(new { success = true, message = result.Message ?? "Đã từ chối yêu cầu hỗ trợ" });
                }
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê yêu cầu hỗ trợ (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetStatistics(string search = "", string status = "", string type = "")
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔍 Support GetStatistics called with: search='{search}', status='{status}', type='{type}'");

                // If no filter, return global stats (fast path)
                if (string.IsNullOrWhiteSpace(search) && string.IsNullOrWhiteSpace(status) && string.IsNullOrWhiteSpace(type))
                {
                    var stats = await _service.GetStatisticsAsync();
                    System.Diagnostics.Debug.WriteLine($"✅ Global support stats: Total={stats.TotalRequests}, New={stats.NewRequests}, Processing={stats.ProcessingRequests}, Completed={stats.CompletedRequests}, Rejected={stats.RejectedRequests}");
                    return Json(new { success = true, data = stats }, JsonRequestBehavior.AllowGet);
                }

                // Filtered stats: fetch all then calculate locally
                System.Diagnostics.Debug.WriteLine("🔎 Applying filters to support statistics");
                var all = await _service.GetAllSupportsAsync();
                var items = all ?? new System.Collections.Generic.List<ADMIN_QUANLYPHONGTRO.Models.DTO.YeuCauHoTroDto>();

                System.Diagnostics.Debug.WriteLine($"📈 Initial items count: {items.Count}");

                // Filter by search
                if (!string.IsNullOrWhiteSpace(search))
                {
                    items = items
                        .Where(s =>
                            (s.TieuDe ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (s.MoTa ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                        )
                        .ToList();
                    System.Diagnostics.Debug.WriteLine($"📈 After search filter: {items.Count}");
                }

                // Filter by status
                if (!string.IsNullOrWhiteSpace(status))
                {
                    items = items
                        .Where(s => string.Equals((s.TrangThai ?? "").Trim(), status.Trim(), StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    System.Diagnostics.Debug.WriteLine($"📈 After status filter: {items.Count}");
                }

                // Filter by type
                if (!string.IsNullOrWhiteSpace(type))
                {
                    items = items
                        .Where(s => string.Equals(s.LoaiHoTroId.ToString(), type, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    System.Diagnostics.Debug.WriteLine($"📈 After type filter: {items.Count}");
                }

                var filteredStats = new ADMIN_QUANLYPHONGTRO.ApiClients.SupportStatistics
                {
                    TotalRequests = items.Count,
                    NewRequests = items.Count(s => string.Equals((s.TrangThai ?? "").Trim(), "Moi", StringComparison.OrdinalIgnoreCase)),
                    ProcessingRequests = items.Count(s => string.Equals((s.TrangThai ?? "").Trim(), "DangXuLy", StringComparison.OrdinalIgnoreCase)),
                    CompletedRequests = items.Count(s => string.Equals((s.TrangThai ?? "").Trim(), "HoanThanh", StringComparison.OrdinalIgnoreCase)),
                    RejectedRequests = items.Count(s => string.Equals((s.TrangThai ?? "").Trim(), "TuChoi", StringComparison.OrdinalIgnoreCase))
                };

                System.Diagnostics.Debug.WriteLine($"✅ Filtered support stats: Total={filteredStats.TotalRequests}, New={filteredStats.NewRequests}, Processing={filteredStats.ProcessingRequests}, Completed={filteredStats.CompletedRequests}, Rejected={filteredStats.RejectedRequests}");

                return Json(new { success = true, data = filteredStats }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetStatistics error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Lấy danh sách yêu cầu hỗ trợ có phân trang (AJAX)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> GetSupports(int draw, int start, int length, string search = "", string status = "", string type = "")
        {
            try
            {
                // DataTables search[value] fallback
                if (string.IsNullOrEmpty(search) && Request.Form["search[value]"] != null)
                {
                    search = Request.Form["search[value]"];
                }

                // Fetch all data (API doesn't support filtering, so we filter locally)
                var all = await _service.GetAllSupportsAsync();
                var items = all ?? new System.Collections.Generic.List<ADMIN_QUANLYPHONGTRO.Models.DTO.YeuCauHoTroDto>();

                // Filter by search
                if (!string.IsNullOrWhiteSpace(search))
                {
                    items = items
                        .Where(s => 
                            (s.TieuDe ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (s.MoTa ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                        )
                        .ToList();
                }

                // Filter by status
                if (!string.IsNullOrWhiteSpace(status))
                {
                    items = items
                        .Where(s => string.Equals((s.TrangThai ?? "").Trim(), status.Trim(), StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Filter by type
                if (!string.IsNullOrWhiteSpace(type))
                {
                    items = items
                        .Where(s => string.Equals(s.LoaiHoTroId.ToString(), type, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var recordsTotal = all?.Count ?? 0;
                var recordsFiltered = items.Count;

                // Apply paging
                var pageItems = items
                    .OrderByDescending(s => s.ThoiGianTao)
                    .Skip(start)
                    .Take(length)
                    .ToList();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = pageItems
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0],
                    error = ex.Message
                });
            }
        }
    }
}

