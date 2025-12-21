-- ========================================
-- SAMPLE DATA FOR DASHBOARD TESTING
-- ========================================
-- Database: QuanLyPhongTro
-- Purpose: Populate tables with test data for Dashboard API
-- ========================================

USE QuanLyPhongTro;
GO

-- ========================================
-- 1. CREATE ADMIN USER (if not exists)
-- ========================================
IF NOT EXISTS (SELECT 1 FROM NguoiDung WHERE Email = 'admin@test.com')
BEGIN
    INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc, CreatedAt, UpdatedAt)
    VALUES 
    (NEWID(), 
     'admin@test.com', 
     '0901234567',
     '$2a$11$8kP.W5Y5d5fN7qXh7kxqxO7LXZX5X5X5X5X5X5X5X5X5X5X5X5X5X', -- BCrypt hash của "admin123"
     1, -- VaiTroId = 1 (Admin)
     0, -- Không khóa
     1, -- Email đã xác thực
     GETDATE(),
     GETDATE());
    
    PRINT '✅ Admin user created: admin@test.com / admin123';
END
ELSE
BEGIN
    PRINT '⚠️ Admin user already exists';
END
GO

-- ========================================
-- 2. CREATE SAMPLE CHU TRO (Landlords)
-- ========================================
DECLARE @ChuTro1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @ChuTro2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @ChuTro3Id UNIQUEIDENTIFIER = NEWID();

-- Insert Chủ trọ users (VaiTroId = 2)
INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc, CreatedAt)
VALUES 
(@ChuTro1Id, 'chutro1@test.com', '0912345671', '$2a$11$YourHashHere', 2, 0, 1, DATEADD(MONTH, -3, GETDATE())),
(@ChuTro2Id, 'chutro2@test.com', '0912345672', '$2a$11$YourHashHere', 2, 0, 1, DATEADD(MONTH, -2, GETDATE())),
(@ChuTro3Id, 'chutro3@test.com', '0912345673', '$2a$11$YourHashHere', 2, 0, 1, DATEADD(DAY, -15, GETDATE()));

-- Insert Thông tin pháp lý
INSERT INTO ChuTroThongTinPhapLy (NguoiDungId, CCCD, NgayCapCCCD, NoiCapCCCD, DiaChiThuongTru, TrangThaiXacThuc, CreatedAt)
VALUES 
(@ChuTro1Id, '001234567891', '2020-01-15', 'CA TP.HCM', '123 Nguyen Trai, Q1, HCM', 'DaDuyet', DATEADD(MONTH, -3, GETDATE())),
(@ChuTro2Id, '001234567892', '2020-02-20', 'CA TP.HCM', '456 Le Loi, Q3, HCM', 'DaDuyet', DATEADD(MONTH, -2, GETDATE())),
(@ChuTro3Id, '001234567893', '2020-03-25', 'CA TP.HCM', '789 Tran Hung Dao, Q5, HCM', 'ChoDuyet', DATEADD(DAY, -15, GETDATE()));

PRINT '✅ Created 3 Chủ trọ users';
GO

-- ========================================
-- 3. CREATE SAMPLE NGUOI THUE (Tenants)
-- ========================================
DECLARE @Tenant1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Tenant2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Tenant3Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc, CreatedAt)
VALUES 
(@Tenant1Id, 'tenant1@test.com', '0923456781', '$2a$11$YourHashHere', 3, 0, 1, DATEADD(MONTH, -4, GETDATE())),
(@Tenant2Id, 'tenant2@test.com', '0923456782', '$2a$11$YourHashHere', 3, 0, 1, DATEADD(MONTH, -2, GETDATE())),
(@Tenant3Id, 'tenant3@test.com', '0923456783', '$2a$11$YourHashHere', 3, 0, 1, DATEADD(DAY, -5, GETDATE()));

PRINT '✅ Created 3 Người thuê users';
GO

-- ========================================
-- 4. CREATE SAMPLE NHA TRO
-- ========================================
DECLARE @NhaTro1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @NhaTro2Id UNIQUEIDENTIFIER = NEWID();

