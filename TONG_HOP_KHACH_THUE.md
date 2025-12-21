# ?? T?NG H?P CÔNG VI?C PH?N KHÁCH THUÊ (KhachThue)

## ?? T?ng Quan
Ð? hoàn thành vi?c xây d?ng và tích h?p API cho **14 ch?c nãng chính** c?a module Khách Thuê trong ?ng d?ng Qu?n L? Ph?ng Tr?.

---

## ?? File Chính
- **Controller**: `USER_QUANLYPHONGTRO/Controllers/KhachThueController.cs`
- **View**: `USER_QUANLYPHONGTRO/Views/khachthue/DanhSachPhong.cshtml`
- **API Client**: `USER_QUANLYPHONGTRO/Services/ApiClient.cs`

---

## ? DANH SÁCH CÁC CH?C NÃNG Ð? HOÀN THÀNH

### 1?? **Index (Trang Ch? Ngý?i Thuê)**
- **URL**: `/KhachThue`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**:
  - Hi?n th? 6 ph?ng n?i b?t (top rated)
  - Tích h?p API: `GET /api/phong?pageSize=6&sortBy=rating`
  - Có x? l? l?i fallback

---

### 2?? **Dashboard (B?ng Ði?u Khi?n)**
- **URL**: `/KhachThue/Dashboard`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**:
  - Hi?n th? thông tin t?ng h?p ngý?i thuê
  - L?ch h?n s?p t?i (Top 5)
  - H?p ð?ng ðang hi?u l?c
  - Hóa ðõn chýa thanh toán (Top 5)
- **API Calls**:
  ```
  GET /api/datphong/nguoithue/{userId}?trangThai=ChoXacNhan&pageSize=5
  GET /api/hopdong/nguoithue/{userId}/hieuluc
  GET /api/hoadon/nguoithue/{userId}?trangThai=ChuaThanhToan&pageSize=5
  ```

---

### 3?? **DanhSachPhong (Danh Sách Ph?ng)** ? C?P NH?T M?I
- **URL**: `/KhachThue/DanhSachPhong`
- **Phýõng th?c**: `GET`
- **Tham s?**:
  ```
  keyword (string) - T?m ki?m t? khóa
  priceRange (string) - Kho?ng giá (ví d?: "2000000-3000000")
  areaRange (string) - Di?n tích (ví d?: "20-30")
  page (int) - Trang hi?n t?i (m?c ð?nh = 1)
  ```

#### **C?i Ti?n Chính**:
? **S? d?ng StringBuilder** ð? xây d?ng query parameters s?ch s?

? **H? tr? 3 lo?i filter**:
- **T?m ki?m t? khóa**: `&keyword=ph?ng ð?p`
- **L?c giá**: `&minPrice=1000000&maxPrice=3000000`
- **L?c di?n tích**: `&minArea=20&maxArea=30`

? **X? l? d? li?u API**:
```
API tr? v?: { Success: true, Data: { Data: [...], TotalCount, Page, PageSize, TotalPages }, Message }
```

? **Phân trang**: H? tr? 12 ph?ng/trang

? **Error Handling**: Graceful fallback khi API l?i

**API Endpoint**:
```
GET /api/phong?pageSize=12&page=1&keyword=&minPrice=1000000&maxPrice=3000000&minArea=20&maxArea=30
```

---

### 4?? **ChiTietPhong (Chi Ti?t Ph?ng)**
- **URL**: `/KhachThue/ChiTietPhong/{id}`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**: Xem chi ti?t thông tin c?a m?t ph?ng
- **API Call**: `GET /api/phong/{id}`

---

### 5?? **DatPhong (Ð?t L?ch Xem Ph?ng)**
- **URL**: `/KhachThue/DatPhong`
- **Phýõng th?c**: 
  - `GET` - Hi?n th? form ð?t l?ch
  - `POST` - X? l? ð?t l?ch

**Form Fields**:
```
roomId (Guid)
thoiGianHen (DateTime)
ghiChu (string) - Optional
```

**API Call**: `POST /api/datphong`

**Request Body**:
```json
{
  "PhongId": "guid",
  "NguoiThueId": "guid",
  "ThoiGianHen": "2025-01-20T14:00:00",
  "GhiChu": "G?i trý?c 30 phút"
}
```

