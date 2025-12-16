# 🏠 HỆ THỐNG QUẢN LÝ PHÒNG TRỌ - HƯỚNG DẪN SỬ DỤNG

## 📋 MỤC LỤC
1. [Tổng quan hệ thống](#tổng-quan)
2. [Cấu trúc dự án](#cấu-trúc)
3. [Cách khởi động](#khởi-động)
4. [Khắc phục lỗi thường gặp](#khắc-phục-lỗi)
5. [Cấu hình hệ thống](#cấu-hình)

---

## 🎯 TỔNG QUAN

Hệ thống gồm 3 project chính:

| Project | Công nghệ | Port | Mô tả |
|---------|-----------|------|-------|
| **RestAPI_QUANLYPHONGTRO** | .NET 8 Web API | 7039 (HTTPS) | Backend API, xử lý logic & database |
| **ADMIN_QUANLYPHONGTRO** | .NET Framework 4.7.2 MVC | IIS Express | Giao diện quản trị cho Admin |
| **USER_QUANLYPHONGTRO** | .NET Framework 4.7.2 MVC | IIS Express | Giao diện cho Người dùng & Chủ trọ |

---

## 📁 CẤU TRÚC DỰ ÁN

```
Bai_Tap_Nhom_Quan_Ly_Phong_Tro/
│
├── Backend/
│   └── RestAPI_QUANLYPHONGTRO/        # 🔷 Backend API (.NET 8)
│       ├── Controllers/
│       ├── Services/
│       ├── Models/
│       ├── Data/
│       └── appsettings.json           # Config DB & JWT
│
├── ADMIN_QUANLYPHONGTRO/              # 🔷 Admin MVC (.NET 4.7.2)
│   ├── Controllers/
│   ├── Views/
│   ├── Models/
│   ├── Services/
│   ├── ApiClients/                    # Gọi API
│   └── Web.config                     # Config API URL
│
├── USER_QUANLYPHONGTRO/               # 🔷 User MVC (.NET 4.7.2)
│   ├── Controllers/
│   ├── Views/
│   └── Models/
│
└── SQL/
    └── QuanLyPhongTro.sql             # Database script
```

---

## 🚀 KHỞI ĐỘNG HỆ THỐNG

### ⚡ **Cách 1: Tự động (Khuyến nghị)**

**Windows:**
```bash
# Double-click vào file:
START_ALL_PROJECTS.bat
```

**PowerShell:**
```powershell
.\START_ALL_PROJECTS.ps1
```

### 🛠️ **Cách 2: Thủ công**

#### Bước 1️⃣: Khởi động Backend API
```powershell
cd Backend\RestAPI_QUANLYPHONGTRO
dotnet run --launch-profile https
```

**Kiểm tra:** Mở https://localhost:7039/swagger

#### Bước 2️⃣: Khởi động Admin MVC
1. Mở `ADMIN_QUANLYPHONGTRO.sln` trong Visual Studio
2. Nhấn **F5**

#### Bước 3️⃣: Khởi động User MVC (Tùy chọn)
1. Mở `USER_QUANLYPHONGTRO.sln` trong Visual Studio
2. Nhấn **F5**

---

## 🐛 KHẮC PHỤC LỖI THƯỜNG GẶP

### ❌ Lỗi: "No connection could be made because the target machine actively refused it"

**Nguyên nhân:** Backend API chưa chạy

**Giải pháp:**
1. Chạy Backend API trước
2. Kiểm tra URL trong `ADMIN_QUANLYPHONGTRO\Web.config`:
   ```xml
   <add key="ApiBaseUrl" value="https://localhost:7039/api/" />
   ```
3. Đảm bảo cổng là **7039** (không phải 5001)

📖 Chi tiết: Xem file [`FIX_CONNECTION_ERROR.md`](FIX_CONNECTION_ERROR.md)

---

### ❌ Lỗi: "The SSL connection could not be established"

**Giải pháp:**
```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

---

### ❌ Lỗi: "Cannot open database"

**Giải pháp:**
1. Kiểm tra SQL Server đang chạy
2. Kiểm tra connection string trong `Backend\RestAPI_QUANLYPHONGTRO\appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuanLyPhongTro;..."
   }
   ```
3. Chạy file `SQL\QuanLyPhongTro.sql` để tạo database

---

### ❌ Lỗi: "Port 7039 is already in use"

**Giải pháp:**
```powershell
# Tìm process đang dùng port
netstat -ano | findstr :7039

# Kill process (thay PID bằng số tìm được)
taskkill /PID <PID> /F
```

---

## ⚙️ CẤU HÌNH HỆ THỐNG

### 🔧 Backend API (RestAPI_QUANLYPHONGTRO)

**File:** `Backend\RestAPI_QUANLYPHONGTRO\appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuanLyPhongTro;..."
  },
  "Jwt": {
    "Key": "your-super-secret-key-here-min-32-chars",
    "Issuer": "TroTotAPI",
    "Audience": "TroTotClients"
  }
}
```

**File:** `Backend\RestAPI_QUANLYPHONGTRO\Properties\launchSettings.json`

```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:7039;http://localhost:5101"
    }
  }
}
```

---

### 🔧 Admin MVC (ADMIN_QUANLYPHONGTRO)

**File:** `ADMIN_QUANLYPHONGTRO\Web.config`

```xml
<appSettings>
  <add key="ApiBaseUrl" value="https://localhost:7039/api/" />
</appSettings>
```

⚠️ **LƯU Ý:** URL phải khớp với cổng Backend API!

---

## 📊 KIỂM TRA HỆ THỐNG

### ✅ Checklist trước khi chạy:

- [ ] SQL Server đang chạy
- [ ] Database `QuanLyPhongTro` đã được tạo
- [ ] Backend API chạy thành công (https://localhost:7039/swagger)
- [ ] SSL certificate đã được trust (`dotnet dev-certs https --trust`)
- [ ] Web.config có URL đúng (https://localhost:7039/api/)

### 🧪 Test Backend API:

```powershell
# Test health endpoint
curl https://localhost:7039/api/health -k

# Test Swagger
start https://localhost:7039/swagger
```

---

## 📚 TÀI LIỆU THAM KHẢO

- [`START_BACKEND_API.md`](START_BACKEND_API.md) - Hướng dẫn khởi động Backend API
- [`FIX_CONNECTION_ERROR.md`](FIX_CONNECTION_ERROR.md) - Khắc phục lỗi kết nối
- [`IMPLEMENTATION_GUIDE.md`](ADMIN_QUANLYPHONGTRO/IMPLEMENTATION_GUIDE.md) - Hướng dẫn triển khai

---

## 🎓 THÔNG TIN HỌC THUẬT

**Môn học:** Lập trình Web nâng cao  
**Trường:** [Tên trường]  
**Nhóm:** [Tên nhóm]

---

## 📞 HỖ TRỢ

Nếu gặp vấn đề:
1. Kiểm tra các file hướng dẫn ở trên
2. Xem logs trong Console
3. Kiểm tra event viewer (Windows Logs > Application)
4. Liên hệ nhóm phát triển

---

✅ **Chúc bạn sử dụng hệ thống thành công!** 🎉