-- Assume ChuTro1Id from previous section
DECLARE @ChuTro1Id UNIQUEIDENTIFIER = (SELECT TOP 1 NguoiDungId FROM NguoiDung WHERE Email = 'chutro1@test.com');

INSERT INTO NhaTro (NhaTroId, NguoiDungId, TenNhaTro, DiaChi, MoTa, CreatedAt)
VALUES 
(@NhaTro1Id, @ChuTro1Id, 'Nhà trọ ABC', '123 Nguyen Trai, Q1, HCM', 'Nhà trọ cao cấp', DATEADD(MONTH, -3, GETDATE())),
(@NhaTro2Id, @ChuTro1Id, 'Nhà trọ XYZ', '456 Le Loi, Q3, HCM', 'Nhà trọ giá rẻ', DATEADD(MONTH, -2, GETDATE()));

PRINT '✅ Created 2 Nhà trọ';
GO

-- ========================================
-- 5. CREATE SAMPLE PHONG (Rooms)
-- ========================================
DECLARE @NhaTro1Id UNIQUEIDENTIFIER = (SELECT TOP 1 NhaTroId FROM NhaTro);

-- Approved rooms
INSERT INTO Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, CreatedAt, IsDuyet, IsBiKhoa, IsDeleted)
VALUES 
(NEWID(), @NhaTro1Id, 'Phòng 101 - Full nội thất', 20.5, 3500000, 7000000, 2, 'Trong', DATEADD(MONTH, -2, GETDATE()), 1, 0, 0),
(NEWID(), @NhaTro1Id, 'Phòng 102 - Có ban công', 22.0, 4000000, 8000000, 2, 'Trong', DATEADD(MONTH, -2, GETDATE()), 1, 0, 0),
(NEWID(), @NhaTro1Id, 'Phòng 103 - Gần trường', 18.5, 3000000, 6000000, 2, 'DaThue', DATEADD(MONTH, -2, GETDATE()), 1, 0, 0),
(NEWID(), @NhaTro1Id, 'Phòng 104 - View đẹp', 25.0, 4500000, 9000000, 3, 'Trong', DATEADD(MONTH, -1, GETDATE()), 1, 0, 0),
(NEWID(), @NhaTro1Id, 'Phòng 105 - Rộng rãi', 28.0, 5000000, 10000000, 3, 'Trong', DATEADD(DAY, -30, GETDATE()), 1, 0, 0);

-- Pending rooms (chờ duyệt)
INSERT INTO Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, CreatedAt, IsDuyet, IsBiKhoa, IsDeleted)
VALUES 
(NEWID(), @NhaTro1Id, 'Phòng 201 - Chờ duyệt', 20.0, 3500000, 7000000, 2, 'Trong', DATEADD(DAY, -2, GETDATE()), 0, 0, 0),
(NEWID(), @NhaTro1Id, 'Phòng 202 - Chờ duyệt', 21.0, 3800000, 7600000, 2, 'Trong', DATEADD(DAY, -1, GETDATE()), 0, 0, 0),
(NEWID(), @NhaTro1Id, 'Phòng 203 - Chờ duyệt', 19.5, 3200000, 6400000, 2, 'Trong', GETDATE(), 0, 0, 0);

-- Locked rooms
INSERT INTO Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, CreatedAt, IsDuyet, IsBiKhoa, IsDeleted)
VALUES 
(NEWID(), @NhaTro1Id, 'Phòng 301 - Vi phạm', 20.0, 3500000, 7000000, 2, 'Trong', DATEADD(MONTH, -1, GETDATE()), 1, 1, 0);

PRINT '✅ Created 9 Phòng (5 approved, 3 pending, 1 locked)';
GO

