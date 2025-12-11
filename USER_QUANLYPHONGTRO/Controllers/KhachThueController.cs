using System;
using System.Collections.Generic;
using System.Web.Mvc;
using USER_QUANLYPHONGTRO.Models.ViewModels.KhachThue; // NEW

namespace USER_QUANLYPHONGTRO.Controllers
{
    public class KhachThueController : Controller
    {
        // NEW: kiểm tra có đúng role Người thuê không
        private bool CheckKhachThueRole()
        {
            var role = Session["UserRole"]?.ToString();
            return role == "KhachThue";
        }

        // NEW: GET: /KhachThue → chuyển thẳng về Dashboard
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        // NEW: Dashboard người thuê
        public ActionResult Dashboard()
        {
            if (!CheckKhachThueRole())
            {
                // Nếu chưa login hoặc không phải Khách thuê → về trang Login cho người thuê
                return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            }

            var model = new TenantDashboardViewModel
            {
                TenNguoiThue = Session["HoTen"] as string ?? "Người thuê",
                Email = Session["UserName"] as string ?? "unknown@example.com",

                SoPhongDaXem = 12,
                SoLichHenSapToi = 2,
                SoHopDongDangHieuLuc = 1,
                SoHoaDonChuaThanhToan = 1,

                LichHenSapToi = GetMockLichHenSapToi(),
                HopDongHieuLuc = GetMockHopDong(),
                HoaDonGanDay = GetMockHoaDonGanDay()
            };

            ViewBag.Title = "Trang chủ người thuê";
            return View(model);
        }

        // NEW: Lịch đã đặt
        public ActionResult LichDaDat()
        {
            if (!CheckKhachThueRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            }

            var list = GetMockLichHenSapToi(); // tạm dùng lại mock
            return View(list);
        }

        // NEW: Hợp đồng
        public ActionResult HopDong()
        {
            if (!CheckKhachThueRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            }

            var list = GetMockHopDong();
            return View(list);
        }

        // NEW: Hóa đơn
        public ActionResult HoaDon()
        {
            if (!CheckKhachThueRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            }

            var list = GetMockHoaDonGanDay();
            return View(list);
        }

        // NEW: Thông tin cá nhân (placeholder)
        public ActionResult ThongTinCaNhan()
        {
            if (!CheckKhachThueRole())
            {
                return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
            }

            ViewBag.Message = "Trang thông tin cá nhân đang được phát triển.";
            return View();
        }

        // ================== MOCK DATA ==================

        private List<TenantScheduleItem> GetMockLichHenSapToi()
        {
            return new List<TenantScheduleItem>
            {
                new TenantScheduleItem
                {
                    DatPhongId = Guid.NewGuid(),
                    TenPhong = "Phòng trọ sinh viên gần UTE",
                    TenChuTro = "Nguyễn Văn A",
                    ThoiGianXem = DateTime.Now.AddHours(3),
                    TrangThai = "Đã xác nhận"
                },
                new TenantScheduleItem
                {
                    DatPhongId = Guid.NewGuid(),
                    TenPhong = "Căn hộ mini 1PN",
                    TenChuTro = "Trần Thị B",
                    ThoiGianXem = DateTime.Now.AddDays(1).AddHours(2),
                    TrangThai = "Chờ xác nhận"
                }
            };
        }

        private List<TenantContractItem> GetMockHopDong()
        {
            return new List<TenantContractItem>
            {
                new TenantContractItem
                {
                    HopDongId = Guid.NewGuid(),
                    TenPhong = "Phòng trọ sinh viên gần UTE",
                    DiaChi = "123 Lê Văn Việt, Q.9",
                    NgayBatDau = DateTime.Now.AddMonths(-2),
                    NgayKetThuc = DateTime.Now.AddMonths(10),
                    GiaThue = 1800000,
                    TrangThai = "Đang hiệu lực"
                }
            };
        }

        private List<TenantInvoiceItem> GetMockHoaDonGanDay()
        {
            return new List<TenantInvoiceItem>
            {
                new TenantInvoiceItem
                {
                    HoaDonId = Guid.NewGuid(),
                    ThangNam = "11/2025",
                    TongTien = 2100000,
                    TrangThaiThanhToan = "Đã thanh toán",
                    NgayThanhToan = DateTime.Now.AddDays(-5)
                },
                new TenantInvoiceItem
                {
                    HoaDonId = Guid.NewGuid(),
                    ThangNam = "12/2025",
                    TongTien = 2200000,
                    TrangThaiThanhToan = "Chưa thanh toán",
                    NgayThanhToan = null
                }
            };
        }
    }
}
