using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Models.ViewModels.Home;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiClient _apiClient;

        public HomeController()
        {
            _apiClient = new ApiClient();
        }

        // --- TRANG CHỦ: Chỉ giới thiệu (Static) ---
        public ActionResult Index()
        {
            // Có thể truyền banner text nếu muốn
            ViewBag.BannerTitle = "Chào mừng đến với Hệ thống Quản lý Phòng trọ";
            return View();
        }

        // --- TRANG DANH SÁCH: Chứa Tabs và Grid Phòng ---
        public async Task<ActionResult> DanhSachPhong(Guid? nhaTroId, int page = 1)
        {
            try
            {
                // 1. URL API lấy danh sách phòng (Filter theo nhaTroId nếu có)
                string urlPhong = $"api/phong?page={page}&pageSize=12";
                if (nhaTroId.HasValue)
                {
                    urlPhong += $"&nhaTroId={nhaTroId}";
                }

                // 2. URL API lấy danh sách nhà trọ (Để vẽ Tabs)
                // Lưu ý: Đảm bảo Backend có API này (GET api/nhatro)
                string urlNhaTro = "api/nhatro";

                // 3. Gọi song song
                var taskPhong = _apiClient.GetAsync<PagedResultDto<PhongDto>>(urlPhong);
                var taskNhaTro = _apiClient.GetAsync<List<NhaTroDto>>(urlNhaTro);

                await Task.WhenAll(taskPhong, taskNhaTro);

                var resultPhong = taskPhong.Result;
                var resultNhaTro = taskNhaTro.Result ?? new List<NhaTroDto>();

                // 4. Map dữ liệu Phòng sang ViewModel
                var rawList = resultPhong?.Data ?? new List<PhongDto>();
                var phongVms = rawList.Select(p => new PhongTroListItemViewModel
                {
                    PhongId = p.PhongId,
                    TieuDe = p.TieuDe,
                    TenNhaTro = !string.IsNullOrEmpty(p.TenNhaTro)
                ? p.TenNhaTro
                : (p.NhaTro != null ? p.NhaTro.TieuDe : "Đang cập nhật"),

                    DiaChi = !string.IsNullOrEmpty(p.DiaChi)
             ? p.DiaChi
             : (p.NhaTro != null ? p.NhaTro.DiaChi : "Đang cập nhật địa chỉ"),
                    DienTich = (double)p.DienTich,
                    GiaTien = p.GiaTien,
                    AnhDaiDien = !string.IsNullOrEmpty(p.AnhDaiDien)
                        ? (p.AnhDaiDien.StartsWith("http") ? p.AnhDaiDien : "http://localhost:5101" + p.AnhDaiDien)
                        : "/images/default-room.jpg",
                    DiemTrungBinh = 4.5,
                    SoLuongDanhGia = 10,
                    TienIchNganGon = new string[] { "Wifi", "WC Riêng" }
                }).ToList();

                // 5. Đóng gói vào ViewModel mới
                var model = new PhongSearchViewModel
                {
                    DanhSachNhaTro = resultNhaTro,
                    DanhSachPhong = phongVms,
                    SelectedNhaTroId = nhaTroId,
                    CurrentPage = resultPhong?.Page ?? 1,
                    TotalPages = resultPhong?.TotalPages ?? 0
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi kết nối: " + ex.Message;
                // Trả về view rỗng để không crash web
                return View(new PhongSearchViewModel
                {
                    DanhSachPhong = new List<PhongTroListItemViewModel>(),
                    DanhSachNhaTro = new List<NhaTroDto>()
                });
            }
        }

        public ActionResult About() => View();
        public ActionResult Contact() => View();
    }
}