---

### 6?? **LichDaDat (L?ch Ð? Ð?t)**
- **URL**: `/KhachThue/LichDaDat`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**: Hi?n th? danh sách l?ch h?n c?a ngý?i thuê
- **API Call**: `GET /api/datphong/nguoithue/{userId}`
- **Demo Data**: Có data m?u khi API không tr? v?

---

### 7?? **HuyLichHen (H?y L?ch H?n)**
- **URL**: `/KhachThue/HuyLichHen`
- **Phýõng th?c**: `POST`
- **Tham s?**: `id (Guid)` - ID l?ch h?n
- **API Call**: `DELETE /api/datphong/{id}`

---

### 8?? **HopDong (H?p Ð?ng)**
- **URL**: `/KhachThue/HopDong`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**: Xem h?p ð?ng ðang hi?u l?c
- **API Call**: `GET /api/hopdong/nguoithue/{userId}/hieuluc`
- **Demo Data**: Có data m?u khi API không tr? v?

---

### 9?? **HoaDon (Hóa Ðõn)**
- **URL**: `/KhachThue/HoaDon`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**: Hi?n th? danh sách hóa ðõn
- **API Call**: `GET /api/hoadon/nguoithue/{userId}`
- **Demo Data**: Có 2 hóa ðõn m?u khi API không tr? v?

---

### ?? **ThanhToanHoaDon (Thanh Toán Hóa Ðõn)**
- **URL**: `/KhachThue/ThanhToanHoaDon`
- **Phýõng th?c**: `POST`
- **Tham s?**: `hoaDonId (Guid)`
- **API Call**: `POST /api/hoadon/{hoaDonId}/thanhtoan`

---

### 1??1?? **YeuThich (Ph?ng Yêu Thích)**
- **URL**: `/KhachThue/YeuThich`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**: Hi?n th? danh sách ph?ng yêu thích
- **API Call**: `GET /api/yeuthich/nguoithue/{userId}`
- **Demo Data**: Có 2 ph?ng m?u khi API không tr? v?

---

### 1??2?? **ToggleYeuThich (Thêm/Xóa Yêu Thích)**
- **URL**: `/KhachThue/ToggleYeuThich`
- **Phýõng th?c**: `POST`
- **Tham s?**: 
  ```json
  {
    "phongId": "guid"
  }
  ```
- **API Call**: `POST /api/yeuthich/toggle`
- **Response**: `{ success: bool, message: string }`

---

### 1??3?? **LichSuHoatDong (L?ch S? Ho?t Ð?ng)**
- **URL**: `/KhachThue/LichSuHoatDong`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**: Hi?n th? l?ch s? ho?t ð?ng c?a ngý?i thuê
- **API Call**: `GET /api/lichsu/nguoithue/{userId}`
- **Demo Data**: Có 4 ho?t ð?ng m?u (Xem Ph?ng, Ð?t L?ch, Thanh Toán, K? H?p Ð?ng)

---

### 1??4?? **ThongTinCaNhan (Thông Tin Cá Nhân)**
- **URL**: `/KhachThue/ThongTinCaNhan`
- **Phýõng th?c**: `GET` / `POST`
- **Ch?c nãng**: 
  - Hi?n th? thông tin cá nhân
  - C?p nh?t thông tin
- **API Calls**:
  - `GET /api/nguoidung/{userId}` (L?y thông tin)
  - `PUT /api/nguoidung/{userId}` (C?p nh?t)
- **Fallback**: L?y t? Session n?u API không tr? v?

---

### 1??5?? **ThongBao (Thông Báo)**
- **URL**: `/KhachThue/ThongBao`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**: Hi?n th? danh sách thông báo
- **API Call**: `GET /api/thongbao/nguoithue/{userId}`
- **Demo Data**: Có 4 thông báo m?u (L?ch h?n, Hóa ðõn, H?p ð?ng, Tin nh?n)

---

### 1??6?? **TinNhan (Tin Nh?n)**
- **URL**: `/KhachThue/TinNhan`
- **Phýõng th?c**: `GET`
- **Ch?c nãng**: Trang qu?n l? tin nh?n (placeholder)

---

## ?? B?O M?T VÀ KI?M SOÁT

