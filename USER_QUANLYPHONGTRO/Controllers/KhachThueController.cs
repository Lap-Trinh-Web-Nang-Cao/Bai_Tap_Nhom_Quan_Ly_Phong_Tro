using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.Dtos.Bookings;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue;
using USER_QUANLYPHONGTRO.Services;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class KhachThueController : Controller
    {
        private readonly ApiClient _apiClient;

        public KhachThueController()
        {
            _apiClient = new ApiClient();
        }

        // Action xem chi tiết phòng
        public async Task<ActionResult> ChiTietPhong(Guid id)
        {
            if (id == Guid.Empty) return RedirectToAction("Index", "Home");

            try
            {
                // 1. Gọi API lấy chi tiết (Giả sử endpoint là api/phong/{id})
                var phong = await _apiClient.GetAsync<PhongDetailDto>($"api/phong/{id}");

                if (phong == null) return HttpNotFound();

                // 2. Xử lý dữ liệu hiển thị
                // Vì DB hiện tại chỉ có 1 ảnh đại diện, ta sẽ giả lập list ảnh để Slider hoạt động
                var mainImage = !string.IsNullOrEmpty(phong.AnhDaiDien)
                     ? (phong.AnhDaiDien.StartsWith("http") ? phong.AnhDaiDien : "http://localhost:5101" + phong.AnhDaiDien)
                     : "/images/default-room.jpg";

                var listImages = new List<string> { mainImage };
                // Nếu sau này API trả về DanhSachAnh thật, hãy gán vào đây:
                // if (phong.DanhSachAnh != null) listImages.AddRange(phong.DanhSachAnh);

                // 3. Map sang ViewModel
                var model = new PhongTroDetailViewModel
                {
                    PhongId = phong.PhongId,
                    TieuDe = phong.TieuDe ?? phong.TenPhong,
                    GiaTien = phong.GiaTien,
                    DienTich = (double)phong.DienTich,
                    DiaChi = phong.NhaTro?.DiaChi ?? "Đang cập nhật",
                    TenNhaTro = phong.NhaTro?.TieuDe ?? "Nhà trọ",

                    // Các thuộc tính hỗ trợ View
                    HinhAnh = listImages,
                    TienIchs = phong.TienIchs ?? new List<string> { "Wifi miễn phí", "Giờ giấc tự do", "An ninh tốt" }, // Fake nếu null
                    ChuTroId = Guid.Empty, // Nếu API trả về ID chủ trọ thì gán vào để chat
                    TrangThai = phong.TrangThai ?? "Còn trống"
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không tìm thấy phòng: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> DatLich(Guid id)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");

            // Lấy chi tiết phòng (bao gồm cả ChuTroId)
            var phong = await _apiClient.GetAsync<PhongDetailDto>($"api/phong/{id}");
            if (phong == null) return HttpNotFound();

            var model = new DatLichViewModel
            {
                PhongId = phong.PhongId,
                // Backend yêu cầu ChuTroId, nên ta phải lấy từ API và gán vào đây
                ChuTroId = phong.ChuTroId != Guid.Empty ? phong.ChuTroId : phong.NhaTro.ChuTroId,
                TieuDe = phong.TieuDe ?? phong.TenPhong,
                GiaTien = phong.GiaTien,
                DiaChi = phong.NhaTro?.DiaChi ?? "Đang cập nhật",
                AnhDaiDien = !string.IsNullOrEmpty(phong.AnhDaiDien)
                             ? (phong.AnhDaiDien.StartsWith("http") ? phong.AnhDaiDien : "http://localhost:5101" + phong.AnhDaiDien)
                             : "/images/default-room.jpg"
            };

            return View(model);
        }

        // Trong Action POST DatLich
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DatLich(DatLichViewModel model)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra: Nếu ChuTroId bị rỗng (do chưa load kịp ở View), cần xử lý
                    if (model.ChuTroId == Guid.Empty)
                    {
                        ViewBag.Error = "Lỗi dữ liệu: Không tìm thấy thông tin chủ trọ.";
                        return View(model);
                    }

                    // Chuẩn bị dữ liệu gửi đi (Mapping)
                    var requestData = new CreateDatPhongDto
                    {
                        PhongId = model.PhongId,
                        ChuTroId = model.ChuTroId,
                        Loai = "Ngay", // Backend bắt buộc trường này
                        BatDau = new DateTimeOffset(model.NgayHen),
                        KetThuc = new DateTimeOffset(model.NgayHen.AddHours(1)),
                        GhiChu = model.GhiChu ?? "Đặt lịch xem phòng"
                    };

                    string token = Session["AccessToken"] as string;

                    // Gọi API
                    await _apiClient.PostAsync<CreateDatPhongDto, object>("api/datphong", requestData, token);

                    TempData["SuccessMessage"] = "Đặt lịch thành công!";
                    return RedirectToAction("LichDaDat"); // Chúng ta sẽ làm trang này ngay sau đây
                }
                catch (Exception ex)
                {
                    // In lỗi chi tiết ra để debug
                    ViewBag.Error = "Lỗi từ Server: " + ex.Message;
                }
            }
            return View(model);
        }

    }
}