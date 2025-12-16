# 🔴 LỖI: No connection could be made because the target machine actively refused it

## 📌 NGUYÊN NHÂN:
**Backend API chưa được khởi động!**

ADMIN_QUANLYPHONGTRO (Client MVC) đang cố gắng kết nối tới Backend API ở `https://localhost:7039/api/` nhưng Backend chưa chạy.

---

## ✅ CÁCH KHẮC PHỤC:

### 🚀 **Phương án 1: Sử dụng script tự động (KHUYẾN NGHỊ)**

1. Chuột phải vào file `START_ALL_PROJECTS.ps1`
2. Chọn **"Run with PowerShell"**
3. Script sẽ tự động khởi động Backend API và mở các project

**HOẶC** chạy trong PowerShell:
```powershell
cd E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro
.\START_ALL_PROJECTS.ps1
```

---

### 🛠️ **Phương án 2: Khởi động thủ công**

#### Bước 1: Chạy Backend API
Mở **Terminal/PowerShell** và chạy:
```powershell
cd E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro\Backend\RestAPI_QUANLYPHONGTRO
dotnet run --launch-profile https
```

Hoặc trong **Visual Studio**:
1. Chuột phải project `RestAPI_QUANLYPHONGTRO`
2. **Set as Startup Project**
3. Nhấn **F5**

#### Bước 2: Kiểm tra Backend đang chạy
Mở trình duyệt: https://localhost:7039/swagger

#### Bước 3: Chạy ADMIN_QUANLYPHONGTRO
Nhấn **F5** trong Visual Studio

---

## 📊 KIỂM TRA NHANH:

### ✅ Backend API đã chạy chưa?
Mở PowerShell và chạy:
```powershell
curl https://localhost:7039/api/health -k
```

Nếu có kết quả ➡️ **Backend đang chạy tốt!** ✅

### ❌ Vẫn bị lỗi?

1. **Kiểm tra cấu hình:**
   - File: `ADMIN_QUANLYPHONGTRO\Web.config`
   - Dòng: `<add key="ApiBaseUrl" value="https://localhost:7039/api/" />`
   - ✅ Cổng phải là **7039** (không phải 5001)

2. **Kiểm tra SSL Certificate:**
   ```powershell
   dotnet dev-certs https --trust
   ```

3. **Kiểm tra cổng đã dùng chưa:**
   ```powershell
   netstat -ano | findstr :7039
   ```

4. **Kiểm tra Database:**
   - Connection string trong `Backend\RestAPI_QUANLYPHONGTRO\appsettings.json`
   - Chạy file `SQL\QuanLyPhongTro.sql` để tạo database

---

## 🎯 TÓM TẮT:

```
❌ LỖI: Backend API chưa chạy
✅ GIẢI PHÁP: Chạy Backend trước, sau đó chạy Admin MVC
```

### Thứ tự khởi động đúng:
```
1️⃣ Backend API (RestAPI_QUANLYPHONGTRO) - Port 7039
2️⃣ Admin MVC (ADMIN_QUANLYPHONGTRO)
3️⃣ User MVC (USER_QUANLYPHONGTRO) - Tùy chọn
```

---

## 📞 HỖ TRỢ THÊM:

Nếu vẫn gặp lỗi, kiểm tra:
- [ ] Backend API đang chạy (https://localhost:7039/swagger)
- [ ] Web.config có URL đúng (https://localhost:7039/api/)
- [ ] Database đã được tạo
- [ ] SSL certificate đã tin cậy (`dotnet dev-certs https --trust`)
- [ ] Firewall không chặn port 7039

---

✅ **Sau khi khắc phục, hệ thống sẽ hoạt động bình thường!**
