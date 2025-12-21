# 📱 HƯỚNG DẪN GỘP ADMIN & USER VỚI AUTHENTICATION

## 🎯 Mục tiêu
- ✅ Gộp Admin & User project lại
- ✅ Xác thực thực từ API (JWT Token)
- ✅ Lưu trữ Token trong Session
- ✅ Điều hướng động dựa trên vai trò (Admin, ChuTro, NguoiThue)

---

## 📋 KỸ THUẬT ĐƯỢC DÙNG

### **1. JWT Token Authentication**
- Backend (RestAPI) tạo JWT Token khi login
- Frontend lưu token trong Session (Server-side)
- Mỗi request có thể sử dụng token để xác thực với API

### **2. Role-Based Authorization**
- **VaiTroId = 1**: Admin (Quản trị hệ thống)
- **VaiTroId = 2**: Chủ Trọ (Đăng phòng cho thuê)
- **VaiTroId = 3**: Người Thuê (Tìm phòng)

### **3. Session Management**
- Session timeout: 1440 phút (24 giờ)
- Lưu Token, UserId, Email, VaiTroId trong Session
- Kiểm tra xác thực qua `CustomAuthorize` attribute

---

## 🔧 CẤU TRÚC THAY ĐỔI

### **USER_QUANLYPHONGTRO Project**
```
Services/
  └─ AuthService.cs (NEW)                 ← Xử lý xác thực, token, session
Filters/
  └─ CustomAuthorizeAttribute.cs (NEW)    ← Kiểm tra quyền truy cập
Controllers/
  ├─ AuthController.cs (UPDATED)          ← Xử lý login/logout
  ├─ AuthenticatedControllerBase.cs (NEW) ← Base class cho các controller
  └─ ... (other controllers)
```

### **ADMIN_QUANLYPHONGTRO Project**
```
Services/
  └─ AdminAuthService.cs (NEW)            ← Xác thực Admin
Filters/
  └─ AdminAuthorizeAttribute.cs (NEW)     ← Kiểm tra quyền Admin
Controllers/
  ├─ AuthController.cs (NEW)              ← Login/logout Admin
  ├─ AdminControllerBase.cs (NEW)         ← Base class với xác thực
  └─ ... (other controllers)
```

---

## 🚀 HƯỚNG DẪN SỬ DỤNG

### **1. ĐĂNG NHẬP (USER)**

**Trang đăng nhập**: `/Auth/Login`

```csharp
// Nhập Email & Password
// → System sẽ gọi API: POST /api/nguoidung/login
// → Lấy JWT Token từ API
// → Lưu vào Session
// → Điều hướng đến Dashboard phù hợp
```

**Điều hướng tự động**:
- Admin (VaiTroId=1) → `/admin/dashboard`
- Chủ Trọ (VaiTroId=2) → `/ChuTro/Dashboard`
- Người Thuê (VaiTroId=3) → `/Home/Index`

---

### **2. ĐĂNG NHẬP ADMIN**

**Trang đăng nhập Admin**: `/Auth/Login` (cùng URL, nhưng kiểm tra VaiTroId)

```csharp
// Chỉ cho Admin (VaiTroId = 1) vào được
// Nếu không phải Admin → "Bạn không có quyền truy cập Admin"
```

---

### **3. SỬ DỤNG AUTHORIZATION**

#### **Trong USER Project:**

```csharp
[CustomAuthorize] // Require login (bất kỳ role nào)
public class HomeController : AuthenticatedControllerBase
{
    public ActionResult Index()
    {
        // CurrentUser chứa thông tin người dùng hiện tại
        ViewBag.UserName = CurrentUser.UserEmail;
        ViewBag.IsAdmin = CurrentUser.IsAdmin;
        
        return View();
    }
}

[CustomAuthorize(2, 3)] // Chỉ ChuTro (2) & NguoiThue (3)
public ActionResult RoomSearch()
{
    // ...
}

[AllowAnonymous] // Bỏ qua xác thực
public ActionResult About()
{
    // ...
}
```

#### **Trong ADMIN Project:**

```csharp
[AdminAuthorize] // Require Admin login
public class DashboardController : AdminControllerBase
{
    public ActionResult Index()
    {
        ViewBag.AdminName = CurrentAdmin.UserEmail;
        return View();
    }
}

[AllowAnonymous] // Login page không cần xác thực
public class AuthController : Controller
{
    // ...
}
```

---

## 🔐 FLOW ĐĂNG NHẬP

```
┌─────────────────────────────────────────────────────────────┐
│ 1. User vào /Auth/Login                                     │
│    → Nhập Email & Password                                  │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│ 2. AuthController.Login() [POST]                             │
│    → Gọi AuthService.LoginAsync()                            │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│ 3. AuthService.LoginAsync()                                  │
│    → POST /api/nguoidung/login (API)                         │
│    → Lấy JWT Token từ Response                              │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│ 4. Giải mã JWT Token                                         │
│    → ExtractUserInfoFromToken()                             │
│    → Lấy: UserId, Email, VaiTroId                           │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│ 5. Lưu Session                                               │
│    → SaveUserSessionFromToken()                              │
│    → Session["AuthToken"] = token                            │
│    → Session["VaiTroId"] = 3 (hoặc 2, 1, ...)              │
│    → Session.Timeout = 1440 phút                            │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│ 6. Điều hướng dựa trên VaiTroId                             │
│    ├─ VaiTroId = 1 → /admin/dashboard                      │
│    ├─ VaiTroId = 2 → /ChuTro/Dashboard                     │
│    └─ VaiTroId = 3 → /Home/Index                           │
└─────────────────────────────────────────────────────────────┘
```

