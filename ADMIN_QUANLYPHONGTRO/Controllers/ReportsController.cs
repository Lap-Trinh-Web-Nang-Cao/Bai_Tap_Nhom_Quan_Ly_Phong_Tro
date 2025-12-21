using ADMIN_QUANLYPHONGTRO.Models.Common;
using ADMIN_QUANLYPHONGTRO.Models.DTO;
using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Controller quản lý báo cáo vi phạm
    /// </summary>
    public class ReportsController : Controller
    {
        private readonly IReportService _service;

        public ReportsController()
        {
            _service = new ReportService();
        }

        /// <summary>
        /// Trang danh sách báo cáo vi phạm
        /// </summary>
        public async Task<ActionResult> Index(int page = 1, int pageSize = 10, string keyword = "", string status = "")
        {
            try
            {
                var reports = await _service.GetAllReportsAsync();
                var violationTypes = await _service.GetViolationTypesAsync();
                var statistics = await _service.GetStatisticsAsync();

                ViewBag.ViolationTypes = violationTypes;
                ViewBag.Statistics = statistics;
                ViewBag.CurrentStatus = status;
                ViewBag.Keyword = keyword;

                return View(reports);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải dữ liệu báo cáo";
                ViewBag.ErrorDetails = ex.Message;
                return View(new System.Collections.Generic.List<ADMIN_QUANLYPHONGTRO.Models.DTO.BaoCaoViPhamDto>());
            }
        }

        /// <summary>
        /// API lấy danh sách báo cáo cho DataTable
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> GetReports(int draw, int start, int length, string search = "", string status = "", string loaiThucThe = "")
        {
            try
            {
                // DataTables search[value] fallback
                if (string.IsNullOrEmpty(search) && Request.Form["search[value]"] != null)
                {
                    search = Request.Form["search[value]"];
                }

                System.Diagnostics.Debug.WriteLine($"📊 GetReports: draw={draw}, start={start}, length={length}, search='{search}', status='{status}', loaiThucThe='{loaiThucThe}'");

                // Convert status from SNAKE_CASE to camelCase (Backend format)
                // CHO_XU_LY → ChoXuLy, DANG_XU_LY → DangXuLy, DA_XU_LY → DaXuLy, TU_CHOI → TuChoi
                var backendStatus = ConvertStatusToCamelCase(status);
                System.Diagnostics.Debug.WriteLine($"📊 Converted status: '{status}' → '{backendStatus}'");

                // Total (no filters) for recordsTotal
                var totalResult = await _service.GetReportsAsync(1, 1, "", "");
                var recordsTotal = totalResult?.TotalRecords ?? 0;

                // Fetch with search and status filters (using converted status)
                var all = await _service.GetReportsAsync(1, 100000, search, backendStatus);
                var items = all?.Items ?? new System.Collections.Generic.List<BaoCaoViPhamDto>();

                System.Diagnostics.Debug.WriteLine($"📊 Items after search+status filter: {items.Count}");

                // Apply loaiThucThe filter if provided
                if (!string.IsNullOrEmpty(loaiThucThe))
                {
                    items = items
                        .Where(r => string.Equals(r.LoaiThucThe, loaiThucThe, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    System.Diagnostics.Debug.WriteLine($"📊 Items after loaiThucThe filter: {items.Count}");
                }

                var recordsFiltered = items.Count;

                // Apply paging for DataTables
                var pageItems = items
                    .OrderByDescending(r => r.ThoiGianBaoCao)
                    .Skip(start)
                    .Take(length)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"📊 Final page items: {pageItems.Count}");

                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = pageItems.Select(r => new
                    {
                        r.BaoCaoId,
                        r.SoBaoCao,
                        r.LoaiThucThe,
                        r.TieuDe,
                        r.MoTa,
                        r.TrangThai,
                        ThoiGianBaoCao = r.ThoiGianBaoCao, // let JS format
                        ThoiGianXuLy = r.ThoiGianXuLy,
                        r.KetQua,
                        r.ViPhamId,
                        r.ThucTheId,
                        r.NguoiBaoCao
                    })
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetReports error: {ex.Message}\n{ex.StackTrace}");
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

        /// <summary>
        /// Helper: Convert status from SNAKE_CASE to camelCase
        /// </summary>
        private string ConvertStatusToCamelCase(string status)
        {
            if (string.IsNullOrEmpty(status)) return "";
            
            switch (status)
            {
                case "CHO_XU_LY": return "ChoXuLy";
                case "DANG_XU_LY": return "DangXuLy";
                case "DA_XU_LY": return "DaXuLy";
                case "TU_CHOI": return "TuChoi";
                default: return status;
            }
        }

        /// <summary>
        /// Trang chi tiết báo cáo
        /// </summary>
        public async Task<ActionResult> Detail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index");
                }

                var report = await _service.GetReportByIdAsync(id);
                if (report == null)
                {
                    ViewBag.Error = "Không tìm thấy báo cáo";
                    return RedirectToAction("Index");
                }

                var violationTypes = await _service.GetViolationTypesAsync();
                ViewBag.ViolationTypes = violationTypes;

                return View(report);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải chi tiết báo cáo";
                ViewBag.ErrorDetails = ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// API lấy chi tiết báo cáo (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetReportDetail(string id)
        {
            try
            {
                var report = await _service.GetReportByIdAsync(id);
                if (report == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy báo cáo" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        report.BaoCaoId,
                        report.SoBaoCao,
                        report.LoaiThucThe,
                        report.TieuDe,
                        report.MoTa,
                        report.TrangThai,
                        ThoiGianBaoCao = report.ThoiGianBaoCao?.ToString("dd/MM/yyyy HH:mm"),
                        ThoiGianXuLy = report.ThoiGianXuLy?.ToString("dd/MM/yyyy HH:mm"),
                        report.KetQua,
                        report.ViPhamId,
                        report.ThucTheId,
                        report.NguoiBaoCao,
                        report.NguoiXuLy
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Xử lý báo cáo (đánh dấu đã xử lý)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Resolve(string id, string ketQua = "Đã xử lý vi phạm")
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID báo cáo không hợp lệ" });
                }

                var result = await _service.ResolveReportAsync(id, ketQua);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Từ chối báo cáo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Reject(string id, string lyDo)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID báo cáo không hợp lệ" });
                }

                if (string.IsNullOrEmpty(lyDo))
                {
                    return Json(new { success = false, message = "Vui lòng nhập lý do từ chối" });
                }

                var result = await _service.RejectReportAsync(id, lyDo);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa báo cáo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID báo cáo không hợp lệ" });
                }

                var result = await _service.DeleteReportAsync(id);
                return Json(new { success = result, message = result ? "Xóa thành công" : "Không thể xóa báo cáo" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API lấy thống kê báo cáo
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetStatistics(string search = "", string status = "", string loaiThucThe = "")
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔍 GetStatistics called with: search='{search}', status='{status}', loaiThucThe='{loaiThucThe}'");

                // Always fetch from Backend API - don't do local filtering
                var stats = await _service.GetStatisticsAsync();
                
                System.Diagnostics.Debug.WriteLine($"✅ Backend stats: Total={stats.TotalReports}, Pending={stats.PendingReports}, Processing={stats.ProcessingReports}, Resolved={stats.ResolvedReports}, Rejected={stats.RejectedReports}");

                return Json(new { success = true, data = stats }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetStatistics error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Bắt đầu xử lý báo cáo (thay đổi status CHO_XU_LY → DANG_XU_LY)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Start(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "ID báo cáo không hợp lệ" });
                }

                // Update status from CHO_XU_LY to DANG_XU_LY
                var result = await _service.UpdateStatusAsync(id, "DANG_XU_LY", "");
                return Json(new { success = result.Success, message = result.Message ?? "Đã bắt đầu xử lý báo cáo" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
