ADMIN_QUANLYPHONGTRO
│
├── App_Start/
│     ├── RouteConfig.cs
│     ├── FilterConfig.cs
│     └── BundleConfig.cs
│     → (Các file hệ thống MVC — giữ nguyên)
│
├── Content/
│     ├── css/
│     │     ├── admin-template.css
│     │     ├── theme-dark.css
│     │     ├── theme-light.css
│     │     └── custom.css
│     │     → (Toàn bộ CSS của template demo1, dùng làm giao diện Admin)
│     ├── img/
│     │     ├── logo-trotot-admin.png
│     │     ├── banner-dashboard.png
│     │     └── icons/…
│     │     → (Logo & ảnh giao diện Admin)
│     └── vendor/
│           ├── bootstrap/
│           ├── fontawesome/
│           └── charts/
│           → (Các thư viện UI được template sử dụng)
│
├── Scripts/
│     ├── admin-template.js
│     ├── theme-settings.js
│     ├── ajax-helper.js
│     └── custom.js
│     → (JS của template demo1 + JS xử lý theme + AJAX gọi API)
│
├── Models/
│     ├── DTO/
│     │     ├── NguoiDungDto.cs
│     │     ├── ChuTroThongTinPhapLyDto.cs
│     │     ├── PhongDto.cs
│     │     ├── BaoCaoViPhamDto.cs
│     │     ├── DatPhongDto.cs
│     │     ├── BienLaiDto.cs
│     │     └── YeuCauHoTroDto.cs
│     │     → (Các DTO ánh xạ đúng theo DB bạn đã xây — dùng để nhận dữ liệu từ RestAPI)
│     │
│     ├── ViewModels/
│     │     ├── DashboardViewModel.cs
│     │     ├── UserListViewModel.cs
│     │     ├── HostVerificationViewModel.cs
│     │     ├── RoomListViewModel.cs
│     │     ├── ReportListViewModel.cs
│     │     └── TransactionListViewModel.cs
│     │     → (Model dành cho hiển thị giao diện)
│     │
│     ├── Enums/
│     │     ├── VaiTroEnum.cs
│     │     ├── TrangThaiDatPhongEnum.cs
│     │     ├── TrangThaiBaoCaoEnum.cs
│     │     └── TrangThaiXacThucHostEnum.cs
│     │     → (Enum giúp View dễ đọc hơn DB)
│     │
│     └── Common/
│           ├── ApiResponse.cs
│           ├── PagedResult.cs
│           └── AppSettings.cs
│           → (Chuẩn hóa response & phân trang)
│
├── ApiClients/
│     ├── BaseApiClient.cs
│     │     → (Chứa hàm GET, POST, PUT, DELETE gọi API)
│     ├── UserApiClient.cs
│     │     → (Gọi API quản lý người dùng / khóa tài khoản / phân quyền)
│     ├── HostApiClient.cs
│     │     → (Gọi API hồ sơ chủ trọ: duyệt / từ chối)
│     ├── RoomApiClient.cs
│     │     → (Duyệt phòng, khóa phòng, lấy danh sách phòng)
│     ├── ReportApiClient.cs
│     │     → (Lấy danh sách báo cáo vi phạm, xử lý báo cáo)
│     ├── SupportApiClient.cs
│     │     → (Quản lý yêu cầu hỗ trợ)
│     └── TransactionApiClient.cs
│           → (Biên lai, giao dịch, xác nhận thanh toán)
│
├── Services/
│     ├── Interfaces/
│     │     ├── IUserService.cs
│     │     ├── IHostService.cs
│     │     ├── IRoomService.cs
│     │     ├── IReportService.cs
│     │     ├── ISupportService.cs
│     │     └── ITransactionService.cs
│     │     → (Interface định nghĩa service — dễ mock test, dễ mở rộng)
│     │
│     ├── Implementations/
│     │     ├── UserService.cs
│     │     ├── HostService.cs
│     │     ├── RoomService.cs
│     │     ├── ReportService.cs
│     │     ├── SupportService.cs
│     │     └── TransactionService.cs
│     │     → (Service gọi ApiClient & xử lý nghiệp vụ trước khi đưa lên Controller)
│
├── Controllers/
│     ├── DashboardController.cs
│     │     → (Trang tổng quan: biểu đồ, thống kê từ DB)
│     ├── UsersController.cs
│     │     → (Khóa/mở tài khoản, xem danh sách user)
│     ├── HostsController.cs
│     │     → (Duyệt hồ sơ chủ trọ — mapping ChuTroThongTinPhapLy)
│     ├── RoomsController.cs
│     │     → (Duyệt bài đăng phòng — mapping Phong)
│     ├── ReportsController.cs
│     │     → (Xử lý báo cáo vi phạm — mapping BaoCaoViPham)
│     ├── SupportController.cs
│     │     → (Tiếp nhận yêu cầu hỗ trợ — mapping YeuCauHoTro)
│     ├── TransactionsController.cs
│     │     → (Biên lai thanh toán — mapping BienLai)
│     └── SettingsController.cs
│           → (Quản lý tiện ích, quận/huyện, cấu hình hệ thống)
│
├── Views/
│     ├── Shared/
│     │     ├── _LayoutAdmin.cshtml
│     │     │     → (Layout chính của Admin – lấy từ demo1)
│     │     ├── _Sidebar.cshtml
│     │     │     → (Menu trái: Dashboard, Users, Hosts, Rooms, Reports, Support…)
│     │     ├── _Header.cshtml
│     │     │     → (Thanh header top: thông báo, avatar admin)
│     │     ├── _ThemeSettings.cshtml
│     │     │     → (Popup chọn màu giao diện — từ template demo1)
│     │     └── _Scripts.cshtml
│     │           → (Khu vực import JS chung)
│     │
│     ├── Dashboard/
│     │     └── Index.cshtml
│     │     → (Biểu đồ, KPI: số user, số phòng, số báo cáo)
│     │
│     ├── Users/
│     │     ├── Index.cshtml
│     │     └── Detail.cshtml
│     │     → (Danh sách người dùng, khóa/mở tài khoản)
│     │
│     ├── Hosts/
│     │     ├── Pending.cshtml
│     │     └── Detail.cshtml
│     │     → (Duyệt hồ sơ chủ trọ)
│     │
│     ├── Rooms/
│     │     ├── Pending.cshtml
│     │     ├── List.cshtml
│     │     └── Detail.cshtml
│     │     → (Duyệt bài đăng phòng)
│     │
│     ├── Reports/
│     │     ├── Index.cshtml
│     │     └── Detail.cshtml
│     │     → (Xử lý báo cáo vi phạm)
│     │
│     ├── Support/
│     │     ├── Index.cshtml
│     │     └── Detail.cshtml
│     │     → (Xử lý yêu cầu hỗ trợ)
│     │
│     ├── Transactions/
│     │     ├── Index.cshtml
│     │     └── Detail.cshtml
│     │     → (Quản lý biên lai, giao dịch)
│     │
│     └── Settings/
│           └── Index.cshtml
│           → (Quản lý tiện ích, quận/huyện)
│
├── Global.asax
│     → File khởi động của MVC
│
└── Web.config
      → Cấu hình API URL, chuỗi kết nối, app settings


      📌 1) Content + Scripts

→ Toàn bộ giao diện, CSS/JS template admin bạn lấy từ demo1.
→ Dùng cho layout admin.

📌 2) Models

→ DTO = dữ liệu lấy từ DB qua API
→ ViewModels = dữ liệu hiển thị giao diện

📌 3) ApiClients

→ Tầng giao tiếp API
→ Mỗi module có 1 API Client riêng

📌 4) Services

→ Thực hiện nghiệp vụ dành cho Admin
→ Tách biệt hoàn toàn khỏi Controller

📌 5) Controllers

→ Điều khiển giao diện
→ Gọi Service để lấy dữ liệu

📌 6) Views

→ Giao diện Razor
→ Tách nhỏ: Dashboard, Users, Hosts, Rooms…
→ Sidebar và Header đặt trong Shared