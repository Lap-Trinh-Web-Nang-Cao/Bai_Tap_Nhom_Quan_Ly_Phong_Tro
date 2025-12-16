# 🧪 HƯỚNG DẪN TEST DASHBOARD API

## 📋 MỤC LỤC
1. [Chuẩn bị](#chuẩn-bị)
2. [Chạy Backend](#chạy-backend)
3. [Test với Swagger](#test-với-swagger)
4. [Test với Postman](#test-với-postman)
5. [Test Cases](#test-cases)
6. [Troubleshooting](#troubleshooting)

---

## 🎯 CHUẨN BỊ

### Kiểm tra Database
1. Mở SQL Server Management Studio (SSMS)
2. Connect tới: `LAPTOP-SGLHG0M9\MVY_350`
3. Kiểm tra database `QuanLyPhongTro` tồn tại
4. Verify các bảng có dữ liệu:
   ```sql
   SELECT COUNT(*) FROM NguoiDung;
   SELECT COUNT(*) FROM Phong;
   SELECT COUNT(*) FROM BaoCaoViPham;
   SELECT COUNT(*) FROM HanhDongAdmin;
   ```

### Tạo Admin User (Nếu chưa có)
```sql
-- Insert Admin user (VaiTroId = 1)
INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc, CreatedAt)
VALUES 
(NEWID(), 
 'admin@test.com', 
 '0901234567',
 '$2a$11$YourHashedPasswordHere', -- BCrypt hash của "admin123"
 1, -- VaiTroId = 1 (Admin)
 0, -- Không khóa
 1, -- Email đã xác thực
 GETDATE());
```

**Hash password "admin123":**
```
$2a$11$j8S8bXU5Z3K8xYc5F8jYxe6QZQX5L5X5X5X5X5X5X5X5X5X5X5X5X
```

---

## 🚀 CHẠY BACKEND

### Option 1: Visual Studio
1. Click phải vào project `RestAPI_QUANLYPHONGTRO`
2. Chọn "Set as Startup Project"
3. Nhấn **F5** hoặc click **Start**

### Option 2: Command Line
```bash
cd E:\LapTrinhWebNangCao\QuanLyPhongTro\Bai_Tap_Nhom_Quan_Ly_Phong_Tro\Backend\RestAPI_QUANLYPHONGTRO
dotnet run
```

### Kết quả mong đợi:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

## 📖 TEST VỚI SWAGGER

### Bước 1: Mở Swagger UI
Sau khi Backend chạy, mở browser:
```
https://localhost:5001/swagger
```

### Bước 2: Login để lấy Token

#### 2.1. Expand endpoint `/api/NguoiDung/login`
#### 2.2. Click "Try it out"
#### 2.3. Nhập request body:
```json
{
  "email": "admin@test.com",
  "password": "admin123"
}
```
#### 2.4. Click "Execute"
#### 2.5. Copy token từ response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Bước 3: Authorize Swagger

#### 3.1. Click nút **"Authorize"** (biểu tượng khóa) ở góc trên bên phải
#### 3.2. Paste token vào:
```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
⚠️ **Lưu ý:** Phải có từ "Bearer " (có dấu space) trước token!

#### 3.3. Click "Authorize" → "Close"

### Bước 4: Test Dashboard Endpoints

#### 4.1. Test `/api/dashboard/stats`
1. Expand endpoint
2. Click "Try it out"
3. Click "Execute"

**Expected Response:**
```json
{
  "totalRooms": 150,
  "pendingRooms": 12,
  "approvedRooms": 130,
  "lockedRooms": 8,
  "totalHosts": 45,
  "verifiedHosts": 38,
  "pendingHosts": 7,
  "totalTenants": 320,
  "newUsersThisMonth": 25,
  "pendingReports": 5,
  "monthlyRevenue": 125000000,
  "revenueGrowth": 12.5
}
```

#### 4.2. Test `/api/dashboard/rooms/monthly`
```
GET /api/dashboard/rooms/monthly?months=12
```

**Expected Response:**
```json
[
  {
    "month": "2024-02",
    "newRooms": 12,
    "approvedRooms": 10
  },
  {
    "month": "2024-03",
    "newRooms": 15,
    "approvedRooms": 13
  },
  ...
]
```

#### 4.3. Test `/api/dashboard/rooms/status-distribution`

**Expected Response:**
```json
{
  "approved": 130,
  "pending": 12,
  "rejected": 0,
  "locked": 8
}
```

#### 4.4. Test `/api/dashboard/rooms/pending`
```
GET /api/dashboard/rooms/pending?top=5
```

**Expected Response:**
```json
[
  {
    "phongId": "guid-here",
    "tieuDe": "Phòng trọ Q1 gần ĐH Văn Lang",
    "giaTien": 3500000,
    "chuTroName": "Chủ trọ",
    "createdAt": "2025-01-15T10:30:00+07:00"
  }
]
```

#### 4.5. Test `/api/dashboard/reports/recent`
```
GET /api/dashboard/reports/recent?top=5
```

#### 4.6. Test `/api/dashboard/activities/recent`
```
GET /api/dashboard/activities/recent?top=10
```

#### 4.7. Test `/api/dashboard/users/new-this-month`

**Expected Response:**
```
25
```

---

## 🔧 TEST VỚI POSTMAN / THUNDER CLIENT

### Setup
1. Download [Postman](https://www.postman.com/downloads/)
   Hoặc cài **Thunder Client** extension trong VS Code

### Create Collection: "Dashboard API Tests"

### Test 1: Login
```
POST https://localhost:5001/api/nguoidung/login
Content-Type: application/json

Body:
{
  "email": "admin@test.com",
  "password": "admin123"
}
```

**Save token** vào Environment variable: `{{token}}`

### Test 2: Get Stats
```
GET https://localhost:5001/api/dashboard/stats
Authorization: Bearer {{token}}
```

### Test 3: Get Monthly Stats
```
GET https://localhost:5001/api/dashboard/rooms/monthly?months=12
Authorization: Bearer {{token}}
```

### Test 4: Get Pending Rooms
```
GET https://localhost:5001/api/dashboard/rooms/pending?top=5
Authorization: Bearer {{token}}
```

### Test 5: Validation Test (Invalid months)
```
GET https://localhost:5001/api/dashboard/rooms/monthly?months=50
Authorization: Bearer {{token}}
```

**Expected:** 400 Bad Request
```json
{
  "message": "Số tháng phải từ 1 đến 24"
}
```

### Test 6: Unauthorized Test (No token)
```
GET https://localhost:5001/api/dashboard/stats
```

**Expected:** 401 Unauthorized

---

## 📊 TEST CASES MATRIX

| # | Endpoint | Method | Auth | Expected Status | Expected Data |
|---|----------|--------|------|-----------------|---------------|
| 1 | `/api/dashboard/stats` | GET | ✅ Admin | 200 OK | Stats object |
| 2 | `/api/dashboard/rooms/monthly?months=12` | GET | ✅ Admin | 200 OK | Array[12] |
| 3 | `/api/dashboard/rooms/status-distribution` | GET | ✅ Admin | 200 OK | Distribution object |
| 4 | `/api/dashboard/rooms/pending?top=5` | GET | ✅ Admin | 200 OK | Array[0-5] |
| 5 | `/api/dashboard/reports/recent?top=5` | GET | ✅ Admin | 200 OK | Array[0-5] |
| 6 | `/api/dashboard/activities/recent?top=10` | GET | ✅ Admin | 200 OK | Array[0-10] |
| 7 | `/api/dashboard/users/new-this-month` | GET | ✅ Admin | 200 OK | Integer |
| 8 | `/api/dashboard/stats` | GET | ❌ No Token | 401 | Unauthorized |
| 9 | `/api/dashboard/rooms/monthly?months=50` | GET | ✅ Admin | 400 | Error message |
| 10 | `/api/dashboard/rooms/pending?top=200` | GET | ✅ Admin | 400 | Error message |

---

## 🔍 VERIFY DỮ LIỆU

### Kiểm tra trong SQL
```sql
-- 1. Total Rooms
SELECT COUNT(*) AS TotalRooms 
FROM Phong 
WHERE IsDeleted = 0;

-- 2. Pending Rooms
SELECT COUNT(*) AS PendingRooms 
FROM Phong 
WHERE IsDeleted = 0 AND IsDuyet = 0;

-- 3. Verified Hosts
SELECT COUNT(*) AS VerifiedHosts 
FROM ChuTroThongTinPhapLy 
WHERE TrangThaiXacThuc = 'DaDuyet';

-- 4. Monthly Revenue
SELECT SUM(SoTien) AS MonthlyRevenue
FROM BienLai
WHERE DaXacNhan = 1
  AND ThoiGianTai >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

-- 5. Pending Reports
SELECT COUNT(*) AS PendingReports
FROM BaoCaoViPham
WHERE TrangThai IN ('CHO_XU_LY', 'DANG_XU_LY');
```

### So sánh kết quả
- API response **phải khớp** với SQL query results
- Nếu khác → Check logic trong DashboardService

---

## 🐛 TROUBLESHOOTING

### Lỗi: Cannot connect to SQL Server
**Giải pháp:**
1. Check SQL Server đang chạy:
   ```
   services.msc → SQL Server (MVY_350)
   ```
2. Check connection string trong `appsettings.json`
3. Test connection trong SSMS

### Lỗi: 401 Unauthorized
**Nguyên nhân:**
- Token không hợp lệ
- Token hết hạn (7 ngày)
- Thiếu "Bearer " prefix

**Giải pháp:**
- Login lại để lấy token mới
- Check format: `Bearer {token}`

### Lỗi: 403 Forbidden
**Nguyên nhân:**
- User không phải Admin (VaiTroId ≠ 1)

**Giải pháp:**
```sql
-- Update user thành admin
UPDATE NguoiDung 
SET VaiTroId = 1 
WHERE Email = 'your@email.com';
```

### Lỗi: 500 Internal Server Error
**Nguyên nhân:**
- Database query failed
- Null reference exception

**Giải pháp:**
1. Check Backend console logs
2. Check Output window trong Visual Studio
3. Debug step-by-step trong DashboardService

### API trả về dữ liệu = 0
**Nguyên nhân:**
- Database chưa có data

**Giải pháp:**
- Insert sample data vào các bảng
- Verify với SQL queries ở trên

---

## ✅ SUCCESS CRITERIA

### API Test Pass khi:
1. ✅ Tất cả endpoints return 200 OK
2. ✅ Response format đúng với DTO definition
3. ✅ Dữ liệu khớp với database
4. ✅ Authorization working (401 when no token)
5. ✅ Validation working (400 for invalid params)
6. ✅ No 500 errors

### Dashboard Ready khi:
1. ✅ Backend API test pass
2. ✅ Frontend có thể gọi API
3. ✅ Charts hiển thị dữ liệu thật
4. ✅ No errors trong console

---

## 📝 SAMPLE TEST RESULTS

### Test Log Template:
```
=== DASHBOARD API TEST RESULTS ===
Date: 2025-01-XX
Tester: [Your Name]

[✅] GET /api/dashboard/stats
Response Time: 250ms
Status: 200 OK
Data: Valid

[✅] GET /api/dashboard/rooms/monthly
Response Time: 180ms
Status: 200 OK
Data: 12 months returned

[✅] GET /api/dashboard/rooms/status-distribution
Response Time: 120ms
Status: 200 OK
Data: All fields present

[✅] GET /api/dashboard/rooms/pending
Response Time: 90ms
Status: 200 OK
Data: 5 rooms returned

[✅] GET /api/dashboard/reports/recent
Response Time: 85ms
Status: 200 OK
Data: 5 reports returned

[✅] GET /api/dashboard/activities/recent
Response Time: 110ms
Status: 200 OK
Data: 10 activities returned

[✅] GET /api/dashboard/users/new-this-month
Response Time: 50ms
Status: 200 OK
Data: Integer value

=== SUMMARY ===
Total Tests: 7
Passed: 7
Failed: 0
Success Rate: 100%

=== NOTES ===
- All endpoints working correctly
- Response times acceptable (<300ms)
- Data validated against database
- Ready for frontend integration
```

---

## 🚀 NEXT STEPS

Sau khi test xong:
1. ✅ Document kết quả test
2. ✅ Run Frontend Admin project
3. ✅ Test Dashboard UI với data thật
4. ✅ Verify charts rendering correctly

---

**Good luck with testing!** 🎉
