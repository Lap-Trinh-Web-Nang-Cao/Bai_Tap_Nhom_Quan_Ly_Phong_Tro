-- =============================================
-- FILE SỬA NHANH HÓA ĐƠN
-- Chạy file này trong SSMS để sửa lỗi
-- =============================================
USE QuanLyPhongTro;
GO

-- 1. Cập nhật NguoiThueId trong HopDong thành user demo
DECLARE @DemoTenantId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';

UPDATE dbo.HopDong
SET NguoiThueId = @DemoTenantId;

PRINT N'Da cap nhat NguoiThueId trong HopDong!';
GO

-- 2. Kiểm tra kết quả
SELECT 
    h.Thang, 
    h.Nam, 
    h.TienPhong,
    h.TienDien,
    h.TienNuoc,
    h.TienDichVu,
    h.TongTien,
    h.TrangThai, 
    hd.NguoiThueId
FROM dbo.HoaDon h
JOIN dbo.HopDong hd ON h.HopDongId = hd.HopDongId
ORDER BY h.Nam, h.Thang;

-- 3. Kiểm tra user demo
SELECT NguoiDungId, Email FROM dbo.NguoiDung 
WHERE NguoiDungId = '00000000-0000-0000-0000-000000000001';

PRINT N'HOAN TAT! Refresh trang web de xem hoa don.';
GO
