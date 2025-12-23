using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class ChuTroController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly string _apiBaseUrl;

        public ChuTroController()
        {
            _apiClient = new ApiClient();
            // Lấy URL từ config, fallback về localhost
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:7039/";
            
            System.Diagnostics.Debug.WriteLine($"✅ ChuTroController initialized with API: {_apiBaseUrl}");
        }

        // --- MIDDLEWARE KIỂM TRA QUYỀN ---
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            if (session["UserId"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Auth");
                return;
            }

            var role = session["UserRole"];
            string roleStr = role?.ToString();

            if (roleStr != "2" && roleStr?.ToUpper() != "CHUTRO")
            {
                filterContext.Result = RedirectToAction("Index", "Home");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        #region Dashboard

        public async Task<ActionResult> Dashboard()
        {
            try
            {
                var userId = Guid.Parse(Session["UserId"].ToString());
                System.Diagnostics.Debug.WriteLine($"🔵 Dashboard - Loading for UserId: {userId}");

                var model = new LandlordDashboardViewModel();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // Lấy danh sách phòng
                    try
                    {
                        var phongResponse = await client.GetAsync($"api/phong/landlord/{userId}");
                        if (phongResponse.IsSuccessStatusCode)
                        {
                            var json = await phongResponse.Content.ReadAsStringAsync();
                            var phongList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                            model.TongSoPhong = phongList?.Count ?? 0;
                            model.TotalRooms = model.TongSoPhong;
                        }
                    }
                    catch { }

                    // Lấy danh sách đơn đặt
                    try
                    {
                        var donResponse = await client.GetAsync($"api/datphong/landlord-requests?userId={userId}");
                        if (donResponse.IsSuccessStatusCode)
                        {
                            var json = await donResponse.Content.ReadAsStringAsync();
                            var donList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                            
                            if (donList != null)
                            {
                                model.DonChoXacNhan = donList.Count(x => (int)x.trangThaiId == 1);
                                model.DonDaCoc = donList.Count(x => (int)x.trangThaiId == 2);
                                model.HopDongHieuLuc = donList.Count(x => (int)x.trangThaiId == 3);
                                model.UpcomingSchedules = model.DonChoXacNhan;
                                
                                // Lịch xem hôm nay
                                var today = DateTime.Today;
                                foreach (var item in donList.Where(x => (int)x.trangThaiId == 1).Take(5))
                                {
                                    DateTime batDau = (DateTime)item.batDau;
                                    string tenKhach = GetTenKhachHang(item);
                                    
                                    model.TodaySchedules.Add(new TodayScheduleItem
                                    {
                                        DatPhongId = item.datPhongId,
                                        TenantName = tenKhach,
                                        RoomName = item.phong?.tieuDe ?? "Phòng",
                                        ViewTime = batDau.ToString("HH:mm dd/MM"),
                                        Status = "Chờ xác nhận"
                                    });
                                }
                            }
                        }
                    }
                    catch { }
                }

                System.Diagnostics.Debug.WriteLine($"✅ Dashboard loaded: {model.TongSoPhong} phòng, {model.DonChoXacNhan} đơn chờ");
                return View(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Dashboard Error: {ex.Message}");
                ViewBag.Error = "Lỗi tải Dashboard: " + ex.Message;
                return View(new LandlordDashboardViewModel());
            }
        }

        #endregion

        #region Quản Lý Phòng

        public async Task<ActionResult> QuanLyPhong()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 QuanLyPhong - Loading for UserId: {userId}");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.GetAsync($"api/phong/landlord/{userId}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var dataRaw = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                        var model = new List<QuanLyPhongViewModel>();

                        if (dataRaw != null)
                        {
                            foreach (var item in dataRaw)
                            {
                                string anhDaiDien = item.anhDaiDien?.ToString() ?? item.hinhAnhDaiDien?.ToString() ?? "";
                                if (!string.IsNullOrEmpty(anhDaiDien) && !anhDaiDien.StartsWith("http"))
                                {
                                    anhDaiDien = _apiBaseUrl.TrimEnd('/') + anhDaiDien;
                                }

                                model.Add(new QuanLyPhongViewModel
                                {
                                    PhongId = item.phongId ?? Guid.Empty,
                                    NhaTroId = item.nhaTroId ?? Guid.Empty,
                                    TieuDe = item.tieuDe ?? "Phòng trọ",
                                    GiaTien = (long)(item.giaTien ?? 0),
                                    DienTich = (decimal?)(item.dienTich ?? 0),
                                    TienCoc = (long?)(item.tienCoc ?? 0),
                                    SoNguoiToiDa = (int?)(item.soNguoiToiDa ?? 1),
                                    HinhAnhDaiDien = anhDaiDien,
                                    TrangThai = item.trangThai ?? "ConTrong",
                                    IsDuyet = item.isDuyet ?? false,
                                    IsBiKhoa = item.isBiKhoa ?? false
                                });
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ QuanLyPhong loaded: {model.Count} phòng");
                        return View(model);
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"❌ API Error: {response.StatusCode} - {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ QuanLyPhong Error: {ex.Message}");
                ViewBag.Error = "Lỗi tải dữ liệu: " + ex.Message;
            }

            return View(new List<QuanLyPhongViewModel>());
        }

        #endregion

        #region Lịch Hẹn

        public async Task<ActionResult> LichHen()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 LichHen - Loading for UserId: {userId}");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.GetAsync($"api/datphong/landlord-requests?userId={userId}");
                    var jsonCheck = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"📡 API Response: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        var dataRaw = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(jsonCheck);
                        var model = new List<LichHenViewModel>();

                        if (dataRaw != null)
                        {
                            // Chỉ lấy các đơn XemPhong hoặc đang chờ xác nhận
                            foreach (var item in dataRaw.Where(x => (int)x.trangThaiId <= 2))
                            {
                                DateTime ngayBatDau = (DateTime)item.batDau;
                                string tenKhach = GetTenKhachHang(item);
                                string sdt = GetSoDienThoai(item);

                                model.Add(new LichHenViewModel
                                {
                                    LichHenId = item.datPhongId,
                                    PhongId = item.phongId ?? Guid.Empty,
                                    TenKhachHang = tenKhach,
                                    SoDienThoai = sdt,
                                    TenPhong = item.phong?.tieuDe ?? "Phòng đã xóa",
                                    NgayXem = ngayBatDau,
                                    GioXem = ngayBatDau.ToString("HH:mm"),
                                    GhiChu = item.ghiChu ?? "",
                                    TrangThai = MapTrangThaiDatPhong((int)item.trangThaiId),
                                    TrangThaiId = (int)item.trangThaiId
                                });
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ LichHen loaded: {model.Count} lịch hẹn");
                        return View(model);
                    }
                    else
                    {
                        ViewBag.Error = $"Lỗi API: {jsonCheck}";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LichHen Error: {ex.Message}");
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
            }

            return View(new List<LichHenViewModel>());
        }

        public async Task<ActionResult> XuLyLichHen(Guid id, int status)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.PutAsync($"api/datphong/status/{id}?status={status}&userId={userId}", null);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = status == 2 ? "Đã xác nhận lịch hẹn!" : "Đã từ chối lịch hẹn!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Lỗi cập nhật trạng thái.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("LichHen");
        }

        #endregion

        #region Đơn Đặt Phòng

        public async Task<ActionResult> DonDatPhong()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                System.Diagnostics.Debug.WriteLine($"🔵 DonDatPhong - Loading for UserId: {userId}");

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.GetAsync($"api/datphong/landlord-requests?userId={userId}");
                    string json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var dataRaw = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                        var model = new List<DonDatPhongViewModel>();

                        if (dataRaw != null)
                        {
                            foreach (var item in dataRaw)
                            {
                                string tenKhach = GetTenKhachHang(item);

                                model.Add(new DonDatPhongViewModel
                                {
                                    DatPhongId = item.datPhongId,
                                    SoDatPhong = (int)item.soDatPhong,
                                    TenKhachHang = tenKhach,
                                    TenPhong = item.phong?.tieuDe ?? "Phòng đã xóa",
                                    LoaiDatPhong = item.loai ?? "Thang",
                                    NgayBatDau = (DateTime)item.batDau,
                                    NgayKetThuc = item.ketThuc != null ? (DateTime?)item.ketThuc : null,
                                    GiaTien = item.phong != null ? (long)item.phong.giaTien : 0,
                                    TrangThai = MapTrangThaiDatPhong((int)item.trangThaiId),
                                    TrangThaiId = (int)item.trangThaiId,
                                    ThoiGianTao = (DateTime)item.thoiGianTao
                                });
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ DonDatPhong loaded: {model.Count} đơn");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DonDatPhong Error: {ex.Message}");
                ViewBag.Error = "Lỗi: " + ex.Message;
            }

            return View(new List<DonDatPhongViewModel>());
        }

        public async Task<ActionResult> DuyetDon(Guid id, int status)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.PutAsync($"api/datphong/status/{id}?status={status}&userId={userId}", null);

                    if (response.IsSuccessStatusCode)
                    {
                        string msg;
                        if (status == 2)
                            msg = "Đã xác nhận đơn (Đã cọc)!";
                        else if (status == 3)
                            msg = "Đã chuyển thành Hợp đồng!";
                        else if (status == 5)
                            msg = "Đã từ chối đơn!";
                        else
                            msg = "Đã cập nhật trạng thái!";
                        
                        TempData["SuccessMessage"] = msg;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Lỗi cập nhật trạng thái.";
                    }
                }
            }
            catch { }

            return RedirectToAction("DonDatPhong");
        }

        public async Task<ActionResult> XoaDon(Guid id)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.DeleteAsync($"api/datphong/{id}?userId={userId}");

                    if (response.IsSuccessStatusCode)
                        TempData["SuccessMessage"] = "Đã xóa đơn đặt phòng.";
                    else
                        TempData["ErrorMessage"] = "Không thể xóa đơn này.";
                }
            }
            catch { }

            return RedirectToAction("DonDatPhong");
        }

        #endregion

        #region Hợp Đồng

        public async Task<ActionResult> HopDong()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            var model = new LandlordHopDongViewModel { DanhSachHopDong = new List<HopDongItemViewModel>() };

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.GetAsync($"api/datphong/contracts?userId={userId}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var dataRaw = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);

                        if (dataRaw != null)
                        {
                            foreach (var item in dataRaw)
                            {
                                string tenKhach = GetTenKhachHang(item);
                                DateTime batDau = (DateTime)item.batDau;
                                DateTime ketThuc = item.ketThuc != null ? (DateTime)item.ketThuc : batDau.AddMonths(6);

                                int statusId = (int)item.trangThaiId;
                                string trangThaiHienThi = "DangHieuLuc";

                                if (statusId == 2)
                                    trangThaiHienThi = "DaCoc";
                                else if (statusId == 4 || DateTime.Now > ketThuc)
                                    trangThaiHienThi = "DaHetHan";
                                else if (statusId == 3 && (ketThuc - DateTime.Now).TotalDays <= 30)
                                    trangThaiHienThi = "SapHetHan";
                                else
                                    trangThaiHienThi = "DangHieuLuc";

                                model.DanhSachHopDong.Add(new HopDongItemViewModel
                                {
                                    HopDongId = item.datPhongId,
                                    SoHopDong = "HD-" + item.soDatPhong,
                                    TenPhong = item.phong?.tieuDe ?? "Phòng đã xóa",
                                    TenNguoiThue = tenKhach,
                                    NgayBatDau = batDau,
                                    NgayKetThuc = ketThuc,
                                    GiaThue = item.phong != null ? (decimal)item.phong.giaTien : 0,
                                    TienCoc = item.phong != null ? (decimal)(item.phong.tienCoc ?? 0) : 0,
                                    TrangThai = trangThaiHienThi
                                });
                            }
                        }

                        model.TongHopDongHieuLuc = model.DanhSachHopDong.Count(x => 
                            x.TrangThai == "DangHieuLuc" || x.TrangThai == "SapHetHan" || x.TrangThai == "DaCoc");
                        model.HopDongSapHetHan = model.DanhSachHopDong.Count(x => x.TrangThai == "SapHetHan");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ HopDong Error: {ex.Message}");
            }

            return View(model);
        }

        public ActionResult ChiTietHopDong(Guid id)
        {
            var model = new ChiTietHopDongViewModel
            {
                HopDongId = id,
                SoHopDong = "HD-2025-001",
                TrangThai = "DangHieuLuc",
                NgayLap = DateTime.Now.AddMonths(-1),
                TenNguoiThue = "Nguyễn Văn A",
                SoDienThoai = "0987 654 321",
                CCCD = "012345678901",
                QueQuan = "Hà Nội",
                TenPhong = "Phòng 101 - Studio",
                DienTich = 25.5,
                GiaThue = 3500000,
                TienCoc = 3500000,
                NgayBatDau = DateTime.Now.AddMonths(-1),
                NgayKetThuc = DateTime.Now.AddMonths(11),
                KyThanhToan = 1,
                GhiChu = "Khách thuê cam kết ở ít nhất 6 tháng."
            };

            return View(model);
        }

        public ActionResult TaoHopDong()
        {
            return View();
        }

        #endregion

        #region Hóa Đơn

        public ActionResult HoaDon()
        {
            // TODO: Implement khi có API
            return View(new List<LandlordHoaDonViewModel>());
        }

        #endregion

        #region Tạo/Sửa Phòng

        public async Task<ActionResult> TaoPhong()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            var model = new TaoPhongViewModel();
            await LoadDropdownNhaTro(userId, model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaoPhong(TaoPhongViewModel model)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            if (!ModelState.IsValid)
            {
                await LoadDropdownNhaTro(userId, model);
                return View(model);
            }

            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(model.NhaTroId.ToString()), "NhaTroId");
                    content.Add(new StringContent(model.TieuDe ?? ""), "TieuDe");
                    content.Add(new StringContent(model.GiaTien.ToString()), "GiaTien");
                    content.Add(new StringContent(model.DienTich?.ToString() ?? "0"), "DienTich");
                    content.Add(new StringContent(model.TienCoc?.ToString() ?? "0"), "TienCoc");
                    content.Add(new StringContent(model.SoNguoiToiDa?.ToString() ?? "1"), "SoNguoiToiDa");

                    if (model.HinhAnhUpload != null && model.HinhAnhUpload.ContentLength > 0)
                    {
                        var fileContent = new StreamContent(model.HinhAnhUpload.InputStream);
                        content.Add(fileContent, "HinhAnhUpload", model.HinhAnhUpload.FileName);
                    }

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_apiBaseUrl);
                        var response = await client.PostAsync($"api/phong?userId={userId}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Thêm phòng thành công!";
                            return RedirectToAction("QuanLyPhong");
                        }
                        else
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            ViewBag.Error = $"Lỗi từ API: {error}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
            }

            await LoadDropdownNhaTro(userId, model);
            return View(model);
        }

        public async Task<ActionResult> SuaPhong(Guid id)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            var model = new CapNhatPhongViewModel { PhongId = id };

            try
            {
                await LoadDropdownNhaTro(userId, model);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.GetAsync($"api/phong/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var p = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                        model.TieuDe = p.tieuDe;
                        model.GiaTien = (long)p.giaTien;
                        model.DienTich = (decimal?)p.dienTich;
                        model.TienCoc = (long?)p.tienCoc;
                        model.SoNguoiToiDa = (int?)p.soNguoiToiDa;
                        model.NhaTroId = (Guid)p.nhaTroId;

                        string anhApi = (string)p.anhDaiDien;
                        if (!string.IsNullOrEmpty(anhApi))
                        {
                            model.AnhHienTai = anhApi.StartsWith("http") ? anhApi : _apiBaseUrl.TrimEnd('/') + anhApi;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải dữ liệu: " + ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SuaPhong(CapNhatPhongViewModel model)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            if (!ModelState.IsValid)
            {
                await LoadDropdownNhaTro(userId, model);
                return View(model);
            }

            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(model.NhaTroId.ToString()), "NhaTroId");
                    content.Add(new StringContent(model.TieuDe ?? ""), "TieuDe");
                    content.Add(new StringContent(model.GiaTien.ToString()), "GiaTien");
                    content.Add(new StringContent(model.DienTich?.ToString() ?? "0"), "DienTich");
                    content.Add(new StringContent(model.TienCoc?.ToString() ?? "0"), "TienCoc");
                    content.Add(new StringContent(model.SoNguoiToiDa?.ToString() ?? "1"), "SoNguoiToiDa");

                    if (model.HinhAnhUpload != null && model.HinhAnhUpload.ContentLength > 0)
                    {
                        var fileContent = new StreamContent(model.HinhAnhUpload.InputStream);
                        content.Add(fileContent, "HinhAnhUpload", model.HinhAnhUpload.FileName);
                    }

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_apiBaseUrl);
                        var response = await client.PutAsync($"api/phong/{model.PhongId}?userId={userId}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Cập nhật phòng thành công!";
                            return RedirectToAction("QuanLyPhong");
                        }
                        else
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            ViewBag.Error = "Lỗi API: " + error;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
            }

            await LoadDropdownNhaTro(userId, model);
            return View(model);
        }

        public async Task<ActionResult> XoaPhong(Guid id)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.DeleteAsync($"api/phong/{id}?userId={userId}");

                    if (response.IsSuccessStatusCode)
                        TempData["SuccessMessage"] = "Đã xóa phòng thành công!";
                    else
                        TempData["ErrorMessage"] = "Lỗi xóa phòng: " + await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("QuanLyPhong");
        }

        #endregion

        #region Các Chức Năng Khác

        public ActionResult TinNhan()
        {
            var model = new ChatViewModel
            {
                Conversations = new List<ConversationItem>
                {
                    new ConversationItem { UserName = "Nguyễn Văn A", LastMessage = "Phòng này còn trống không ạ?", Time = DateTime.Now, UnreadCount = 2, IsActive = true },
                    new ConversationItem { UserName = "Trần Thị B", LastMessage = "Cảm ơn chủ trọ nhiều nhé!", Time = DateTime.Now.AddHours(-2), UnreadCount = 0 }
                },
                CurrentMessages = new List<MessageItem>
                {
                    new MessageItem { Content = "Chào bạn, mình muốn hỏi về phòng 101", Time = DateTime.Now.AddMinutes(-30), IsFromMe = false },
                    new MessageItem { Content = "Chào bạn, phòng 101 vẫn còn trống bạn nhé.", Time = DateTime.Now.AddMinutes(-25), IsFromMe = true },
                    new MessageItem { Content = "Phòng này còn trống không ạ?", Time = DateTime.Now.AddMinutes(-10), IsFromMe = false }
                },
                SelectedUserName = "Nguyễn Văn A"
            };
            return View(model);
        }

        public ActionResult YeuCauHoTro()
        {
            var model = new List<YeuCauHoTroViewModel>
            {
                new YeuCauHoTroViewModel {
                    HoTroId = Guid.NewGuid(),
                    PhongId = Guid.NewGuid(),
                    TenPhong = "Phòng 201",
                    NguoiYeuCau = Guid.NewGuid(),
                    TenNguoiYeuCau = "Lê Văn C",
                    LoaiHoTroId = 1,
                    TenLoaiHoTro = "Sửa chữa",
                    TieuDe = "Hỏng khóa cửa",
                    MoTa = "Khóa cửa chính bị kẹt không mở được từ bên ngoài.",
                    TrangThai = "Moi",
                    ThoiGianTao = DateTime.Now
                }
            };

            return View(model);
        }

        public ActionResult ThongKe()
        {
            var model = new LandlordStatisticsViewModel
            {
                TongDoanhThuNam = 150000000,
                TiLeLapDay = 85.5,
                TongSoHopDong = 12,
                SoPhongTrong = 3,
                SoPhongDaThue = 15,
                SoPhongDangSua = 2,
                NhanThang = new List<string> { "Thg 8", "Thg 9", "Thg 10", "Thg 11", "Thg 12" },
                DoanhThuTheoThang = new List<decimal> { 18000000, 22000000, 25000000, 23000000, 30000000 }
            };

            return View(model);
        }

        public ActionResult DanhGia()
        {
            var model = new ThongKeDanhGiaViewModel
            {
                DiemTrungBinh = 4.5,
                TongLuotDanhGia = 2,
                DanhSachDanhGia = new List<DanhGiaViewModel>
                {
                    new DanhGiaViewModel {
                        DanhGiaId = Guid.NewGuid(),
                        TenNguoiDanhGia = "Nguyễn Văn A",
                        TenPhong = "Phòng 102",
                        Diem = 5,
                        NoiDung = "Phòng sạch sẽ, rất hài lòng.",
                        ThoiGian = DateTime.Now.AddDays(-1)
                    },
                    new DanhGiaViewModel {
                        DanhGiaId = Guid.NewGuid(),
                        TenNguoiDanhGia = "Trần Thị B",
                        TenPhong = "Phòng 305",
                        Diem = 4,
                        NoiDung = "Chủ nhà nhiệt tình, phòng hơi cũ tí.",
                        ThoiGian = DateTime.Now.AddDays(-3)
                    }
                }
            };
            return View(model);
        }

        public ActionResult ThongTinCaNhan()
        {
            return View(new LandlordProfileViewModel());
        }

        #endregion

        #region Helper Methods

        private async Task LoadDropdownNhaTro(Guid userId, TaoPhongViewModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.GetAsync($"api/nhatro/dropdown/{userId}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var listNhaTro = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                        
                        if (listNhaTro != null)
                        {
                            model.DanhSachNhaTro = new List<SelectListItem>();
                            foreach (var item in listNhaTro)
                            {
                                string id = (item.nhaTroId ?? item.NhaTroId)?.ToString();
                                string ten = (item.tieuDe ?? item.TieuDe)?.ToString();
                                if (!string.IsNullOrEmpty(id))
                                {
                                    model.DanhSachNhaTro.Add(new SelectListItem { Value = id, Text = ten });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LoadDropdownNhaTro Error: {ex.Message}");
            }
        }

        private async Task LoadDropdownNhaTro(Guid userId, CapNhatPhongViewModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseUrl);
                    var response = await client.GetAsync($"api/nhatro/dropdown/{userId}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var listNhaTro = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                        
                        if (listNhaTro != null)
                        {
                            model.DanhSachNhaTro = new List<SelectListItem>();
                            foreach (var item in listNhaTro)
                            {
                                string id = (item.nhaTroId ?? item.NhaTroId)?.ToString();
                                string ten = (item.tieuDe ?? item.TieuDe)?.ToString();
                                if (!string.IsNullOrEmpty(id))
                                {
                                    model.DanhSachNhaTro.Add(new SelectListItem { Value = id, Text = ten });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ LoadDropdownNhaTro Error: {ex.Message}");
            }
        }

        private string GetTenKhachHang(dynamic item)
        {
            string tenKhach = "Khách vãng lai";

            if (item.nguoiThue != null)
            {
                if (item.nguoiThue.hoSoNguoiDung != null && item.nguoiThue.hoSoNguoiDung.hoTen != null)
                    tenKhach = (string)item.nguoiThue.hoSoNguoiDung.hoTen;
                else if (item.nguoiThue.hoTen != null)
                    tenKhach = (string)item.nguoiThue.hoTen;
                else if (item.nguoiThue.email != null)
                    tenKhach = (string)item.nguoiThue.email;
            }

            return tenKhach;
        }

        private string GetSoDienThoai(dynamic item)
        {
            string sdt = "---";
            
            if (item.nguoiThue != null)
            {
                if (item.nguoiThue.dienThoai != null)
                    sdt = (string)item.nguoiThue.dienThoai;
                else if (item.nguoiThue.hoSoNguoiDung != null && item.nguoiThue.hoSoNguoiDung.dienThoai != null)
                    sdt = (string)item.nguoiThue.hoSoNguoiDung.dienThoai;
            }

            return sdt;
        }

        private string MapTrangThaiDatPhong(int id)
        {
            switch (id)
            {
                case 1: return "ChoXacNhan";
                case 2: return "DaXacNhan";
                case 3: return "DaThanhToan";
                case 4: return "HoanThanh";
                case 5: return "DaHuy";
                default: return "Khac";
            }
        }

        #endregion
    }
}
