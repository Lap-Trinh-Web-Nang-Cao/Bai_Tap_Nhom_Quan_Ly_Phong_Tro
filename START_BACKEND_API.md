# 🚀 HƯỚNG DẪN KHỞI ĐỘNG BACKEND API

## ✅ Bước 1: Mở Terminal/Command Prompt

## ✅ Bước 2: Di chuyển vào thư mục Backend
```bash
cd E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro\Backend\RestAPI_QUANLYPHONGTRO
```

## ✅ Bước 3: Chạy API bằng lệnh
```bash
dotnet run --launch-profile https
```

**HOẶC** nếu dùng Visual Studio:
1. Chuột phải vào project `RestAPI_QUANLYPHONGTRO`
2. Chọn **"Set as Startup Project"**
3. Nhấn **F5** hoặc nút **▶ Start**

## ✅ Bước 4: Kiểm tra API đang chạy

Mở trình duyệt và truy cập:
```
https://localhost:7039/swagger
```

Nếu thấy trang Swagger UI ➡️ **API đã sẵn sàng!** ✅

## ✅ Bước 5: Chạy ADMIN_QUANLYPHONGTRO

Sau khi Backend API đã chạy, giờ có thể chạy project ADMIN_QUANLYPHONGTRO.

---

## 🔴 LƯU Ý QUAN TRỌNG:

### ⚠️ Backend API phải luôn chạy trước!
- ADMIN_QUANLYPHONGTRO là **Client MVC** cần gọi API
- Backend RestAPI_QUANLYPHONGTRO là **Server API** cung cấp dữ liệu

### 📌 Thứ tự khởi động:
```
1️⃣ Chạy Backend API (RestAPI_QUANLYPHONGTRO)
2️⃣ Chạy Admin MVC (ADMIN_QUANLYPHONGTRO)
3️⃣ (Tùy chọn) Chạy User MVC (USER_QUANLYPHONGTRO)
```

### 🛠️ Cấu hình cổng hiện tại:

| Dự án | Cổng | URL |
|-------|------|-----|
| **Backend API** | 7039 (HTTPS) | https://localhost:7039 |
| **Backend API** | 5101 (HTTP) | http://localhost:5101 |
| **Admin MVC** | IIS Express | (tự động) |
| **User MVC** | IIS Express | (tự động) |

---

## 🐛 Xử lý lỗi thường gặp:

### ❌ Lỗi: "No connection could be made"
**Nguyên nhân:** Backend API chưa chạy  
**Giải pháp:** Chạy Backend API trước

### ❌ Lỗi: "SSL certificate problem"
**Nguyên nhân:** Chứng chỉ HTTPS dev chưa được tin cậy  
**Giải pháp:** 
```bash
dotnet dev-certs https --trust
```

### ❌ Lỗi: "Port already in use"
**Nguyên nhân:** Cổng 7039/5101 đang được sử dụng  
**Giải pháp:** 
- Đóng ứng dụng đang dùng cổng đó
- HOẶC thay đổi cổng trong `launchSettings.json`

---

## 📦 Database Setup (Nếu lần đầu chạy):

1. Kiểm tra connection string trong `appsettings.json`
2. Chạy migration (nếu cần):
```bash
dotnet ef database update
```

3. Import dữ liệu mẫu từ file SQL (nếu có)

---

✅ **Sau khi làm xong các bước trên, mọi thứ sẽ hoạt động bình thường!**
