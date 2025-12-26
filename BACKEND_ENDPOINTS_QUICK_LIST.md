# 📝 QUICK ENDPOINTS SUMMARY

## 🎯 56 ENDPOINTS TOTAL

### ✅ PUBLIC (19 endpoints)

**PHÒNG**
- `GET /api/phong` - Danh sách phòng
- `GET /api/phong/{id}` - Chi tiết phòng

**NHÀ TRỌ**
- `GET /api/nhatro` - Danh sách
- `GET /api/nhatro/{id}` - Chi tiết

**TIỆN ÍCH**
- `GET /api/tienich` - Danh sách
- `GET /api/tienich/{id}` - Chi tiết

**QUẬN HUYỆN**
- `GET /api/quanhuyen` - Danh sách
- `GET /api/quanhuyen/{id}` - Chi tiết

**PHƯỜNG**
- `GET /api/phuong` - Danh sách
- `GET /api/phuong/{id}` - Chi tiết
- `GET /api/phuong/by-quan/{quanId}` - Theo quận

**TRẠNG THÁI ĐẶT PHÒNG**
- `GET /api/trangthaídatphong` - Danh sách
- `GET /api/trangthaídatphong/{id}` - Chi tiết

**CÀI ĐẶT HỆ THỐNG**
- `GET /api/systemsettings` - Danh sách
- `GET /api/systemsettings/by-group/{groupName}` - Theo nhóm
- `GET /api/systemsettings/{key}` - Theo khóa

---

### 🔐 AUTHORIZATION REQUIRED (22 endpoints)

**PHÒNG**
- `POST /api/phong` - Tạo (ChuTro)
- `PUT /api/phong/{id}` - Cập nhật (ChuTro)

**NHÀ TRỌ**
- `GET /api/nhatro/my-houses` - Của tôi
- `POST /api/nhatro` - Tạo
- `PUT /api/nhatro/{id}` - Cập nhật
- `DELETE /api/nhatro/{id}` - Xóa

**PHÒNG TIỆN ÍCH**
- `GET /api/phongtienich/{phongId}` - Danh sách
- `POST /api/phongtienich` - Thêm
- `DELETE /api/phongtienich` - Xóa

**ĐẶT PHÒNG**
- `POST /api/datphong` - Tạo
- `GET /api/datphong/my-bookings` - Của tôi

**HÓA ĐƠN**
- `GET /api/hoadon/nguoithue/{userId}` - Của người thuê
- `GET /api/hoadon` - Danh sách
- `GET /api/hoadon/{id}` - Chi tiết
- `POST /api/hoadon/{id}/thanhtoan` - Thanh toán

**TIN NHẮN**
- `POST /api/tinnhan` - Gửi
- `GET /api/tinnhan/conversation/{otherUserId}` - Hội thoại
- `PUT /api/tinnhan/read/{otherUserId}` - Đánh dấu đã đọc

**CHỦ TRỌ THÔNG TIN PHÁP LÝ**
- `GET /api/chutrothongtinphapły/me` - Thông tin của tôi
- `POST /api/chutrothongtinphapły` - Cập nhật (Upsert)

---

### 👮 ADMIN ONLY (18 endpoints)

**PHÒNG**
- `GET /api/phong/pending` - Danh sách chờ duyệt
- `PUT /api/phong/approve/{id}` - Duyệt
- `PUT /api/phong/{id}/reject` - Từ chối
- `PUT /api/phong/lock/{id}` - Khóa/Mở khóa
- `GET /api/phong/stats` - Thống kê

**TIỆN ÍCH**
- `POST /api/tienich` - Tạo
- `PUT /api/tienich/{id}` - Cập nhật
- `DELETE /api/tienich/{id}` - Xóa

**QUẬN HUYỆN**
- `POST /api/quanhuyen` - Tạo
- `PUT /api/quanhuyen/{id}` - Cập nhật
- `DELETE /api/quanhuyen/{id}` - Xóa

**PHƯỜNG**
- `POST /api/phuong` - Tạo
- `PUT /api/phuong/{id}` - Cập nhật
- `DELETE /api/phuong/{id}` - Xóa

**CHỦ TRỌ THÔNG TIN PHÁP LÝ**
- `PUT /api/chutrothongtinphapły/approve/{userId}` - Duyệt

**TRẠNG THÁI ĐẶT PHÒNG**
- `PUT /api/trangthaídatphong/{id}` - Cập nhật

**CÀI ĐẶT HỆ THỐNG**
- `POST /api/systemsettings` - Tạo
- `PUT /api/systemsettings/{id}` - Cập nhật
- `POST /api/systemsettings/update-by-key` - Cập nhật theo khóa
- `DELETE /api/systemsettings/{id}` - Xóa

---

## 📊 PHÂN LOẠI

| Loại | Số lượng |
|------|---------|
| GET | 25 |
| POST | 15 |
| PUT | 12 |
| DELETE | 4 |

---

## 🎯 PHÂN QUYỀN

| Quyền | Endpoints |
|-------|-----------|
| Public | 19 |
| Cần Token | 22 |
| Admin Only | 15 |
| Total | 56 |

---

## 📝 DANH SÁCH CONTROLLERS

1. **PhongController** - 8 endpoints
2. **NhaTroController** - 7 endpoints
3. **TienIchController** - 5 endpoints
4. **PhongTienIchController** - 3 endpoints
5. **QuanHuyenController** - 5 endpoints
6. **PhuongController** - 6 endpoints
7. **DatPhongController** - 2 endpoints
8. **HoaDonController** - 3 endpoints
9. **TinNhanController** - 3 endpoints
10. **ChuTroThongTinPhapLyController** - 3 endpoints
11. **TrangThaiDatPhongController** - 3 endpoints
12. **SystemSettingsController** - 7 endpoints

---

## 📌 HTTP METHODS

### GET (25 endpoints)
- Lấy danh sách
- Lấy chi tiết
- Lấy thống kê

### POST (15 endpoints)
- Tạo dữ liệu mới
- Gửi tin nhắn
- Thanh toán

### PUT (12 endpoints)
- Cập nhật dữ liệu
- Duyệt/Từ chối
- Khóa/Mở khóa

### DELETE (4 endpoints)
- Xóa dữ liệu

---

## 🔗 LIÊN KẾT TÀI LIỆU

- `BACKEND_API_ALL_ENDPOINTS.md` - Danh sách chi tiết
- `BACKEND_API_DOCUMENTATION.md` - Tài liệu đầy đủ
- `BACKEND_API_EXAMPLES.md` - Ví dụ cụ thể
- `BACKEND_API_SUMMARY.md` - Tóm tắt
- `BACKEND_API_QUICK_REFERENCE.md` - Quick reference

---

**Last Updated:** 2024
**Status:** COMPLETE ✅
