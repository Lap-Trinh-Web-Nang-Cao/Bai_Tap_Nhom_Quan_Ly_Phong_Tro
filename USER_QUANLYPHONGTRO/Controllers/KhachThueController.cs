using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class KhachThueController : Controller
    {
        private readonly IPhongApiService _phongApiService;
        private readonly ApiClient _apiClient;
        private readonly IContractsApiService _contractsApiService;
        private readonly IInvoicesApiService _invoicesApiService;

        public KhachThueController()
        {
            _phongApiService = new PhongApiService();
            _apiClient = new ApiClient();
            _contractsApiService = new ContractsApiService();
            _invoicesApiService = new InvoicesApiService();
        }

        #region HELPER METHODS (ROBUST MAPPING)

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

        private PhongDto MapToPhongDto(Newtonsoft.Json.Linq.JToken item, int imageIndex)
        {
            const string defaultImage = "/images/banner-login.png";
            string apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7039";
            var hinhAnhToken = item["HinhAnhDaiDien"] ?? item["hinhAnhDaiDien"];
            string hinhAnhFromApi = hinhAnhToken?.ToString();
            string finalImagePath;

            if (string.IsNullOrEmpty(hinhAnhFromApi) || hinhAnhFromApi == "string")
            {
                finalImagePath = defaultImage;
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
                try { phong.CreatedAt = DateTimeOffset.Parse(createdAtToken.ToString()); } catch { }
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

        // GET: KhachThue
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 KhachThue.Index - Starting");
                
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6");

                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}");
                System.Diagnostics.Debug.WriteLine($"📡 Response Data Type: {response?.Data?.GetType()}");

                if (response != null && response.Success && response.Data != null)
                {
                    // response.Data might be:
                    // 1. JArray directly (if API returns array)
                    // 2. JObject with "data" or "Data" property (if API returns wrapped response)
                    
                    var dataArray = response.Data as Newtonsoft.Json.Linq.JArray;
                    
                    if (dataArray == null)
                    {
                        // Try to get from wrapped response
                        var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                        if (jData != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"📦 JObject keys: {string.Join(", ", jData.Properties().Select(p => p.Name))}");
                            
                            // Try multiple possible field names for data (case-insensitive)
                            dataArray = jData["data"] as Newtonsoft.Json.Linq.JArray 
                                     ?? jData["Data"] as Newtonsoft.Json.Linq.JArray;
                        }
                    }
                    
                    if (dataArray == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Could not extract data array from response!");
                        System.Diagnostics.Debug.WriteLine($"📦 Full response Data: {response.Data.ToString()}");
                    }
                    else
                    {
                        var roomsList = new List<PhongDto>();
                        int imageIndex = 0;
                        
                        foreach (var item in dataArray)
                        {
                            roomsList.Add(MapToPhongDto(item, imageIndex));
                            imageIndex++;
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"✅ Mapped {roomsList.Count} rooms");
                        return View(roomsList);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"⚠️ Response success but no data or null");
                return View(new List<PhongDto>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ KhachThue Index Error: {ex.Message}\n{ex.StackTrace}");
                return View(new List<PhongDto>());
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> DanhSachPhong(
            int page = 1,
            int pageSize = 12,
            string keyword = "",
            string priceRange = "",
            string areaRange = "")
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 DanhSachPhong - Starting (page={page}, pageSize={pageSize})");
                
                long? minPrice = null, maxPrice = null;
                if (!string.IsNullOrEmpty(priceRange))
                {
                    var priceParts = priceRange.Split('-');
                    if (priceParts.Length == 2)
                    {
                        if (long.TryParse(priceParts[0], out long min)) minPrice = min;
                        if (long.TryParse(priceParts[1], out long max)) maxPrice = max;
                    }
                }

                // ✅ ApiClient tự động lấy token từ Session nếu có
                var apiUrl = $"/api/phong?page={page}&pageSize={pageSize}";
                if (minPrice.HasValue) apiUrl += $"&minPrice={minPrice}";
                if (maxPrice.HasValue) apiUrl += $"&maxPrice={maxPrice}";

                System.Diagnostics.Debug.WriteLine($"📡 Calling API: {apiUrl}");
                var response = await _apiClient.GetAsync<dynamic>(apiUrl);

                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}");
                if (response?.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"📡 Response Data Type: {response.Data.GetType().Name}");
                }

                if (response != null && response.Success && response.Data != null)
                {
                    // Try to extract data array - handle multiple response formats
                    Newtonsoft.Json.Linq.JArray dataArray = null;
                    int totalCount = 0;
                    int totalPages = 1;
                    
                    // Case 1: response.Data is JArray directly
                    dataArray = response.Data as Newtonsoft.Json.Linq.JArray;
                    
                    if (dataArray != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"📦 Case 1: Data is JArray directly with {dataArray.Count} items");
                        totalCount = dataArray.Count;
                        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    }
                    else
                    {
                        // Case 2: response.Data is JObject with nested structure
                        var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                        if (jData != null)
                        {
                            var keys = string.Join(", ", jData.Properties().Select(p => p.Name));
                            System.Diagnostics.Debug.WriteLine($"📦 Case 2: Data is JObject with keys: {keys}");
                            
                            // Try all possible variations (case-insensitive)
                            dataArray = jData["data"] as Newtonsoft.Json.Linq.JArray 
                                     ?? jData["Data"] as Newtonsoft.Json.Linq.JArray
                                     ?? jData["DATA"] as Newtonsoft.Json.Linq.JArray;
                            
                            // Extract metadata
                            var totalCountToken = jData["totalCount"] ?? jData["TotalCount"] ?? jData["TOTALCOUNT"];
                            var totalPagesToken = jData["totalPages"] ?? jData["TotalPages"] ?? jData["TOTALPAGES"];

                            if (totalCountToken != null)
                            {
                                totalCount = (int)totalCountToken;
                                System.Diagnostics.Debug.WriteLine($"📊 TotalCount from response: {totalCount}");
                            }
                            else if (dataArray != null)
                            {
                                totalCount = dataArray.Count;
                                System.Diagnostics.Debug.WriteLine($"📊 TotalCount from array length: {totalCount}");
                            }
                            
                            if (totalPagesToken != null)
                            {
                                totalPages = (int)totalPagesToken;
                            }
                            else if (totalCount > 0)
                            {
                                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"📊 Metadata: totalCount={totalCount}, totalPages={totalPages}");
                            
                            if (dataArray == null)
                            {
                                System.Diagnostics.Debug.WriteLine($"❌ Could not find data array in JObject!");
                                System.Diagnostics.Debug.WriteLine($"📦 Full JObject: {jData.ToString().Substring(0, Math.Min(500, jData.ToString().Length))}");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ response.Data is neither JArray nor JObject! Type: {response.Data?.GetType().Name}");
                        }
                    }

                    // Build rooms list
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
                                System.Diagnostics.Debug.WriteLine($"⚠️ Error mapping room {imageIndex}: {mapEx.Message}");
                            }
                        }
                        System.Diagnostics.Debug.WriteLine($"✅ DanhSachPhong - Mapped {roomsList.Count}/{dataArray.Count} rooms successfully");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ DanhSachPhong - No data array found or empty array");
                    }

                    // Apply client-side keyword filter if provided
                    if (!string.IsNullOrEmpty(keyword) && roomsList.Count > 0)
                    {
                        var beforeFilter = roomsList.Count;
                        roomsList = roomsList.Where(r =>
                            (r.TieuDe != null && r.TieuDe.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (r.NhaTro != null && r.NhaTro.DiaChi != null && r.NhaTro.DiaChi.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        ).ToList();
                        System.Diagnostics.Debug.WriteLine($"🔍 Keyword filter: {beforeFilter} → {roomsList.Count} rooms");
                    }

                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = totalPages;
                    ViewBag.TotalCount = totalCount;
                    ViewBag.Keyword = keyword;
                    ViewBag.PriceRange = priceRange;
                    ViewBag.AreaRange = areaRange;
                    ViewBag.PageSize = pageSize;

                    System.Diagnostics.Debug.WriteLine($"✅ DanhSachPhong - Returning {roomsList.Count} rooms to view");
                    return View(roomsList);
                }

                System.Diagnostics.Debug.WriteLine($"❌ DanhSachPhong - Invalid response: Success={response?.Success}, HasData={response?.Data != null}");
                return View(new List<PhongDto>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ KhachThue DanhSachPhong Error: {ex.Message}\n{ex.StackTrace}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View(new List<PhongDto>());
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ChiTietPhong(Guid? id)
        {
            if (id == null) return RedirectToAction("Index");

            try
            {
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.GetAsync<dynamic>($"/api/phong/{id.Value}");

                System.Diagnostics.Debug.WriteLine($"📡 ChiTietPhong Response Success: {response?.Success}");

                if (response != null && response.Success && response.Data != null)
                {
                    var phongData = response.Data;
                    
                    // Handle both direct object and wrapped in Data property
                    var jToken = phongData as Newtonsoft.Json.Linq.JToken;
                    if (jToken != null)
                    {
                        var phong = MapToPhongDto(jToken, 0);
                        return View(phong);
                    }
                }

                ViewBag.ErrorMessage = "Không tìm thấy thông tin phòng trọ.";
                return View(new PhongDto());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ KhachThue ChiTietPhong Error: {ex.Message}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View(new PhongDto());
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> DatPhong(Guid roomId)
        {
            try
            {
                var room = await _phongApiService.GetRoomDetailAsync(roomId);
                if (room == null) return HttpNotFound();

                ViewBag.RoomId = roomId;
                ViewBag.RoomTitle = room.TieuDe;
                ViewBag.UserFullName = Session["HoTen"] as string;
                ViewBag.UserEmail = Session["UserName"] as string;
                ViewBag.UserPhone = Session["Sdt"] as string ?? "";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> DatPhong(Guid roomId, string hoTen, string sdt, string email, DateTime ngayXem, string gioXem, string ghiChu)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 DatPhong POST - Starting for RoomId: {roomId}");
                
                var room = await _phongApiService.GetRoomDetailAsync(roomId);
                
                if (room == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ DatPhong - Room not found");
                    ViewBag.ErrorMessage = "Không tìm thấy thông tin phòng trọ.";
                    return View();
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Room found: {room.TieuDe}");
                
                // Ghép ngày và giờ
                DateTime appointmentTime = ngayXem.Date;
                if (!string.IsNullOrEmpty(gioXem) && TimeSpan.TryParse(gioXem, out var timeSpan))
                {
                    appointmentTime = appointmentTime.Add(timeSpan);
                }
                
                System.Diagnostics.Debug.WriteLine($"📅 Appointment time: {appointmentTime}");
                
                // ✅ WORKAROUND: Backend không trả về NhaTro, dùng dummy ChuTroId
                Guid chuTroId = Guid.Empty;
                
                if (room.NhaTro != null && room.NhaTro.ChuTroId != Guid.Empty)
                {
                    chuTroId = room.NhaTro.ChuTroId;
                    System.Diagnostics.Debug.WriteLine($"✅ ChuTroId from NhaTro: {chuTroId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ DatPhong - NhaTro is null, trying workaround...");
                    
                    // WORKAROUND: Gọi API /api/nhatro/{nhaTroId} để lấy thông tin nhà trọ
                    try
                    {
                        var nhaTroResponse = await _apiClient.GetAsync<dynamic>($"/api/nhatro/{room.NhaTroId}");
                        
                        if (nhaTroResponse != null && nhaTroResponse.Success && nhaTroResponse.Data != null)
                        {
                            var nhaTroData = nhaTroResponse.Data as Newtonsoft.Json.Linq.JObject;
                            if (nhaTroData != null)
                            {
                                var chuTroIdStr = nhaTroData["chuTroId"]?.ToString() ?? nhaTroData["ChuTroId"]?.ToString();
                                if (!string.IsNullOrEmpty(chuTroIdStr) && Guid.TryParse(chuTroIdStr, out chuTroId))
                                {
                                    System.Diagnostics.Debug.WriteLine($"✅ WORKAROUND: Got ChuTroId from /api/nhatro: {chuTroId}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ WORKAROUND failed: {ex.Message}");
                    }
                    
                    // Nếu vẫn không có ChuTroId, dùng default
                    if (chuTroId == Guid.Empty)
                    {
                        // FALLBACK: Sử dụng ChuTroId mặc định từ backend (từ logs thấy "33333333-3333-3333...")
                        chuTroId = new Guid("33333333-3333-3333-3333-333333333333");
                        System.Diagnostics.Debug.WriteLine($"⚠️ Using fallback ChuTroId: {chuTroId}");
                    }
                }

                var request = new
                {
                    PhongId = roomId,
                    ChuTroId = chuTroId,
                    Loai = "XemPhong",
                    BatDau = new DateTimeOffset(appointmentTime),
                    GhiChu = $"Khách: {hoTen} - SĐT: {sdt}. Ghi chú: {ghiChu}"
                };
                
                System.Diagnostics.Debug.WriteLine($"📤 Sending booking request to API with ChuTroId: {chuTroId}");

                // ✅ ApiClient tự động lấy token từ Session nếu có
                var result = await _apiClient.PostAsync<dynamic, object>("/api/datphong", request);
                
                if (result != null && result.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Booking request successful");
                    return RedirectToAction("BookingSuccess", new { type = "view" });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Booking request failed: {result?.Message}");
                    ViewBag.ErrorMessage = result?.Message ?? "Không thể gửi yêu cầu đặt lịch. Vui lòng thử lại.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DatPhong POST Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> DatPhongTrucTiep(Guid roomId)
        {
            try
            {
                var room = await _phongApiService.GetRoomDetailAsync(roomId);
                if (room == null) return HttpNotFound();

                ViewBag.RoomId = roomId;
                ViewBag.RoomTitle = room.TieuDe;
                ViewBag.RoomPrice = room.GiaTien;
                ViewBag.UserFullName = Session["HoTen"] as string;
                ViewBag.UserEmail = Session["UserName"] as string;
                ViewBag.UserPhone = Session["Sdt"] as string ?? "";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> XacNhanDatPhong(Guid roomId, string hoTen, string sdt, string email, DateTime ngayChuyenVao, string ghiChu)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 XacNhanDatPhong - Starting for RoomId: {roomId}");
                
                var room = await _phongApiService.GetRoomDetailAsync(roomId);
                
                if (room == null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ XacNhanDatPhong - Room not found");
                    ViewBag.ErrorMessage = "Không tìm thấy thông tin phòng trọ.";
                    return RedirectToAction("BookingSuccess", new { type = "booking", error = "room_not_found" });
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Room found: {room.TieuDe}");
                
                // ✅ WORKAROUND: Backend không trả về NhaTro, dùng dummy ChuTroId
                Guid chuTroId = Guid.Empty;
                
                if (room.NhaTro != null && room.NhaTro.ChuTroId != Guid.Empty)
                {
                    chuTroId = room.NhaTro.ChuTroId;
                    System.Diagnostics.Debug.WriteLine($"✅ ChuTroId from NhaTro: {chuTroId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ XacNhanDatPhong - NhaTro is null, trying workaround...");
                    
                    // WORKAROUND: Gọi API /api/nhatro/{nhaTroId} để lấy thông tin nhà trọ
                    try
                    {
                        var nhaTroResponse = await _apiClient.GetAsync<dynamic>($"/api/nhatro/{room.NhaTroId}");
                        
                        if (nhaTroResponse != null && nhaTroResponse.Success && nhaTroResponse.Data != null)
                        {
                            var nhaTroData = nhaTroResponse.Data as Newtonsoft.Json.Linq.JObject;
                            if (nhaTroData != null)
                            {
                                var chuTroIdStr = nhaTroData["chuTroId"]?.ToString() ?? nhaTroData["ChuTroId"]?.ToString();
                                if (!string.IsNullOrEmpty(chuTroIdStr) && Guid.TryParse(chuTroIdStr, out chuTroId))
                                {
                                    System.Diagnostics.Debug.WriteLine($"✅ WORKAROUND: Got ChuTroId from /api/nhatro: {chuTroId}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ WORKAROUND failed: {ex.Message}");
                    }
                    
                    // Nếu vẫn không có ChuTroId, dùng default
                    if (chuTroId == Guid.Empty)
                    {
                        // FALLBACK: Sử dụng ChuTroId mặc định từ backend
                        chuTroId = new Guid("33333333-3333-3333-3333-333333333333");
                        System.Diagnostics.Debug.WriteLine($"⚠️ Using fallback ChuTroId: {chuTroId}");
                    }
                }
                
                var request = new
                {
                    PhongId = roomId,
                    ChuTroId = chuTroId,
                    Loai = "booking",
                    BatDau = ngayChuyenVao,
                    GhiChu = ghiChu
                };
                
                System.Diagnostics.Debug.WriteLine($"📤 Sending booking request to API with ChuTroId: {chuTroId}");
                
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var result = await _apiClient.PostAsync<object, object>("/api/datphong", request);
                
                if (result != null && result.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Booking request successful");
                    return RedirectToAction("BookingSuccess", new { type = "booking" });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Booking request failed: {result?.Message}");
                    return RedirectToAction("BookingSuccess", new { type = "booking", error = "api_failed" });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ XacNhanDatPhong Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                return RedirectToAction("BookingSuccess", new { type = "booking", error = "exception" });
            }
        }

        [AllowAnonymous]
        public ActionResult BookingSuccess(string type)
        {
            ViewBag.Type = type;
            return View();
        }

        [AllowAnonymous]
        public ActionResult TinNhan()
        {
            return View();
        }

        // ✅ Những action này cần xác thực nên giữ require login
        public async Task<ActionResult> ThongBao()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 ThongBao - Starting");
                
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.GetAsync<List<ThongBaoDto>>("/api/thongbao");
                
                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}");
                System.Diagnostics.Debug.WriteLine($"📡 Response Data Count: {response?.Data?.Count}");
                
                return View(response.Data ?? new List<ThongBaoDto>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ThongBao Error: {ex.Message}");
                return View(new List<ThongBaoDto>());
            }
        }

        public async Task<ActionResult> GetNotifications()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 GetNotifications - Starting");
                
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.GetAsync<List<ThongBaoDto>>("/api/thongbao");
                
                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}");
                System.Diagnostics.Debug.WriteLine($"📡 Response Data Count: {response?.Data?.Count}");
                
                if (response.Success && response.Data != null)
                {
                    var list = response.Data.Select(x => new
                    {
                        id = x.ThongBaoId,
                        title = x.TieuDe,
                        content = x.NoiDung,
                        type = x.Loai,
                        time = x.ThoiGianTao,
                        isRead = x.DaXem,
                        url = x.RedirectUrl
                    }).ToList();
                    return Json(new { success = true, data = list }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Không thể lấy thông báo" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetNotifications Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            try
            {
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.PostAsync<object, object>($"/api/thongbao/{id}/mark-as-read", null);
                return Json(new { success = response.Success });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ MarkAsRead Error: {ex.Message}");
                return Json(new { success = false });
            }
        }

        public async Task<ActionResult> ClearNotifications()
        {
            try
            {
                // ✅ ApiClient tự động lấy token từ Session nếu có
                await _apiClient.PostAsync<object, object>("/api/thongbao/mark-all-as-read", null);
                Session["NotificationCount"] = 0;
                return Redirect(Request.UrlReferrer?.ToString() ?? "/KhachThue");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ClearNotifications Error: {ex.Message}");
                return Redirect(Request.UrlReferrer?.ToString() ?? "/KhachThue");
            }
        }

        #region CHAT AJAX

        public async Task<ActionResult> GetConversations()
        {
            try
            {
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.GetAsync<List<dynamic>>("/api/tinnhan/my-conversations");
                return Json(new { success = response.Success, data = response.Data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetConversations Error: {ex.Message}");
                return Json(new { success = false, data = new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetChatMessages(Guid otherUserId)
        {
            try
            {
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.GetAsync<List<dynamic>>($"/api/tinnhan/conversation/{otherUserId}");
                // Sau khi lấy tin nhắn, đánh dấu đã đọc luôn
                await _apiClient.PutAsync<object, object>($"/api/tinnhan/{otherUserId}/read", null);
                return Json(new { success = response.Success, data = response.Data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetChatMessages Error: {ex.Message}");
                return Json(new { success = false, data = new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SendChatMessage(Guid toUserId, string content)
        {
            try
            {
                var request = new { ToUser = toUserId, NoiDung = content };
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.PostAsync<object, object>("/api/tinnhan", request);
                return Json(new { success = response.Success, data = response.Data });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SendChatMessage Error: {ex.Message}");
                return Json(new { success = false });
            }
        }

        #endregion

        public async Task<ActionResult> LichDaDat()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 LichDaDat - Starting");
                
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.GetAsync<List<DatPhongDto>>("/api/datphong/my-bookings");
                
                System.Diagnostics.Debug.WriteLine($"📡 LichDaDat Response Success: {response?.Success}");
                System.Diagnostics.Debug.WriteLine($"📡 LichDaDat Response StatusCode: {response?.StatusCode}");
                
                var viewModels = new List<TenantScheduleViewModel>();

                // ✅ Handle 500 error - backend có thể có bug với endpoint này
                if (response == null || !response.Success)
                {
                    var errorMsg = response?.Message ?? "Không thể tải danh sách lịch đã đặt";
                    
                    if (response?.StatusCode == 500)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Backend 500 Error - API /api/datphong/my-bookings có lỗi");
                        errorMsg = "Chức năng đang được bảo trì. Vui lòng thử lại sau.";
                    }
                    
                    ViewBag.ErrorMessage = errorMsg;
                    ViewBag.IsBackendError = true;
                    return View(viewModels);
                }

                if (response.Data != null && response.Data.Count > 0)
                {
                    foreach (var d in response.Data)
                    {
                        viewModels.Add(new TenantScheduleViewModel
                        {
                            TieuDePhong = d.TieuDePhong ?? "Phòng trọ",
                            DiaChi = d.DiaChi ?? "Đang cập nhật",
                            TrangThai = d.TenTrangThai ?? "Chờ xác nhận",
                            ThoiGianHen = d.BatDau.DateTime,
                            SdtChuTro = d.SdtChuTro ?? "N/A",
                            GhiChu = d.GhiChu
                        });
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"✅ LichDaDat - Loaded {viewModels.Count} bookings");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ LichDaDat - No bookings found");
                    ViewBag.NoDataMessage = "Bạn chưa có lịch hẹn nào. Hãy đặt lịch xem phòng!";
                }

                return View(viewModels);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ KhachThue LichDaDat Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải danh sách lịch hẹn.";
                ViewBag.IsBackendError = true;
                return View(new List<TenantScheduleViewModel>());
            }
        }

        /// <summary>
        /// Hợp đồng của tôi (người thuê)
        /// Backend hiện expose: GET /api/hopdong/nguoithue/{userId}/hieuluc
        /// </summary>
        public async Task<ActionResult> HopDong()
        {
            try
            {
                // TODO: khi có auth thật, lấy userId từ token/session. Tạm thời ưu tiên Session["UserId"] nếu có.
                var userIdObj = Session["UserId"];
                if (userIdObj == null || !Guid.TryParse(userIdObj.ToString(), out var userId))
                {
                    // Chưa có userId -> hiển thị empty state
                    return View(model: null);
                }

                // ✅ ApiClient tự động lấy token từ Session nếu có
                var apiRes = await _contractsApiService.GetActiveContractByTenantAsync(userId, null);

                if (apiRes == null || !apiRes.Success)
                {
                    ViewBag.ErrorMessage = apiRes?.Message ?? "Không thể tải hợp đồng";
                    return View(model: null);
                }

                return View(apiRes.Data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi khi tải hợp đồng: " + ex.Message;
                return View(model: null);
            }
        }

        /// <summary>
        /// Hóa đơn của tôi (người thuê)
        /// Backend hiện expose: GET /api/hoadon/nguoithue/{userId}
        /// </summary>
        public async Task<ActionResult> HoaDon()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 HoaDon - Starting");
                
                var userIdObj = Session["UserId"];
                if (userIdObj == null || !Guid.TryParse(userIdObj.ToString(), out var userId))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ HoaDon - UserId not found in session");
                    ViewBag.ErrorMessage = "Vui lòng đăng nhập để xem hóa đơn.";
                    return View(new List<Models.ViewModels.KhachThue.TenantInvoiceViewModel>());
                }

                System.Diagnostics.Debug.WriteLine($"✅ HoaDon - UserId: {userId}");
                
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var apiRes = await _invoicesApiService.GetInvoicesByTenantAsync(userId, null);

                System.Diagnostics.Debug.WriteLine($"📡 HoaDon Response Success: {apiRes?.Success}");
                System.Diagnostics.Debug.WriteLine($"📡 HoaDon Response StatusCode: {apiRes?.StatusCode}");
                
                if (apiRes == null || !apiRes.Success)
                {
                    var errorMsg = apiRes?.Message ?? "Không thể tải hóa đơn";
                    
                    // Handle 500 error
                    if (apiRes?.StatusCode == 500)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Backend 500 Error - API /api/hoadon/nguoithue/{userId} có lỗi");
                        errorMsg = "Chức năng đang được bảo trì. Vui lòng thử lại sau.";
                    }
                    
                    ViewBag.ErrorMessage = errorMsg;
                    ViewBag.IsBackendError = true;
                    return View(new List<Models.ViewModels.KhachThue.TenantInvoiceViewModel>());
                }

                var invoices = apiRes.Data ?? new List<Models.ViewModels.KhachThue.TenantInvoiceViewModel>();
                
                if (invoices.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ HoaDon - No invoices found");
                    ViewBag.NoDataMessage = "Bạn chưa có hóa đơn nào.";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ HoaDon - Loaded {invoices.Count} invoices");
                }

                return View(invoices);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HoaDon Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                
                ViewBag.ErrorMessage = "Có lỗi khi tải hóa đơn: " + ex.Message;
                ViewBag.IsBackendError = true;
                return View(new List<Models.ViewModels.KhachThue.TenantInvoiceViewModel>());
            }
        }

        // GET: KhachThue/ThongTinCaNhan
        public async Task<ActionResult> ThongTinCaNhan()
        {
            try
            {
                var userIdObj = Session["UserId"];
                if (userIdObj == null || !Guid.TryParse(userIdObj.ToString(), out var userId))
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Giả lập hoặc lấy từ API nếu có
                var profile = new Models.Dtos.Users.UserProfileDto
                {
                    HoTen = Session["HoTen"] as string,
                    Email = Session["UserName"] as string,
                    DienThoai = Session["Sdt"] as string ?? "0909123456",
                    AvatarUrl = Session["AvatarUrl"] as string ?? "/images/default-avatar.png",
                    DiaChi = "TP. Hồ Chí Minh",
                    NgaySinh = DateTime.Now.AddYears(-20)
                };

                return View(profile);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        // GET: KhachThue/YeuThich
        public async Task<ActionResult> YeuThich()
        {
            try
            {
                // Gọi API lấy danh sách phòng (giả lập là danh sách yêu thích)
                // Trong thực tế sẽ gọi /api/yeuthich
                // ✅ ApiClient tự động lấy token từ Session nếu có
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=4");
                var roomsList = new List<PhongDto>();

                if (response != null && response.Success && response.Data != null)
                {
                    var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                    if (jData != null)
                    {
                        // ✅ Extract data array (case-insensitive)
                        var dataArray = jData["data"] as Newtonsoft.Json.Linq.JArray 
                                     ?? jData["Data"] as Newtonsoft.Json.Linq.JArray;
                        
                        if (dataArray != null)
                        {
                            int imageIndex = 0;
                            foreach (var item in dataArray)
                            {
                                roomsList.Add(MapToPhongDto(item, imageIndex));
                                imageIndex++;
                            }
                            System.Diagnostics.Debug.WriteLine($"✅ YeuThich - Mapped {roomsList.Count} rooms");
                        }
                    }
                }

                return View(roomsList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ YeuThich Error: {ex.Message}");
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<PhongDto>());
            }
        }

    }
}
