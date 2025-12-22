# Hướng Dẫn Cài Đặt và Chạy Dữ Liệu Mẫu

## 📋 Tổng Quan

Hệ thống **Quản Lý Phòng Trọ** bao gồm:
- **Backend API**: ASP.NET Core Web API + SQL Server
- **Frontend MVC**: ASP.NET MVC (USER_QUANLYPHONGTRO)
- **Admin Panel**: ASP.NET MVC (ADMIN_QUANLYPHONGTRO)

## 🗃️ Cài Đặt Database

### Bước 1: Tạo Database và Structure

1. Mở **SQL Server Management Studio** (SSMS)
2. Kết nối đến SQL Server instance của bạn
3. Mở file `SQL/QuanLyPhongTro.sql`
4. Thực thi script để tạo database và tables

```sql
-- Chạy file này trước
SQL/QuanLyPhongTro.sql
```

Script này sẽ:
- ✅ Tạo database `QuanLyPhongTro`
- ✅ Tạo tất cả tables (NguoiDung, NhaTro, Phong, v.v.)
- ✅ Tạo stored procedures
- ✅ Tạo indexes
- ✅ Insert dữ liệu cơ bản (VaiTro, TrangThaiDatPhong, v.v.)

### Bước 2: Insert Dữ Liệu Mẫu

1. Sau khi chạy xong `QuanLyPhongTro.sql`
2. Mở file `SQL/InsertSampleData.sql`
3. Thực thi script để insert dữ liệu mẫu

```sql
-- Chạy file này sau
SQL/InsertSampleData.sql
```

Script này sẽ insert:
- 👤 **Users**: 1 Admin, 3 Chủ Trọ, 2 Người Thuê
- 🏠 **Nhà Trọ**: 5 nhà trọ
- 🏘️ **Phòng**: 8 phòng đã được duyệt
- 🔧 **Tiện Ích**: 16 loại tiện ích
- 📍 **Quận/Phường**: Quận 9, Thủ Đức, Bình Thạnh, Gò Vấp
- ⭐ **Đánh Giá**: Một số đánh giá mẫu

## ⚙️ Cấu Hình Connection String

### Backend API

Mở file `Backend/RestAPI_QUANLYPHONGTRO/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=QuanLyPhongTro;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Thay `YOUR_SERVER_NAME`** bằng tên SQL Server của bạn:
- Ví dụ: `DESKTOP-ABC123`
- Ví dụ: `localhost`
- Ví dụ: `(localdb)\\MSSQLLocalDB`

### Frontend MVC

Mở file `USER_QUANLYPHONGTRO/Web.config`:

```xml
<appSettings>
  <add key="ApiBaseUrl" value="https://localhost:5001" />
</appSettings>
```

Đảm bảo URL khớp với port Backend API đang chạy.

## 🚀 Chạy Ứng Dụng

### 1. Chạy Backend API

```bash
cd Backend/RestAPI_QUANLYPHONGTRO
dotnet run
```

Backend sẽ chạy tại: `https://localhost:5001`

### 2. Chạy Frontend MVC

Mở solution bằng Visual Studio:
1. Set `USER_QUANLYPHONGTRO` làm Startup Project
2. Press `F5` hoặc click **Run**

Frontend sẽ chạy tại: `https://localhost:xxxxx` (port tự động)

## 🧪 Test GuestController

### Các Endpoint Có Sẵn:

1. **Trang chủ danh sách phòng**
   ```
   GET /Guest/Index
   GET /Guest/Index?minPrice=1500000&maxPrice=3000000
   GET /Guest/Index?page=2
   ```

2. **Chi tiết phòng**
   ```
   GET /Guest/ChiTietPhong/{PhongId}
   ```

3. **Tìm kiếm**
   ```
   GET /Guest/TimKiem?q=sinh viên
   ```

4. **Phòng nổi bật**
   ```
   GET /Guest/PhongNoiBat
   ```

5. **Lọc theo giá**
   ```
   GET /Guest/TheoGia?min=1000000&max=2000000
   ```

