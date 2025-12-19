using ADMIN_QUANLYPHONGTRO.Services.Implementations;
using ADMIN_QUANLYPHONGTRO.Services.Interfaces;
using ADMIN_QUANLYPHONGTRO.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ADMIN_QUANLYPHONGTRO.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _service;

        public RoomsController()
        {
            _service = new RoomService();
        }

        // GET: Rooms/Pending - Danh sách phòng chờ duyệt
        public async Task<ActionResult> Pending(int page = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                var rooms = await _service.GetPendingRoomsAsync(page, pageSize, keyword);
                
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;
                
                return View(rooms != null ? rooms.Items : new List<RoomPendingItemViewModel>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomsController.Pending Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Error = "Lỗi khi tải danh sách phòng";
                ViewBag.ErrorDetails = ex.Message;
                return View(new List<RoomPendingItemViewModel>());
            }
        }

        // GET: Rooms/Approved - Danh sách phòng đã duyệt
        public async Task<ActionResult> Approved(int page = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                var rooms = await _service.GetPendingRoomsAsync(page, pageSize, keyword);
                
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = "approved";
                
                return View("Pending", rooms != null ? rooms.Items : new List<RoomPendingItemViewModel>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomsController.Approved Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Error = "Lỗi khi tải danh sách phòng đã duyệt";
                return View("Pending", new List<RoomPendingItemViewModel>());
            }
        }

        // GET: Rooms/Locked - Danh sách phòng bị khóa
        public async Task<ActionResult> Locked(int page = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                var rooms = await _service.GetPendingRoomsAsync(page, pageSize, keyword);
                
                ViewBag.Keyword = keyword;
                ViewBag.PageIndex = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = "locked";
                
                return View("Pending", rooms != null ? rooms.Items : new List<RoomPendingItemViewModel>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomsController.Locked Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.Error = "Lỗi khi tải danh sách phòng bị khóa";
                return View("Pending", new List<RoomPendingItemViewModel>());
            }
        }

        // ============ DATATABLES API METHODS ============

        /// <summary>
        /// API cho DataTables: Lấy danh sách phòng chờ duyệt
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetPendingRooms(int draw, int start, int length, string search = "")
        {
            try
            {
                int pageIndex = (start / length) + 1;
                var pagedResult = await _service.GetPendingRoomsAsync(pageIndex, length, search);

                return Json(new
                {
                    draw = draw,
                    recordsTotal = pagedResult?.TotalRecords ?? 0,
                    recordsFiltered = pagedResult?.TotalRecords ?? 0,
                    data = pagedResult?.Items ?? new List<RoomPendingItemViewModel>()
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomsController.GetPendingRooms Error: {ex.Message}");
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
        /// API: Lấy chi tiết phòng để hiển thị trong Modal
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetRoomDetail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(
                        new { success = false, message = "ID không hợp lệ" },
                        JsonRequestBehavior.AllowGet
                    );

                var room = await _service.GetRoomDetailAsync(id);
                
                if (room == null)
                    return Json(
                        new { success = false, message = "Không tìm thấy phòng" },
                        JsonRequestBehavior.AllowGet
                    );

                return Json(
                    new
                    {
                        success = true,
                        data = room
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomsController.GetRoomDetail Error: {ex.Message}");
                return Json(
                    new { success = false, message = ex.Message },
                    JsonRequestBehavior.AllowGet
                );
            }
        }

        /// <summary>
        /// API: Duyệt phòng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> ApproveRoom(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" });

                var result = await _service.ApproveRoomAsync(id);

                if (result)
                    return Json(new { success = true, message = "Đã duyệt phòng" });
                else
                    return Json(new { success = false, message = "Duyệt phòng thất bại" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomsController.ApproveRoom Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Từ chối phòng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> RejectRoom(string id, string reason)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" });

                if (string.IsNullOrEmpty(reason))
                    return Json(new { success = false, message = "Lý do từ chối không được để trống" });

                var result = await _service.RejectRoomAsync(id, reason);

                if (result)
                    return Json(new { success = true, message = "Đã từ chối phòng" });
                else
                    return Json(new { success = false, message = "Từ chối thất bại" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomsController.RejectRoom Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Khóa/Mở khóa phòng
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> ToggleLockRoom(string id, bool isLocked)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID không hợp lệ" });

                var result = await _service.ToggleLockRoomAsync(id, isLocked);

                if (result)
                    return Json(new { 
                        success = true, 
                        message = isLocked ? "Đã khóa phòng" : "Đã mở khóa phòng" 
                    });
                else
                    return Json(new { success = false, message = "Thao tác thất bại" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RoomsController.ToggleLockRoom Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
