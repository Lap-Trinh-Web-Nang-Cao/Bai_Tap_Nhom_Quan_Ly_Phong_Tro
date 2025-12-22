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
        public async Task<ActionResult> Index()
        {
            try
            {
                // Gọi API lấy 6 phòng nổi bật
                var response = await _apiClient.GetAsync<dynamic>("/api/phong?pageSize=6");

                if (response != null && response.Success && response.Data != null)
                {
                    var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                    if (jData != null)
                    {
                        var dataArrayToken = jData["Data"] ?? jData["data"] ?? jData["Data"]?["Data"] ?? jData["data"]?["data"];
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
                        return View(roomsList);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"⚠️ KhachThue Index - API did not return expected data: {response.Message}");
                return View(new List<PhongDto>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ KhachThue Index Error: {ex.Message}");
                return View(new List<PhongDto>());
            }
        }

        public async Task<ActionResult> DanhSachPhong(
            int page = 1,
            int pageSize = 12,
            string keyword = "",
            string priceRange = "",
            string areaRange = "")
        {
            try
            {
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

                var response = await _apiClient.GetAsync<dynamic>(apiUrl);

                if (response != null && response.Success && response.Data != null)
                {
                    var jData = response.Data as Newtonsoft.Json.Linq.JObject;
                    if (jData != null)
                    {
                        var dataArrayToken = jData["Data"] ?? jData["data"] ?? jData["Data"]?["Data"] ?? jData["data"]?["data"];
                        var totalCountToken = jData["TotalCount"] ?? jData["totalCount"] ?? jData["Data"]?["TotalCount"] ?? jData["data"]?["totalCount"];
                        var totalPagesToken = jData["TotalPages"] ?? jData["totalPages"] ?? jData["Data"]?["TotalPages"] ?? jData["data"]?["totalPages"];

                        var totalCount = (int)(totalCountToken ?? 0);
                        var totalPages = (int)(totalPagesToken ?? 1);

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

                        if (!string.IsNullOrEmpty(keyword))
                        {
                            roomsList = roomsList.Where(r =>
                                (r.TieuDe != null && r.TieuDe.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                (r.NhaTro != null && r.NhaTro.DiaChi != null && r.NhaTro.DiaChi.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                            ).ToList();
                        }

                        ViewBag.CurrentPage = page;
                        ViewBag.TotalPages = totalPages;
                        ViewBag.TotalCount = totalCount;
                        ViewBag.Keyword = keyword;
                        ViewBag.PriceRange = priceRange;
                        ViewBag.AreaRange = areaRange;
                        ViewBag.PageSize = pageSize;

                        return View(roomsList);
                    }
                }

                return View(new List<PhongDto>());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ KhachThue DanhSachPhong Error: {ex.Message}");
                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View(new List<PhongDto>());
            }
        }

        public async Task<ActionResult> ChiTietPhong(Guid? id)
        {
            if (id == null) return RedirectToAction("Index");

            try
            {
                var response = await _apiClient.GetAsync<dynamic>($"/api/phong/{id.Value}");

                if (response != null && response.Success && response.Data != null)
                {
                    var phong = MapToPhongDto(response.Data as Newtonsoft.Json.Linq.JToken, 0);
                    return View(phong);
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
        public async Task<ActionResult> DatPhong(Guid roomId, string hoTen, string sdt, string email, DateTime ngayXem, string gioXem, string ghiChu)
        {
            try
            {
                var room = await _phongApiService.GetRoomDetailAsync(roomId);
                if (room != null)
                {
                    // Ghép ngày và giờ
                    DateTime appointmentTime = ngayXem.Date;
                    if (!string.IsNullOrEmpty(gioXem) && TimeSpan.TryParse(gioXem, out var timeSpan))
                    {
                        appointmentTime = appointmentTime.Add(timeSpan);
                    }

                    if (room.NhaTro == null || room.NhaTro.ChuTroId == Guid.Empty)
                    {
                        ViewBag.ErrorMessage = "Không thể tìm thấy thông tin chủ trọ cho phòng này.";
                        return View();
                    }

                    var request = new
                    {
                        PhongId = roomId,
                        ChuTroId = room.NhaTro.ChuTroId,
                        Loai = "XemPhong",
                        BatDau = new DateTimeOffset(appointmentTime),
                        GhiChu = $"Khách: {hoTen} - SĐT: {sdt}. Ghi chú: {ghiChu}"
                    };

                    var result = await _apiClient.PostAsync<dynamic, object>("/api/datphong", request);
                    if (result != null && result.Success)
                    {
                        return RedirectToAction("BookingSuccess", new { type = "view" });
                    }
                }

                ViewBag.ErrorMessage = "Không thể gửi yêu cầu đặt lịch. Vui lòng thử lại.";
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DatPhong POST Error: {ex.Message}");
                return RedirectToAction("BookingSuccess", new { type = "view" });
            }
        }

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
        public async Task<ActionResult> XacNhanDatPhong(Guid roomId, string hoTen, string sdt, string email, DateTime ngayChuyenVao, string ghiChu)
        {
            try
            {
                var room = await _phongApiService.GetRoomDetailAsync(roomId);
                if (room != null)
                {
                    var request = new
                    {
                        PhongId = roomId,
                        ChuTroId = room.NhaTro.ChuTroId,
                        Loai = "booking",
                        BatDau = ngayChuyenVao,
                        GhiChu = ghiChu
                    };
                    await _apiClient.PostAsync<object, object>("api/DatPhong", request);
                }
                return RedirectToAction("BookingSuccess", new { type = "booking" });
            }
            catch
            {
                return RedirectToAction("BookingSuccess", new { type = "booking" });
            }
        }

        public ActionResult BookingSuccess(string type)
        {
            ViewBag.Type = type;
            return View();
        }

        public ActionResult TinNhan()
        {
            return View();
        }

        public async Task<ActionResult> ThongBao()
        {
            var response = await _apiClient.GetAsync<List<ThongBaoDto>>("api/ThongBao");
            return View(response.Data ?? new List<ThongBaoDto>());
        }

        public async Task<ActionResult> GetNotifications()
        {
            var response = await _apiClient.GetAsync<List<ThongBaoDto>>("api/ThongBao");
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

        [HttpPost]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            var response = await _apiClient.PostAsync<object, object>($"api/ThongBao/mark-as-read/{id}", null);
            return Json(new { success = response.Success });
        }

        public async Task<ActionResult> ClearNotifications()
        {
            await _apiClient.PostAsync<object, object>("api/ThongBao/mark-all-as-read", null);
            Session["NotificationCount"] = 0;
            return Redirect(Request.UrlReferrer?.ToString() ?? "/KhachThue");
        }

        #region CHAT AJAX

        public async Task<ActionResult> GetConversations()
        {
            var response = await _apiClient.GetAsync<List<dynamic>>("api/TinNhan/my-conversations");
            return Json(new { success = response.Success, data = response.Data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetChatMessages(Guid otherUserId)
        {
            var response = await _apiClient.GetAsync<List<dynamic>>($"api/TinNhan/conversation/{otherUserId}");
            // Sau khi lấy tin nhắn, đánh dấu đã đọc luôn
            await _apiClient.PutAsync<object, object>($"api/TinNhan/read/{otherUserId}", null);
            return Json(new { success = response.Success, data = response.Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SendChatMessage(Guid toUserId, string content)
        {
            var request = new { ToUser = toUserId, NoiDung = content };
            var response = await _apiClient.PostAsync<object, object>("api/TinNhan", request);
            return Json(new { success = response.Success, data = response.Data });
        }

        #endregion

        public async Task<ActionResult> LichDaDat()
        {
            try
            {
                var response = await _apiClient.GetAsync<List<DatPhongDto>>("/api/datphong/my-bookings");
                var viewModels = new List<TenantScheduleViewModel>();

                if (response != null && response.Success && response.Data != null)
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
                }

                return View(viewModels);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ KhachThue LichDaDat Error: {ex.Message}");
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

                var bearer = Session["AccessToken"]?.ToString();
                var apiRes = await _contractsApiService.GetActiveContractByTenantAsync(userId, bearer);

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
                var userIdObj = Session["UserId"];
                if (userIdObj == null || !Guid.TryParse(userIdObj.ToString(), out var userId))
                {
                    return View(new List<Models.ViewModels.KhachThue.TenantInvoiceViewModel>());
                }

                var bearer = Session["AccessToken"]?.ToString();
                var apiRes = await _invoicesApiService.GetInvoicesByTenantAsync(userId, bearer);

                if (apiRes == null || !apiRes.Success)
                {
                    ViewBag.ErrorMessage = apiRes?.Message ?? "Không thể tải hóa đơn";
                    return View(new List<Models.ViewModels.KhachThue.TenantInvoiceViewModel>());
                }

                return View(apiRes.Data ?? new List<Models.ViewModels.KhachThue.TenantInvoiceViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi khi tải hóa đơn: " + ex.Message;
                return View(new List<Models.ViewModels.KhachThue.TenantInvoiceViewModel>());
            }
        }
    }
}
