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
                System.Diagnostics.Debug.WriteLine("🔵 Guest.Index - Starting");
                
                // Lấy 6 phòng
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6");

                System.Diagnostics.Debug.WriteLine($"📡 Guest Response Success: {response?.Success}");
                System.Diagnostics.Debug.WriteLine($"📡 Guest Response Data Type: {response?.Data?.GetType()}");

                if (response != null && response.Success && response.Data != null)
                {
                    // Handle both JArray and JObject responses
                    Newtonsoft.Json.Linq.JArray dataArray = null;
                    int totalCount = 0;
                    
                    // Try JArray first
                    dataArray = response.Data as Newtonsoft.Json.Linq.JArray;
                    
                    if (dataArray != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"📦 Guest: Data is JArray with {dataArray.Count} items");
                        totalCount = dataArray.Count;
                    }
                    else
                    {
                        // Try JObject
                        var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                        if (jData != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"📦 Guest: Data is JObject with keys: {string.Join(", ", jData.Properties().Select(p => p.Name))}");
                            
                            dataArray = jData["data"] as Newtonsoft.Json.Linq.JArray 
                                     ?? jData["Data"] as Newtonsoft.Json.Linq.JArray;
                            
                            var totalCountToken = jData["totalCount"] ?? jData["TotalCount"];
                            totalCount = totalCountToken != null ? (int)totalCountToken : (dataArray?.Count ?? 0);
                        }
                    }

                    if (dataArray != null)
                    {
                        var roomsList = new List<PhongDto>();
                        int imageIndex = 0;

                        foreach (var item in dataArray)
                        {
                            roomsList.Add(MapToPhongDto(item, imageIndex));
                            imageIndex++;
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ Guest.Index - Mapped {roomsList.Count} rooms");
                        
                        ViewBag.TotalRooms = totalCount;
                        ViewBag.ApiSuccess = true;
                        return View(roomsList);
                    }
                }

                System.Diagnostics.Debug.WriteLine("⚠️ Guest.Index - No data");
                ViewBag.TotalRooms = 0;
                ViewBag.ApiSuccess = false;
                ViewBag.ErrorMessage = response?.Message ?? "Không nhận được dữ liệu từ hệ thống. Vui lòng kiểm tra lại kết nối.";
                return View(new List<PhongDto>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Guest.Index Error: {ex.Message}\n{ex.StackTrace}");
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
                System.Diagnostics.Debug.WriteLine($"🔵 Guest.DanhSachPhong - page={page}, pageSize={pageSize}");
                
                // Gọi API backend để lấy danh sách phòng
                var apiUrl = $"/api/phong?page={page}&pageSize={pageSize}";

                if (minPrice.HasValue)
                    apiUrl += $"&minPrice={minPrice}";
                if (maxPrice.HasValue)
                    apiUrl += $"&maxPrice={maxPrice}";

                System.Diagnostics.Debug.WriteLine($"📡 Guest calling: {apiUrl}");
                var response = await _apiClient.GetAsync<dynamic>(apiUrl);

                System.Diagnostics.Debug.WriteLine($"📡 Guest Response Success: {response?.Success}");
                
                if (response != null && response.Success && response.Data != null)
                {
                    // Handle both JArray and JObject responses
                    Newtonsoft.Json.Linq.JArray dataArray = null;
                    int totalCount = 0;
                    int totalPages = 1;
                    
                    // Try JArray first
                    dataArray = response.Data as Newtonsoft.Json.Linq.JArray;
                    
                    if (dataArray != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"📦 Guest: Data is JArray with {dataArray.Count} items");
                        totalCount = dataArray.Count;
                        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    }
                    else
                    {
                        // Try JObject
                        var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                        if (jData != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"📦 Guest: Data is JObject with keys: {string.Join(", ", jData.Properties().Select(p => p.Name))}");
                            
                            dataArray = jData["data"] as Newtonsoft.Json.Linq.JArray 
                                     ?? jData["Data"] as Newtonsoft.Json.Linq.JArray;
                            
                            var totalCountToken = jData["totalCount"] ?? jData["TotalCount"];
                            var totalPagesToken = jData["totalPages"] ?? jData["TotalPages"];

                            totalCount = totalCountToken != null ? (int)totalCountToken : (dataArray?.Count ?? 0);
                            totalPages = totalPagesToken != null ? (int)totalPagesToken : 1;
                        }
                    }

                    // Convert JArray to List<PhongDto>
                    var roomsList = new List<PhongDto>();
                    
                    if (dataArray != null && dataArray.Count > 0)
                    {
                        int imageIndex = 0;
                        foreach (var item in dataArray)
                        {
                            try
                            {
                                roomsList.Add(MapToPhongDto(item, imageIndex));
                                imageIndex++;
                            }
                            catch (Exception mapEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ Guest map error: {mapEx.Message}");
                            }
                        }
                        System.Diagnostics.Debug.WriteLine($"✅ Guest.DanhSachPhong - Mapped {roomsList.Count} rooms");
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
                else
                {
                    ViewBag.ErrorMessage = response?.Message ?? "Không thể tải danh sách phòng";
                    return View(new List<PhongDto>());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Guest.DanhSachPhong Error: {ex.Message}");
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
                var response = await _apiClient.GetAsync<dynamic>($"/api/phong/{id.Value}");

                if (response.Success && response.Data != null)
                {
                    // Map từ JToken sang PhongDto bằng helper đã có
                    var phong = MapToPhongDto(response.Data as Newtonsoft.Json.Linq.JToken, 0);

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
                // Gọi API lấy 6 phòng cho featured
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6");

                if (response.Success && response.Data != null)
                {
                    var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                    if (jData != null)
                    {
                        var dataArrayToken = jData["Data"] ?? jData["data"];
                        var roomsList = new List<PhongDto>();
                        int imageIndex = 0;

                        if (dataArrayToken != null)
                        {
                            foreach (var item in dataArrayToken)
                            {
                                roomsList.Add(MapToPhongDto(item, imageIndex));
                                imageIndex++;
                            }
                        }
                        return PartialView(roomsList);
                    }
                }

                return PartialView(new List<PhongDto>());
            }
            catch (Exception)
            {
                return PartialView(new List<PhongDto>());
            }
        }

        #region Helper Methods

        /// <summary>
        /// Rút trích giá trị từ JToken an toàn
        /// </summary>
        private T GetValue<T>(Newtonsoft.Json.Linq.JToken token, T defaultValue = default)
        {
            if (token == null || token.Type == Newtonsoft.Json.Linq.JTokenType.Null)
                return defaultValue;
            try
            {
                if (token.Type == Newtonsoft.Json.Linq.JTokenType.String && string.IsNullOrWhiteSpace(token.ToString()))
                {
                    return defaultValue;
                }
                return token.ToObject<T>();
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Map dynamic JToken from API to PhongDto
        /// </summary>
        private PhongDto MapToPhongDto(Newtonsoft.Json.Linq.JToken item, int imageIndex)
        {
            var defaultImages = new[] {
                "~/images/banner-login.png",
                "~/images/banner-register-host.png",
                "~/images/banner-register-tenant.png",
                "~/images/Background_vien.jpg"
            };

            string apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7039";
            var hinhAnhToken = item["HinhAnhDaiDien"] ?? item["hinhAnhDaiDien"];
            var hinhAnhFromApi = hinhAnhToken?.ToString();
            string finalImagePath;

            if (string.IsNullOrEmpty(hinhAnhFromApi) || hinhAnhFromApi == "string")
            {
                finalImagePath = defaultImages[imageIndex % defaultImages.Length];
            }
            else if (hinhAnhFromApi.StartsWith("http") || hinhAnhFromApi.StartsWith("~"))
            {
                finalImagePath = hinhAnhFromApi;
            }
            else if (hinhAnhFromApi.StartsWith("/"))
            {
                finalImagePath = apiBaseUrl.TrimEnd('/') + hinhAnhFromApi;
            }
            else
            {
                finalImagePath = apiBaseUrl.TrimEnd('/') + "/uploads/" + hinhAnhFromApi;
            }

            var phong = new PhongDto
            {
                PhongId = GetValue<Guid>(item["PhongId"] ?? item["phongId"], Guid.Empty),
                NhaTroId = GetValue<Guid>(item["NhaTroId"] ?? item["nhaTroId"], Guid.Empty),
                TieuDe = GetValue<string>(item["TieuDe"] ?? item["tieuDe"], "Không có tiêu đề"),
                DienTich = GetValue<decimal?>(item["DienTich"] ?? item["dienTich"], null),
                GiaTien = GetValue<long>(item["GiaTien"] ?? item["giaTien"], 0),
                TienCoc = GetValue<long?>(item["TienCoc"] ?? item["tienCoc"], null),
                SoNguoiToiDa = GetValue<int>(item["SoNguoiToiDa"] ?? item["soNguoiToiDa"], 1),
                TrangThai = GetValue<string>(item["TrangThai"] ?? item["trangThai"], ""),
                DiemTrungBinh = GetValue<double?>(item["DiemTrungBinh"] ?? item["diemTrungBinh"], null),
                SoLuongDanhGia = GetValue<int>(item["SoLuongDanhGia"] ?? item["soLuongDanhGia"], 0),
                IsDuyet = GetValue<bool>(item["IsDuyet"] ?? item["isDuyet"], false),
                IsBiKhoa = GetValue<bool>(item["IsBiKhoa"] ?? item["isBiKhoa"], false),
                HinhAnhDaiDien = finalImagePath,
                MoTa = GetValue<string>(item["MoTa"] ?? item["moTa"], "")
            };

            // Map tiện ích
            var tienIchsToken = item["TienIchs"] ?? item["tienIchs"] ?? item["TienIchList"] ?? item["tienIchList"];
            if (tienIchsToken != null && tienIchsToken.Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {
                foreach (var ti in tienIchsToken)
                {
                    phong.TienIchs.Add(new TienIchDto
                    {
                        TienIchId = GetValue<int>(ti["TienIchId"] ?? ti["tienIchId"], 0),
                        Ten = GetValue<string>(ti["Ten"] ?? ti["ten"], "")
                    });
                }
            }

            // Map danh sách hình ảnh
            var hinhAnhsToken = item["DanhSachHinhAnh"] ?? item["danhSachHinhAnh"];
            if (hinhAnhsToken != null && hinhAnhsToken.Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {
                foreach (var img in hinhAnhsToken)
                {
                    string imgStr = img.ToString();
                    if (imgStr.StartsWith("http") || imgStr.StartsWith("~"))
                        phong.DanhSachHinhAnh.Add(imgStr);
                    else if (imgStr.StartsWith("/"))
                        phong.DanhSachHinhAnh.Add(apiBaseUrl.TrimEnd('/') + imgStr);
                    else
                        phong.DanhSachHinhAnh.Add(apiBaseUrl.TrimEnd('/') + "/uploads/" + imgStr);
                }
            }

            var createdAtToken = item["CreatedAt"] ?? item["createdAt"];
            if (createdAtToken != null && createdAtToken.Type != Newtonsoft.Json.Linq.JTokenType.Null)
            {
                try
                {
                    phong.CreatedAt = DateTimeOffset.Parse(createdAtToken.ToString());
                }
                catch { }
            }

            var nhaTroToken = item["NhaTro"] ?? item["nhaTro"];
            if (nhaTroToken != null && nhaTroToken.Type != Newtonsoft.Json.Linq.JTokenType.Null)
            {
                phong.NhaTro = new NhaTroDto
                {
                    NhaTroId = GetValue<Guid>(nhaTroToken["NhaTroId"] ?? nhaTroToken["nhaTroId"], Guid.Empty),
                    TieuDe = GetValue<string>(nhaTroToken["TieuDe"] ?? nhaTroToken["tieuDe"], ""),
                    DiaChi = GetValue<string>(nhaTroToken["DiaChi"] ?? nhaTroToken["diaChi"], "")
                };
            }

            return phong;
        }

        #endregion

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
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=1");

                if (response.Success && response.Data != null)
                {
                    var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                    var totalCountToken = jData?["TotalCount"] ?? jData?["totalCount"];
                    ViewBag.TotalRooms = (int)(totalCountToken ?? 0);
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
