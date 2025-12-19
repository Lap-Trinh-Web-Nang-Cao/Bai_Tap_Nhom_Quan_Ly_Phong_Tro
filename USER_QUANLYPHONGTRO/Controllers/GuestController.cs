
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    /// <summary>
    /// Controller dành cho khách vãng lai (Guest) - không cần đăng nhập
    /// Cho phép xem danh sách phòng, tìm kiếm, xem chi tiết
    /// Sử dụng API backend để lấy dữ liệu thực từ database
    /// </summary>
    public class GuestController : Controller
    {
        private readonly ApiClient _apiClient;

        public GuestController()
        {
            _apiClient = new ApiClient();
        }

        #region Main Pages

        /// <summary>
        /// GET: /Guest/Index - Landing Page với Featured Rooms
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            try
            {
                // Lấy 6 phòng (tạm thời không filter theo rating vì API chưa hỗ trợ)
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6");

                // ApiClient wraps response in ApiResponse<dynamic>
                // response.Data contains the actual API response: { Data: [...], TotalCount: 15 }
                if (response != null && response.Success && response.Data != null)
                {
                    // DEBUG: Serialize to see structure
                    var jsonDebug = Newtonsoft.Json.JsonConvert.SerializeObject(response.Data);
                    ViewBag.DebugResponseData = jsonDebug;

                    // Parse - response.Data is JObject, use indexer
                    var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                    if (jData != null)
                    {
                        var dataArrayToken = jData["Data"];
                        var totalCount = (int)(jData["TotalCount"] ?? 0);

                        if (dataArrayToken != null)
                        {
                            ViewBag.TotalRooms = totalCount;
                            ViewBag.ApiSuccess = true;

                            // Convert JArray to List<PhongDto>
                            var roomsList = new List<PhongDto>();
                            // Danh sách ảnh mặc định
                            var defaultImages = new[] {
                                "/images/banner-login.png",
                                "/images/banner-register-host.png",
                                "/images/banner-register-tenant.png",
                                "/images/Background_vien.jpg"
                            };
                            int imageIndex = 0;

                            foreach (var item in dataArrayToken)
                            {
                                roomsList.Add(new PhongDto
                                {
                                    PhongId = Guid.Parse(item["PhongId"].ToString()),
                                    NhaTroId = Guid.Parse(item["NhaTroId"].ToString()),
                                    TieuDe = item["TieuDe"]?.ToString() ?? "",
                                    DienTich = item["DienTich"] != null ? (decimal?)Convert.ToDecimal(item["DienTich"]) : null,
                                    GiaTien = (long)item["GiaTien"],
                                    TienCoc = item["TienCoc"] != null ? (long?)Convert.ToInt64(item["TienCoc"]) : null,
                                    SoNguoiToiDa = (int)item["SoNguoiToiDa"],
                                    TrangThai = item["TrangThai"]?.ToString() ?? "",
                                    DiemTrungBinh = item["DiemTrungBinh"] != null ? (double?)Convert.ToDouble(item["DiemTrungBinh"]) : null,
                                    SoLuongDanhGia = item["SoLuongDanhGia"] != null ? (int)item["SoLuongDanhGia"] : 0,
                                    IsDuyet = item["IsDuyet"] != null ? (bool)item["IsDuyet"] : false,
                                    IsBiKhoa = item["IsBiKhoa"] != null ? (bool)item["IsBiKhoa"] : false,
                                    CreatedAt = DateTimeOffset.Parse(item["CreatedAt"].ToString()),
                                    HinhAnhDaiDien = defaultImages[imageIndex % defaultImages.Length], // Rotate images
                                    NhaTro = item["NhaTro"] != null ? new NhaTroDto
                                    {
                                        NhaTroId = Guid.Parse(item["NhaTro"]["NhaTroId"].ToString()),
                                        TieuDe = item["NhaTro"]["TieuDe"]?.ToString() ?? "",
                                        DiaChi = item["NhaTro"]["DiaChi"]?.ToString()
                                    } : null
                                });
                                imageIndex++;
                            }

                            return View(roomsList);
                        }
                    }
                }

                ViewBag.TotalRooms = 0;
                ViewBag.ApiSuccess = false;
                ViewBag.ErrorMessage = "Không nhận được dữ liệu từ hệ thống. Vui lòng kiểm tra lại kết nối.";
                return View(new List<PhongDto>());
            }
            catch (Exception ex)
            {
                ViewBag.TotalRooms = 0;
                ViewBag.ApiSuccess = false;
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình xử lý: " + ex.Message;
                return View(new List<PhongDto>());
            }
        }

        /// <summary>
        /// GET: /Guest/DanhSachPhong - Danh sách phòng với API
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> DanhSachPhong(
            string keyword,
            string district,
            decimal? minPrice,
            decimal? maxPrice,
            string sort = "latest",
            int page = 1,
            int pageSize = 12)
        {
            try
            {
                // Gọi API backend để lấy danh sách phòng
                var apiUrl = $"/api/phong?page={page}&pageSize={pageSize}";

                if (minPrice.HasValue)
                    apiUrl += $"&minPrice={minPrice}";
                if (maxPrice.HasValue)
                    apiUrl += $"&maxPrice={maxPrice}";

                var response = await _apiClient.GetAsync<dynamic>(apiUrl);

                if (response.Success && response.Data != null)
                {
                    // Parse JObject
                    var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                    if (jData != null)
                    {
                        var dataArrayToken = jData["Data"];
                        var totalCount = (int)(jData["TotalCount"] ?? 0);
                        var totalPages = (int)(jData["TotalPages"] ?? 1);

                        // Convert JArray to List<PhongDto>
                        var roomsList = new List<PhongDto>();
                        if (dataArrayToken != null)
                        {
                            foreach (var item in dataArrayToken)
                            {
                                roomsList.Add(new PhongDto
                                {
                                    PhongId = Guid.Parse(item["PhongId"].ToString()),
                                    NhaTroId = Guid.Parse(item["NhaTroId"].ToString()),
                                    TieuDe = item["TieuDe"]?.ToString() ?? "",
                                    GiaTien = (long)item["GiaTien"],
                                    TrangThai = item["TrangThai"]?.ToString() ?? "",
                                    IsDuyet = item["IsDuyet"] != null ? (bool)item["IsDuyet"] : false,
                                    IsBiKhoa = item["IsBiKhoa"] != null ? (bool)item["IsBiKhoa"] : false,
                                    NhaTro = item["NhaTro"] != null ? new NhaTroDto
                                    {
                                        NhaTroId = Guid.Parse(item["NhaTro"]["NhaTroId"].ToString()),
                                        TieuDe = item["NhaTro"]["TieuDe"]?.ToString() ?? "",
                                        DiaChi = item["NhaTro"]["DiaChi"]?.ToString()
                                    } : null
                                });
                            }
                        }

                        // ViewBag cho filter và pagination
                        ViewBag.Keyword = keyword;
                        ViewBag.District = district;
                        ViewBag.MinPrice = minPrice;
                        ViewBag.MaxPrice = maxPrice;
                        ViewBag.Sort = sort;
                        ViewBag.CurrentPage = page;
                        ViewBag.TotalPages = totalPages;
                        ViewBag.TotalItems = totalCount;
                        ViewBag.PageSize = pageSize;


                        return View(roomsList);
                    }

                    ViewBag.ErrorMessage = "Invalid API data format";
                    return View(new List<PhongDto>());
                }
                else
                {
                    ViewBag.ErrorMessage = response.Message ?? "Không thể tải danh sách phòng";
                    return View(new List<PhongDto>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi: {ex.Message}";
                return View(new List<PhongDto>());
            }
        }

        #endregion

        // ===== CHI TIẾT PHÒNG TRỌ =====
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ChiTietPhong(Guid? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            try
            {
                // Gọi API để lấy chi tiết phòng
                var response = await _apiClient.GetAsync<PhongDto>($"/api/phong/{id.Value}");

                if (response.Success && response.Data != null)
                {
                    var phong = response.Data;

                    // Ẩn thông tin liên hệ cho khách vãng lai
                    ViewBag.ShowContactPrompt = true;
                    ViewBag.ContactMessage = "Đăng nhập để xem thông tin liên hệ chủ trọ";

                    return View(phong);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không tìm thấy phòng trọ";
                    return RedirectToAction("DanhSachPhong");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi: {ex.Message}";
                return RedirectToAction("DanhSachPhong");
            }
        }

        // ===== TÌM KIẾM NHANH =====
        [HttpGet]
        [AllowAnonymous]
        public ActionResult TimKiem(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index");
            }

            // Redirect sang trang danh sách với keyword
            return RedirectToAction("Index", new { keyword = q });
        }

        // ===== LẤY DANH SÁCH PHÒNG NỔI BẬT (cho trang chủ) =====
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> PhongNoiBat()
        {
            try
            {
                // Lấy phòng (tạm thời không filter vì API chưa hỗ trợ)
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6");

                if (response.Success && response.Data != null)
                {
                    var rooms = response.Data.Data ?? new List<PhongDto>();
                    return View(rooms);
                }
                else
                {
                    return View(new List<PhongDto>());
                }
            }
            catch (Exception)
            {
                return View(new List<PhongDto>());
            }
        }

        // ===== LỌC THEO KHU VỰC =====
        [HttpGet]
        [AllowAnonymous]
        public ActionResult TheoKhuVuc(string district)
        {
            if (string.IsNullOrWhiteSpace(district))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", new { district = district });
        }

        // ===== LỌC THEO KHOẢNG GIÁ =====
        [HttpGet]
        [AllowAnonymous]
        public ActionResult TheoGia(decimal? min, decimal? max)
        {
            return RedirectToAction("Index", new { minPrice = min, maxPrice = max });
        }

        // ===== THỐNG KÊ TỔNG QUAN (PUBLIC) =====
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ThongKe()
        {
            try
            {
                // Có thể tạo thêm API endpoint cho thống kê
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=100");

                if (response.Success && response.Data != null)
                {
                    ViewBag.TotalRooms = response.Data.TotalCount ?? 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalRooms = 0;
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.TotalRooms = 0;
                return View();
            }
        }
    }
}