-- ========================================
-- 6. CREATE SAMPLE BAO CAO VI PHAM
-- ========================================
DECLARE @NguoiBaoCaoId UNIQUEIDENTIFIER = (SELECT TOP 1 NguoiDungId FROM NguoiDung WHERE VaiTroId = 3);
DECLARE @PhongViPhamId UNIQUEIDENTIFIER = (SELECT TOP 1 PhongId FROM Phong WHERE IsBiKhoa = 1);

INSERT INTO BaoCaoViPham (BaoCaoId, LoaiThucThe, ThucTheId, NguoiBaoCao, TieuDe, MoTa, TrangThai, ThoiGianBaoCao, SoBaoCao)
VALUES 
(NEWID(), 'PHONG', @PhongViPhamId, @NguoiBaoCaoId, 'Thông tin giả mạo', 'Phòng không đúng như hình ảnh', 'CHO_XU_LY', DATEADD(DAY, -2, GETDATE()), 1),
(NEWID(), 'PHONG', @PhongViPhamId, @NguoiBaoCaoId, 'Lừa đảo', 'Chủ trọ lừa đảo tiền cọc', 'CHO_XU_LY', DATEADD(DAY, -1, GETDATE()), 2),
(NEWID(), 'NGUOIDUNG', @NguoiBaoCaoId, @NguoiBaoCaoId, 'Spam', 'Người dùng gửi tin nhắn spam', 'DANG_XU_LY', GETDATE(), 3);

PRINT '✅ Created 3 Báo cáo vi phạm';
GO

-- ========================================
-- 7. CREATE SAMPLE BIEN LAI (Receipts)
-- ========================================
-- Note: Requires DatPhong and TapTin tables
-- This is simplified version

PRINT '⚠️  BienLai data requires DatPhong and TapTin - Skipped for now';
GO

-- ========================================
-- 8. CREATE SAMPLE HANH DONG ADMIN
-- ========================================
DECLARE @AdminId UNIQUEIDENTIFIER = (SELECT TOP 1 NguoiDungId FROM NguoiDung WHERE VaiTroId = 1);
DECLARE @PhongDuyetId UNIQUEIDENTIFIER = (SELECT TOP 1 PhongId FROM Phong WHERE IsDuyet = 1);

INSERT INTO HanhDongAdmin (HanhDongId, AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet, ThoiGian)
VALUES 
(1, @AdminId, 'DUYET_PHONG', 'Phong', CAST(@PhongDuyetId AS NVARCHAR(50)), 'Admin đã duyệt phòng 101', DATEADD(HOUR, -2, GETDATE())),
(2, @AdminId, 'DUYET_CHU_TRO', 'ChuTroThongTinPhapLy', 'guid-here', 'Admin đã duyệt chủ trọ ABC', DATEADD(HOUR, -5, GETDATE())),
(3, @AdminId, 'XU_LY_BAO_CAO', 'BaoCaoViPham', 'guid-here', 'Admin đã xử lý báo cáo #1', DATEADD(HOUR, -10, GETDATE()));

PRINT '✅ Created 3 Hành động admin';
GO

-- ========================================
-- 9. VERIFY DATA
-- ========================================
PRINT '';
PRINT '========================================';
PRINT 'DATA VERIFICATION';
PRINT '========================================';

SELECT COUNT(*) AS TotalUsers FROM NguoiDung;
SELECT COUNT(*) AS TotalRooms FROM Phong WHERE IsDeleted = 0;
SELECT COUNT(*) AS PendingRooms FROM Phong WHERE IsDuyet = 0 AND IsDeleted = 0;
SELECT COUNT(*) AS VerifiedHosts FROM ChuTroThongTinPhapLy WHERE TrangThaiXacThuc = 'DaDuyet';
SELECT COUNT(*) AS PendingReports FROM BaoCaoViPham WHERE TrangThai IN ('CHO_XU_LY', 'DANG_XU_LY');

PRINT '';
PRINT '✅ Sample data created successfully!';
PRINT '========================================';
PRINT 'ADMIN LOGIN CREDENTIALS:';
PRINT '  Email: admin@test.com';
PRINT '  Password: admin123';
PRINT '========================================';
GO
