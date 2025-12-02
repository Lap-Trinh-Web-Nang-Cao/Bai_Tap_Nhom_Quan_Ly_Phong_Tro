USER_QUANLYPHONGTRO/
│
├─ Controllers/
│   ├─ HomeController.cs          // Trang chủ / landing (public)
│   ├─ AuthController.cs          // Login, Register, Logout (gọi API)
│   ├─ KhachThueController.cs     // Màn cho Người thuê
│   ├─ ChuTroController.cs        // Màn cho Chủ trọ
│   └─ ChatController.cs          // Màn chat real-time ChuTrọ <-> KháchThuê
│
├─ Models/
│   ├─ ViewModels/
│   │   ├─ Auth/
│   │   │   ├─ LoginViewModel.cs
│   │   │   └─ RegisterViewModel.cs
│   │   │
│   │   ├─ KhachThue/
│   │   │   ├─ TenantDashboardViewModel.cs
│   │   │   ├─ PhongTroListItemViewModel.cs
│   │   │   ├─ PhongTroDetailViewModel.cs
│   │   │   ├─ DatLichViewModel.cs
│   │   │   ├─ LichDaDatViewModel.cs
│   │   │   ├─ TenantHopDongViewModel.cs
│   │   │   ├─ TenantHoaDonViewModel.cs
│   │   │   └─ TenantProfileViewModel.cs
│   │   │
│   │   ├─ ChuTro/
│   │   │   ├─ LandlordDashboardViewModel.cs
│   │   │   ├─ PhongTroEditViewModel.cs
│   │   │   ├─ LichHenViewModel.cs
│   │   │   ├─ LandlordHopDongViewModel.cs
│   │   │   ├─ LandlordHoaDonViewModel.cs
│   │   │   └─ LandlordProfileViewModel.cs
│   │   │
│   │   └─ Chat/
│   │       ├─ ChatMessageViewModel.cs
│   │       └─ ChatRoomViewModel.cs
│   │
│   └─ Dtos/                      // (nếu cần thêm lớp DTO riêng cho gọi API)
│       ├─ ApiResponse.cs
│       └─ ... (có thể bỏ qua giai đoạn đầu)
│
├─ Services/
│   ├─ Interfaces/
│   │   ├─ IApiClient.cs          // interface gọi API chung
│   │   └─ IAuthService.cs        // (nếu muốn tách riêng xử lý auth phía client)
│   │
│   ├─ ApiClient.cs               // HttpClient wrapper: GET/POST, gắn token
│   └─ AuthService.cs             // xử lý lưu token, đọc role,... (tuỳ chọn)
│
├─ Views/
│   ├─ Shared/
│   │   ├─ _Layout.cshtml         // Layout chung cho toàn app (header, footer, css/js)
│   │   ├─ _Navbar.cshtml         // Thanh menu trên cùng
│   │   ├─ _Footer.cshtml         // Chân trang
│   │   ├─ _TenantSidebar.cshtml  // Sidebar cho Người thuê
│   │   ├─ _LandlordSidebar.cshtml// Sidebar cho Chủ trọ
│   │   └─ Error.cshtml           // Trang lỗi chung
│   │
│   ├─ Home/
│   │   └─ Index.cshtml           // Landing page (giới thiệu hệ thống, link tới Login)
│   │
│   ├─ Auth/
│   │   ├─ Login.cshtml           // Form login (gọi API)
│   │   └─ Register.cshtml        // Form đăng ký Người thuê / Chủ trọ
│   │
│   ├─ KhachThue/
│   │   ├─ Index.cshtml           // Dashboard người thuê
│   │   ├─ DanhSachPhong.cshtml   // Danh sách phòng có thể thuê
│   │   ├─ ChiTietPhong.cshtml    // Chi tiết phòng, nút "Đặt lịch"
│   │   ├─ DatLich.cshtml         // Form đặt lịch xem phòng
│   │   ├─ LichDaDat.cshtml       // Danh sách lịch đã đặt
│   │   ├─ HopDong.cshtml         // Hợp đồng của người thuê
│   │   ├─ HoaDon.cshtml          // Hóa đơn của người thuê
│   │   ├─ ThanhToan.cshtml       // Màn xác nhận thanh toán (UI)
│   │   └─ ThongTinCaNhan.cshtml  // Thông tin cá nhân người thuê
│   │
│   ├─ ChuTro/
│   │   ├─ Index.cshtml           // Dashboard chủ trọ
│   │   ├─ DanhSachPhong.cshtml   // Danh sách phòng do chủ trọ quản lý
│   │   ├─ TaoPhong.cshtml        // Thêm phòng mới
│   │   ├─ SuaPhong.cshtml        // Sửa thông tin phòng
│   │   ├─ LichHen.cshtml         // Lịch hẹn xem phòng của khách
│   │   ├─ HopDong.cshtml         // Danh sách hợp đồng
│   │   ├─ ChiTietHopDong.cshtml  // Chi tiết một hợp đồng
│   │   ├─ HoaDon.cshtml          // Hóa đơn cần thu
│   │   └─ ThongTinCaNhan.cshtml  // Thông tin chủ trọ
│   │
│   └─ Chat/
│       └─ PhongChat.cshtml       // Giao diện chat real-time (SignalR)
│
├─ Content/
│   ├─ css/
│   │   ├─ app-base.css           // màu sắc, font, button, form chung
│   │   ├─ app-layout.css         // layout header, footer, sidebar
│   │   ├─ app-tenant.css         // style riêng cho màn Khách thuê
│   │   ├─ app-landlord.css       // style riêng cho màn Chủ trọ
│   │   ├─ app-chat.css           // style bong bóng chat, khung chat
│   │   └─ app-booking.css        // (nếu cần) style riêng cho DatLich
│   │
│   └─ images/
│       ├─ logo.png
│       └─ ... (icon, banner, avatar mẫu,…)
│
├─ Scripts/
│   ├─ app-api.js                 // hàm JS gọi API chung (fetch/xhr)
│   ├─ app-common.js              // JS dùng chung (toast, modal,…)
│   ├─ app-chat.js                // JS kết nối SignalR, xử lý chat
│   └─ app-booking.js             // JS cho màn đặt lịch (validate, AJAX nếu cần)
│
├─ Views/web.config               // file MVC
├─ Global.asax                    // nếu là .NET Framework
├─ Web.config                     // cấu hình web
└─ (hoặc Program.cs / appsettings.json nếu là ASP.NET Core MVC)