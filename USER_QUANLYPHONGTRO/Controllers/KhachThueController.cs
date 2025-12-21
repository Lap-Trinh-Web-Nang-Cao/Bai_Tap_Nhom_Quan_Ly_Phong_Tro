using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class KhachThueController : Controller
    {
        private readonly IPhongApiService _phongApiService;

        public KhachThueController()
        {
            // Khởi tạo service (có thể dùng Dependency Injection container nếu có)
            _phongApiService = new PhongApiService();
        }

        // GET: KhachThue
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Danh sách phòng trọ - gọi API Backend để lấy dữ liệu
        /// </summary>
        /// <param name="page">Số trang (mặc định 1)</param>
        /// <param name="pageSize">Số phòng/trang (mặc định 10)</param>
        /// <param name="keyword">Từ khóa tìm kiếm</param>
        /// <param name="priceRange">Khoảng giá (vd: "1000000-2000000")</param>
        /// <param name="areaRange">Khoảng diện tích (vd: "20-30")</param>
        public async Task<ActionResult> DanhSachPhong(
            int page = 1,
            int pageSize = 10,
            string keyword = "",
            string priceRange = "",
            string areaRange = "")
        {
            try
            {
                // Xử lý khoảng giá
                long? minPrice = null, maxPrice = null;
                if (!string.IsNullOrEmpty(priceRange))
                {
                    var priceParts = priceRange.Split('-');
                    if (priceParts.Length == 2)
                    {
                        if (long.TryParse(priceParts[0], out long min))
                            minPrice = min;
                        if (long.TryParse(priceParts[1], out long max))
                            maxPrice = max;
                    }
                }

                // Gọi API lấy danh sách phòng
                var (rooms, totalCount, totalPages) = await _phongApiService.GetPublicRoomsAsync(
                    page: page,
                    pageSize: pageSize,
                    minPrice: minPrice,
                    maxPrice: maxPrice);

                System.Diagnostics.Debug.WriteLine($"✅ DanhSachPhong - API returned: {rooms.Count} rooms, TotalCount: {totalCount}, TotalPages: {totalPages}");

                // Lọc theo từ khóa client-side (nếu cần - có thể chuyển sang API)
                if (!string.IsNullOrEmpty(keyword))
                {
                    rooms = rooms
                        .Where(r => (r.TieuDe != null && r.TieuDe.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                   (r.NhaTro != null && r.NhaTro.DiaChi != null && r.NhaTro.DiaChi.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                        .ToList();
                }

                // Lọc theo diện tích client-side (nếu cần - có thể chuyển sang API)
                if (!string.IsNullOrEmpty(areaRange))
                {
                    var areaParts = areaRange.Split('-');
                    if (areaParts.Length == 2)
                    {
                        if (decimal.TryParse(areaParts[0], out decimal minArea) &&
                            decimal.TryParse(areaParts[1], out decimal maxArea))
                        {
                            rooms = rooms
                                .Where(r => r.DienTich.HasValue && r.DienTich >= minArea && r.DienTich <= maxArea)
                                .ToList();
                        }
                    }
                }

                // Gán dữ liệu cho View
                // Giữ nguyên totalCount từ API (số phòng theo filter giá)
                // Tính lại totalPages nếu có filter thêm ở client
                int filteredCount = rooms.Count;
                int recalculatedTotalPages = (int)Math.Ceiling(filteredCount / (double)pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = recalculatedTotalPages > 0 ? recalculatedTotalPages : 1;
                ViewBag.TotalCount = totalCount; // Giữ total từ API
                ViewBag.Keyword = keyword;
                ViewBag.PriceRange = priceRange;
                ViewBag.AreaRange = areaRange;
                ViewBag.PageSize = pageSize;

                System.Diagnostics.Debug.WriteLine($"✅ DanhSachPhong - After filtering: {rooms.Count} rooms displayed, TotalCount: {totalCount}");

                return View(rooms);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DanhSachPhong Error: {ex.Message}");
                ViewBag.ErrorMessage = "Có lỗi khi tải danh sách phòng: " + ex.Message;
                return View(new List<PhongDto>());
            }
        }

        /// <summary>
        /// Chi tiết phòng trọ
        /// </summary>
        public async Task<ActionResult> ChiTietPhong(Guid id)
        {
            try
            {
                var room = await _phongApiService.GetRoomDetailAsync(id);
                if (room == null)
                    return HttpNotFound();

                return View(room);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi khi tải chi tiết phòng: " + ex.Message;
                return View(new PhongDto());
            }
        }

        /// <summary>
        /// Trang đặt lịch xem phòng
        /// </summary>
        public async Task<ActionResult> DatPhong(Guid roomId)
        {
            try
            {
                var room = await _phongApiService.GetRoomDetailAsync(roomId);
                if (room == null)
                    return HttpNotFound();

                // Truyền thông tin phòng vào View để hiển thị
                ViewBag.PhongId = roomId;
                ViewBag.PhongTieuDe = room.TieuDe;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi: " + ex.Message;
                return View();
            }
        }
    }
}