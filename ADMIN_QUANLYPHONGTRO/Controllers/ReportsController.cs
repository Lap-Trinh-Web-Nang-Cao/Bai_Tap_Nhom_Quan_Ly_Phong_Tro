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
        public async Task<ActionResult> GetReports(int draw, int start, int length, string search = "", string status = "")
        {
            try
            {
                var pageIndex = (start / length) + 1;
                var result = await _service.GetReportsAsync(pageIndex, length, search, status);

                return Json(new
                {
                    draw = draw,
                    recordsTotal = result.TotalRecords,
                    recordsFiltered = result.TotalRecords,
                    data = result.Items.Select(r => new
                    {
                        r.BaoCaoId,
                        r.SoBaoCao,
                        r.LoaiThucThe,
                        r.TieuDe,
                        r.MoTa,
                        r.TrangThai,
                        ThoiGianBaoCao = r.ThoiGianBaoCao?.ToString("dd/MM/yyyy HH:mm"),
                        ThoiGianXuLy = r.ThoiGianXuLy?.ToString("dd/MM/yyyy HH:mm"),
                        r.KetQua,
                        r.ViPhamId,
                        r.ThucTheId,
                        r.NguoiBaoCao
                    })
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
        public async Task<ActionResult> GetStatistics()
        {
            try
            {
                var stats = await _service.GetStatisticsAsync();
                return Json(new { success = true, data = stats }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
