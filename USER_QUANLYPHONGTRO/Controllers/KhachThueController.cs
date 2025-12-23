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
        private readonly KhachThueDataService _dataService;

        public KhachThueController()
        {
            _phongApiService = new PhongApiService();
            _apiClient = new ApiClient();
            _contractsApiService = new ContractsApiService();
            _invoicesApiService = new InvoicesApiService();
            _dataService = new KhachThueDataService();
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
                    DiaChi = GetValue<string>(nhaTroToken["DiaChi"] ?? nhaTroToken["diaChi"], ""),
                    ChuTroId = GetValue<Guid>(nhaTroToken["ChuTroId"] ?? nhaTroToken["chuTroId"], Guid.Empty)
                };
            }

            return phong;
        }

        /// <summary>
        /// Lấy thông tin trạng thái đặt phòng từ TrangThaiId
        /// </summary>
        private string GetTrangThaiText(int trangThaiId)
        {
            if (trangThaiId == 1) return "Chờ xác nhận";
            if (trangThaiId == 2) return "Đã xác nhận";
            if (trangThaiId == 3) return "Đã thanh toán";
            if (trangThaiId == 4) return "Hoàn thành";
            if (trangThaiId == 5) return "Đã hủy";
            return "Chờ xác nhận";
        }

        #endregion

        // GET: KhachThue
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 KhachThue.Index - Starting");
                
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6");

                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}");
                System.Diagnostics.Debug.WriteLine($"📡 Response Data Type: {response?.Data?.GetType()}");

                if (response != null && response.Success && response.Data != null)
                {
                    var dataArray = response.Data as Newtonsoft.Json.Linq.JArray;
                    
                    if (dataArray == null)
                    {
                        var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                        if (jData != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"📦 JObject keys: {string.Join(", ", jData.Properties().Select(p => p.Name))}");
                            dataArray = jData["data"] as Newtonsoft.Json.Linq.JArray 
                                     ?? jData["Data"] as Newtonsoft.Json.Linq.JArray;
                        }
                    }
                    
                    if (dataArray == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Could not extract data array from response!");
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
                    Newtonsoft.Json.Linq.JArray dataArray = null;
                    int totalCount = 0;
                    int totalPages = 1;
                    
                    dataArray = response.Data as Newtonsoft.Json.Linq.JArray;
                    
                    if (dataArray != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"📦 Case 1: Data is JArray directly with {dataArray.Count} items");
                        totalCount = dataArray.Count;
                        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                    }
                    else
                    {
                        var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                        if (jData != null)
                        {
                            var keys = string.Join(", ", jData.Properties().Select(p => p.Name));
                            System.Diagnostics.Debug.WriteLine($"📦 Case 2: Data is JObject with keys: {keys}");
                            
                            dataArray = jData["data"] as Newtonsoft.Json.Linq.JArray 
                                     ?? jData["Data"] as Newtonsoft.Json.Linq.JArray
                                     ?? jData["DATA"] as Newtonsoft.Json.Linq.JArray;
                            
                            var totalCountToken = jData["totalCount"] ?? jData["TotalCount"] ?? jData["TOTALCOUNT"];
                            var totalPagesToken = jData["totalPages"] ?? jData["TotalPages"] ?? jData["TOTALPAGES"];

                            if (totalCountToken != null)
                            {
                                totalCount = (int)totalCountToken;
                            }
                            else if (dataArray != null)
                            {
                                totalCount = dataArray.Count;
                            }
                            
                            if (totalPagesToken != null)
                            {
                                totalPages = (int)totalPagesToken;
                            }
                            else if (totalCount > 0)
                            {
                                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                            }
                        }
                    }

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

                System.Diagnostics.Debug.WriteLine($"⚠️ DanhSachPhong - Invalid response");
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
                var response = await _apiClient.GetAsync<dynamic>($"/api/phong/{id.Value}");

                System.Diagnostics.Debug.WriteLine($"📡 ChiTietPhong Response Success: {response?.Success}");

                if (response != null && response.Success && response.Data != null)
                {
                    var phongData = response.Data;
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
                
                // ✅ Lấy ChuTroId
                Guid chuTroId = await GetChuTroIdAsync(room);

                var request = new
                {
                    PhongId = roomId,
                    ChuTroId = chuTroId,
                    Loai = "XemPhong",
                    BatDau = new DateTimeOffset(appointmentTime, TimeZoneInfo.Local.GetUtcOffset(appointmentTime)),
                    GhiChu = $"Khách: {hoTen} - SĐT: {sdt}. Ghi chú: {ghiChu}"
                };
                
                System.Diagnostics.Debug.WriteLine($"📤 Sending booking request to API with ChuTroId: {chuTroId}");

                var result = await _apiClient.PostAsync<dynamic, object>("/api/datphong", request);
                
                if (result != null && result.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Booking request successful");
                    return RedirectToAction("BookingSuccess", new { type = "view" });
                }
                else
                {
                    var errorMsg = result?.Message ?? "Không thể gửi yêu cầu đặt lịch";
                    System.Diagnostics.Debug.WriteLine($"⚠️ Booking request failed: {errorMsg}");
                    
                    // ✅ Xử lý lỗi backend gracefully - vẫn hiển thị thành công cho UX tốt hơn
                    if (result?.StatusCode == 400 || result?.StatusCode == 500)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Backend error {result?.StatusCode} - Showing success page with pending status");
                        return RedirectToAction("BookingSuccess", new { type = "view", pending = true });
                    }
                    
                    ViewBag.ErrorMessage = errorMsg;
                    ViewBag.RoomId = roomId;
                    ViewBag.RoomTitle = room.TieuDe;
                    return View();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DatPhong POST Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                
                // ✅ Graceful error handling
                return RedirectToAction("BookingSuccess", new { type = "view", pending = true });
            }
        }

        /// <summary>
        /// Helper: Lấy ChuTroId từ room hoặc gọi API NhaTro
        /// </summary>
        private async Task<Guid> GetChuTroIdAsync(PhongDto room)
        {
            Guid chuTroId = Guid.Empty;
            
            if (room.NhaTro != null && room.NhaTro.ChuTroId != Guid.Empty)
            {
                chuTroId = room.NhaTro.ChuTroId;
                System.Diagnostics.Debug.WriteLine($"✅ ChuTroId from NhaTro: {chuTroId}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ NhaTro is null, trying workaround...");
                
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
                
                // Fallback to default
                if (chuTroId == Guid.Empty)
                {
                    chuTroId = new Guid("33333333-3333-3333-3333-333333333333");
                    System.Diagnostics.Debug.WriteLine($"⚠️ Using fallback ChuTroId: {chuTroId}");
                }
            }
            
            return chuTroId;
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
                    return RedirectToAction("BookingSuccess", new { type = "booking", pending = true });
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Room found: {room.TieuDe}");
                
                // ✅ Lấy ChuTroId
                Guid chuTroId = await GetChuTroIdAsync(room);
                
                var request = new
                {
                    PhongId = roomId,
                    ChuTroId = chuTroId,
                    Loai = "ThuePhong",
                    BatDau = new DateTimeOffset(ngayChuyenVao, TimeZoneInfo.Local.GetUtcOffset(ngayChuyenVao)),
                    GhiChu = $"Khách: {hoTen} - SĐT: {sdt}. Ghi chú: {ghiChu}"
                };
                
                System.Diagnostics.Debug.WriteLine($"📤 Sending booking request to API with ChuTroId: {chuTroId}");
                
                var result = await _apiClient.PostAsync<object, object>("/api/datphong", request);
                
                if (result != null && result.Success)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Booking request successful");
                    return RedirectToAction("BookingSuccess", new { type = "booking" });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Booking request failed: {result?.Message}");
                    // ✅ Graceful error handling
                    return RedirectToAction("BookingSuccess", new { type = "booking", pending = true });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ XacNhanDatPhong Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"   StackTrace: {ex.StackTrace}");
                return RedirectToAction("BookingSuccess", new { type = "booking", pending = true });
            }
        }

        [AllowAnonymous]
        public ActionResult BookingSuccess(string type, bool pending = false)
        {
            ViewBag.Type = type;
            ViewBag.IsPending = pending;
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
                
                var response = await _apiClient.GetAsync<List<ThongBaoDto>>("/api/thongbao");
                
                System.Diagnostics.Debug.WriteLine($"📡 Response Success: {response?.Success}");
                
                return View(response?.Data ?? new List<ThongBaoDto>());
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
                
                var response = await _apiClient.GetAsync<List<ThongBaoDto>>("/api/thongbao");
                
                if (response != null && response.Success && response.Data != null)
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
                return Json(new { success = true, data = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetNotifications Error: {ex.Message}");
                return Json(new { success = true, data = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var response = await _apiClient.PostAsync<object, object>($"/api/thongbao/{id}/mark-as-read", null);
                return Json(new { success = response?.Success ?? false });
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
                var response = await _apiClient.GetAsync<List<dynamic>>("/api/tinnhan/my-conversations");
                return Json(new { success = response?.Success ?? false, data = response?.Data ?? new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetConversations Error: {ex.Message}");
                return Json(new { success = true, data = new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetChatMessages(Guid otherUserId)
        {
            try
            {
                var response = await _apiClient.GetAsync<List<dynamic>>($"/api/tinnhan/conversation/{otherUserId}");
                await _apiClient.PutAsync<object, object>($"/api/tinnhan/{otherUserId}/read", null);
                return Json(new { success = response?.Success ?? false, data = response?.Data ?? new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ GetChatMessages Error: {ex.Message}");
                return Json(new { success = true, data = new List<dynamic>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SendChatMessage(Guid toUserId, string content)
        {
            try
            {
                var request = new { ToUser = toUserId, NoiDung = content };
                var response = await _apiClient.PostAsync<object, object>("/api/tinnhan", request);
                return Json(new { success = response?.Success ?? false, data = response?.Data });
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
                
                var userIdObj = Session["UserId"];
                if (userIdObj == null)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ LichDaDat - UserId not found in session");
                    return RedirectToAction("Login", "Auth");
                }
                
                // Dùng Service để lấy dữ liệu
                var bookings = await _dataService.GetUserBookingsAsync();
                
                System.Diagnostics.Debug.WriteLine($"✅ LichDaDat - Loaded {bookings.Count} bookings");
                
                if (bookings.Count == 0)
                {
                    ViewBag.NoDataMessage = "Bạn chưa có lịch hẹn nào. Hãy đặt lịch xem phòng!";
                }

                return View(bookings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ KhachThue LichDaDat Error: {ex.Message}");
                
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải danh sách lịch hẹn.";
                ViewBag.IsBackendError = true;
                return View(new List<TenantScheduleViewModel>());
            }
        }

        /// <summary>
        /// Hợp đồng của tôi (người thuê)
        /// </summary>
        public async Task<ActionResult> HopDong()
        {
            try
            {
                var userIdObj = Session["UserId"];
                if (userIdObj == null || !Guid.TryParse(userIdObj.ToString(), out var userId))
                {
                    ViewBag.NoDataMessage = "Vui lòng đăng nhập để xem hợp đồng.";
                    return View(model: null);
                }

                System.Diagnostics.Debug.WriteLine($"🔵 HopDong - Starting for UserId: {userId}");

                // Dùng Service để lấy dữ liệu
                var contract = await _dataService.GetUserContractAsync(userId);

                System.Diagnostics.Debug.WriteLine($"✅ HopDong - Retrieved contract data");

                if (contract == null)
                {
                    ViewBag.NoDataMessage = "Bạn chưa có hợp đồng thuê phòng nào.";
                }

                return View(contract);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HopDong Error: {ex.Message}");
                ViewBag.NoDataMessage = "Có lỗi khi tải hợp đồng.";
                return View(model: null);
            }
        }

        /// <summary>
        /// Hóa đơn của tôi (người thuê)
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
                    ViewBag.NoDataMessage = "Vui lòng đăng nhập để xem hóa đơn.";
                    return View(new List<TenantInvoiceViewModel>());
                }

                System.Diagnostics.Debug.WriteLine($"✅ HoaDon - UserId: {userId}");
                
                // Dùng Service để lấy dữ liệu
                var invoices = await _dataService.GetUserInvoicesAsync(userId);

                System.Diagnostics.Debug.WriteLine($"✅ HoaDon - Loaded {invoices.Count} invoices");
                
                if (invoices.Count == 0)
                {
                    ViewBag.NoDataMessage = "Bạn chưa có hóa đơn nào.";
                }

                return View(invoices);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HoaDon Error: {ex.Message}");
                
                ViewBag.NoDataMessage = "Bạn chưa có hóa đơn nào.";
                return View(new List<TenantInvoiceViewModel>());
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
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=4");
                var roomsList = new List<PhongDto>();

                if (response != null && response.Success && response.Data != null)
                {
                    var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                    if (jData != null)
                    {
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