### ?? **Ki?m Tra Role**
T?t c? các action ð?u có ki?m tra:
```csharp
if (!CheckKhachThueRole())
{
    return RedirectToAction("Login", "Auth", new { type = "nguoithue" });
}
```

### ?? **Xác Th?c Token**
- L?y token t? Session: `Session["Token"]`
- G?i kèm Authorization header: `Bearer {token}`
- Áp d?ng cho các action c?n quy?n

### ?? **Xác Th?c Anti-CSRF**
```csharp
[ValidateAntiForgeryToken]
```

---

## ?? HELPER METHODS

### 1. `CheckKhachThueRole()`
```csharp
private bool CheckKhachThueRole()
{
    var role = Session["UserRole"]?.ToString();
    return role == "KhachThue";
}
```

### 2. `GetToken()`
```csharp
private string GetToken()
{
    return Session["Token"]?.ToString();
}
```

### 3. `GetUserId()`
```csharp
private Guid? GetUserId()
{
    var userIdStr = Session["UserId"]?.ToString();
    if (Guid.TryParse(userIdStr, out Guid userId))
   return userId;
    return null;
}
```

---

## ?? API CLIENT WRAPPER

### Các Phýõng Th?c Chính
```csharp
// GET request
await _apiClient.GetAsync<T>(string url, string bearerToken = null)

// POST request
await _apiClient.PostAsync<TRequest, TResponse>(
    string url, TRequest data, string bearerToken = null)

// PUT request
await _apiClient.PutAsync<TRequest, TResponse>(
  string url, TRequest data, string bearerToken = null)

// DELETE request
await _apiClient.DeleteAsync(string url, string bearerToken = null)
```

### Response Format
```json
{
  "Success": true,
  "Data": { ... },
  "Message": "..."
}
```

---

## ?? DEMO DATA FALLBACK

Các action có demo data khi API không tr? v? d? li?u:
- ? **LichDaDat**: 2 l?ch h?n m?u
- ? **HopDong**: 1 h?p ð?ng m?u
- ? **HoaDon**: 2 hóa ðõn m?u
- ? **YeuThich**: 2 ph?ng m?u
- ? **LichSuHoatDong**: 4 ho?t ð?ng m?u
- ? **ThongBao**: 4 thông báo m?u

---

## ??? CÁC CÔNG NGH? ÐÝ?C S? D?NG

| Công Ngh? | Phiên B?n | M?c Ðích |
|-----------|----------|---------|
| ASP.NET MVC | 5.x | Framework chính |
| .NET Framework | 4.7.2 | Target framework |
| Entity Framework | 6.x | ORM |
| Newtonsoft.Json | 13.x+ | JSON serialization |
| HttpClient | Built-in | HTTP requests |
| Bootstrap 5 | 5.x | UI Framework |
| Font Awesome | 6.x | Icons |

---

## ?? GHI CHÚ QUAN TR?NG

### ?? Lýu ? V? DEMO DATA
- Demo data ch? ðý?c s? d?ng khi API không tr? v? d? li?u
- **PH?I XÓA** khi API integration hoàn t?t
- T?t c? demo data ð?u có comment `// DEMO DATA - Xóa khi API có d? li?u th?c`

### ?? Lýu ? V? ERROR HANDLING
- S? d?ng try-catch ð? b?t l?i API
- L?i ðý?c log b?ng `System.Diagnostics.Debug.WriteLine()`
- Graceful fallback khi API l?i (tr? v? empty list ho?c null)

### ?? Lýu ? V? MESSAGE
- Success/Error messages ðý?c lýu trong `TempData`
- Hi?n th? cho user sau khi redirect

---

## ?? NEXT STEPS

### Ti?p theo c?n làm:
1. ? Xác nh?n API endpoints ho?t ð?ng ðúng
2. ? Xóa DEMO DATA khi API có d? li?u th?c
3. ? Thêm validation cho các input
4. ? Thêm logging/monitoring
5. ? Performance optimization (caching, pagination)
6. ? Unit tests cho controller

---

## ?? LIÊN H? & H? TR?

- **File Main**: `USER_QUANLYPHONGTRO/Controllers/KhachThueController.cs`
- **Build Status**: ? **SUCCESSFUL**
- **Compilation**: ? **NO ERRORS**

---

*Tài li?u này ðý?c c?p nh?t l?n cu?i: 2025*
