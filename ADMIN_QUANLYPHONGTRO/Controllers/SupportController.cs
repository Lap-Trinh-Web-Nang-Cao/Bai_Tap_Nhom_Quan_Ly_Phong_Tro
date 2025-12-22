using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using System;
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
        public async Task<ActionResult> Reject(string id)
        {
            try
            {
                var result = await _service.UpdateStatusAsync(id, "TuChoi");
                if (result.Success)
                {
                    return Json(new { success = true, message = "Đã từ chối yêu cầu hỗ trợ" });
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