---

## 📊 BẢNG MAPPING VAI TRÒ

| VaiTroId | Tên | Giao diện | Controller |
|----------|-----|----------|-----------|
| 1 | Admin | Admin Dashboard | /Admin/Dashboard |
| 2 | Chủ Trọ | Seller Center | /ChuTro/Dashboard |
| 3 | Người Thuê | Home (Buyer) | /Home/Index |

---

## ✅ CHECKLIST TRIỂN KHAI

### **Phía Frontend (MVC)**

- [ ] Cài đặt `AuthService.cs` (USER_QUANLYPHONGTRO)
- [ ] Cài đặt `CustomAuthorizeAttribute.cs` (USER_QUANLYPHONGTRO)
- [ ] Cài đặt `AuthenticatedControllerBase.cs` (USER_QUANLYPHONGTRO)
- [ ] Cập nhật `AuthController.cs` để gọi API
- [ ] Cập nhật `Global.asax.cs` (cấu hình Session)
- [ ] Cập nhật `Web.config` (ApiBaseUrl, Session)

- [ ] Cài đặt `AdminAuthService.cs` (ADMIN_QUANLYPHONGTRO)
- [ ] Cài đặt `AdminAuthorizeAttribute.cs` (ADMIN_QUANLYPHONGTRO)
- [ ] Cài đặt `AdminControllerBase.cs` (ADMIN_QUANLYPHONGTRO)
- [ ] Cài đặt `AuthController.cs` (ADMIN_QUANLYPHONGTRO)

### **Phía Backend (API)**

- [ ] Kiểm tra `/api/nguoidung/login` endpoint
- [ ] Đảm bảo JWT Token chứa Claims: `NameIdentifier`, `Email`, `VaiTroId`
- [ ] Enable CORS nếu Frontend & API ở domain khác

### **Testing**

- [ ] Test login với tài khoản Admin (VaiTroId=1)
  - Kỳ vọng: Redirect `/admin/dashboard`
  
- [ ] Test login với tài khoản ChuTro (VaiTroId=2)
  - Kỳ vọng: Redirect `/ChuTro/Dashboard`
  
- [ ] Test login với tài khoản NguoiThue (VaiTroId=3)
  - Kỳ vọng: Redirect `/Home/Index`

- [ ] Test redirect nếu không xác thực
  - Kỳ vọng: `/Auth/Login`

- [ ] Test Session timeout
  - Kỳ vọng: Tự động logout sau 1440 phút

---

## 🐛 TROUBLESHOOTING

### **Lỗi: "Email hoặc mật khẩu không đúng"**
- ✓ Kiểm tra API BaseUrl trong Web.config
- ✓ Kiểm tra API server có chạy không
- ✓ Kiểm tra email/password có tồn tại trong DB không

### **Lỗi: "Token không chứa thông tin cần thiết"**
- ✓ Backend cần trả JWT Token với đúng Claims:
  ```csharp
  new Claim(ClaimTypes.NameIdentifier, user.Id)
  new Claim(ClaimTypes.Email, user.Email)
  new Claim("VaiTroId", user.VaiTroId.ToString())
  ```

### **Lỗi: "Bạn không có quyền truy cập Admin"**
- ✓ Chỉ Admin (VaiTroId=1) mới vào được /Admin
- ✓ Kiểm tra VaiTroId của tài khoản đó

### **Session hết giữa chừng**
- ✓ Tăng timeout trong Web.config: `<sessionState timeout="1440" />`
- ✓ Hoặc refresh lại Request để reset timer

---

## 📚 THAM KHẢO

**Files đã tạo/cập nhật:**
- `USER_QUANLYPHONGTRO/Services/AuthService.cs`
- `USER_QUANLYPHONGTRO/Filters/CustomAuthorizeAttribute.cs`
- `USER_QUANLYPHONGTRO/Controllers/AuthController.cs` (UPDATED)
- `USER_QUANLYPHONGTRO/Controllers/AuthenticatedControllerBase.cs`
- `USER_QUANLYPHONGTRO/Global.asax.cs` (UPDATED)
- `USER_QUANLYPHONGTRO/Web.config` (UPDATED)

- `ADMIN_QUANLYPHONGTRO/Services/AdminAuthService.cs`
- `ADMIN_QUANLYPHONGTRO/Filters/AdminAuthorizeAttribute.cs`
- `ADMIN_QUANLYPHONGTRO/Controllers/AuthController.cs` (NEW)
- `ADMIN_QUANLYPHONGTRO/Controllers/AdminControllerBase.cs`

---

## 🎉 HOÀN TẤT!

Bây giờ bạn có thể:
✅ Đăng nhập từ một nơi duy nhất (`/Auth/Login`)
✅ Tự động điều hướng đến giao diện phù hợp
✅ Quản lý quyền truy cập dễ dàng
✅ Sử dụng JWT Token từ API

Chúc mừng! 🚀