## 📊 Dữ Liệu Mẫu

### Users Mặc Định

| Email | Password | Role | Họ Tên |
|-------|----------|------|--------|
| admin@example.com | HashedPassword123 | Admin | Administrator |
| chutro1@example.com | HashedPassword123 | ChuTro | Nguyễn Văn A |
| chutro2@example.com | HashedPassword123 | ChuTro | Trần Thị B |
| chutro3@example.com | HashedPassword123 | ChuTro | Lê Văn C |
| nguoithue1@example.com | HashedPassword123 | NguoiThue | Phạm Thị D |
| nguoithue2@example.com | HashedPassword123 | NguoiThue | Hoàng Văn E |

### Phòng Trọ Mẫu

1. **Phòng trọ sinh viên gần UTE** - 1,800,000đ - 18m² - Rating 4.5⭐
2. **Phòng trọ mới xây, full nội thất** - 2,500,000đ - 22m² - Rating 4.8⭐
3. **Căn hộ mini 1PN cho sinh viên** - 3,200,000đ - 28m² - Rating 4.2⭐
4. **Phòng giá rẻ cho sinh viên** - 1,300,000đ - 16m² - Rating 3.9⭐
5. **Phòng trọ có gác lửng, rộng rãi** - 2,200,000đ - 25m² - Rating 4.3⭐
6. **Phòng studio cao cấp** - 4,500,000đ - 30m² - Rating 4.9⭐
7. **Phòng trọ nữ only, an ninh** - 1,900,000đ - 20m² - Rating 4.6⭐
8. **Phòng trọ có bếp riêng** - 2,800,000đ - 24m² - Rating 4.4⭐

## 🔍 Kiểm Tra Dữ Liệu

Chạy các query sau trong SSMS để kiểm tra:

```sql
-- Xem tất cả users
SELECT * FROM NguoiDung;

-- Xem tất cả nhà trọ
SELECT * FROM NhaTro;

-- Xem tất cả phòng
SELECT 
    p.PhongId,
    p.TieuDe,
    p.GiaTien,
    p.DienTich,
    p.DiemTrungBinh,
    p.IsDuyet,
    nt.TieuDe AS TenNhaTro
FROM Phong p
INNER JOIN NhaTro nt ON p.NhaTroId = nt.NhaTroId;

-- Xem tiện ích của phòng
SELECT 
    p.TieuDe AS PhongTro,
    ti.Ten AS TienIch
FROM Phong p
INNER JOIN PhongTienIch pti ON p.PhongId = pti.PhongId
INNER JOIN TienIch ti ON pti.TienIchId = ti.TienIchId;
```

## ❗ Troubleshooting

### Lỗi: "Cannot connect to SQL Server"

**Giải pháp:**
1. Kiểm tra SQL Server đang chạy
2. Kiểm tra Connection String đúng
3. Thử `Server=localhost` hoặc `Server=(localdb)\\MSSQLLocalDB`

### Lỗi: "API không trả về dữ liệu"

**Giải pháp:**
1. Kiểm tra Backend API đang chạy
2. Kiểm tra `ApiBaseUrl` trong `Web.config`
3. Xem console log của Backend API

### Lỗi: "Foreign Key constraint"

**Giải pháp:**
1. Chạy lại script `QuanLyPhongTro.sql` trước
2. Sau đó mới chạy `InsertSampleData.sql`

## 📝 Notes

- Tất cả phòng mẫu đã được **duyệt** (IsDuyet = 1)
- Khách vãng lai có thể xem danh sách và chi tiết nhưng **không thể xem thông tin liên hệ** chủ trọ
- Để xem thông tin liên hệ, cần **đăng nhập**

## 🎯 Next Steps

1. ✅ Tạo Views cho GuestController
2. ✅ Implement Authentication/Authorization
3. ✅ Thêm upload hình ảnh
4. ✅ Tạo Booking flow
5. ✅ Implement Chat system

---

**Good luck! 🚀**
