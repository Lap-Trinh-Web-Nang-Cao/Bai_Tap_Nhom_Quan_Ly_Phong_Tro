# 📚 DANH SÁCH TẤT CẢ API ENDPOINTS - BACKEND .NET 8

**API Base URL:** `http://18.140.64.80:5000`

---

## 📋 MỤC LỤC

1. [Phòng (Phong)](#-phòng-phong)
2. [Nhà Trọ (NhaTro)](#-nhà-trọ-nhatropublic-chủ-trọ)
3. [Tiện Ích (TienIch)](#-tiện-ích-tienich)
4. [Phòng Tiện Ích (PhongTienIch)](#-phòng-tiện-ích-phongtienich)
5. [Quận Huyện (QuanHuyen)](#-quận-huyện-quanhuyen)
6. [Phường (Phuong)](#-phường-phuong)
7. [Đặt Phòng (DatPhong)](#-đặt-phòng-datphong)
8. [Hóa Đơn (HoaDon)](#-hóa-đơn-hoadon)
9. [Tin Nhắn (TinNhan)](#-tin-nhắn-tinnhan)
10. [Chủ Trọ Thông Tin Pháp Lý (ChuTroThongTinPhapLy)](#-chủ-trọ-thông-tin-pháp-lý-chutrothongtinphapły)
11. [Trạng Thái Đặt Phòng (TrangThaiDatPhong)](#-trạng-thái-đặt-phòng-trangthaídatphong)
12. [Cài Đặt Hệ Thống (SystemSettings)](#-cài-đặt-hệ-thống-systemsettings)

---

## 🏠 **Phòng (Phong)**

### PUBLIC ENDPOINTS
```
GET    /api/phong                  → Danh sách phòng (phân trang, lọc)
       Query: ?nhaTroId={guid}&minPrice={long}&maxPrice={long}&page={int}&pageSize={int}
       Response: { success, Data[], TotalCount, Page, PageSize, TotalPages }

GET    /api/phong/{id}             → Chi tiết phòng
       Response: { success, Data{...} }
```

### CHỦ TRỌ ENDPOINTS (Authorization: Bearer {token}, Policy: ChuTroOnly)
```
POST   /api/phong                  → Tạo phòng mới
       Body: { tieuDe, moTa, dienTich, giaTien, tienCoc, soNguoiToiDa, ... }

PUT    /api/phong/{id}             → Cập nhật phòng
       Body: { tieuDe, moTa, dienTich, ... }
```

### ADMIN ENDPOINTS (Authorization: Bearer {token}, Policy: AdminOnly)
```
GET    /api/phong/pending          → Danh sách phòng chờ duyệt
       Query: ?pageIndex={int}&pageSize={int}&keyword={string}
       Response: { success, Data[], TotalCount, PageIndex, PageSize, TotalPages }

PUT    /api/phong/approve/{id}     → Duyệt phòng
       Response: { success, message, data }

PUT    /api/phong/{id}/reject      → Từ chối phòng
       Body: { reason }
       Response: { success, message, data }

PUT    /api/phong/lock/{id}        → Khóa/Mở khóa phòng
       Query: ?isLocked={bool}
       Response: { success, message, data }

GET    /api/phong/stats            → Lấy thống kê phòng
       Response: { success, message, data: { total, pending, approved, locked } }
```

---

## 🏢 **Nhà Trọ (NhaTro) - PUBLIC, Chủ Trọ**

### PUBLIC ENDPOINTS
```
GET    /api/nhatro                 → Danh sách nhà trọ hoạt động
       Response: List<NhaTroDto>

GET    /api/nhatro/{id}            → Chi tiết nhà trọ
       Response: NhaTroDto
```

### CHỦ TRỌ ENDPOINTS (Authorization Required)
```
GET    /api/nhatro/my-houses       → Danh sách nhà trọ của tôi
       Response: List<NhaTroDto>

POST   /api/nhatro                 → Tạo nhà trọ mới
       Body: { tieuDe, diaChi, phuongId, quanId, ... }
       Response: NhaTroDto

PUT    /api/nhatro/{id}            → Cập nhật nhà trọ
       Body: { tieuDe, diaChi, ... }
       Response: NhaTroDto

DELETE /api/nhatro/{id}            → Xóa nhà trọ (Admin hoặc chính chủ)
       Response: 204 No Content
```

---

## 🛎️ **Tiện Ích (TienIch)**

### PUBLIC ENDPOINTS
```
GET    /api/tienich                → Danh sách tiện ích
       Response: { success, data: List<TienIchDto> }

GET    /api/tienich/{id}           → Chi tiết tiện ích
       Response: { success, data: TienIchDto }
```

### ADMIN ENDPOINTS (Authorize, Policy: AdminOnly)
```
POST   /api/tienich                → Tạo tiện ích
       Body: { ten, icon, ... }
       Response: { success, data }

PUT    /api/tienich/{id}           → Cập nhật tiện ích
       Body: { ten, icon, ... }
       Response: { success, data }

DELETE /api/tienich/{id}           → Xóa tiện ích
       Response: { success, message }
```

---

## 🔗 **Phòng Tiện Ích (PhongTienIch)**

### PUBLIC ENDPOINTS
```
GET    /api/phongtienich/{phongId} → Danh sách tiện ích của phòng
       Response: List<PhongTienIchDto>
```

### CHỦ TRỌ ENDPOINTS (Authorize)
```
POST   /api/phongtienich           → Thêm tiện ích vào phòng
       Body: { phongId, tienIchId }
       Response: { message }

DELETE /api/phongtienich           → Xóa tiện ích khỏi phòng
       Query: ?phongId={guid}&tienIchId={int}
       Response: { message }
```

---

## 📍 **Quận Huyện (QuanHuyen)**

### PUBLIC ENDPOINTS
```
GET    /api/quanhuyen              → Danh sách quận huyện
       Response: List<QuanHuyenDto>

GET    /api/quanhuyen/{id}         → Chi tiết quận huyện
       Response: QuanHuyenDto
```

### ADMIN ENDPOINTS (Authorize, Roles: Admin)
```
POST   /api/quanhuyen              → Tạo quận huyện
       Body: { tenQuan, ... }

PUT    /api/quanhuyen/{id}         → Cập nhật quận huyện
       Body: { tenQuan, ... }

DELETE /api/quanhuyen/{id}         → Xóa quận huyện
       Response: 204 No Content
```

---

## 🏘️ **Phường (Phuong)**

### PUBLIC ENDPOINTS
```
GET    /api/phuong                 → Danh sách phường
       Response: List<PhuongDto>

GET    /api/phuong/{id}            → Chi tiết phường
       Response: PhuongDto

GET    /api/phuong/by-quan/{quanId} → Phường theo quận
       Response: List<PhuongDto>
```

### ADMIN ENDPOINTS (Authorize, Roles: Admin)
```
POST   /api/phuong                 → Tạo phường
       Body: { tenPhuong, quanId, ... }

PUT    /api/phuong/{id}            → Cập nhật phường
       Body: { tenPhuong, quanId, ... }

DELETE /api/phuong/{id}            → Xóa phường
       Response: 204 No Content
```

---

## 📅 **Đặt Phòng (DatPhong)**

### PUBLIC/USER ENDPOINTS
```
POST   /api/datphong               → Tạo đặt phòng (xem phòng hoặc booking)
       Body: { phongId, chuTroId, loai, batDau, ghiChu }
       Response: DatPhongDto

GET    /api/datphong/my-bookings   → Danh sách đặt phòng của tôi (Authorization)
       Response: List<DatPhongDto>
```

### ADMIN ENDPOINTS
```
Các endpoints admin quản lý đặt phòng (có thể có thêm)
```

---

## 💰 **Hóa Đơn (HoaDon)**

### USER ENDPOINTS
```
GET    /api/hoadon/nguoithue/{userId} → Danh sách hóa đơn của người thuê
       Response: { Success, Data: List<HoaDonDto>, Message }
```

### HÓA ĐƠN ENDPOINTS
```
GET    /api/hoadon                 → Lấy tất cả hóa đơn
       Response: { Success, Data, Message }

GET    /api/hoadon/{id}            → Chi tiết hóa đơn
       Response: { Success, Data, Message }

POST   /api/hoadon/{id}/thanhtoan  → Thanh toán hóa đơn
       Response: { Success, Message }
```

---

## 💬 **Tin Nhắn (TinNhan)**

### USER ENDPOINTS (Authorization Required)
```
POST   /api/tinnhan                → Gửi tin nhắn
       Body: { toUser, noiDung }
       Response: TinNhanDto

GET    /api/tinnhan/conversation/{otherUserId} → Lấy hội thoại với 1 người
       Response: List<TinNhanDto>

PUT    /api/tinnhan/read/{otherUserId} → Đánh dấu đã đọc
       Response: { message }
```

---

## 📋 **Chủ Trọ Thông Tin Pháp Lý (ChuTroThongTinPhapLy)**

### CHỦ TRỌ ENDPOINTS (Authorization Required)
```
GET    /api/chutrothongtinphapły/me → Lấy thông tin pháp lý của tôi
       Response: ChuTroThongTinPhapLyDto

POST   /api/chutrothongtinphapły   → Cập nhật thông tin pháp lý (Upsert)
       Body: { cmnd, diaChi, ngaySinh, ... }
       Response: ChuTroThongTinPhapLyDto
```

### ADMIN ENDPOINTS (Authorization, Roles: Admin)
```
PUT    /api/chutrothongtinphapły/approve/{userId} → Duyệt hồ sơ pháp lý
       Query: ?status={string}
       Response: { message }
```

---

## 📊 **Trạng Thái Đặt Phòng (TrangThaiDatPhong)**

### PUBLIC ENDPOINTS
```
GET    /api/trangthaídatphong      → Danh sách trạng thái đặt phòng
       Response: List<TrangThaiDatPhongDto>

GET    /api/trangthaídatphong/{id} → Chi tiết trạng thái
       Response: TrangThaiDatPhongDto
```

### ADMIN ENDPOINTS (Authorization, Roles: Admin)
```
PUT    /api/trangthaídatphong/{id} → Cập nhật trạng thái
       Body: { ten, ... }
       Response: TrangThaiDatPhongDto
```

---

## ⚙️ **Cài Đặt Hệ Thống (SystemSettings)**

### PUBLIC ENDPOINTS
```
GET    /api/systemsettings         → Lấy tất cả cài đặt
       Response: { success, data: List<SystemSettingDto> }

GET    /api/systemsettings/by-group/{groupName} → Cài đặt theo nhóm
       Response: { success, data }

GET    /api/systemsettings/{key}   → Cài đặt theo khóa
       Response: { success, data }
```

### ADMIN ENDPOINTS
```
POST   /api/systemsettings         → Tạo cài đặt
       Body: { settingKey, settingValue, groupName, ... }

PUT    /api/systemsettings/{id}    → Cập nhật cài đặt
       Body: { settingKey, settingValue, groupName, ... }

POST   /api/systemsettings/update-by-key → Cập nhật cài đặt theo khóa
       Body: { key1: value1, key2: value2, ... }

DELETE /api/systemsettings/{id}    → Xóa cài đặt
```

---

## 🔐 **AUTHORIZATION**

### Không Cần Token (AllowAnonymous)
- `GET /api/phong`
- `GET /api/phong/{id}`
- `GET /api/nhatro`
- `GET /api/nhatro/{id}`
- `GET /api/tienich`
- `GET /api/tienich/{id}`
- `GET /api/quanhuyen`
- `GET /api/quanhuyen/{id}`
- `GET /api/phuong`
- `GET /api/phuong/{id}`
- `GET /api/phuong/by-quan/{id}`
- `GET /api/trangthaídatphong`
- `GET /api/trangthaídatphong/{id}`
- `GET /api/systemsettings/*`

### Cần Token (Authorization: Bearer {token})
- Hầu hết các endpoint `POST`, `PUT`, `DELETE`
- `GET /api/nhatro/my-houses`
- `GET /api/datphong/my-bookings`
- `GET /api/hoadon/nguoithue/{userId}`
- `GET /api/tinnhan/conversation/{otherUserId}`
- `PUT /api/tinnhan/read/{otherUserId}`
- `GET /api/chutrothongtinphapły/me`

### Admin Only (Policy: AdminOnly)
- `/api/phong/pending`
- `/api/phong/approve/{id}`
- `/api/phong/{id}/reject`
- `/api/phong/lock/{id}`
- `/api/phong/stats`
- `/api/tienich` (POST, PUT, DELETE)
- `/api/quanhuyen` (POST, PUT, DELETE)
- `/api/phuong` (POST, PUT, DELETE)
- `/api/chutrothongtinphapły/approve/{userId}`
- `/api/trangthaídatphong` (PUT)
- `/api/systemsettings/*` (POST, PUT, DELETE)

### Chủ Trọ Only (Policy: ChuTroOnly)
- `POST /api/phong` (Tạo phòng)
- `PUT /api/phong/{id}` (Cập nhật phòng)
- `GET /api/nhatro/my-houses`
- `POST /api/nhatro` (Tạo nhà trọ)
- `PUT /api/nhatro/{id}` (Cập nhật nhà trọ)
- `POST /api/phongtienich` (Thêm tiện ích)
- `DELETE /api/phongtienich` (Xóa tiện ích)

---

## 📊 **RESPONSE FORMAT**

### Success Response
```json
{
  "success": true,
  "Data": [...],          // Uppercase 'D'
  "TotalCount": 100,      // Nếu có phân trang
  "Page": 1,
  "PageSize": 10,
  "TotalPages": 10,
  "message": "Success"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Chi tiết lỗi",
  "data": null
}
```

---

## 🧪 **CURL EXAMPLES**

### Lấy danh sách phòng
```bash
curl -X GET "http://18.140.64.80:5000/api/phong?page=1&pageSize=10"
```

### Duyệt phòng (Admin)
```bash
curl -X PUT "http://18.140.64.80:5000/api/phong/approve/{phongId}" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### Tạo phòng (Chủ trọ)
```bash
curl -X POST "http://18.140.64.80:5000/api/phong" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tieuDe": "Phòng 101",
    "moTa": "Phòng đẹp, thoáng mát",
    "dienTich": 25,
    "giaTien": 3000000,
    "tienCoc": 3000000,
    "soNguoiToiDa": 2,
    "nhaTroId": "12345678-1234-1234-1234-123456789012"
  }'
```

---

## 📌 **TỔNG HỢP**

| Controller | Endpoints | Public | Auth | Admin | ChuTro |
|-----------|-----------|--------|------|-------|--------|
| Phong | 8 | 2 | 2 | 4 | 2 |
| NhaTro | 7 | 2 | 5 | 1 | 1 |
| TienIch | 5 | 2 | 3 | 3 | 0 |
| PhongTienIch | 3 | 1 | 2 | 0 | 2 |
| QuanHuyen | 5 | 3 | 0 | 2 | 0 |
| Phuong | 6 | 4 | 0 | 2 | 0 |
| DatPhong | 2 | 0 | 2 | 0 | 0 |
| HoaDon | 3 | 0 | 3 | 0 | 0 |
| TinNhan | 3 | 0 | 3 | 0 | 0 |
| ChuTroThongTinPhapLy | 3 | 0 | 2 | 1 | 2 |
| TrangThaiDatPhong | 3 | 2 | 0 | 1 | 0 |
| SystemSettings | 7 | 3 | 0 | 4 | 0 |
| **TOTAL** | **56** | **19** | **22** | **18** | **7** |

---

**Status: COMPLETE DOCUMENTATION** ✅
