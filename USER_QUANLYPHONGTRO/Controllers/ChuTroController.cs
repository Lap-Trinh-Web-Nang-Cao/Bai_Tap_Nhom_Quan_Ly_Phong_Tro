using System;
using System.Collections.Generic;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.ChuTro;

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class ChuTroController : Controller
    {
        // ============================================================
        // 1. DASHBOARD (TỔNG QUAN)
        // ============================================================
        public ActionResult Dashboard()
        {
            // Khởi tạo model rỗng nhưng có các List bên trong để tránh lỗi null khi View chạy vòng lặp
            var model = new LandlordDashboardViewModel
            {
                // Số liệu thống kê giả định bằng 0
                TongSoPhong = 0,
                LuotXemHomNay = 0,
                LichHenSapToi = 0,
                DoanhThuThang = 0,

                // Các danh sách rỗng
                DanhSachPhongCho = new List<PhongChoDuyetItem>(),
                LichHenHomNay = new List<LichHenItem>(),
                TinNhanGanDay = new List<TinNhanItem>(),
                YeuCauSuaChua = new List<YeuCauSuaChuaItem>()
            };

            return View(model);
        }

        // ============================================================
        // 2. QUẢN LÝ PHÒNG
        // ============================================================
        public ActionResult DanhSachPhong()
        {
            // Trả về danh sách rỗng đúng kiểu PhongTroEditViewModel
            return View(new List<PhongTroEditViewModel>());
        }

        // GET: Tạo phòng mới
        public ActionResult TaoPhong()
        {
            return View(new PhongTroEditViewModel());
        }

        // ============================================================
        // 3. LỊCH HẸN / ĐẶT PHÒNG (ĐÃ SỬA THEO VIEWMODEL MỚI)
        // ============================================================
        public ActionResult LichHen()
        {
            // Trả về danh sách rỗng đúng kiểu LichHenViewModel mới của bạn
            // View sẽ tự động hiển thị giao diện "Hiện không có yêu cầu..." (Empty State)
            return View(new List<LichHenViewModel>());
        }
        // 4. HỢP ĐỒNG KHÁCH THUÊ
        public ActionResult HopDong()
        {
            // Trả về model chứa danh sách rỗng để hiển thị giao diện
            var model = new LandlordHopDongViewModel
            {
                HopDongList = new List<LandlordHopDongItemViewModel>()
            };
            return View(model);
        }

        // ============================================================
        // 5. HÓA ĐƠN & THU TIỀN
        // ============================================================
        public ActionResult HoaDon()
        {
            // Trả về danh sách hóa đơn rỗng
            return View(new List<LandlordHoaDonViewModel>());
        }

        // ============================================================
        // 6. HỒ SƠ CÁ NHÂN
        // ============================================================
        public ActionResult ThongTinCaNhan()
        {
            // Trả về model hồ sơ rỗng
            return View(new LandlordProfileViewModel());
        }
        // GET: Xem chi tiết hợp đồng
        public ActionResult ChiTietHopDong(Guid? id)
        {
            // Mock data (Dữ liệu giả lập để test giao diện)
            var model = new ChiTietHopDongViewModel
            {
                HopDongId = id ?? Guid.NewGuid(),
                TenPhong = "P.101 - Phòng Studio Ban Công",
                DiaChiPhong = "123 Võ Văn Tần, Q.3, TP.HCM",

                TenKhach = "Nguyễn Văn A",
                DienThoai = "0987654321",
                CCCD = "079123456789",
                DiaChiThuongTru = "Xã A, Huyện B, Tỉnh C",

                NgayBatDau = DateTime.Now.AddMonths(-2),
                NgayKetThuc = DateTime.Now.AddMonths(10),
                GiaThue = 5500000,
                TienCoc = 5500000,

                DieuKhoan = "- Bên B cam kết thanh toán tiền thuê trước ngày 5 hàng tháng.\n- Không được nuôi thú cưng.\n- Giữ gìn vệ sinh chung.",
                TrangThai = "HieuLuc"
            };

            return View(model);
        }
    }
}