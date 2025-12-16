# ✅ DASHBOARD API - READY TO TEST!

## 📊 TÓM TẮT

Tất cả files đã được tạo và Backend đã build thành công!

---

## 📁 FILES CREATED FOR TESTING

### 1. Documentation
- ✅ `TESTING_GUIDE.md` - Hướng dẫn test chi tiết từng bước
- ✅ `DASHBOARD_API_COMPLETE.md` - Technical documentation

### 2. Database
- ✅ `SampleData.sql` - Script tạo sample data cho testing

### 3. API Testing
- ✅ `Dashboard_API_Tests.postman_collection.json` - Postman collection
- ✅ `Start-Backend.ps1` - Quick start script

---

## 🚀 3 CÁCH ĐỂ TEST

### CÁCH 1: SWAGGER UI ⭐ (Dễ nhất - Đề xuất)

#### Bước 1: Chạy Backend
```powershell
# Option A: PowerShell
cd Backend\RestAPI_QUANLYPHONGTRO
.\Start-Backend.ps1

# Option B: Visual Studio
Right-click RestAPI_QUANLYPHONGTRO project → Set as Startup Project → F5
```

#### Bước 2: Mở Swagger
```
https://localhost:5001/swagger
```

#### Bước 3: Test từng endpoint
1. Login → Copy token
2. Click "Authorize" → Paste token
3. Test các endpoints:
   - ✅ GET /api/dashboard/stats
   - ✅ GET /api/dashboard/rooms/monthly
   - ✅ GET /api/dashboard/rooms/pending
   - ...

---

### CÁCH 2: POSTMAN

#### Import Collection
1. Mở Postman
2. Import file: `Dashboard_API_Tests.postman_collection.json`
3. Run Collection
4. View Test Results

---

### CÁCH 3: CURL (Command Line)

#### Login
```bash
curl -X POST https://localhost:5001/api/nguoidung/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"admin123"}' \
  -k
```

#### Get Stats (Replace {TOKEN})
```bash
curl -X GET https://localhost:5001/api/dashboard/stats \
  -H "Authorization: Bearer {TOKEN}" \
  -k
```

---

## 📋 CHECKLIST TRƯỚC KHI TEST

### Database:
- [ ] SQL Server đang chạy
- [ ] Database `QuanLyPhongTro` tồn tại
- [ ] Có admin user (admin@test.com)
- [ ] Có sample data (chạy SampleData.sql nếu cần)

### Backend:
- [ ] .NET 8 SDK đã cài
- [ ] Build successful
- [ ] Server đang chạy (port 5001)

### Tools:
- [ ] Browser (Chrome/Edge)
- [ ] Postman (optional)
- [ ] SQL Server Management Studio

---

## 🧪 TEST CASES

| # | Endpoint | Expected Result |
|---|----------|-----------------|
| 1 | `/api/dashboard/stats` | 200 OK + Stats object |
| 2 | `/api/dashboard/rooms/monthly` | 200 OK + Array[12] |
| 3 | `/api/dashboard/rooms/status-distribution` | 200 OK + Distribution |
| 4 | `/api/dashboard/rooms/pending` | 200 OK + Array[0-5] |
| 5 | `/api/dashboard/reports/recent` | 200 OK + Array[0-5] |
| 6 | `/api/dashboard/activities/recent` | 200 OK + Array[0-10] |
| 7 | `/api/dashboard/users/new-this-month` | 200 OK + Integer |

---

## ✅ SUCCESS CRITERIA

### API Pass khi:
- ✅ All endpoints return 200 OK
- ✅ Response format match DTOs
- ✅ Data valid (not all zeros)
- ✅ Authorization working (401 without token)
- ✅ Validation working (400 for invalid params)

---

## 📊 SAMPLE RESPONSES

### GET /api/dashboard/stats
```json
{
  "totalRooms": 9,
  "pendingRooms": 3,
  "approvedRooms": 5,
  "lockedRooms": 1,
  "totalHosts": 3,
  "verifiedHosts": 2,
  "pendingHosts": 1,
  "totalTenants": 3,
  "newUsersThisMonth": 1,
  "pendingReports": 3,
  "monthlyRevenue": 0,
  "revenueGrowth": 0
}
```

### GET /api/dashboard/rooms/monthly?months=12
```json
[
  {
    "month": "2024-02",
    "newRooms": 0,
    "approvedRooms": 0
  },
  {
    "month": "2024-03",
    "newRooms": 0,
    "approvedRooms": 0
  },
  ...
  {
    "month": "2025-01",
    "newRooms": 9,
    "approvedRooms": 5
  }
]
```

### GET /api/dashboard/rooms/pending?top=5
```json
[
  {
    "phongId": "guid-here",
    "tieuDe": "Phòng 203 - Chờ duyệt",
    "giaTien": 3200000,
    "chuTroName": "Chủ trọ",
    "createdAt": "2025-01-20T..."
  },
  ...
]
```

---

## 🐛 TROUBLESHOOTING

### Lỗi: Cannot connect to database
```sql
-- Check SQL Server đang chạy
services.msc → SQL Server (MVY_350)

-- Test connection trong SSMS
Server: LAPTOP-SGLHG0M9\MVY_350
Database: QuanLyPhongTro
```

### Lỗi: 401 Unauthorized
```
1. Login lại để lấy token mới
2. Check format: "Bearer {token}" (có space)
3. Token hết hạn sau 7 ngày
```

### Lỗi: Build failed
```powershell
# Restore packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build
```

---

## 📞 NEXT STEPS

### Sau khi API test pass:

#### 1. Test Frontend Integration
```powershell
# Giữ Backend đang chạy
# Mở terminal mới và run Admin Frontend
cd ADMIN_QUANLYPHONGTRO
# F5 trong Visual Studio
```

#### 2. Login Admin
```
URL: http://localhost:xxxxx/Auth/Login
Email: admin@test.com
Password: admin123
```

#### 3. Navigate to Dashboard
```
URL: http://localhost:xxxxx/Dashboard
```

#### 4. Verify
- ✅ Stats cards hiển thị số đúng
- ✅ Charts render với data
- ✅ Tables hiển thị pending items
- ✅ No errors trong console

---

## 📚 DOCUMENTATION

### Full guides:
1. **TESTING_GUIDE.md** - Chi tiết từng bước test
2. **DASHBOARD_API_COMPLETE.md** - Technical specs
3. **SampleData.sql** - Database setup

### Import vào Postman:
- **Dashboard_API_Tests.postman_collection.json**

---

## 🎯 CURRENT STATUS

### ✅ COMPLETED:
- [x] Backend API implementation
- [x] Frontend integration layer
- [x] Test documentation
- [x] Sample data script
- [x] Postman collection
- [x] Quick start scripts

### 🔄 TODO:
- [ ] Run Backend
- [ ] Test API với Swagger
- [ ] Verify data
- [ ] Test Frontend Dashboard

---

## 🚀 START TESTING NOW!

### Quick Start Command:
```powershell
cd Backend\RestAPI_QUANLYPHONGTRO
.\Start-Backend.ps1
```

### Then open:
```
https://localhost:5001/swagger
```

---

**Good luck!** 🎉

**Need help?** Check `TESTING_GUIDE.md` for detailed instructions.
