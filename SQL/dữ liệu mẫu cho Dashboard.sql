USE QuanLyPhongTro
GO

SET NOCOUNT ON;
PRINT N'Đang bắt đầu tạo dữ liệu mẫu cho Dashboard...';

------------------------------------------------------------
-- 1. TẠO 50 NGƯỜI DÙNG (RENTER & CHUTRO)
-- Mục đích: Test 'Tổng người dùng' và 'Người dùng mới trong tháng'
------------------------------------------------------------
DECLARE @i INT = 1;
DECLARE @NewUserID UNIQUEIDENTIFIER;
DECLARE @RoleThueID INT = (SELECT TOP 1 VaiTroId FROM VaiTro WHERE TenVaiTro = 'NguoiThue');
DECLARE @RoleChuID INT = (SELECT TOP 1 VaiTroId FROM VaiTro WHERE TenVaiTro = 'ChuTro');

WHILE @i <= 50
BEGIN
    SET @NewUserID = NEWID();
    
    -- Random ngày tạo: 80% trong quá khứ (1 năm trở lại), 20% trong tháng hiện tại
    DECLARE @CreateDate DATETIMEOFFSET;
    IF (@i % 5 = 0) -- Cứ 5 người thì có 1 người tạo trong tháng này
        SET @CreateDate = DATEADD(DAY, -CAST(RAND()*20 AS INT), SYSDATETIMEOFFSET());
    ELSE
        SET @CreateDate = DATEADD(DAY, -CAST(RAND()*300 + 30 AS INT), SYSDATETIMEOFFSET());

    INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc, CreatedAt)
    VALUES (
        @NewUserID, 
        CONCAT('user_dashboard_', @i, '@test.com'), 
        CONCAT('090', 1000000 + @i), 
        'hash_dummy', 
        CASE WHEN @i % 3 = 0 THEN @RoleChuID ELSE @RoleThueID END, -- Random vai trò
        0, 1, @CreateDate
    );

    INSERT INTO HoSoNguoiDung (NguoiDungId, HoTen, GhiChu)
    VALUES (@NewUserID, CONCAT(N'Người dùng mẫu ', @i), N'Auto generated for dashboard');

    SET @i = @i + 1;
END
PRINT N'> Đã tạo xong 50 người dùng.';

------------------------------------------------------------
-- 2. TẠO 100 PHÒNG TRỌ
-- Mục đích: Test 'Thống kê phòng theo tháng', 'Phân bố trạng thái', 'Phòng chờ duyệt'
------------------------------------------------------------
SET @i = 1;
DECLARE @NhaTroID UNIQUEIDENTIFIER = (SELECT TOP 1 NhaTroId FROM NhaTro); -- Lấy tạm 1 nhà trọ có sẵn
DECLARE @RandomStatus NVARCHAR(50);
DECLARE @RandomIsDuyet BIT;
DECLARE @RoomCreateDate DATETIMEOFFSET;

WHILE @i <= 100
BEGIN
    -- Random trạng thái
    DECLARE @Rnd INT = CAST(RAND() * 100 AS INT);
    IF @Rnd < 60 SET @RandomStatus = N'con_trong';
    ELSE IF @Rnd < 90 SET @RandomStatus = N'da_thue';
    ELSE SET @RandomStatus = N'da_coc';

    -- Random duyệt: 10% là chưa duyệt (để hiện lên bảng "Pending Rooms")
    IF @Rnd < 10 SET @RandomIsDuyet = 0; ELSE SET @RandomIsDuyet = 1;

    -- Random ngày tạo: Rải đều trong 12 tháng qua (Quan trọng cho biểu đồ Monthly)
    SET @RoomCreateDate = DATEADD(MONTH, -CAST(RAND()*12 AS INT), SYSDATETIMEOFFSET());

    INSERT INTO Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    VALUES (
        NEWID(),
        @NhaTroID,
        CONCAT(N'Phòng Dashboard Test ', @i),
        15 + (@i % 20), -- Diện tích 15-35m2
        2000000 + (@i * 10000), -- Giá tiền
        @RandomStatus,
        @RandomIsDuyet,
        0,
        @RoomCreateDate
    );

    SET @i = @i + 1;
END
PRINT N'> Đã tạo xong 100 phòng (có chia theo tháng).';

------------------------------------------------------------
-- 3. TẠO 20 BÁO CÁO VI PHẠM
-- Mục đích: Test API 'GET reports/recent'
------------------------------------------------------------
SET @i = 1;
DECLARE @ReporterID UNIQUEIDENTIFIER;
DECLARE @RoomTargetID UNIQUEIDENTIFIER;

WHILE @i <= 20
BEGIN
    SELECT TOP 1 @ReporterID = NguoiDungId FROM NguoiDung ORDER BY NEWID();
    SELECT TOP 1 @RoomTargetID = PhongId FROM Phong ORDER BY NEWID();

    INSERT INTO BaoCaoViPham (BaoCaoId, LoaiThucThe, ThucTheId, NguoiBaoCao, TieuDe, MoTa, TrangThai, ThoiGianBaoCao)
    VALUES (
        NEWID(),
        N'Phong',
        @RoomTargetID,
        @ReporterID,
        CONCAT(N'Báo cáo phòng ảo số ', @i),
        N'Nội dung báo cáo mẫu để test giao diện admin dashboard.',
        CASE WHEN @i % 2 = 0 THEN N'ChoXuLy' ELSE N'DaXuLy' END,
        DATEADD(DAY, -CAST(RAND()*30 AS INT), SYSDATETIMEOFFSET()) -- 30 ngày gần đây
    );
    SET @i = @i + 1;
END
PRINT N'> Đã tạo xong 20 báo cáo vi phạm.';

------------------------------------------------------------
-- 4. TẠO 50 LỊCH SỬ HOẠT ĐỘNG
-- Mục đích: Test API 'GET activities/recent'
------------------------------------------------------------
SET @i = 1;
DECLARE @ActorID UNIQUEIDENTIFIER;
DECLARE @ActionType NVARCHAR(200);

WHILE @i <= 50
BEGIN
    SELECT TOP 1 @ActorID = NguoiDungId FROM NguoiDung ORDER BY NEWID();
    
    -- Random hành động
    DECLARE @ActRnd INT = CAST(RAND() * 3 AS INT);
    IF @ActRnd = 0 SET @ActionType = N'Đăng nhập hệ thống';
    ELSE IF @ActRnd = 1 SET @ActionType = N'Đăng tin mới';
    ELSE SET @ActionType = N'Cập nhật hồ sơ';

    INSERT INTO LichSu (NguoiDungId, HanhDong, TenBang, ChiTiet, ThoiGian)
    VALUES (
        @ActorID,
        @ActionType,
        N'System',
        CONCAT(N'Thực hiện hành động test số ', @i),
        DATEADD(HOUR, -CAST(RAND()*100 AS INT), SYSDATETIMEOFFSET()) -- 100 giờ gần đây
    );
    SET @i = @i + 1;
END
PRINT N'> Đã tạo xong 50 dòng lịch sử hoạt động.';

PRINT N'=== HOÀN TẤT TẠO DATA DASHBOARD ===';
GO