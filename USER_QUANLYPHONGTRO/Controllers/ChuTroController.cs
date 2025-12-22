using System;
using System.Collections.Generic;
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

        public ChuTroController()
        {
            _apiClient = new ApiClient();
        }

        // --- MIDDLEWARE KIỂM TRA QUYỀN (QUAN TRỌNG) ---
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            // 1. Kiểm tra đăng nhập
            if (session["UserId"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Auth");
                return;
            }

            // 2. Kiểm tra Role
            // LƯU Ý QUAN TRỌNG: Session["UserRole"] đang lưu số nguyên (int) là 2
            // Nếu so sánh với chuỗi "CHUTRO" hoặc "ChuTro" sẽ bị SAI -> Dẫn đến bị đá về Home

            var role = session["UserRole"];
            string roleStr = role?.ToString(); // Chuyển sang chuỗi để dễ so sánh: "2"

            // Kiểm tra: Nếu KHÔNG PHẢI là "2" VÀ KHÔNG PHẢI "CHUTRO" thì mới đá ra
            if (roleStr != "2" && roleStr?.ToUpper() != "CHUTRO")
            {
                // Đây chính là nguyên nhân bạn bị đá về Home
                filterContext.Result = RedirectToAction("Index", "Home");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        // GET: Dashboard
        public async Task<ActionResult> Dashboard()
        {
            string token = Session["AccessToken"] as string;

            try
            {
                // Gọi API lấy thống kê (nếu chưa có API thì dùng dữ liệu mẫu)
                // var stats = await _apiClient.GetAsync<LandlordDashboardViewModel>("api/dashboard/stats", token);

                // --- DỮ LIỆU MẪU ĐỂ TEST GIAO DIỆN ---
                var model = new LandlordDashboardViewModel
                {
                    TongSoPhong = 15,
                    
                    DoanhThuThang = 25000000,
                };
                // -------------------------------------

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải Dashboard: " + ex.Message;
                return View(new LandlordDashboardViewModel());
            }
        }


        // ============================================================
        // 2. QUẢN LÝ PHÒNG (DANH SÁCH + CHỜ DUYỆT + NÚT THÊM)
        // ============================================================
        public async Task<ActionResult> QuanLyPhong()
        {
            // 1. Kiểm tra đăng nhập
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");

            // Parse an toàn hơn
            if (!Guid.TryParse(Session["UserId"].ToString(), out Guid userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // 2. Gọi API và Deserialize thẳng sang ViewModel
                // Cách này tự động khớp "phongId" (API) vào "PhongId" (C#) -> Không bị lỗi null
                var listPhong = await _apiClient.GetAsync<List<QuanLyPhongViewModel>>($"api/phong/landlord/{userId}");

                // Nếu API trả về null hoặc rỗng thì khởi tạo list mới
                return View(listPhong ?? new List<QuanLyPhongViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi tải dữ liệu: " + ex.Message;
                return View(new List<QuanLyPhongViewModel>());
            }
        }

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
        public async Task<ActionResult> DonDatPhong()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5101/");

                    // Gọi API lấy danh sách (truyền userId trực tiếp)
                    var response = await client.GetAsync($"api/datphong/landlord-requests?userId={userId}");
                    string json = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var dataRaw = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                        var model = new List<DonDatPhongViewModel>();

                        foreach (var item in dataRaw)
                        {
                            // 1. Xử lý Tên Khách Hàng (Logic giống Lịch Hẹn)
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

                            // 2. Map dữ liệu
                            model.Add(new DonDatPhongViewModel
                            {
                                DatPhongId = item.datPhongId,
                                SoDatPhong = (int)item.soDatPhong,
                                TenKhachHang = tenKhach,
                                TenPhong = (item.phong != null) ? item.phong.tieuDe : "Phòng đã xóa",
                                LoaiDatPhong = item.loai, // Ngay/Thang
                                NgayBatDau = (DateTime)item.batDau,
                                NgayKetThuc = item.ketThuc != null ? (DateTime)item.ketThuc : (DateTime?)null,
                                GiaTien = (item.phong != null) ? (long)item.phong.giaTien : 0,
                                TrangThai = MapTrangThaiDatPhong((int)item.trangThaiId),
                                ThoiGianTao = (DateTime)item.thoiGianTao
                            });
                        }
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
            }
            return View(new List<DonDatPhongViewModel>());
        }

        // Action: Duyệt đơn (Sửa trạng thái)
        public async Task<ActionResult> DuyetDon(Guid id, int status)
        {
            // status: 2 = Duyệt, 3 = Hủy/Từ chối
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5101/");
                    var response = await client.PutAsync($"api/datphong/status/{id}?status={status}&userId={userId}", null);

                    if (response.IsSuccessStatusCode)
                        TempData["SuccessMessage"] = (status == 2) ? "Đã duyệt đơn thành công!" : "Đã từ chối đơn!";
                    else
                        TempData["ErrorMessage"] = "Lỗi cập nhật trạng thái.";
                }
            }
            catch { }
            return RedirectToAction("DonDatPhong");
        }

        // Action: Xóa đơn vĩnh viễn
        public async Task<ActionResult> XoaDon(Guid id)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5101/");
                    var response = await client.DeleteAsync($"api/datphong/{id}?userId={userId}");

                    if (response.IsSuccessStatusCode) TempData["SuccessMessage"] = "Đã xóa đơn đặt phòng.";
                    else TempData["ErrorMessage"] = "Không thể xóa đơn này.";
                }
            }
            catch { }
            return RedirectToAction("DonDatPhong");
        }
        // GET: ChuTro/YeuCauHoTro
        public ActionResult YeuCauHoTro()
        {
            // Giả lập dữ liệu dựa trên các trường SQL mới
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
        // GET: Hiện form tạo phòng
        public async Task<ActionResult> TaoPhong()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            var model = new TaoPhongViewModel();

            // 1. Load danh sách Khu trọ vào Dropdown
            try
            {
                // Gọi API lấy danh sách nhà trọ (Cần đảm bảo API này tồn tại)
                var listNhaTro = await _apiClient.GetAsync<List<dynamic>>($"api/nhatro/dropdown/{userId}");
                if (listNhaTro != null)
                {
                    foreach (var item in listNhaTro)
                    {
                        // Dynamic mapping
                        string id = item.nhaTroId ?? item.NhaTroId;
                        string ten = item.tieuDe ?? item.TieuDe;
                        model.DanhSachNhaTro.Add(new SelectListItem { Value = id, Text = ten });
                    }
                }
            }
            catch
            {
                // Không làm gì, dropdown sẽ trống
            }

            return View(model);
        }

        // POST: Tạo phòng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaoPhong(TaoPhongViewModel model)
        {
            // --- DEBUG 1: Xác nhận request đã vào Controller ---
            System.Diagnostics.Debug.WriteLine("--------------------------------------------------");
            System.Diagnostics.Debug.WriteLine($"---> [DEBUG] Bắt đầu xử lý Tạo Phòng cho User: {Session["UserId"]}");

            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            // --- DEBUG 2: Kiểm tra lỗi Validation ---
            if (!ModelState.IsValid)
            {
                // In chi tiết lỗi ra cửa sổ Output
                var errors = string.Join("; ", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));
                System.Diagnostics.Debug.WriteLine($"---> [DEBUG] Lỗi Validation (ModelState): {errors}");

                // QUAN TRỌNG: Load lại Dropdown trước khi trả về View (để không bị mất danh sách)
                await LoadDropdownNhaTro(userId, model);

                // Hiển thị lỗi lên màn hình để bạn thấy
                ViewBag.Error = "Dữ liệu nhập vào chưa đúng: " + errors;
                return View(model);
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("---> [DEBUG] Dữ liệu hợp lệ, bắt đầu gọi API...");

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
                        var fileStream = model.HinhAnhUpload.InputStream;
                        var fileContent = new StreamContent(fileStream);
                        content.Add(fileContent, "HinhAnhUpload", model.HinhAnhUpload.FileName);
                    }

                    using (var client = new HttpClient())
                    {
                        // ⚠️ LƯU Ý: KIỂM TRA LẠI CỔNG API CỦA BẠN (Ví dụ: 7123 hoặc 5101)
                        // Bạn có thể xem cổng này khi chạy project API lên (trên thanh địa chỉ trình duyệt)
                        client.BaseAddress = new Uri("http://localhost:5101/");

                        System.Diagnostics.Debug.WriteLine($"---> [DEBUG] Đang gửi đến API: {client.BaseAddress}api/phong?userId={userId}");

                        var response = await client.PostAsync($"api/phong?userId={userId}", content);
                        string responseContent = await response.Content.ReadAsStringAsync();

                        System.Diagnostics.Debug.WriteLine($"---> [DEBUG] Kết quả API: {response.StatusCode} - {responseContent}");

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Thêm phòng thành công!";
                            return RedirectToAction("QuanLyPhong");
                        }
                        else
                        {
                            ViewBag.Error = $"Lỗi từ API ({response.StatusCode}): {responseContent}";
                            await LoadDropdownNhaTro(userId, model); // Load lại dropdown nếu lỗi API
                            return View(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"---> [DEBUG] Lỗi Exception: {ex.Message}");
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
                await LoadDropdownNhaTro(userId, model); // Load lại dropdown nếu lỗi Exception
            }

            return View(model);
        }

        // Hàm phụ trợ để load lại Dropdown (Tránh lặp code)
        private async Task LoadDropdownNhaTro(Guid userId, TaoPhongViewModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Sửa lại cổng cho đúng với API của bạn
                    client.BaseAddress = new Uri("http://localhost:5101/");
                    var response = await client.GetAsync($"api/nhatro/dropdown/{userId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var listNhaTro = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
                        if (listNhaTro != null)
                        {
                            model.DanhSachNhaTro = new List<SelectListItem>(); // Reset list
                            foreach (var item in listNhaTro)
                            {
                                string id = item.nhaTroId ?? item.NhaTroId;
                                string ten = item.tieuDe ?? item.TieuDe;
                                model.DanhSachNhaTro.Add(new SelectListItem { Value = id, Text = ten });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"---> [DEBUG] Lỗi Load Dropdown: {ex.Message}");
            }
        }

        public async Task<ActionResult> SuaPhong(Guid id)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            var model = new CapNhatPhongViewModel();
            model.PhongId = id;

            try
            {
                // 1. Load Dropdown Khu trọ (tái sử dụng hàm cũ)
                await LoadDropdownNhaTro(userId, model);

                // 2. Lấy chi tiết phòng hiện tại từ API
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5101/"); // Cổng API
                    var response = await client.GetAsync($"api/phong/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        // Deserialize dynamic cho nhanh
                        var p = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                        // Map dữ liệu vào ViewModel
                        model.TieuDe = p.tieuDe;
                        model.GiaTien = (long)p.giaTien;
                        model.DienTich = (decimal?)p.dienTich;
                        model.TienCoc = (long?)p.tienCoc;
                        model.SoNguoiToiDa = (int?)p.soNguoiToiDa;
                        model.NhaTroId = (Guid)p.nhaTroId;

                        // Xử lý hiển thị ảnh cũ
                        string anhApi = (string)p.anhDaiDien;
                        if (!string.IsNullOrEmpty(anhApi))
                        {
                            // Nếu ảnh chưa có domain thì thêm vào
                            model.AnhHienTai = anhApi.StartsWith("http") ? anhApi : "http://localhost:5101" + anhApi;
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

        // POST: Thực hiện sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SuaPhong(CapNhatPhongViewModel model)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            // Nếu ModelState lỗi, load lại dropdown và trả về view
            if (!ModelState.IsValid)
            {
                await LoadDropdownNhaTro(userId, model);
                return View(model);
            }

            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Add các trường dữ liệu
                    content.Add(new StringContent(model.NhaTroId.ToString()), "NhaTroId");
                    content.Add(new StringContent(model.TieuDe ?? ""), "TieuDe");
                    content.Add(new StringContent(model.GiaTien.ToString()), "GiaTien");
                    content.Add(new StringContent(model.DienTich?.ToString() ?? "0"), "DienTich");
                    content.Add(new StringContent(model.TienCoc?.ToString() ?? "0"), "TienCoc");
                    content.Add(new StringContent(model.SoNguoiToiDa?.ToString() ?? "1"), "SoNguoiToiDa");

                    // Nếu user chọn ảnh mới thì gửi, không thì thôi
                    if (model.HinhAnhUpload != null && model.HinhAnhUpload.ContentLength > 0)
                    {
                        var fileContent = new StreamContent(model.HinhAnhUpload.InputStream);
                        content.Add(fileContent, "HinhAnhUpload", model.HinhAnhUpload.FileName);
                    }

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:5101/");
                        // Gọi PUT thay vì POST
                        var response = await client.PutAsync($"api/phong/{model.PhongId}?userId={userId}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Cập nhật phòng thành công!";
                            return RedirectToAction("QuanLyPhong");
                        }
                        else
                        {
                            string err = await response.Content.ReadAsStringAsync();
                            ViewBag.Error = "Lỗi API: " + err;
                            await LoadDropdownNhaTro(userId, model);
                            return View(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
                await LoadDropdownNhaTro(userId, model);
                return View(model);
            }
        }

        // ============================================================
        // 3. THỐNG KÊ CHI TIẾT
        // ============================================================
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

        public async Task<ActionResult> XoaPhong(Guid id)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    // Sửa lại cổng API cho đúng (http://localhost:5101/)
                    client.BaseAddress = new Uri("http://localhost:5101/");

                    // Gọi API Delete
                    var response = await client.DeleteAsync($"api/phong/{id}?userId={userId}");

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Đã xóa phòng thành công!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Lỗi xóa phòng: " + await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
            }

            // Quay lại trang danh sách
            return RedirectToAction("QuanLyPhong");
        }

        // ============================================================
        // 4. CÁC CHỨC NĂNG KHÁC (HỢP ĐỒNG, LỊCH HẸN, HÓA ĐƠN)
        // ============================================================
        public async Task<ActionResult> LichHen()
        {
            // MVC tự kiểm tra Session (Đây là lớp bảo mật duy nhất)
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5101/");

                    // --- BỎ ĐOẠN NÀY ---
                    // client.DefaultRequestHeaders.Authorization = ...
                    // -------------------

                    // --- SỬA ĐOẠN NÀY: Truyền userId vào URL ---
                    var response = await client.GetAsync($"api/datphong/landlord-requests?userId={userId}");

                    // Log kết quả để kiểm tra
                    string jsonCheck = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"---> [API] Result: {jsonCheck}");

                    if (response.IsSuccessStatusCode)
                    {
                        var dataRaw = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(jsonCheck);
                        var model = new List<LichHenViewModel>();

                        foreach (var item in dataRaw)
                        {
                            DateTime ngayBatDau = (DateTime)item.batDau;

                            string tenKhach = "Khách vãng lai";

                            if (item.nguoiThue != null)
                            {
                                // 1. Thử lấy tên trong HoSoNguoiDung
                                if (item.nguoiThue.hoSoNguoiDung != null && item.nguoiThue.hoSoNguoiDung.hoTen != null)
                                {
                                    tenKhach = (string)item.nguoiThue.hoSoNguoiDung.hoTen;
                                }
                                // 2. Nếu không có hồ sơ, thử lấy tên đăng nhập/Email (nếu bảng User có trường HoTen)
                                else if (item.nguoiThue.hoTen != null)
                                {
                                    tenKhach = (string)item.nguoiThue.hoTen;
                                }
                                else if (item.nguoiThue.email != null)
                                {
                                    tenKhach = (string)item.nguoiThue.email;
                                }
                            }
                            string sdt = "---";
                            if (item.nguoiThue != null && item.nguoiThue.dienThoai != null)
                            {
                                sdt = (string)item.nguoiThue.dienThoai;
                            }
                            model.Add(new LichHenViewModel
                            {
                                LichHenId = item.datPhongId,
                                // Null Check an toàn
                                TenKhachHang = tenKhach,
                                SoDienThoai = sdt,
                                TenPhong = (item.phong != null && item.phong.tieuDe != null) ? item.phong.tieuDe : "Phòng đã xóa",
                                NgayXem = ngayBatDau,
                                GioXem = ngayBatDau.ToString("HH:mm"),
                                GhiChu = item.ghiChu,
                                TrangThai = MapTrangThaiDatPhong((int)item.trangThaiId)
                            });
                        }

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
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
            }

            return View(new List<LichHenViewModel>());
        }

        // Action Xử lý (Sửa lại URL gọi API)
        public async Task<ActionResult> XuLyLichHen(Guid id, int status)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");
            var userId = Guid.Parse(Session["UserId"].ToString());

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5101/");

                    // Gọi API Update, truyền thêm userId để Backend check quyền sở hữu
                    var response = await client.PutAsync($"api/datphong/status/{id}?status={status}&userId={userId}", null);
                }
            }
            catch { }
            return RedirectToAction("LichHen");
        }

        // Hàm Map trạng thái số sang chữ để hiển thị (Dùng chung cho DatPhong)
        private string MapTrangThaiDatPhong(int id)
        {
            switch (id)
            {
                case 1: return "ChoXacNhan";
                case 2: return "DaXacNhan"; // Đã duyệt
                case 3: return "DaHuy";     // Đã hủy/Từ chối
                default: return "Khac";
            }
        }



        public ActionResult ChiTietHopDong(Guid id)
        {
            // Giả lập lấy dữ liệu chi tiết từ Database theo ID
            var model = new ChiTietHopDongViewModel
            {
                HopDongId = id,
                SoHopDong = "HD-2025-001",
                TrangThai = "DangHieuLuc",
                NgayLap = new DateTime(2025, 1, 1),
                TenNguoiThue = "Nguyễn Văn A",
                SoDienThoai = "0987 654 321",
                CCCD = "012345678901",
                QueQuan = "Hà Nội",
                TenPhong = "Phòng 101 - Studio",
                DienTich = 25.5,
                GiaThue = 3500000,
                TienCoc = 3500000,
                NgayBatDau = new DateTime(2025, 1, 1),
                NgayKetThuc = new DateTime(2025, 12, 31),
                KyThanhToan = 1,
                GhiChu = "Khách thuê cam kết ở ít nhất 6 tháng."
            };

            return View(model);
        }
        // GET: ChuTro/TaoHopDong
        public ActionResult TaoHopDong()
        {
            // Bạn có thể lấy danh sách phòng trống và khách hàng từ DB để đổ vào DropdownList tại đây
            return View();
        }
        public ActionResult DanhGia()
        {
            // Giả lập dữ liệu khớp với cấu trúc SQL
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
        public ActionResult HopDong()
        {
            var model = new LandlordHopDongViewModel
            {
                TongHopDongHieuLuc = 2,
                HopDongSapHetHan = 1,
                DanhSachHopDong = new List<HopDongItemViewModel>
        {
            new HopDongItemViewModel {
                HopDongId = Guid.NewGuid(),
                SoHopDong = "HD-2025-001",
                TenPhong = "Phòng 101",
                TenNguoiThue = "Nguyễn Văn A",
                NgayBatDau = new DateTime(2025, 1, 1),
                NgayKetThuc = new DateTime(2025, 12, 31),
                GiaThue = 3500000,
                TienCoc = 3500000,
                TrangThai = "DangHieuLuc"
            },
            new HopDongItemViewModel {
                HopDongId = Guid.NewGuid(),
                SoHopDong = "HD-2024-015",
                TenPhong = "Phòng 202",
                TenNguoiThue = "Trần Thị B",
                NgayBatDau = new DateTime(2024, 6, 15),
                NgayKetThuc = new DateTime(2025, 2, 15), // Sắp hết hạn
                GiaThue = 4000000,
                TienCoc = 4000000,
                TrangThai = "SapHetHan"
            }
        }
            };
            return View(model);
        }
        public ActionResult HoaDon()
        {
            return View(new List<LandlordHoaDonViewModel>());
        }

        public ActionResult ThongTinCaNhan()
        {
            return View(new LandlordProfileViewModel());
        }



    }
}