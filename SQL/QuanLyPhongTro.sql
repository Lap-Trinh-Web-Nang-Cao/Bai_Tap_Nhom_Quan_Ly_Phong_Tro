    /**********************************************************************
    Fixed full init script for DB "QuanLyPhongTro"
    - Ensure admin-related columns exist BEFORE creating stored procedures
    - Idempotent: checks existence before CREATE / ALTER
    **********************************************************************/
    -- Create DB if missing
    IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'QuanLyPhongTro')
    BEGIN
        CREATE DATABASE [QuanLyPhongTro];
        PRINT N'Created database QuanLyPhongTro.';
    END
    ELSE
        PRINT N'Database QuanLyPhongTro already exists.';
    GO

    ------------------------------------------------------------
-- 0. TẠO DAT

USE QuanLyPhongTro
GO

------------------------------------------------------------
-- 1. BẢNG LOOKUP CƠ BẢN
------------------------------------------------------------

IF OBJECT_ID(N'dbo.VaiTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.VaiTro (
        VaiTroId   INT IDENTITY(1,1) PRIMARY KEY,
        TenVaiTro  NVARCHAR(100) NOT NULL UNIQUE
    );
END;
GO

IF OBJECT_ID(N'dbo.TrangThaiDatPhong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TrangThaiDatPhong (
        TrangThaiId   INT IDENTITY(1,1) PRIMARY KEY,
        TenTrangThai  NVARCHAR(100) NOT NULL UNIQUE
    );
END;
GO

IF OBJECT_ID(N'dbo.TienIch', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TienIch (
        TienIchId INT IDENTITY(1,1) PRIMARY KEY,
        Ten       NVARCHAR(200) NOT NULL UNIQUE
    );
END;
GO

IF OBJECT_ID(N'dbo.LoaiHoTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoaiHoTro (
        LoaiHoTroId INT IDENTITY(1,1) PRIMARY KEY,
        TenLoai     NVARCHAR(200) NOT NULL UNIQUE
    );
END;
GO

IF OBJECT_ID(N'dbo.ViPham', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ViPham (
        ViPhamId      INT IDENTITY(1,1) PRIMARY KEY,
        TenViPham     NVARCHAR(200) NOT NULL,
        MoTa          NVARCHAR(1000) NULL,
        HinhPhatTien  BIGINT NULL,
        SoDiemTru     INT NULL
    );
END;
GO

------------------------------------------------------------
-- 2. BẢNG NGƯỜI DÙNG & HỒ SƠ
------------------------------------------------------------

IF OBJECT_ID(N'dbo.NguoiDung', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NguoiDung (
        NguoiDungId       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        Email             NVARCHAR(255) NULL,
        DienThoai         NVARCHAR(50) NULL,
        PasswordHash      NVARCHAR(512) NULL,
        VaiTroId          INT NOT NULL,  -- vai trò mặc định (để filter nhanh)
        IsKhoa            BIT NOT NULL DEFAULT 0,
        IsEmailXacThuc    BIT NOT NULL DEFAULT 0,
        CreatedAt         DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        UpdatedAt         DATETIMEOFFSET NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.HoSoNguoiDung', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HoSoNguoiDung (
        NguoiDungId   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        HoTen         NVARCHAR(200) NULL,
        NgaySinh      DATE NULL,
        LoaiGiayTo    NVARCHAR(100) NULL,
        GhiChu        NVARCHAR(1000) NULL,
        CreatedAt     DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
    );
END;
GO

------------------------------------------------------------
-- 3. BẢNG PHÂN QUYỀN NHIỀU ROLE / 1 USER
------------------------------------------------------------

IF OBJECT_ID(N'dbo.NguoiDungVaiTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NguoiDungVaiTro (
        NguoiDungId  UNIQUEIDENTIFIER NOT NULL,
        VaiTroId     INT NOT NULL,
        NgayBatDau   DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        NgayKetThuc  DATETIMEOFFSET NULL,
        GhiChu       NVARCHAR(500) NULL,
        CONSTRAINT PK_NguoiDungVaiTro PRIMARY KEY (NguoiDungId, VaiTroId)
    );
END;
GO

------------------------------------------------------------
-- 4. ĐỊA LÝ, NHÀ TRỌ, PHÒNG
------------------------------------------------------------

IF OBJECT_ID(N'dbo.QuanHuyen', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QuanHuyen (
        QuanHuyenId INT IDENTITY(1,1) PRIMARY KEY,
        Ten         NVARCHAR(200) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Phuong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Phuong (
        PhuongId     INT IDENTITY(1,1) PRIMARY KEY,
        QuanHuyenId  INT NOT NULL,
        Ten          NVARCHAR(200) NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.NhaTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhaTro (
        NhaTroId     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        ChuTroId     UNIQUEIDENTIFIER NOT NULL,  -- FK tới NguoiDung
        TieuDe       NVARCHAR(300) NOT NULL,
        DiaChi       NVARCHAR(500) NULL,
        QuanHuyenId  INT NULL,
        PhuongId     INT NULL,
        CreatedAt    DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        IsHoatDong   BIT NOT NULL DEFAULT 1
    );
END;
GO

IF OBJECT_ID(N'dbo.Phong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Phong (
        PhongId          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        NhaTroId         UNIQUEIDENTIFIER NOT NULL,
        TieuDe           NVARCHAR(250) NULL,
        DienTich         DECIMAL(8,2) NULL,
        GiaTien          BIGINT NOT NULL,
        TienCoc          BIGINT NULL,
        SoNguoiToiDa     INT NOT NULL DEFAULT 1,
        TrangThai        NVARCHAR(50) NOT NULL DEFAULT N'con_trong',
        CreatedAt        DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        UpdatedAt        DATETIMEOFFSET NULL,
        DiemTrungBinh    FLOAT NULL,
        SoLuongDanhGia   INT NOT NULL DEFAULT 0,
        IsDuyet          BIT NOT NULL DEFAULT 0,
        NguoiDuyet       UNIQUEIDENTIFIER NULL,
        ThoiGianDuyet    DATETIMEOFFSET NULL,
        IsBiKhoa         BIT NOT NULL DEFAULT 0,
        MoTa             NVARCHAR(MAX) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.PhongTienIch', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhongTienIch (
        PId       INT IDENTITY(1,1) PRIMARY KEY,
        PhongId   UNIQUEIDENTIFIER NOT NULL,
        TienIchId INT NOT NULL
    );
END;
GO

------------------------------------------------------------
-- 5. TẬP TIN, THÔNG BÁO, LỊCH SỬ, HÀNH ĐỘNG ADMIN
------------------------------------------------------------

IF OBJECT_ID(N'dbo.TapTin', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TapTin (
        TapTinId       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        DuongDan       NVARCHAR(1000) NOT NULL,
        MimeType       NVARCHAR(100) NULL,
        TaiBangNguoi   UNIQUEIDENTIFIER NULL,
        ThoiGianTai    DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
    );
END;
GO

IF OBJECT_ID(N'dbo.HanhDongAdmin', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HanhDongAdmin (
        HanhDongId    BIGINT IDENTITY(1,1) PRIMARY KEY,
        AdminId       UNIQUEIDENTIFIER NOT NULL,
        HanhDong      NVARCHAR(200) NOT NULL,
        MucTieuBang   NVARCHAR(200) NULL,
        BanGhiId      NVARCHAR(200) NULL,
        ChiTiet       NVARCHAR(MAX) NULL,
        ThoiGian      DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
    );
END;
GO

IF OBJECT_ID(N'dbo.LichSu', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LichSu (
        LichSuId     BIGINT IDENTITY(1,1) PRIMARY KEY,
        NguoiDungId  UNIQUEIDENTIFIER NULL,
        HanhDong     NVARCHAR(200) NOT NULL,
        TenBang      NVARCHAR(200) NULL,
        BanGhiId     NVARCHAR(200) NULL,
        ChiTiet      NVARCHAR(MAX) NULL,
        ThoiGian     DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
    );
END;
GO
select * from TokenThongBao
IF OBJECT_ID(N'dbo.TokenThongBao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TokenThongBao (
        TokenId      UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        NguoiDungId  UNIQUEIDENTIFIER NOT NULL,
        Token        NVARCHAR(1000) NOT NULL,
        ThoiGianTao  DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        IsActive     BIT NOT NULL DEFAULT 1
    );
END;
GO

------------------------------------------------------------
-- 6. ĐẶT PHÒNG, BIÊN LAI, HỖ TRỢ, CHAT, ĐÁNH GIÁ, BÁO CÁO
------------------------------------------------------------

IF OBJECT_ID(N'dbo.DatPhong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DatPhong (
        DatPhongId       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        PhongId          UNIQUEIDENTIFIER NOT NULL,
        NguoiThueId      UNIQUEIDENTIFIER NOT NULL,
        Loai             NVARCHAR(30) NOT NULL,
        BatDau           DATETIMEOFFSET NOT NULL,
        KetThuc          DATETIMEOFFSET NULL,
        ThoiGianTao      DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        TrangThaiId      INT NOT NULL,
        TapTinBienLaiId  UNIQUEIDENTIFIER NULL
        -- SoDatPhong (INT auto) sẽ được thêm phía dưới bằng ALTER để idempotent
    );
END;
GO

IF OBJECT_ID(N'dbo.BienLai', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BienLai (
        BienLaiId    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        DatPhongId   UNIQUEIDENTIFIER NOT NULL,
        NguoiTai     UNIQUEIDENTIFIER NOT NULL,
        TapTinId     UNIQUEIDENTIFIER NULL,
        SoTien       BIGINT NULL,
        ThoiGianTai  DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        DaXacNhan    BIT NOT NULL DEFAULT 0
        -- SoBienLai (INT auto) sẽ thêm bằng ALTER
    );
END;
GO
-- Thêm các column thiếu vào bảng BienLai
IF COL_LENGTH('dbo.BienLai','NguoiXacNhan') IS NULL
BEGIN
    ALTER TABLE dbo.BienLai ADD NguoiXacNhan UNIQUEIDENTIFIER NULL;
    PRINT N'✓ Added column BienLai.NguoiXacNhan';
END

IF COL_LENGTH('dbo.BienLai','SoBienLai') IS NULL
BEGIN
    ALTER TABLE dbo.BienLai ADD SoBienLai NVARCHAR(100) NULL;
    PRINT N'✓ Added column BienLai.SoBienLai';
END
GO
IF OBJECT_ID(N'dbo.YeuCauHoTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YeuCauHoTro (
        HoTroId        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        PhongId        UNIQUEIDENTIFIER NULL,
        NguoiYeuCau    UNIQUEIDENTIFIER NULL,
        LoaiHoTroId    INT NOT NULL,
        TieuDe         NVARCHAR(300) NOT NULL,
        MoTa           NVARCHAR(MAX) NULL,
        TrangThai      NVARCHAR(50) NOT NULL DEFAULT N'Moi',
        ThoiGianTao    DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
    );
END;
GO

IF OBJECT_ID(N'dbo.TinNhan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TinNhan (
        TinNhanId   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        FromUser    UNIQUEIDENTIFIER NOT NULL,
        ToUser      UNIQUEIDENTIFIER NOT NULL,
        NoiDung     NVARCHAR(MAX) NULL,
        TapTinId    UNIQUEIDENTIFIER NULL,
        ThoiGian    DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        DaDoc       BIT NOT NULL DEFAULT 0
    );
END;
GO

IF OBJECT_ID(N'dbo.DanhGiaPhong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGiaPhong (
        DanhGiaId     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        PhongId       UNIQUEIDENTIFIER NOT NULL,
        NguoiDanhGia  UNIQUEIDENTIFIER NOT NULL,
        Diem          INT NOT NULL CHECK (Diem BETWEEN 1 AND 5),
        NoiDung       NVARCHAR(1000) NULL,
        ThoiGian      DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
    );
END;
GO

IF OBJECT_ID(N'dbo.BaoCaoViPham', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BaoCaoViPham (
        BaoCaoId        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        LoaiThucThe     NVARCHAR(50) NOT NULL, -- 'NguoiDung', 'Phong', ...
        ThucTheId       UNIQUEIDENTIFIER NULL,
        NguoiBaoCao     UNIQUEIDENTIFIER NOT NULL,
        ViPhamId        INT NULL,
        TieuDe          NVARCHAR(300) NOT NULL,
        MoTa            NVARCHAR(MAX) NULL,
        TrangThai       NVARCHAR(50) NOT NULL DEFAULT N'ChoXuLy',
        KetQua          NVARCHAR(1000) NULL,
        NguoiXuLy       UNIQUEIDENTIFIER NULL,
        ThoiGianBaoCao  DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        ThoiGianXuLy    DATETIMEOFFSET NULL
        -- SoBaoCao (INT auto) sẽ thêm bằng ALTER
    );
END;
GO

------------------------------------------------------------
-- 7. BẢNG THÔNG TIN PHÁP LÝ CHỦ TRỌ (1-1 VỚI NGUOIDUNG)
------------------------------------------------------------

IF OBJECT_ID(N'dbo.ChuTroThongTinPhapLy', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChuTroThongTinPhapLy (
        NguoiDungId         UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        CCCD                NVARCHAR(20)  NOT NULL,
        NgayCapCCCD         DATE          NULL,
        NoiCapCCCD          NVARCHAR(200) NULL,
        DiaChiThuongTru     NVARCHAR(500) NOT NULL,
        DiaChiLienHe        NVARCHAR(500) NULL,
        SoDienThoaiLienHe   NVARCHAR(50)  NULL,
        MaSoThueCaNhan      NVARCHAR(50)  NULL,
        SoTaiKhoanNganHang  NVARCHAR(50)  NULL,
        TenNganHang         NVARCHAR(200) NULL,
        ChiNhanhNganHang    NVARCHAR(200) NULL,
        TapTinGiayToId      UNIQUEIDENTIFIER NULL,
        TrangThaiXacThuc    NVARCHAR(50) NOT NULL DEFAULT N'ChoDuyet', -- ChoDuyet/DaDuyet/TuChoi
        GhiChu              NVARCHAR(1000) NULL,
        CreatedAt           DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        UpdatedAt           DATETIMEOFFSET NULL
    );
END;
GO

------------------------------------------------------------
-- 8. KHÓA NGOẠI (FOREIGN KEY) - IDP SAFE
------------------------------------------------------------

-- NguoiDung -> VaiTro (vai trò mặc định)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NguoiDung_VaiTro')
BEGIN
    ALTER TABLE dbo.NguoiDung
        ADD CONSTRAINT FK_NguoiDung_VaiTro
        FOREIGN KEY (VaiTroId) REFERENCES dbo.VaiTro(VaiTroId);
END;
GO

-- HoSoNguoiDung -> NguoiDung
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_HoSoNguoiDung_NguoiDung')
BEGIN
    ALTER TABLE dbo.HoSoNguoiDung
        ADD CONSTRAINT FK_HoSoNguoiDung_NguoiDung
        FOREIGN KEY (NguoiDungId) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

-- NguoiDungVaiTro -> NguoiDung, VaiTro
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NguoiDungVaiTro_NguoiDung')
BEGIN
    ALTER TABLE dbo.NguoiDungVaiTro
        ADD CONSTRAINT FK_NguoiDungVaiTro_NguoiDung
        FOREIGN KEY (NguoiDungId) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NguoiDungVaiTro_VaiTro')
BEGIN
    ALTER TABLE dbo.NguoiDungVaiTro
        ADD CONSTRAINT FK_NguoiDungVaiTro_VaiTro
        FOREIGN KEY (VaiTroId) REFERENCES dbo.VaiTro(VaiTroId);
END;
GO

-- Phuong -> QuanHuyen
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Phuong_QuanHuyen')
BEGIN
    ALTER TABLE dbo.Phuong
        ADD CONSTRAINT FK_Phuong_QuanHuyen
        FOREIGN KEY (QuanHuyenId) REFERENCES dbo.QuanHuyen(QuanHuyenId);
END;
GO

-- NhaTro -> NguoiDung (ChuTro)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_NhaTro_NguoiDung')
BEGIN
    ALTER TABLE dbo.NhaTro
        ADD CONSTRAINT FK_NhaTro_NguoiDung
        FOREIGN KEY (ChuTroId) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

-- Phong -> NhaTro
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Phong_NhaTro')
BEGIN
    ALTER TABLE dbo.Phong
        ADD CONSTRAINT FK_Phong_NhaTro
        FOREIGN KEY (NhaTroId) REFERENCES dbo.NhaTro(NhaTroId);
END;
GO

-- PhongTienIch -> Phong, TienIch
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PhongTienIch_Phong')
BEGIN
    ALTER TABLE dbo.PhongTienIch
        ADD CONSTRAINT FK_PhongTienIch_Phong
        FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PhongTienIch_TienIch')
BEGIN
    ALTER TABLE dbo.PhongTienIch
        ADD CONSTRAINT FK_PhongTienIch_TienIch
        FOREIGN KEY (TienIchId) REFERENCES dbo.TienIch(TienIchId);
END;
GO

-- DatPhong -> Phong, NguoiDung, TrangThaiDatPhong
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DatPhong_Phong')
BEGIN
    ALTER TABLE dbo.DatPhong
        ADD CONSTRAINT FK_DatPhong_Phong
        FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DatPhong_NguoiThue')
BEGIN
    ALTER TABLE dbo.DatPhong
        ADD CONSTRAINT FK_DatPhong_NguoiThue
        FOREIGN KEY (NguoiThueId) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DatPhong_TrangThai')
BEGIN
    ALTER TABLE dbo.DatPhong
        ADD CONSTRAINT FK_DatPhong_TrangThai
        FOREIGN KEY (TrangThaiId) REFERENCES dbo.TrangThaiDatPhong(TrangThaiId);
END;
GO

-- BienLai -> DatPhong, NguoiDung(NguoiTai), TapTin
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BienLai_DatPhong')
BEGIN
    ALTER TABLE dbo.BienLai
        ADD CONSTRAINT FK_BienLai_DatPhong
        FOREIGN KEY (DatPhongId) REFERENCES dbo.DatPhong(DatPhongId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BienLai_NguoiTai')
BEGIN
    ALTER TABLE dbo.BienLai
        ADD CONSTRAINT FK_BienLai_NguoiTai
        FOREIGN KEY (NguoiTai) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BienLai_TapTin')
BEGIN
    ALTER TABLE dbo.BienLai
        ADD CONSTRAINT FK_BienLai_TapTin
        FOREIGN KEY (TapTinId) REFERENCES dbo.TapTin(TapTinId);
END;
GO

-- YeuCauHoTro -> NguoiDung, LoaiHoTro, Phong
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_YeuCauHoTro_NguoiYeuCau')
BEGIN
    ALTER TABLE dbo.YeuCauHoTro
        ADD CONSTRAINT FK_YeuCauHoTro_NguoiYeuCau
        FOREIGN KEY (NguoiYeuCau) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_YeuCauHoTro_LoaiHoTro')
BEGIN
    ALTER TABLE dbo.YeuCauHoTro
        ADD CONSTRAINT FK_YeuCauHoTro_LoaiHoTro
        FOREIGN KEY (LoaiHoTroId) REFERENCES dbo.LoaiHoTro(LoaiHoTroId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_YeuCauHoTro_Phong')
BEGIN
    ALTER TABLE dbo.YeuCauHoTro
        ADD CONSTRAINT FK_YeuCauHoTro_Phong
        FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);
END;
GO

-- TinNhan -> NguoiDung, TapTin
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TinNhan_FromUser')
BEGIN
    ALTER TABLE dbo.TinNhan
        ADD CONSTRAINT FK_TinNhan_FromUser
        FOREIGN KEY (FromUser) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TinNhan_ToUser')
BEGIN
    ALTER TABLE dbo.TinNhan
        ADD CONSTRAINT FK_TinNhan_ToUser
        FOREIGN KEY (ToUser) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TinNhan_TapTin')
BEGIN
    ALTER TABLE dbo.TinNhan
        ADD CONSTRAINT FK_TinNhan_TapTin
        FOREIGN KEY (TapTinId) REFERENCES dbo.TapTin(TapTinId);
END;
GO

-- DanhGiaPhong -> Phong, NguoiDung
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DanhGiaPhong_Phong')
BEGIN
    ALTER TABLE dbo.DanhGiaPhong
        ADD CONSTRAINT FK_DanhGiaPhong_Phong
        FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DanhGiaPhong_NguoiDung')
BEGIN
    ALTER TABLE dbo.DanhGiaPhong
        ADD CONSTRAINT FK_DanhGiaPhong_NguoiDung
        FOREIGN KEY (NguoiDanhGia) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

-- BaoCaoViPham -> NguoiDung, ViPham
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BaoCaoViPham_NguoiBaoCao')
BEGIN
    ALTER TABLE dbo.BaoCaoViPham
        ADD CONSTRAINT FK_BaoCaoViPham_NguoiBaoCao
        FOREIGN KEY (NguoiBaoCao) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BaoCaoViPham_ViPham')
BEGIN
    ALTER TABLE dbo.BaoCaoViPham
        ADD CONSTRAINT FK_BaoCaoViPham_ViPham
        FOREIGN KEY (ViPhamId) REFERENCES dbo.ViPham(ViPhamId);
END;
GO

-- HanhDongAdmin -> NguoiDung(AdminId)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_HanhDongAdmin_Admin')
BEGIN
    ALTER TABLE dbo.HanhDongAdmin
        ADD CONSTRAINT FK_HanhDongAdmin_Admin
        FOREIGN KEY (AdminId) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

-- LichSu -> NguoiDung
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_LichSu_NguoiDung')
BEGIN
    ALTER TABLE dbo.LichSu
        ADD CONSTRAINT FK_LichSu_NguoiDung
        FOREIGN KEY (NguoiDungId) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

-- TapTin -> NguoiDung (TaiBangNguoi)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TapTin_NguoiDung')
BEGIN
    ALTER TABLE dbo.TapTin
        ADD CONSTRAINT FK_TapTin_NguoiDung
        FOREIGN KEY (TaiBangNguoi) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

-- TokenThongBao -> NguoiDung
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TokenThongBao_NguoiDung')
BEGIN
    ALTER TABLE dbo.TokenThongBao
        ADD CONSTRAINT FK_TokenThongBao_NguoiDung
        FOREIGN KEY (NguoiDungId) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

-- ChuTroThongTinPhapLy -> NguoiDung, TapTin
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ChuTroThongTinPhapLy_NguoiDung')
BEGIN
    ALTER TABLE dbo.ChuTroThongTinPhapLy
        ADD CONSTRAINT FK_ChuTroThongTinPhapLy_NguoiDung
        FOREIGN KEY (NguoiDungId) REFERENCES dbo.NguoiDung(NguoiDungId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ChuTroThongTinPhapLy_TapTin')
BEGIN
    ALTER TABLE dbo.ChuTroThongTinPhapLy
        ADD CONSTRAINT FK_ChuTroThongTinPhapLy_TapTin
        FOREIGN KEY (TapTinGiayToId) REFERENCES dbo.TapTin(TapTinId);
END;
GO

------------------------------------------------------------
-- 9. SEQUENCE & CỘT TỰ ĐỘNG TĂNG CHO MÃ NGHIỆP VỤ
------------------------------------------------------------
/* 1. SEQUENCE cho số tự tăng */
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'SEQ_DatPhong')
BEGIN
    CREATE SEQUENCE dbo.SEQ_DatPhong AS INT
        START WITH 1
        INCREMENT BY 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'SEQ_BaoCao')
BEGIN
    CREATE SEQUENCE dbo.SEQ_BaoCao AS INT
        START WITH 1
        INCREMENT BY 1;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'SEQ_BienLai')
BEGIN
    CREATE SEQUENCE dbo.SEQ_BienLai AS INT
        START WITH 1
        INCREMENT BY 1;
END
GO

/* 2. Thêm cột SoDatPhong / SoBaoCao / SoBienLai nếu chưa có */

IF COL_LENGTH('dbo.DatPhong', 'SoDatPhong') IS NULL
BEGIN
    ALTER TABLE dbo.DatPhong
        ADD SoDatPhong INT NOT NULL
            CONSTRAINT DF_DatPhong_SoDatPhong
                DEFAULT (NEXT VALUE FOR dbo.SEQ_DatPhong);
END
GO

IF COL_LENGTH('dbo.BaoCaoViPham', 'SoBaoCao') IS NULL
BEGIN
    ALTER TABLE dbo.BaoCaoViPham
        ADD SoBaoCao INT NOT NULL
            CONSTRAINT DF_BaoCaoViPham_SoBaoCao
                DEFAULT (NEXT VALUE FOR dbo.SEQ_BaoCao);
END
GO

IF COL_LENGTH('dbo.BienLai', 'SoBienLai') IS NULL
BEGIN
    ALTER TABLE dbo.BienLai
        ADD SoBienLai INT NOT NULL
            CONSTRAINT DF_BienLai_SoBienLai
                DEFAULT (NEXT VALUE FOR dbo.SEQ_BienLai);
END
GO

------------------------------------------------------------
-- 10. CHUẨN HÓA CÁC CỘT BIT (KHÔNG ĐỂ NULL)
------------------------------------------------------------

-- NguoiDung: IsKhoa, IsEmailXacThuc
IF COL_LENGTH('dbo.NguoiDung', 'IsKhoa') IS NOT NULL
BEGIN
    UPDATE dbo.NguoiDung SET IsKhoa = 0 WHERE IsKhoa IS NULL;
    ALTER TABLE dbo.NguoiDung ALTER COLUMN IsKhoa BIT NOT NULL;
END;
GO

IF COL_LENGTH('dbo.NguoiDung', 'IsEmailXacThuc') IS NOT NULL
BEGIN
    UPDATE dbo.NguoiDung SET IsEmailXacThuc = 0 WHERE IsEmailXacThuc IS NULL;
    ALTER TABLE dbo.NguoiDung ALTER COLUMN IsEmailXacThuc BIT NOT NULL;
END;
GO

-- Phong: IsDuyet, IsBiKhoa
IF COL_LENGTH('dbo.Phong', 'IsDuyet') IS NOT NULL
BEGIN
    UPDATE dbo.Phong SET IsDuyet = 0 WHERE IsDuyet IS NULL;
    ALTER TABLE dbo.Phong ALTER COLUMN IsDuyet BIT NOT NULL;
END;
GO

IF COL_LENGTH('dbo.Phong', 'IsBiKhoa') IS NOT NULL
BEGIN
    UPDATE dbo.Phong SET IsBiKhoa = 0 WHERE IsBiKhoa IS NULL;
    ALTER TABLE dbo.Phong ALTER COLUMN IsBiKhoa BIT NOT NULL;
END;
GO

-- TinNhan: DaDoc
IF COL_LENGTH('dbo.TinNhan', 'DaDoc') IS NOT NULL
BEGIN
    UPDATE dbo.TinNhan SET DaDoc = 0 WHERE DaDoc IS NULL;
    ALTER TABLE dbo.TinNhan ALTER COLUMN DaDoc BIT NOT NULL;
END;
GO

-- TokenThongBao: IsActive
IF COL_LENGTH('dbo.TokenThongBao', 'IsActive') IS NOT NULL
BEGIN
    UPDATE dbo.TokenThongBao SET IsActive = 0 WHERE IsActive IS NULL;
    ALTER TABLE dbo.TokenThongBao ALTER COLUMN IsActive BIT NOT NULL;
END;
GO

-- BienLai: DaXacNhan
IF COL_LENGTH('dbo.BienLai', 'DaXacNhan') IS NOT NULL
BEGIN
    UPDATE dbo.BienLai SET DaXacNhan = 0 WHERE DaXacNhan IS NULL;
    ALTER TABLE dbo.BienLai ALTER COLUMN DaXacNhan BIT NOT NULL;
END;
GO

------------------------------------------------------------
-- 11. SEED DATA CHO LOOKUP
------------------------------------------------------------

-- VaiTro
IF NOT EXISTS (SELECT 1 FROM dbo.VaiTro WHERE TenVaiTro = N'Admin')
    INSERT INTO dbo.VaiTro (TenVaiTro) VALUES (N'Admin');
IF NOT EXISTS (SELECT 1 FROM dbo.VaiTro WHERE TenVaiTro = N'ChuTro')
    INSERT INTO dbo.VaiTro (TenVaiTro) VALUES (N'ChuTro');
IF NOT EXISTS (SELECT 1 FROM dbo.VaiTro WHERE TenVaiTro = N'NguoiThue')
    INSERT INTO dbo.VaiTro (TenVaiTro) VALUES (N'NguoiThue');

-- TrangThaiDatPhong
IF NOT EXISTS (SELECT 1 FROM dbo.TrangThaiDatPhong WHERE TenTrangThai = N'ChoXacNhan')
    INSERT INTO dbo.TrangThaiDatPhong (TenTrangThai) VALUES (N'ChoXacNhan');
IF NOT EXISTS (SELECT 1 FROM dbo.TrangThaiDatPhong WHERE TenTrangThai = N'DaXacNhan')
    INSERT INTO dbo.TrangThaiDatPhong (TenTrangThai) VALUES (N'DaXacNhan');
IF NOT EXISTS (SELECT 1 FROM dbo.TrangThaiDatPhong WHERE TenTrangThai = N'DaThanhToan')
    INSERT INTO dbo.TrangThaiDatPhong (TenTrangThai) VALUES (N'DaThanhToan');
IF NOT EXISTS (SELECT 1 FROM dbo.TrangThaiDatPhong WHERE TenTrangThai = N'HoanThanh')
    INSERT INTO dbo.TrangThaiDatPhong (TenTrangThai) VALUES (N'HoanThanh');
IF NOT EXISTS (SELECT 1 FROM dbo.TrangThaiDatPhong WHERE TenTrangThai = N'DaHuy')
    INSERT INTO dbo.TrangThaiDatPhong (TenTrangThai) VALUES (N'DaHuy');

-- TienIch
IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Wifi')
    INSERT INTO dbo.TienIch (Ten) VALUES (N'Wifi');
IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'BanCong')
    INSERT INTO dbo.TienIch (Ten) VALUES (N'BanCong');

-- LoaiHoTro
IF NOT EXISTS (SELECT 1 FROM dbo.LoaiHoTro WHERE TenLoai = N'SuaChua')
    INSERT INTO dbo.LoaiHoTro (TenLoai) VALUES (N'SuaChua');
IF NOT EXISTS (SELECT 1 FROM dbo.LoaiHoTro WHERE TenLoai = N'VeSinh')
    INSERT INTO dbo.LoaiHoTro (TenLoai) VALUES (N'VeSinh');

-- ViPham mẫu
IF NOT EXISTS (SELECT 1 FROM dbo.ViPham WHERE TenViPham = N'Báo tin sai sự thật')
    INSERT INTO dbo.ViPham (TenViPham, MoTa, HinhPhatTien) VALUES (N'Báo tin sai sự thật', N'Người dùng báo tin sai / thông tin giả', 0);
IF NOT EXISTS (SELECT 1 FROM dbo.ViPham WHERE TenViPham = N'Trộm cắp / lừa đảo')
    INSERT INTO dbo.ViPham (TenViPham, MoTa, HinhPhatTien) VALUES (N'Trộm cắp / lừa đảo', N'Hành vi lừa đảo, trộm cắp', 0);
IF NOT EXISTS (SELECT 1 FROM dbo.ViPham WHERE TenViPham = N'Vi phạm nội quy')
    INSERT INTO dbo.ViPham (TenViPham, MoTa, HinhPhatTien) VALUES (N'Vi phạm nội quy', N'Vi phạm nội quy cộng đồng', 0);
GO

------------------------------------------------------------
-- 12. INDEX PHỔ BIẾN
------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_Phong_GiaTien' AND object_id = OBJECT_ID(N'dbo.Phong'))
    CREATE INDEX IDX_Phong_GiaTien ON dbo.Phong(GiaTien);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_DatPhong_Phong' AND object_id = OBJECT_ID(N'dbo.DatPhong'))
    CREATE INDEX IDX_DatPhong_Phong ON dbo.DatPhong(PhongId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IDX_DatPhong_NguoiThue' AND object_id = OBJECT_ID(N'dbo.DatPhong'))
    CREATE INDEX IDX_DatPhong_NguoiThue ON dbo.DatPhong(NguoiThueId);
GO

------------------------------------------------------------
-- 13. STORED PROCEDURES (DROP & TẠO LẠI)
------------------------------------------------------------

-- Xóa nếu đã tồn tại
IF OBJECT_ID(N'dbo.sp_CreateBooking', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CreateBooking;
IF OBJECT_ID(N'dbo.sp_UploadReceipt', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_UploadReceipt;
IF OBJECT_ID(N'dbo.sp_CreateReview', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CreateReview;
IF OBJECT_ID(N'dbo.sp_CreateSupport', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_CreateSupport;
IF OBJECT_ID(N'dbo.sp_Admin_XacThucTaiKhoan', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_XacThucTaiKhoan;
IF OBJECT_ID(N'dbo.sp_Admin_KhoaTaiKhoan', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_KhoaTaiKhoan;
IF OBJECT_ID(N'dbo.sp_Admin_MoKhoaTaiKhoan', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_MoKhoaTaiKhoan;
IF OBJECT_ID(N'dbo.sp_Admin_DuyetBaiDang', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_DuyetBaiDang;
IF OBJECT_ID(N'dbo.sp_Admin_KhoaBaiDang', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_KhoaBaiDang;
IF OBJECT_ID(N'dbo.sp_TaoBaoCaoViPham', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_TaoBaoCaoViPham;
IF OBJECT_ID(N'dbo.sp_Admin_XuLyBaoCao', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_XuLyBaoCao;
IF OBJECT_ID(N'dbo.sp_Admin_CreateUser', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_CreateUser;
GO

-- Tạo SP đặt phòng
CREATE PROCEDURE dbo.sp_CreateBooking
    @PhongId        UNIQUEIDENTIFIER,
    @NguoiThueId    UNIQUEIDENTIFIER,
    @Loai           NVARCHAR(30),
    @BatDau         DATETIMEOFFSET,
    @KetThuc        DATETIMEOFFSET = NULL,
    @NewDatPhongId  UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE PhongId = @PhongId)
        BEGIN
            RAISERROR(N'Phòng không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        SET @NewDatPhongId = NEWID();

        INSERT INTO dbo.DatPhong (DatPhongId, PhongId, NguoiThueId, Loai, BatDau, KetThuc, TrangThaiId)
        VALUES(
            @NewDatPhongId,
            @PhongId,
            @NguoiThueId,
            @Loai,
            @BatDau,
            @KetThuc,
            (SELECT TOP 1 TrangThaiId FROM dbo.TrangThaiDatPhong WHERE TenTrangThai = N'ChoXacNhan')
        );

        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (
            @NguoiThueId,
            N'Tạo đặt phòng',
            N'DatPhong',
            CAST(@NewDatPhongId AS NVARCHAR(50)),
            N'PhongId=' + CAST(@PhongId AS NVARCHAR(50))
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Tải biên lai
CREATE PROCEDURE dbo.sp_UploadReceipt
    @DatPhongId     UNIQUEIDENTIFIER,
    @NguoiTai       UNIQUEIDENTIFIER,
    @TapTinId       UNIQUEIDENTIFIER,
    @SoTien         BIGINT = NULL,
    @NewBienLaiId   UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.DatPhong WHERE DatPhongId = @DatPhongId)
        BEGIN
            RAISERROR(N'Đặt phòng không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        SET @NewBienLaiId = NEWID();

        INSERT INTO dbo.BienLai (BienLaiId, DatPhongId, NguoiTai, TapTinId, SoTien)
        VALUES (@NewBienLaiId, @DatPhongId, @NguoiTai, @TapTinId, @SoTien);

        UPDATE dbo.DatPhong
        SET TapTinBienLaiId = @TapTinId
        WHERE DatPhongId = @DatPhongId;

        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (
            @NguoiTai,
            N'Upload biên lai',
            N'BienLai',
            CAST(@NewBienLaiId AS NVARCHAR(50)),
            N'DatPhongId=' + CAST(@DatPhongId AS NVARCHAR(50))
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Tạo đánh giá
CREATE PROCEDURE dbo.sp_CreateReview
    @PhongId          UNIQUEIDENTIFIER,
    @NguoiDanhGia     UNIQUEIDENTIFIER,
    @Diem             INT,
    @NoiDung          NVARCHAR(1000) = NULL,
    @NewDanhGiaId     UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Diem IS NULL OR @Diem < 1 OR @Diem > 5
    BEGIN
        RAISERROR(N'Điểm phải trong khoảng 1..5.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE PhongId = @PhongId)
        BEGIN
            RAISERROR(N'Phòng không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        SET @NewDanhGiaId = NEWID();

        INSERT INTO dbo.DanhGiaPhong (DanhGiaId, PhongId, NguoiDanhGia, Diem, NoiDung)
        VALUES (@NewDanhGiaId, @PhongId, @NguoiDanhGia, @Diem, @NoiDung);

        UPDATE dbo.Phong
        SET SoLuongDanhGia = ISNULL(SoLuongDanhGia, 0) + 1,
            DiemTrungBinh =
                CASE WHEN ISNULL(SoLuongDanhGia, 0) = 0
                     THEN @Diem
                     ELSE ((ISNULL(DiemTrungBinh, 0) * ISNULL(SoLuongDanhGia, 0)) + @Diem)
                          / (ISNULL(SoLuongDanhGia, 0) + 1)
                END,
            UpdatedAt = SYSDATETIMEOFFSET()
        WHERE PhongId = @PhongId;

        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (
            @NguoiDanhGia,
            N'Tạo đánh giá',
            N'DanhGiaPhong',
            CAST(@NewDanhGiaId AS NVARCHAR(50)),
            N'PhongId=' + CAST(@PhongId AS NVARCHAR(50))
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Tạo yêu cầu hỗ trợ
CREATE PROCEDURE dbo.sp_CreateSupport
    @PhongId        UNIQUEIDENTIFIER = NULL,
    @NguoiYeuCau    UNIQUEIDENTIFIER,
    @LoaiHoTroId    INT,
    @TieuDe         NVARCHAR(300),
    @MoTa           NVARCHAR(MAX) = NULL,
    @NewHoTroId     UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        SET @NewHoTroId = NEWID();

        INSERT INTO dbo.YeuCauHoTro (HoTroId, PhongId, NguoiYeuCau, LoaiHoTroId, TieuDe, MoTa)
        VALUES (@NewHoTroId, @PhongId, @NguoiYeuCau, @LoaiHoTroId, @TieuDe, @MoTa);

        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (
            @NguoiYeuCau,
            N'Tạo yêu cầu hỗ trợ',
            N'YeuCauHoTro',
            CAST(@NewHoTroId AS NVARCHAR(50)),
            @TieuDe
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Admin: Xác thực tài khoản
CREATE PROCEDURE dbo.sp_Admin_XacThucTaiKhoan
    @AdminId      UNIQUEIDENTIFIER,
    @NguoiDungId  UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE NguoiDungId = @NguoiDungId)
        BEGIN
            RAISERROR(N'Người dùng không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        UPDATE dbo.NguoiDung
        SET IsEmailXacThuc = 1,
            UpdatedAt = SYSDATETIMEOFFSET()
        WHERE NguoiDungId = @NguoiDungId;

        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (
            @AdminId,
            N'Xác thực tài khoản',
            N'NguoiDung',
            CAST(@NguoiDungId AS NVARCHAR(50)),
            N'Xác thực email / tài khoản'
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Admin: Khóa tài khoản
CREATE PROCEDURE dbo.sp_Admin_KhoaTaiKhoan
    @AdminId      UNIQUEIDENTIFIER,
    @NguoiDungId  UNIQUEIDENTIFIER,
    @LyDo         NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE NguoiDungId = @NguoiDungId)
        BEGIN
            RAISERROR(N'Người dùng không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        UPDATE dbo.NguoiDung
        SET IsKhoa = 1,
            UpdatedAt = SYSDATETIMEOFFSET()
        WHERE NguoiDungId = @NguoiDungId;

        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (
            @AdminId,
            N'Khóa tài khoản',
            N'NguoiDung',
            CAST(@NguoiDungId AS NVARCHAR(50)),
            ISNULL(@LyDo, N'')
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Admin: Mở khóa tài khoản
CREATE PROCEDURE dbo.sp_Admin_MoKhoaTaiKhoan
    @AdminId      UNIQUEIDENTIFIER,
    @NguoiDungId  UNIQUEIDENTIFIER,
    @LyDo         NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE NguoiDungId = @NguoiDungId)
        BEGIN
            RAISERROR(N'Người dùng không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        UPDATE dbo.NguoiDung
        SET IsKhoa = 0,
            UpdatedAt = SYSDATETIMEOFFSET()
        WHERE NguoiDungId = @NguoiDungId;

        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (
            @AdminId,
            N'Mở khóa tài khoản',
            N'NguoiDung',
            CAST(@NguoiDungId AS NVARCHAR(50)),
            ISNULL(@LyDo, N'')
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Admin: Duyệt bài đăng
CREATE PROCEDURE dbo.sp_Admin_DuyetBaiDang
    @AdminId   UNIQUEIDENTIFIER,
    @PhongId   UNIQUEIDENTIFIER,
    @ChapNhan  BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE PhongId = @PhongId)
        BEGIN
            RAISERROR(N'Phòng không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        IF @ChapNhan = 1
        BEGIN
            UPDATE dbo.Phong
            SET IsDuyet = 1,
                NguoiDuyet = @AdminId,
                ThoiGianDuyet = SYSDATETIMEOFFSET(),
                IsBiKhoa = 0,
                UpdatedAt = SYSDATETIMEOFFSET()
            WHERE PhongId = @PhongId;

            INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
            VALUES (
                @AdminId,
                N'Duyệt bài đăng',
                N'Phong',
                CAST(@PhongId AS NVARCHAR(50)),
                N'Chấp nhận hiển thị'
            );
        END
        ELSE
        BEGIN
            UPDATE dbo.Phong
            SET IsDuyet = 0,
                IsBiKhoa = 1,
                NguoiDuyet = @AdminId,
                ThoiGianDuyet = SYSDATETIMEOFFSET(),
                UpdatedAt = SYSDATETIMEOFFSET()
            WHERE PhongId = @PhongId;

            INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
            VALUES (
                @AdminId,
                N'Từ chối bài đăng',
                N'Phong',
                CAST(@PhongId AS NVARCHAR(50)),
                N'Từ chối hiển thị'
            );
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Admin: Khóa bài đăng
CREATE PROCEDURE dbo.sp_Admin_KhoaBaiDang
    @AdminId  UNIQUEIDENTIFIER,
    @PhongId  UNIQUEIDENTIFIER,
    @LyDo     NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE PhongId = @PhongId)
        BEGIN
            RAISERROR(N'Phòng không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        UPDATE dbo.Phong
        SET IsBiKhoa = 1,
            IsDuyet = 0,
            UpdatedAt = SYSDATETIMEOFFSET()
        WHERE PhongId = @PhongId;

        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (
            @AdminId,
            N'Khóa bài đăng',
            N'Phong',
            CAST(@PhongId AS NVARCHAR(50)),
            ISNULL(@LyDo, N'')
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Tạo báo cáo vi phạm
CREATE PROCEDURE dbo.sp_TaoBaoCaoViPham
    @NguoiBaoCao   UNIQUEIDENTIFIER,
    @LoaiThucThe   NVARCHAR(50),
    @ThucTheId     UNIQUEIDENTIFIER = NULL,
    @ViPhamId      INT = NULL,
    @TieuDe        NVARCHAR(300),
    @MoTa          NVARCHAR(MAX) = NULL,
    @NewBaoCaoId   UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        SET @NewBaoCaoId = NEWID();

        INSERT INTO dbo.BaoCaoViPham (BaoCaoId, LoaiThucThe, ThucTheId, NguoiBaoCao, ViPhamId, TieuDe, MoTa)
        VALUES (@NewBaoCaoId, @LoaiThucThe, @ThucTheId, @NguoiBaoCao, @ViPhamId, @TieuDe, @MoTa);

        -- Hành động này có thể coi như hành động người dùng (không nhất thiết admin)
        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (
            @NguoiBaoCao,
            N'Tạo báo cáo vi phạm',
            N'BaoCaoViPham',
            CAST(@NewBaoCaoId AS NVARCHAR(50)),
            @TieuDe
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Admin xử lý báo cáo
CREATE PROCEDURE dbo.sp_Admin_XuLyBaoCao
    @AdminId            UNIQUEIDENTIFIER,
    @BaoCaoId           UNIQUEIDENTIFIER,
    @HanhDong           NVARCHAR(200),
    @ViPhamId           INT = NULL,
    @KetQua             NVARCHAR(1000) = NULL,
    @ApDungKhoaTaiKhoan BIT = 0,
    @ApDungKhoaBaiDang  BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        IF NOT EXISTS (SELECT 1 FROM dbo.BaoCaoViPham WHERE BaoCaoId = @BaoCaoId)
        BEGIN
            RAISERROR(N'Báo cáo không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        UPDATE dbo.BaoCaoViPham
        SET ViPhamId = COALESCE(@ViPhamId, ViPhamId),
            TrangThai = N'DaXuLy',
            KetQua = @KetQua,
            NguoiXuLy = @AdminId,
            ThoiGianXuLy = SYSDATETIMEOFFSET()
        WHERE BaoCaoId = @BaoCaoId;

        DECLARE @LoaiThucThe NVARCHAR(50);
        DECLARE @ThucTheId   UNIQUEIDENTIFIER;

        SELECT @LoaiThucThe = LoaiThucThe,
               @ThucTheId   = ThucTheId
        FROM dbo.BaoCaoViPham
        WHERE BaoCaoId = @BaoCaoId;

        IF @ApDungKhoaTaiKhoan = 1
           AND @LoaiThucThe = N'NguoiDung'
           AND @ThucTheId IS NOT NULL
        BEGIN
            UPDATE dbo.NguoiDung
            SET IsKhoa = 1,
                UpdatedAt = SYSDATETIMEOFFSET()
            WHERE NguoiDungId = @ThucTheId;

            INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
            VALUES (
                @AdminId,
                N'Khóa tài khoản do vi phạm',
                N'NguoiDung',
                CAST(@ThucTheId AS NVARCHAR(50)),
                ISNULL(@KetQua, N'')
            );
        END

        IF @ApDungKhoaBaiDang = 1
           AND @LoaiThucThe = N'Phong'
           AND @ThucTheId IS NOT NULL
        BEGIN
            UPDATE dbo.Phong
            SET IsBiKhoa = 1,
                IsDuyet = 0,
                UpdatedAt = SYSDATETIMEOFFSET()
            WHERE PhongId = @ThucTheId;

            INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
            VALUES (
                @AdminId,
                N'Khóa bài đăng do vi phạm',
                N'Phong',
                CAST(@ThucTheId AS NVARCHAR(50)),
                ISNULL(@KetQua, N'')
            );
        END

        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (
            @AdminId,
            @HanhDong,
            N'BaoCaoViPham',
            CAST(@BaoCaoId AS NVARCHAR(50)),
            ISNULL(@KetQua, N'')
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

-- Admin: Tạo tài khoản mới (Admin, ChuTro, NguoiThue)
CREATE PROCEDURE dbo.sp_Admin_CreateUser
    @AdminId        UNIQUEIDENTIFIER, -- ID của người thực hiện (để log lịch sử)
    @Email          NVARCHAR(255),
    @PasswordHash   NVARCHAR(512),
    @TenVaiTro      NVARCHAR(50),     -- 'Admin', 'ChuTro', hoặc 'NguoiThue'
    @HoTen          NVARCHAR(200),
    @DienThoai      NVARCHAR(50) = NULL,
    @IsActive       BIT = 1,          -- Mặc định tạo xong kích hoạt luôn
    @NewUserId      UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra quyền của người gọi
        IF NOT EXISTS (
            SELECT 1 FROM dbo.NguoiDung u 
            JOIN dbo.VaiTro vt ON u.VaiTroId = vt.VaiTroId 
            WHERE u.NguoiDungId = @AdminId AND vt.TenVaiTro = N'Admin'
        )
        BEGIN
            RAISERROR(N'Bạn không có quyền thực hiện chức năng này.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        -- 2. Lấy ID của Vai trò muốn tạo
        DECLARE @TargetRoleId INT;
        SELECT @TargetRoleId = VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = @TenVaiTro;

        IF @TargetRoleId IS NULL
        BEGIN
            RAISERROR(N'Vai trò không hợp lệ (Phải là Admin, ChuTro, NguoiThue).', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        -- 3. Kiểm tra trùng Email
        IF EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE Email = @Email)
        BEGIN
            RAISERROR(N'Email này đã tồn tại trong hệ thống.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        -- 4. Tạo User
        SET @NewUserId = NEWID();

        INSERT INTO dbo.NguoiDung (
            NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, 
            IsKhoa, IsEmailXacThuc, CreatedAt
        )
        VALUES (
            @NewUserId, @Email, @DienThoai, @PasswordHash, @TargetRoleId, 
            CASE WHEN @IsActive = 1 THEN 0 ELSE 1 END, -- IsKhoa (0 là mở, 1 là khóa)
            1, -- Admin tạo thì mặc định coi như đã xác thực Email
            SYSDATETIMEOFFSET()
        );

        -- 5. Tạo Hồ sơ cơ bản
        INSERT INTO dbo.HoSoNguoiDung (NguoiDungId, HoTen, GhiChu)
        VALUES (@NewUserId, @HoTen, N'Tạo bởi Admin');

        -- 6. Gán quyền vào bảng phân quyền
        INSERT INTO dbo.NguoiDungVaiTro (NguoiDungId, VaiTroId, NgayBatDau)
        VALUES (@NewUserId, @TargetRoleId, SYSDATETIMEOFFSET());

        -- Nếu tạo Chủ Trọ, cần tạo thêm bảng pháp lý rỗng để họ tự cập nhật sau (tránh lỗi code)
        IF @TenVaiTro = N'ChuTro'
        BEGIN
             INSERT INTO dbo.ChuTroThongTinPhapLy (
                NguoiDungId, CCCD, DiaChiThuongTru, TrangThaiXacThuc
            )
            VALUES (@NewUserId, N'Updating', N'Updating', N'DaDuyet'); -- Admin tạo thì cho Duyệt luôn hoặc ChoDuyet tùy ý
        END

        -- 7. Ghi Log hành động Admin
        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (@AdminId, N'Tạo tài khoản mới', N'NguoiDung', CAST(@NewUserId AS NVARCHAR(50)), N'Tạo role: ' + @TenVaiTro);

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

------------------------------------------------------------
-- 14. KIỂM TRA NHANH
------------------------------------------------------------

SELECT TOP 5 * FROM dbo.VaiTro;
SELECT TOP 5 NguoiDungId, Email, DienThoai, VaiTroId, CreatedAt FROM dbo.NguoiDung;
SELECT TOP 5 PhongId, TieuDe, GiaTien, TrangThai, IsDuyet, IsBiKhoa FROM dbo.Phong;
SELECT TOP 5 DatPhongId, SoDatPhong, PhongId, NguoiThueId FROM dbo.DatPhong;
SELECT TOP 5 BaoCaoId, SoBaoCao, LoaiThucThe, TieuDe, TrangThai FROM dbo.BaoCaoViPham;
SELECT TOP 5 BienLaiId, SoBienLai, DatPhongId, SoTien FROM dbo.BienLai;
GO

PRINT N'Hoàn tất: QuanLyPhongTro DB đã được tạo/cập nhật chuẩn 3NF, có phân quyền và tự động tăng.';
GO


/*======================================================================
 PHẦN BỔ SUNG STORED PROCEDURE PHÂN QUYỀN NGƯỜI DÙNG
 - Đăng ký người thuê (User_RegisterNguoiThue)
 - Đăng ký chủ trọ (User_RegisterChuTro)
 - Nâng cấp người thuê thành chủ trọ (User_UpgradeToChuTro)
 - Phù hợp mô hình: NguoiDung, NguoiDungVaiTro, HoSoNguoiDung, ChuTroThongTinPhapLy
 - An toàn chạy lại: luôn DROP IF EXISTS trước khi CREATE
======================================================================*/

------------------------------------------------------------
-- DỌN DẸP: XÓA PROC CŨ NẾU CÓ
------------------------------------------------------------
IF OBJECT_ID(N'dbo.sp_User_RegisterNguoiThue', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_User_RegisterNguoiThue;
IF OBJECT_ID(N'dbo.sp_User_RegisterChuTro', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_User_RegisterChuTro;
IF OBJECT_ID(N'dbo.sp_User_UpgradeToChuTro', N'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_User_UpgradeToChuTro;
GO

/*======================================================================
 1. ĐĂNG KÝ NGƯỜI THUÊ (chỉ role "NguoiThue")
    - Tạo NguoiDung (VaiTroId mặc định = NguoiThue)
    - Tạo HoSoNguoiDung
    - Thêm dòng NguoiDungVaiTro (NguoiThue)
======================================================================*/
CREATE PROCEDURE dbo.sp_User_RegisterNguoiThue
    @Email          NVARCHAR(255),
    @DienThoai      NVARCHAR(50),
    @PasswordHash   NVARCHAR(512),
    @HoTen          NVARCHAR(200) = NULL,
    @NgaySinh       DATE = NULL,
    @LoaiGiayTo     NVARCHAR(100) = NULL,
    @GhiChu         NVARCHAR(1000) = NULL,
    @NewNguoiDungId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        /* Lấy VaiTroId cho NguoiThue */
        DECLARE @VaiTroNguoiThueId INT;
        SELECT @VaiTroNguoiThueId = VaiTroId
        FROM dbo.VaiTro
        WHERE TenVaiTro = N'NguoiThue';

        IF @VaiTroNguoiThueId IS NULL
        BEGIN
            RAISERROR(N'Không tìm thấy vai trò NguoiThue trong bảng VaiTro.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        /* Kiểm tra trùng email (nếu bạn muốn enforce) */
        IF EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE Email = @Email)
        BEGIN
            RAISERROR(N'Email đã tồn tại trong hệ thống.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        /* Tạo tài khoản NguoiDung */
        SET @NewNguoiDungId = NEWID();

        INSERT INTO dbo.NguoiDung (
            NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId,
            IsKhoa, IsEmailXacThuc, CreatedAt
        )
        VALUES (
            @NewNguoiDungId, @Email, @DienThoai, @PasswordHash, @VaiTroNguoiThueId,
            0, 0, SYSDATETIMEOFFSET()
        );

        /* Hồ sơ người dùng (thông tin cá nhân cơ bản) */
        INSERT INTO dbo.HoSoNguoiDung (
            NguoiDungId, HoTen, NgaySinh, LoaiGiayTo, GhiChu
        )
        VALUES (
            @NewNguoiDungId, @HoTen, @NgaySinh, @LoaiGiayTo, @GhiChu
        );

        /* Gán vai trò NguoiThue vào bảng NguoiDungVaiTro */
        INSERT INTO dbo.NguoiDungVaiTro (NguoiDungId, VaiTroId)
        VALUES (@NewNguoiDungId, @VaiTroNguoiThueId);

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO
SELECT COUNT(*) FROM Phong WHERE IsDuyet = 1 AND IsBiKhoa = 0;
/*======================================================================
 2. ĐĂNG KÝ CHỦ TRỌ MỚI
    - Tạo NguoiDung
    - Thêm 2 role: NguoiThue + ChuTro (1 tài khoản dùng được 2 UI)
    - Tạo HoSoNguoiDung
    - Tạo ChuTroThongTinPhapLy (CCCD, địa chỉ, ngân hàng,...)
======================================================================*/
Go
CREATE PROCEDURE dbo.sp_User_RegisterChuTro
    @Email              NVARCHAR(255),
    @DienThoai          NVARCHAR(50),
    @PasswordHash       NVARCHAR(512),
    @HoTen              NVARCHAR(200),
    @NgaySinh           DATE = NULL,
    @LoaiGiayTo         NVARCHAR(100) = NULL,
    @GhiChuHoSo         NVARCHAR(1000) = NULL,

    @CCCD               NVARCHAR(20),
    @NgayCapCCCD        DATE = NULL,
    @NoiCapCCCD         NVARCHAR(200) = NULL,
    @DiaChiThuongTru    NVARCHAR(500),
    @DiaChiLienHe       NVARCHAR(500) = NULL,
    @SoDienThoaiLienHe  NVARCHAR(50) = NULL,
    @MaSoThueCaNhan     NVARCHAR(50) = NULL,
    @SoTaiKhoanNganHang NVARCHAR(50) = NULL,
    @TenNganHang        NVARCHAR(200) = NULL,
    @ChiNhanhNganHang   NVARCHAR(200) = NULL,
    @TapTinGiayToId     UNIQUEIDENTIFIER = NULL, -- file scan CCCD/hợp đồng
    @GhiChuPhapLy       NVARCHAR(1000) = NULL,

    @NewNguoiDungId     UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        /* Lấy VaiTroId cho ChuTro và NguoiThue */
        DECLARE @VaiTroChuTroId INT, @VaiTroNguoiThueId INT;
        SELECT @VaiTroChuTroId   = VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'ChuTro';
        SELECT @VaiTroNguoiThueId = VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'NguoiThue';

        IF @VaiTroChuTroId IS NULL OR @VaiTroNguoiThueId IS NULL
        BEGIN
            RAISERROR(N'Không tìm thấy vai trò ChuTro hoặc NguoiThue.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        /* Kiểm tra trùng email (nếu muốn enforce) */
        IF EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE Email = @Email)
        BEGIN
            RAISERROR(N'Email đã tồn tại trong hệ thống.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        /* Tạo tài khoản NguoiDung (vai trò mặc định = ChuTro) */
        SET @NewNguoiDungId = NEWID();

        INSERT INTO dbo.NguoiDung (
            NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId,
            IsKhoa, IsEmailXacThuc, CreatedAt
        )
        VALUES (
            @NewNguoiDungId, @Email, @DienThoai, @PasswordHash, @VaiTroChuTroId,
            0, 0, SYSDATETIMEOFFSET()
        );

        /* Hồ sơ người dùng chung */
        INSERT INTO dbo.HoSoNguoiDung (
            NguoiDungId, HoTen, NgaySinh, LoaiGiayTo, GhiChu
        )
        VALUES (
            @NewNguoiDungId, @HoTen, @NgaySinh, @LoaiGiayTo, @GhiChuHoSo
        );

        /* Gán role ChuTro + NguoiThue (1 account dùng cả 2 UI) */
        INSERT INTO dbo.NguoiDungVaiTro (NguoiDungId, VaiTroId)
        VALUES (@NewNguoiDungId, @VaiTroChuTroId);

        INSERT INTO dbo.NguoiDungVaiTro (NguoiDungId, VaiTroId)
        VALUES (@NewNguoiDungId, @VaiTroNguoiThueId);

        /* Thông tin pháp lý chủ trọ */
        INSERT INTO dbo.ChuTroThongTinPhapLy (
            NguoiDungId, CCCD, NgayCapCCCD, NoiCapCCCD,
            DiaChiThuongTru, DiaChiLienHe, SoDienThoaiLienHe,
            MaSoThueCaNhan,
            SoTaiKhoanNganHang, TenNganHang, ChiNhanhNganHang,
            TapTinGiayToId, TrangThaiXacThuc, GhiChu,
            CreatedAt
        )
        VALUES (
            @NewNguoiDungId, @CCCD, @NgayCapCCCD, @NoiCapCCCD,
            @DiaChiThuongTru, @DiaChiLienHe, @SoDienThoaiLienHe,
            @MaSoThueCaNhan,
            @SoTaiKhoanNganHang, @TenNganHang, @ChiNhanhNganHang,
            @TapTinGiayToId, N'ChoDuyet', @GhiChuPhapLy,
            SYSDATETIMEOFFSET()
        );

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

/*======================================================================
 3. NÂNG CẤP NGƯỜI THUÊ THÀNH CHỦ TRỌ
    - Yêu cầu tài khoản đã tồn tại (đang là NguoiThue)
    - Thêm/Update ChuTroThongTinPhapLy
    - Thêm role ChuTro vào NguoiDungVaiTro (nếu chưa có)
    - Có thể set VaiTroId mặc định = ChuTro để đăng nhập ưu tiên UI chủ trọ
======================================================================*/
CREATE PROCEDURE dbo.sp_User_UpgradeToChuTro
    @NguoiDungId         UNIQUEIDENTIFIER,

    @CCCD                NVARCHAR(20),
    @NgayCapCCCD         DATE = NULL,
    @NoiCapCCCD          NVARCHAR(200) = NULL,
    @DiaChiThuongTru     NVARCHAR(500),
    @DiaChiLienHe        NVARCHAR(500) = NULL,
    @SoDienThoaiLienHe   NVARCHAR(50) = NULL,
    @MaSoThueCaNhan      NVARCHAR(50) = NULL,
    @SoTaiKhoanNganHang  NVARCHAR(50) = NULL,
    @TenNganHang         NVARCHAR(200) = NULL,
    @ChiNhanhNganHang    NVARCHAR(200) = NULL,
    @TapTinGiayToId      UNIQUEIDENTIFIER = NULL,
    @GhiChuPhapLy        NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        /* Kiểm tra tồn tại tài khoản */
        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE NguoiDungId = @NguoiDungId)
        BEGIN
            RAISERROR(N'Tài khoản không tồn tại.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        /* Lấy VaiTroId ChuTro & NguoiThue */
        DECLARE @VaiTroChuTroId INT, @VaiTroNguoiThueId INT;
        SELECT @VaiTroChuTroId = VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'ChuTro';
        SELECT @VaiTroNguoiThueId = VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'NguoiThue';

        IF @VaiTroChuTroId IS NULL OR @VaiTroNguoiThueId IS NULL
        BEGIN
            RAISERROR(N'Không tìm thấy vai trò ChuTro hoặc NguoiThue.', 16, 1);
            ROLLBACK TRAN;
            RETURN;
        END

        /* Đảm bảo tài khoản có vai trò NguoiThue (nếu yêu cầu) */
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.NguoiDungVaiTro
            WHERE NguoiDungId = @NguoiDungId
              AND VaiTroId = @VaiTroNguoiThueId
        )
        BEGIN
            -- Nếu chưa có thì thêm luôn role NguoiThue (option)
            INSERT INTO dbo.NguoiDungVaiTro (NguoiDungId, VaiTroId)
            VALUES (@NguoiDungId, @VaiTroNguoiThueId);
        END

        /* Thêm role ChuTro nếu chưa có */
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.NguoiDungVaiTro
            WHERE NguoiDungId = @NguoiDungId
              AND VaiTroId = @VaiTroChuTroId
        )
        BEGIN
            INSERT INTO dbo.NguoiDungVaiTro (NguoiDungId, VaiTroId)
            VALUES (@NguoiDungId, @VaiTroChuTroId);
        END

        /* Cập nhật VaiTroId mặc định = ChuTro (để sau này login default vào UI chủ trọ) */
        UPDATE dbo.NguoiDung
        SET VaiTroId = @VaiTroChuTroId,
            UpdatedAt = SYSDATETIMEOFFSET()
        WHERE NguoiDungId = @NguoiDungId;

        /* Thêm hoặc cập nhật thông tin pháp lý chủ trọ */
        IF NOT EXISTS (
            SELECT 1 FROM dbo.ChuTroThongTinPhapLy WHERE NguoiDungId = @NguoiDungId
        )
        BEGIN
            INSERT INTO dbo.ChuTroThongTinPhapLy (
                NguoiDungId, CCCD, NgayCapCCCD, NoiCapCCCD,
                DiaChiThuongTru, DiaChiLienHe, SoDienThoaiLienHe,
                MaSoThueCaNhan,
                SoTaiKhoanNganHang, TenNganHang, ChiNhanhNganHang,
                TapTinGiayToId, TrangThaiXacThuc, GhiChu,
                CreatedAt
            )
            VALUES (
                @NguoiDungId, @CCCD, @NgayCapCCCD, @NoiCapCCCD,
                @DiaChiThuongTru, @DiaChiLienHe, @SoDienThoaiLienHe,
                @MaSoThueCaNhan,
                @SoTaiKhoanNganHang, @TenNganHang, @ChiNhanhNganHang,
                @TapTinGiayToId, N'ChoDuyet', @GhiChuPhapLy,
                SYSDATETIMEOFFSET()
            );
        END
        ELSE
        BEGIN
            UPDATE dbo.ChuTroThongTinPhapLy
            SET CCCD               = @CCCD,
                NgayCapCCCD        = @NgayCapCCCD,
                NoiCapCCCD         = @NoiCapCCCD,
                DiaChiThuongTru    = @DiaChiThuongTru,
                DiaChiLienHe       = @DiaChiLienHe,
                SoDienThoaiLienHe  = @SoDienThoaiLienHe,
                MaSoThueCaNhan     = @MaSoThueCaNhan,
                SoTaiKhoanNganHang = @SoTaiKhoanNganHang,
                TenNganHang        = @TenNganHang,
                ChiNhanhNganHang   = @ChiNhanhNganHang,
                TapTinGiayToId     = @TapTinGiayToId,
                TrangThaiXacThuc   = N'ChoDuyet', -- gửi lại duyệt
                GhiChu             = @GhiChuPhapLy,
                UpdatedAt          = SYSDATETIMEOFFSET()
            WHERE NguoiDungId = @NguoiDungId;
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @Err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Err, 16, 1);
    END CATCH
END;
GO

PRINT N'Đã tạo xong các SP phân quyền: sp_User_RegisterNguoiThue, sp_User_RegisterChuTro, sp_User_UpgradeToChuTro.';
GO
-- Check if column exists
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Phong' 
    AND COLUMN_NAME = 'IsDeleted'
)
BEGIN
    -- Add IsDeleted column
    ALTER TABLE Phong
    ADD IsDeleted BIT NOT NULL DEFAULT 0;
    
    PRINT '✅ Column IsDeleted added to Phong table';
END
ELSE
BEGIN
    PRINT '⚠️ Column IsDeleted already exists in Phong table';
END
GO

-- Update existing records to ensure they have IsDeleted = 0
UPDATE Phong 
SET IsDeleted = 0 
WHERE IsDeleted IS NULL;

PRINT '✅ All existing Phong records updated with IsDeleted = 0';
GO

/* ==========================================================
   1. INSERT QUẬN / HUYỆN (ĐÀ NẴNG)
========================================================== */
INSERT INTO QuanHuyen (Ten) 
SELECT N'Hải Châu' WHERE NOT EXISTS (SELECT 1 FROM QuanHuyen WHERE Ten=N'Hải Châu') UNION ALL
SELECT N'Thanh Khê' WHERE NOT EXISTS (SELECT 1 FROM QuanHuyen WHERE Ten=N'Thanh Khê') UNION ALL
SELECT N'Sơn Trà' WHERE NOT EXISTS (SELECT 1 FROM QuanHuyen WHERE Ten=N'Sơn Trà') UNION ALL
SELECT N'Ngũ Hành Sơn' WHERE NOT EXISTS (SELECT 1 FROM QuanHuyen WHERE Ten=N'Ngũ Hành Sơn') UNION ALL
SELECT N'Liên Chiểu' WHERE NOT EXISTS (SELECT 1 FROM QuanHuyen WHERE Ten=N'Liên Chiểu') UNION ALL
SELECT N'Cẩm Lệ' WHERE NOT EXISTS (SELECT 1 FROM QuanHuyen WHERE Ten=N'Cẩm Lệ') UNION ALL
SELECT N'Hòa Vang' WHERE NOT EXISTS (SELECT 1 FROM QuanHuyen WHERE Ten=N'Hòa Vang') UNION ALL
SELECT N'Hoàng Sa' WHERE NOT EXISTS (SELECT 1 FROM QuanHuyen WHERE Ten=N'Hoàng Sa');

-- Insert Phường (Mẫu đại diện, đảm bảo QuanHuyenId khớp với ID tự tăng)
-- Lưu ý: ID Quận Huyện là Identity(1,1) nên ta lấy theo tên để chính xác
DECLARE @Q_HaiChau INT = (SELECT TOP 1 QuanHuyenId FROM QuanHuyen WHERE Ten=N'Hải Châu');
DECLARE @Q_ThanhKhe INT = (SELECT TOP 1 QuanHuyenId FROM QuanHuyen WHERE Ten=N'Thanh Khê');
DECLARE @Q_SonTra INT = (SELECT TOP 1 QuanHuyenId FROM QuanHuyen WHERE Ten=N'Sơn Trà');
DECLARE @Q_NguHanhSon INT = (SELECT TOP 1 QuanHuyenId FROM QuanHuyen WHERE Ten=N'Ngũ Hành Sơn');
DECLARE @Q_LienChieu INT = (SELECT TOP 1 QuanHuyenId FROM QuanHuyen WHERE Ten=N'Liên Chiểu');

IF @Q_HaiChau IS NOT NULL
BEGIN
    INSERT INTO Phuong (QuanHuyenId, Ten) VALUES
    (@Q_HaiChau, N'Hải Châu I'), (@Q_HaiChau, N'Hải Châu II'), (@Q_HaiChau, N'Thạch Thang');
END

IF @Q_ThanhKhe IS NOT NULL
BEGIN
    INSERT INTO Phuong (QuanHuyenId, Ten) VALUES
    (@Q_ThanhKhe, N'Thanh Khê Đông'), (@Q_ThanhKhe, N'Thanh Khê Tây'), (@Q_ThanhKhe, N'Vĩnh Trung');
END

IF @Q_LienChieu IS NOT NULL
BEGIN
    INSERT INTO Phuong (QuanHuyenId, Ten) VALUES
    (@Q_LienChieu, N'Hòa Khánh Bắc'), (@Q_LienChieu, N'Hòa Khánh Nam'), (@Q_LienChieu, N'Hòa Minh');
END

IF @Q_NguHanhSon IS NOT NULL
BEGIN
    INSERT INTO Phuong (QuanHuyenId, Ten) VALUES
    (@Q_NguHanhSon, N'Mỹ An'), (@Q_NguHanhSon, N'Khuê Mỹ');
END

/* ==========================================================
   2. VAI TRÒ
========================================================== */
INSERT INTO VaiTro (TenVaiTro) SELECT N'Admin' WHERE NOT EXISTS (SELECT 1 FROM VaiTro WHERE TenVaiTro=N'Admin');
INSERT INTO VaiTro (TenVaiTro) SELECT N'ChuTro' WHERE NOT EXISTS (SELECT 1 FROM VaiTro WHERE TenVaiTro=N'ChuTro');
INSERT INTO VaiTro (TenVaiTro) SELECT N'NguoiThue' WHERE NOT EXISTS (SELECT 1 FROM VaiTro WHERE TenVaiTro=N'NguoiThue');

/* ==========================================================
   3. NGƯỜI DÙNG MẪU (Biến được khai báo chạy liền mạch trong 1 Batch)
========================================================== */
DECLARE @NguoiThue1 UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @NguoiThue2 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @ChuTro1    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @ChuTro2    UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';
DECLARE @Admin      UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555';

-- Demo IDs used for testing "My Bookings" and Notifications
DECLARE @TenantDemoId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
DECLARE @LandlordDemoId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000002';


-- Lấy Role ID
DECLARE @Role_Thue INT = (SELECT TOP 1 VaiTroId FROM VaiTro WHERE TenVaiTro='NguoiThue');
DECLARE @Role_Chu INT = (SELECT TOP 1 VaiTroId FROM VaiTro WHERE TenVaiTro='ChuTro');
DECLARE @Role_Admin INT = (SELECT TOP 1 VaiTroId FROM VaiTro WHERE TenVaiTro='Admin');

-- INSERT USER
INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId)
SELECT @NguoiThue1, 'rentera@example.com','0905000001','hash123', @Role_Thue WHERE NOT EXISTS (SELECT 1 FROM NguoiDung WHERE NguoiDungId=@NguoiThue1);
INSERT INTO HoSoNguoiDung (NguoiDungId, HoTen) SELECT @NguoiThue1, N'Người Thuê A' WHERE NOT EXISTS (SELECT 1 FROM HoSoNguoiDung WHERE NguoiDungId=@NguoiThue1);

INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId)
SELECT @NguoiThue2, 'renterb@example.com','0905000002','hash123', @Role_Thue WHERE NOT EXISTS (SELECT 1 FROM NguoiDung WHERE NguoiDungId=@NguoiThue2);
INSERT INTO HoSoNguoiDung (NguoiDungId, HoTen) SELECT @NguoiThue2, N'Người Thuê B' WHERE NOT EXISTS (SELECT 1 FROM HoSoNguoiDung WHERE NguoiDungId=@NguoiThue2);

INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId)
SELECT @ChuTro1, 'hosta@example.com','0905123456','hash123', @Role_Chu WHERE NOT EXISTS (SELECT 1 FROM NguoiDung WHERE NguoiDungId=@ChuTro1);
INSERT INTO HoSoNguoiDung (NguoiDungId, HoTen) SELECT @ChuTro1, N'Chủ Trọ A' WHERE NOT EXISTS (SELECT 1 FROM HoSoNguoiDung WHERE NguoiDungId=@ChuTro1);

INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId)
SELECT @ChuTro2, 'hostb@example.com','0905234567','hash123', @Role_Chu WHERE NOT EXISTS (SELECT 1 FROM NguoiDung WHERE NguoiDungId=@ChuTro2);
INSERT INTO HoSoNguoiDung (NguoiDungId, HoTen) SELECT @ChuTro2, N'Chủ Trọ B' WHERE NOT EXISTS (SELECT 1 FROM HoSoNguoiDung WHERE NguoiDungId=@ChuTro2);

INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId)
SELECT @Admin, 'admin@trotot.com','0905999999','adminhash', @Role_Admin WHERE NOT EXISTS (SELECT 1 FROM NguoiDung WHERE NguoiDungId=@Admin);
INSERT INTO HoSoNguoiDung (NguoiDungId, HoTen) SELECT @Admin, N'Quản trị viên' WHERE NOT EXISTS (SELECT 1 FROM HoSoNguoiDung WHERE NguoiDungId=@Admin);

-- INSERT DEMO USERS
INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId)
SELECT @TenantDemoId, 'nguoithue@test.com','0911222333','hash123', @Role_Thue WHERE NOT EXISTS (SELECT 1 FROM NguoiDung WHERE NguoiDungId=@TenantDemoId);
INSERT INTO HoSoNguoiDung (NguoiDungId, HoTen) SELECT @TenantDemoId, N'Trần Thị B (Người Thuê)' WHERE NOT EXISTS (SELECT 1 FROM HoSoNguoiDung WHERE NguoiDungId=@TenantDemoId);

INSERT INTO NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId)
SELECT @LandlordDemoId, 'chutro@test.com','0988777666','hash123', @Role_Chu WHERE NOT EXISTS (SELECT 1 FROM NguoiDung WHERE NguoiDungId=@LandlordDemoId);
INSERT INTO HoSoNguoiDung (NguoiDungId, HoTen) SELECT @LandlordDemoId, N'Nguyễn Văn A (Chủ Trọ)' WHERE NOT EXISTS (SELECT 1 FROM HoSoNguoiDung WHERE NguoiDungId=@LandlordDemoId);


/* ==========================================================
   4. NHÀ TRỌ & PHÒNG
========================================================== */
DECLARE @NhaTro1 UNIQUEIDENTIFIER = 'aaaa1111-aaaa-1111-aaaa-111111111111';
DECLARE @NhaTro2 UNIQUEIDENTIFIER = 'aaaa2222-aaaa-2222-aaaa-222222222222';

-- Lấy ID Phường (Nếu có dữ liệu ở trên)
DECLARE @P_HoaKhanhBac INT = (SELECT TOP 1 PhuongId FROM Phuong WHERE Ten=N'Hòa Khánh Bắc');
DECLARE @P_MyAn INT = (SELECT TOP 1 PhuongId FROM Phuong WHERE Ten=N'Mỹ An');

INSERT INTO NhaTro (NhaTroId, ChuTroId, TieuDe, DiaChi, QuanHuyenId, PhuongId)
SELECT @NhaTro1, @ChuTro1, N'Nhà trọ Hòa Khánh', N'123 Hòa Khánh Nam', @Q_LienChieu, @P_HoaKhanhBac WHERE NOT EXISTS (SELECT 1 FROM NhaTro WHERE NhaTroId=@NhaTro1);

INSERT INTO NhaTro (NhaTroId, ChuTroId, TieuDe, DiaChi, QuanHuyenId, PhuongId)
SELECT @NhaTro2, @ChuTro2, N'Nhà trọ Mỹ An', N'55 Mỹ An', @Q_NguHanhSon, @P_MyAn WHERE NOT EXISTS (SELECT 1 FROM NhaTro WHERE NhaTroId=@NhaTro2);

-- TẠO 20 PHÒNG MẪU
DECLARE @i INT = 1;
WHILE @i <= 20
BEGIN
    DECLARE @PhongId UNIQUEIDENTIFIER = NEWID();
    DECLARE @NhaTro UNIQUEIDENTIFIER = CASE WHEN @i <= 10 THEN @NhaTro1 ELSE @NhaTro2 END;
    
    -- Chỉ insert nếu chưa đủ 20 phòng (tránh chạy lại bị double nếu dùng NEWID)
    -- Ở đây ta chạy luôn vì NEWID() luôn mới, nhưng để test ta chấp nhận tạo thêm nếu chạy nhiều lần
    INSERT INTO Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet)
    VALUES (@PhongId, @NhaTro, CONCAT(N'Phòng mẫu số ', @i), 15 + (@i % 10), 2000000 + (@i * 50000), N'con_trong', 1);
    
    SET @i += 1;
END

/* ==========================================================
   5. TIỆN ÍCH
========================================================== */
INSERT INTO TienIch (Ten) SELECT N'Wifi' WHERE NOT EXISTS (SELECT 1 FROM TienIch WHERE Ten=N'Wifi');
INSERT INTO TienIch (Ten) SELECT N'Máy lạnh' WHERE NOT EXISTS (SELECT 1 FROM TienIch WHERE Ten=N'Máy lạnh');
INSERT INTO TienIch (Ten) SELECT N'Giường nệm' WHERE NOT EXISTS (SELECT 1 FROM TienIch WHERE Ten=N'Giường nệm');
INSERT INTO TienIch (Ten) SELECT N'Bãi đỗ xe' WHERE NOT EXISTS (SELECT 1 FROM TienIch WHERE Ten=N'Bãi đỗ xe');

-- GẮN MẶC ĐỊNH 2 TIỆN ÍCH NGẪU NHIÊN CHO CÁC PHÒNG VỪA TẠO
INSERT INTO PhongTienIch (PhongId, TienIchId)
SELECT P.PhongId, TI.TienIchId
FROM Phong P
CROSS JOIN (SELECT TOP 2 TienIchId FROM TienIch ORDER BY NEWID()) TI
WHERE NOT EXISTS (SELECT 1 FROM PhongTienIch PT WHERE PT.PhongId = P.PhongId AND PT.TienIchId = TI.TienIchId);

/* ==========================================================
   6. ĐẶT PHÒNG + BIÊN LAI + TIN NHẮN + BÁO CÁO
========================================================== */
DECLARE @DatId UNIQUEIDENTIFIER = 'dddd1111-dddd-1111-dddd-111111111111';
DECLARE @BienLai UNIQUEIDENTIFIER = 'eeee1111-eeee-1111-eeee-111111111111';

INSERT INTO DatPhong (DatPhongId, PhongId, NguoiThueId, Loai, BatDau, TrangThaiId)
SELECT 
    @DatId, 
    (SELECT TOP 1 PhongId FROM Phong), 
    @NguoiThue1, 
    N'giucho', 
    SYSDATETIMEOFFSET(), 
    (SELECT TOP 1 TrangThaiId FROM TrangThaiDatPhong WHERE TenTrangThai=N'ChoXacNhan')
WHERE NOT EXISTS (SELECT 1 FROM DatPhong WHERE DatPhongId=@DatId);
-- Insert Biên Lai (Lúc này TapTinId NULL đã được phép nhờ lệnh ALTER ở đầu)
INSERT INTO BienLai (BienLaiId, DatPhongId, NguoiTai, TapTinId, SoTien)
SELECT @BienLai, @DatId, @NguoiThue1, NULL, 500000
WHERE NOT EXISTS (SELECT 1 FROM BienLai WHERE BienLaiId=@BienLai);

INSERT INTO TinNhan (FromUser, ToUser, NoiDung) SELECT @NguoiThue1, @ChuTro1, N'Cho em hỏi phòng này còn trống không ạ?' WHERE NOT EXISTS (SELECT 1 FROM TinNhan WHERE FromUser=@NguoiThue1);
INSERT INTO TinNhan (FromUser, ToUser, NoiDung) SELECT @ChuTro1, @NguoiThue1, N'Phòng còn em nhé!' WHERE NOT EXISTS (SELECT 1 FROM TinNhan WHERE FromUser=@ChuTro1);

DECLARE @Report UNIQUEIDENTIFIER = 'cccc1111-cccc-1111-cccc-111111111111';
INSERT INTO BaoCaoViPham (BaoCaoId, LoaiThucThe, ThucTheId, NguoiBaoCao, ViPhamId, TieuDe, MoTa)
SELECT @Report, N'Phong', (SELECT TOP 1 PhongId FROM Phong), @NguoiThue2, NULL, N'Phòng không đúng mô tả', N'Hình ảnh phòng không giống thực tế'
WHERE NOT EXISTS (SELECT 1 FROM BaoCaoViPham WHERE BaoCaoId=@Report);
GO

/* ==========================================================
   7. DỮ LIỆU TEST: DUYỆT CHỦ TRỌ (HOST VERIFICATION)
   (Phần này chạy Batch mới để tránh lỗi biến)
========================================================== */
DECLARE @VaiTroChuTro INT = (SELECT TOP 1 VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'ChuTro');
DECLARE @Id_DaDuyet UNIQUEIDENTIFIER = NEWID();
DECLARE @Id_BiTuChoi UNIQUEIDENTIFIER = NEWID();
DECLARE @Id_ChoDuyet2 UNIQUEIDENTIFIER = NEWID();

-- 1. CHỦ TRỌ "ĐÃ DUYỆT"
INSERT INTO dbo.NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc, CreatedAt)
VALUES (@Id_DaDuyet, 'host_approved@test.com', '0905888888', 'hash123', @VaiTroChuTro, 0, 1, DATEADD(day, -5, SYSDATETIMEOFFSET()));

INSERT INTO dbo.HoSoNguoiDung (NguoiDungId, HoTen, NgaySinh, LoaiGiayTo, GhiChu)
VALUES (@Id_DaDuyet, N'Trần Văn Uy Tín', '1985-10-20', N'CCCD: 001085000001', N'Đã xác minh đầy đủ');

INSERT INTO dbo.TapTin (TapTinId, DuongDan, MimeType, TaiBangNguoi)
VALUES (NEWID(), 'https://upload.wikimedia.org/wikipedia/commons/9/9e/C%C4%83n_c%C6%B0%E1%BB%9Bc_c%C3%B4ng_d%C3%A2n_g%E1%BA%ABn_chip_m%E1%BA%B7t_tr%C6%B0%E1%BB%9Bc.jpg', 'image/jpeg', @Id_DaDuyet);

-- 2. CHỦ TRỌ "BỊ TỪ CHỐI / KHÓA"
INSERT INTO dbo.NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc, CreatedAt)
VALUES (@Id_BiTuChoi, 'host_rejected@test.com', '0905666666', 'hash123', @VaiTroChuTro, 1, 0, DATEADD(day, -2, SYSDATETIMEOFFSET()));

INSERT INTO dbo.HoSoNguoiDung (NguoiDungId, HoTen, NgaySinh, LoaiGiayTo, GhiChu)
VALUES (@Id_BiTuChoi, N'Lê Văn Gian Dối', '1992-01-01', N'CCCD: 001092000999', N'Lý do từ chối: Ảnh mờ, nghi vấn giả mạo');

INSERT INTO dbo.TapTin (TapTinId, DuongDan, MimeType, TaiBangNguoi)
VALUES (NEWID(), 'https://upload.wikimedia.org/wikipedia/commons/8/87/C%C4%83n_c%C6%B0%E1%BB%9Bc_c%C3%B4ng_d%C3%A2n_g%E1%BA%ABn_chip_m%E1%BA%B7t_sau.jpg', 'image/jpeg', @Id_BiTuChoi);

-- 3. CHỦ TRỌ "CHỜ DUYỆT"
INSERT INTO dbo.NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc, CreatedAt)
VALUES (@Id_ChoDuyet2, 'host_new@test.com', '0905111222', 'hash123', @VaiTroChuTro, 0, 0, SYSDATETIMEOFFSET());

INSERT INTO dbo.HoSoNguoiDung (NguoiDungId, HoTen, NgaySinh, LoaiGiayTo, GhiChu)
VALUES (@Id_ChoDuyet2, N'Phạm Thị Mới Mẻ', '1998-12-12', N'CCCD: 001098000555', N'Đang chờ admin duyệt');

PRINT N'Hoàn tất thêm dữ liệu test Đà Nẵng, Phòng mẫu và Chủ trọ!';
GO


/* ==========================================================
   PHẦN 1: BỔ SUNG 15 NGƯỜI THUÊ (RENTERS)
   ========================================================== */
DECLARE @DanhSachNguoiThue TABLE (HoTen NVARCHAR(100), Email NVARCHAR(100), SoDienThoai NVARCHAR(20));

INSERT INTO @DanhSachNguoiThue (HoTen, Email, SoDienThoai) VALUES
(N'Nguyễn Thị Lan', 'lan.nguyen@test.com', '0905000101'), (N'Trần Văn Hùng', 'hung.tran@test.com', '0905000102'),
(N'Lê Minh Tuấn', 'tuan.le@test.com', '0905000103'), (N'Phạm Thị Mai', 'mai.pham@test.com', '0905000104'),
(N'Hoàng Văn Long', 'long.hoang@test.com', '0905000105'), (N'Đặng Thị Thảo', 'thao.dang@test.com', '0905000106'),
(N'Bùi Văn Dũng', 'dung.bui@test.com', '0905000107'), (N'Đỗ Thị Hằng', 'hang.do@test.com', '0905000108'),
(N'Hồ Văn Nam', 'nam.ho@test.com', '0905000109'), (N'Ngô Thị Tuyết', 'tuyet.ngo@test.com', '0905000110'),
(N'Vũ Văn Kiên', 'kien.vu@test.com', '0905000111'), (N'Dương Thị Yến', 'yen.duong@test.com', '0905000112'),
(N'Lý Văn Phúc', 'phuc.ly@test.com', '0905000113'), (N'Mai Thị Ngọc', 'ngoc.mai@test.com', '0905000114'),
(N'Trương Văn Tài', 'tai.truong@test.com', '0905000115');

DECLARE @RoleThueId INT = (SELECT TOP 1 VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'NguoiThue');

IF @RoleThueId IS NOT NULL
BEGIN
    DECLARE @Ten NVARCHAR(100), @Mail NVARCHAR(100), @Sdt NVARCHAR(20);
    DECLARE @NewId UNIQUEIDENTIFIER;

    DECLARE cur CURSOR FOR SELECT HoTen, Email, SoDienThoai FROM @DanhSachNguoiThue;
    OPEN cur;
    FETCH NEXT FROM cur INTO @Ten, @Mail, @Sdt;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE Email = @Mail)
        BEGIN
            SET @NewId = NEWID();
            INSERT INTO dbo.NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsKhoa, IsEmailXacThuc)
            VALUES (@NewId, @Mail, @Sdt, 'hash123', @RoleThueId, 0, 1);

            INSERT INTO dbo.HoSoNguoiDung (NguoiDungId, HoTen, NgaySinh, LoaiGiayTo, GhiChu)
            VALUES (@NewId, @Ten, DATEADD(DAY, -CAST(RAND()*10000 AS INT), GETDATE()), N'CCCD: ' + @Sdt, N'Auto Generated');

            INSERT INTO dbo.NguoiDungVaiTro (NguoiDungId, VaiTroId) VALUES (@NewId, @RoleThueId);
        END
        FETCH NEXT FROM cur INTO @Ten, @Mail, @Sdt;
    END;
    CLOSE cur; DEALLOCATE cur;
    PRINT N'> Đã thêm xong 15 người thuê.';
END

/* ==========================================================
   PHẦN 2: BỔ SUNG DỮ LIỆU TEST ADMIN (CHỜ DUYỆT, KHÓA)
   ========================================================== */
DECLARE @NhaTroTest UNIQUEIDENTIFIER = (SELECT TOP 1 NhaTroId FROM dbo.NhaTro);
IF @NhaTroTest IS NOT NULL
BEGIN
    -- Tạo 5 phòng chờ duyệt
    INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    SELECT NEWID(), @NhaTroTest, N'Phòng chờ duyệt A1', 25, 1500000, N'con_trong', 0, 0, DATEADD(MINUTE, -10, SYSDATETIMEOFFSET())
    WHERE NOT EXISTS (SELECT 1 FROM Phong WHERE TieuDe = N'Phòng chờ duyệt A1');
    
    INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    SELECT NEWID(), @NhaTroTest, N'Phòng chờ duyệt A2', 20, 1200000, N'con_trong', 0, 0, DATEADD(MINUTE, -30, SYSDATETIMEOFFSET())
    WHERE NOT EXISTS (SELECT 1 FROM Phong WHERE TieuDe = N'Phòng chờ duyệt A2');

    INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    SELECT NEWID(), @NhaTroTest, N'Phòng chờ duyệt A3 (Cao cấp)', 40, 5000000, N'con_trong', 0, 0, DATEADD(HOUR, -1, SYSDATETIMEOFFSET())
    WHERE NOT EXISTS (SELECT 1 FROM Phong WHERE TieuDe = N'Phòng chờ duyệt A3 (Cao cấp)');
    
    INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    SELECT NEWID(), @NhaTroTest, N'Phòng chờ duyệt A4', 18, 1800000, N'con_trong', 0, 0, DATEADD(HOUR, -2, SYSDATETIMEOFFSET())
    WHERE NOT EXISTS (SELECT 1 FROM Phong WHERE TieuDe = N'Phòng chờ duyệt A4');
    
    INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    SELECT NEWID(), @NhaTroTest, N'Phòng chờ duyệt A5', 30, 3500000, N'con_trong', 0, 0, DATEADD(DAY, -1, SYSDATETIMEOFFSET())
    WHERE NOT EXISTS (SELECT 1 FROM Phong WHERE TieuDe = N'Phòng chờ duyệt A5');

    -- Tạo 3 phòng bị khóa
    INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    SELECT NEWID(), @NhaTroTest, N'Phòng vi phạm (Bị khóa)', 15, 500000, N'con_trong', 0, 1, DATEADD(DAY, -5, SYSDATETIMEOFFSET())
    WHERE NOT EXISTS (SELECT 1 FROM Phong WHERE TieuDe = N'Phòng vi phạm (Bị khóa)');

    INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    SELECT NEWID(), @NhaTroTest, N'Phòng tin rác (Bị khóa)', 100, 100000, N'con_trong', 0, 1, DATEADD(DAY, -4, SYSDATETIMEOFFSET())
    WHERE NOT EXISTS (SELECT 1 FROM Phong WHERE TieuDe = N'Phòng tin rác (Bị khóa)');

    INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TrangThai, IsDuyet, IsBiKhoa, CreatedAt)
    SELECT NEWID(), @NhaTroTest, N'Phòng lừa đảo (Bị khóa)', 25, 9000000, N'con_trong', 0, 1, DATEADD(DAY, -3, SYSDATETIMEOFFSET())
    WHERE NOT EXISTS (SELECT 1 FROM Phong WHERE TieuDe = N'Phòng lừa đảo (Bị khóa)');

    PRINT N'> Đã thêm xong 5 phòng chờ duyệt và 3 phòng bị khóa.';
END
GO

/* ==========================================================
   PHẦN 3: CÁC LỆNH SELECT KIỂM TRA DỮ LIỆU
   ========================================================== */
PRINT N'';
PRINT N'--- 1. KIỂM TRA DANH SÁCH NGƯỜI DÙNG MỚI (TOP 10) ---';
SELECT TOP 10 hs.HoTen, u.Email, u.DienThoai, vt.TenVaiTro, u.CreatedAt
FROM dbo.NguoiDung u
JOIN dbo.HoSoNguoiDung hs ON u.NguoiDungId = hs.NguoiDungId
JOIN dbo.VaiTro vt ON u.VaiTroId = vt.VaiTroId
WHERE vt.TenVaiTro = N'NguoiThue'
ORDER BY u.CreatedAt DESC;

PRINT N'';
PRINT N'--- 2. KIỂM TRA PHÒNG CHỜ DUYỆT (CHO ADMIN) ---';
SELECT TieuDe, GiaTien, DienTich, CreatedAt 
FROM dbo.Phong 
WHERE IsDuyet = 0 AND IsBiKhoa = 0
ORDER BY CreatedAt DESC;

PRINT N'';
PRINT N'--- 3. KIỂM TRA PHÒNG BỊ KHÓA (CHO ADMIN) ---';
SELECT TieuDe, GiaTien, DienTich, CreatedAt 
FROM dbo.Phong 
WHERE IsBiKhoa = 1
ORDER BY CreatedAt DESC;

PRINT N'';
PRINT N'--- 4. THỐNG KÊ TỔNG QUAN HỆ THỐNG ---';
SELECT 
    (SELECT COUNT(*) FROM dbo.NguoiDung) AS TongUser,
    (SELECT COUNT(*) FROM dbo.Phong) AS TongPhong,
    (SELECT COUNT(*) FROM dbo.Phong WHERE IsDuyet = 0 AND IsBiKhoa = 0) AS ChoDuyet,
    (SELECT COUNT(*) FROM dbo.Phong WHERE IsBiKhoa = 1) AS DaKhoa,
    (SELECT COUNT(*) FROM dbo.DatPhong) AS DonDatPhong;
GO

-- 1. BỔ SUNG THÊM LOẠI HỖ TRỢ (CATEGORY)
-- Hiện tại mới chỉ có SuaChua, VeSinh. Thêm các loại phổ biến khác:
INSERT INTO dbo.LoaiHoTro (TenLoai) SELECT N'AnNinh' WHERE NOT EXISTS (SELECT 1 FROM LoaiHoTro WHERE TenLoai = N'AnNinh');
INSERT INTO dbo.LoaiHoTro (TenLoai) SELECT N'ThanhToan' WHERE NOT EXISTS (SELECT 1 FROM LoaiHoTro WHERE TenLoai = N'ThanhToan');
INSERT INTO dbo.LoaiHoTro (TenLoai) SELECT N'Khac' WHERE NOT EXISTS (SELECT 1 FROM LoaiHoTro WHERE TenLoai = N'Khac');

-- 2. LẤY ID CẦN THIẾT ĐỂ INSERT
-- Lấy ID các loại hỗ trợ
DECLARE @L_SuaChua INT = (SELECT TOP 1 LoaiHoTroId FROM LoaiHoTro WHERE TenLoai = N'SuaChua');
DECLARE @L_VeSinh INT = (SELECT TOP 1 LoaiHoTroId FROM LoaiHoTro WHERE TenLoai = N'VeSinh');
DECLARE @L_AnNinh INT = (SELECT TOP 1 LoaiHoTroId FROM LoaiHoTro WHERE TenLoai = N'AnNinh');
DECLARE @L_ThanhToan INT = (SELECT TOP 1 LoaiHoTroId FROM LoaiHoTro WHERE TenLoai = N'ThanhToan');

-- Lấy ID Người thuê và Phòng mẫu (Lấy ngẫu nhiên từ data đã có)
DECLARE @User1 UNIQUEIDENTIFIER = (SELECT TOP 1 NguoiDungId FROM NguoiDung u JOIN VaiTro vt ON u.VaiTroId = vt.VaiTroId WHERE vt.TenVaiTro = N'NguoiThue');
DECLARE @User2 UNIQUEIDENTIFIER = (SELECT TOP 1 NguoiDungId FROM NguoiDung u JOIN VaiTro vt ON u.VaiTroId = vt.VaiTroId WHERE vt.TenVaiTro = N'NguoiThue' ORDER BY u.CreatedAt DESC);
DECLARE @PhongTest UNIQUEIDENTIFIER = (SELECT TOP 1 PhongId FROM Phong);

-- 3. INSERT CÁC TICKET MẪU VỚI CÁC TRẠNG THÁI KHÁC NHAU

-- Ticket 1: Yêu cầu sửa chữa (TRẠNG THÁI: MỚI)
-- Tình huống: Máy lạnh không mát
INSERT INTO dbo.YeuCauHoTro (HoTroId, PhongId, NguoiYeuCau, LoaiHoTroId, TieuDe, MoTa, TrangThai, ThoiGianTao)
SELECT 
    NEWID(), 
    @PhongTest, 
    @User1, 
    @L_SuaChua, 
    N'Máy lạnh phòng 101 không mát', 
    N'Máy lạnh bật 18 độ nhưng vẫn rất nóng, có tiếng kêu lạ ở cục nóng. Nhờ chủ trọ kiểm tra giúp.', 
    N'Moi', 
    SYSDATETIMEOFFSET()
WHERE NOT EXISTS (SELECT 1 FROM YeuCauHoTro WHERE TieuDe LIKE N'%Máy lạnh%');

-- Ticket 2: Vấn đề an ninh (TRẠNG THÁI: ĐANG XỬ LÝ)
-- Tình huống: Hàng xóm ồn ào
INSERT INTO dbo.YeuCauHoTro (HoTroId, PhongId, NguoiYeuCau, LoaiHoTroId, TieuDe, MoTa, TrangThai, ThoiGianTao)
SELECT 
    NEWID(), 
    @PhongTest, 
    @User2, 
    @L_AnNinh, 
    N'Phản ánh tiếng ồn sau 10h đêm', 
    N'Phòng bên cạnh thường xuyên tụ tập hát karaoke rất to sau 10 giờ đêm làm ảnh hưởng đến mọi người.', 
    N'DangXuLy', 
    DATEADD(HOUR, -5, SYSDATETIMEOFFSET())
WHERE NOT EXISTS (SELECT 1 FROM YeuCauHoTro WHERE TieuDe LIKE N'%tiếng ồn%');

-- Ticket 3: Vấn đề thanh toán (TRẠNG THÁI: ĐÃ XONG)
-- Tình huống: Thắc mắc tiền điện
INSERT INTO dbo.YeuCauHoTro (HoTroId, PhongId, NguoiYeuCau, LoaiHoTroId, TieuDe, MoTa, TrangThai, ThoiGianTao)
SELECT 
    NEWID(), 
    @PhongTest, 
    @User1, 
    @L_ThanhToan, 
    N'Thắc mắc về chỉ số điện tháng này', 
    N'Tháng này tôi về quê 2 tuần sao tiền điện lại cao hơn tháng trước? Nhờ admin check lại công tơ.', 
    N'DaXong', 
    DATEADD(DAY, -3, SYSDATETIMEOFFSET())
WHERE NOT EXISTS (SELECT 1 FROM YeuCauHoTro WHERE TieuDe LIKE N'%chỉ số điện%');

-- Ticket 4: Yêu cầu vệ sinh (TRẠNG THÁI: MỚI)
INSERT INTO dbo.YeuCauHoTro (HoTroId, PhongId, NguoiYeuCau, LoaiHoTroId, TieuDe, MoTa, TrangThai, ThoiGianTao)
SELECT 
    NEWID(), 
    @PhongTest, 
    @User2, 
    @L_VeSinh, 
    N'Đăng ký dọn vệ sinh cuối tuần', 
    N'Tôi muốn đăng ký dịch vụ dọn phòng vào sáng Chủ Nhật tuần này (Gói cơ bản).', 
    N'Moi', 
    DATEADD(MINUTE, -30, SYSDATETIMEOFFSET())
WHERE NOT EXISTS (SELECT 1 FROM YeuCauHoTro WHERE TieuDe LIKE N'%dọn vệ sinh%');

-- Ticket 5: Hư hỏng thiết bị nước (TRẠNG THÁI: ĐANG XỬ LÝ)
INSERT INTO dbo.YeuCauHoTro (HoTroId, PhongId, NguoiYeuCau, LoaiHoTroId, TieuDe, MoTa, TrangThai, ThoiGianTao)
SELECT 
    NEWID(), 
    @PhongTest, 
    @User1, 
    @L_SuaChua, 
    N'Vòi nước bồn rửa mặt bị rò rỉ', 
    N'Nước chảy nhỏ giọt suốt đêm gây lãng phí và ẩm mốc.', 
    N'DangXuLy', 
    DATEADD(DAY, -1, SYSDATETIMEOFFSET())
WHERE NOT EXISTS (SELECT 1 FROM YeuCauHoTro WHERE TieuDe LIKE N'%Vòi nước%');

PRINT N'=== ĐÃ TẠO XONG 5 TICKET MẪU ===';
GO

-- KIỂM TRA LẠI DỮ LIỆU VỪA TẠO
SELECT 
    t.TieuDe,
    l.TenLoai AS [LoaiHoTro],
    u.Email AS [NguoiGui],
    t.TrangThai,
    t.ThoiGianTao 
FROM dbo.YeuCauHoTro t
JOIN dbo.LoaiHoTro l ON t.LoaiHoTroId = l.LoaiHoTroId
JOIN dbo.NguoiDung u ON t.NguoiYeuCau = u.NguoiDungId
ORDER BY t.ThoiGianTao DESC;

IF OBJECT_ID(N'dbo.PhongAnh', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhongAnh (
        PhongAnhId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        PhongId    UNIQUEIDENTIFIER NOT NULL,
        TapTinId   UNIQUEIDENTIFIER NOT NULL,
        ThuTu      INT NOT NULL DEFAULT 1
    );
END;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DatPhong_Phong')
BEGIN
    ALTER TABLE dbo.PhongAnh
		ADD CONSTRAINT FK_PhongAnh_Phong
		FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);
END;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DatPhong_Phong')
BEGIN
    ALTER TABLE dbo.PhongAnh
	ADD CONSTRAINT FK_PhongAnh_TapTin
	FOREIGN KEY (TapTinId) REFERENCES dbo.TapTin(TapTinId);
END;
GO

INSERT INTO dbo.TapTin (DuongDan, MimeType)
VALUES
(N'https://ankhoadesign.com.vn/wp-content/uploads/2024/09/mau-cai-tao-phong-tro-cu-su-dung-anh-sang-mem-am-ap.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-46.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-13.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-59.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-29.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-28.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-61.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-55.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-20.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-19.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-45.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-56.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-54.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-4.jpg',  N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-41.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-30.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-21.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-47.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-60.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-14.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-26.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-58.jpg', N'image/jpeg'),
(N'https://s-housing.vn/wp-content/uploads/2022/09/thiet-ke-phong-tro-dep-7.jpg',  N'image/jpeg');
GO

select * from Phong
;WITH PhongCTE AS (
    SELECT PhongId, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS rn
    FROM dbo.Phong
),
AnhCTE AS (
    SELECT TapTinId, ROW_NUMBER() OVER (ORDER BY ThoiGianTai) AS rn
    FROM dbo.TapTin
)
INSERT INTO dbo.PhongAnh (PhongId, TapTinId, ThuTu)
SELECT
    p.PhongId,
    a.TapTinId,
    1
FROM PhongCTE p
JOIN AnhCTE a ON p.rn = a.rn;
GO
SELECT
    p.PhongId,
    p.TieuDe,
    p.GiaTien,
    t.DuongDan AS AnhDaiDien
FROM Phong p
LEFT JOIN PhongAnh pa ON p.PhongId = pa.PhongId
LEFT JOIN TapTin t ON pa.TapTinId = t.TapTinId
WHERE p.IsDuyet = 1
  AND p.IsBiKhoa = 0;



--------------------------------------------------
-- 15. TẠO BẢNG HOPDONG, HOADON, PAYMENT (Merged from QuanLyPhongTro1.sql)
--------------------------------------------------
BEGIN TRY
    BEGIN TRANSACTION;

    -- 1. BẢNG HOPDONG
    IF OBJECT_ID(N'dbo.HopDong', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.HopDong (
            HopDongId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
            DatPhongId UNIQUEIDENTIFIER NOT NULL,
            PhongId UNIQUEIDENTIFIER NOT NULL,
            ChuTroId UNIQUEIDENTIFIER NOT NULL,
            NguoiThueId UNIQUEIDENTIFIER NOT NULL,
            NgayBatDau DATE NOT NULL,
            NgayKetThuc DATE NULL,
            TienThue BIGINT NOT NULL,
            TienCoc BIGINT NULL,
            TrangThai NVARCHAR(50) NOT NULL DEFAULT N'ConHieuLuc',
            CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
        );
    END;

    -- 2. KHÓA NGOẠI HOPDONG
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_HopDong_DatPhong')
        ALTER TABLE dbo.HopDong ADD CONSTRAINT FK_HopDong_DatPhong FOREIGN KEY (DatPhongId) REFERENCES dbo.DatPhong(DatPhongId);

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_HopDong_Phong')
        ALTER TABLE dbo.HopDong ADD CONSTRAINT FK_HopDong_Phong FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_HopDong_ChuTro')
        ALTER TABLE dbo.HopDong ADD CONSTRAINT FK_HopDong_ChuTro FOREIGN KEY (ChuTroId) REFERENCES dbo.NguoiDung(NguoiDungId);

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_HopDong_NguoiThue')
        ALTER TABLE dbo.HopDong ADD CONSTRAINT FK_HopDong_NguoiThue FOREIGN KEY (NguoiThueId) REFERENCES dbo.NguoiDung(NguoiDungId);

    -- 3. BẢNG HOADON
    IF OBJECT_ID(N'dbo.HoaDon', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.HoaDon (
            HoaDonId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
            HopDongId UNIQUEIDENTIFIER NOT NULL,
            Thang INT NOT NULL,
            Nam INT NOT NULL,
            TienPhong BIGINT NOT NULL,
            TienDien BIGINT NULL,
            TienNuoc BIGINT NULL,
            TienDichVu BIGINT NULL,
            TongTien BIGINT NOT NULL,
            TrangThai NVARCHAR(50) NOT NULL DEFAULT N'ChuaThanhToan',
            NgayLap DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
            NgayThanhToan DATETIMEOFFSET NULL
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_HoaDon_HopDong')
        ALTER TABLE dbo.HoaDon ADD CONSTRAINT FK_HoaDon_HopDong FOREIGN KEY (HopDongId) REFERENCES dbo.HopDong(HopDongId);

    -- 4. BẢNG PAYMENT
    IF OBJECT_ID(N'dbo.Payment', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Payment (
            PaymentId uniqueidentifier NOT NULL PRIMARY KEY DEFAULT NEWID(),
            HoaDonId uniqueidentifier NOT NULL,
            Amount bigint NOT NULL,
            Reference nvarchar(200) NULL,
            QrImageUrl nvarchar(2000) NULL,
            Status nvarchar(50) NOT NULL DEFAULT N'UNPAID',
            EvidenceTapTinId uniqueidentifier NULL,
            CreatedAt datetimeoffset NOT NULL DEFAULT SYSDATETIMEOFFSET(),
            VerifiedAt datetimeoffset NULL,
            VerifiedBy uniqueidentifier NULL
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Payment_HoaDon')
        ALTER TABLE dbo.Payment ADD CONSTRAINT FK_Payment_HoaDon FOREIGN KEY (HoaDonId) REFERENCES dbo.HoaDon(HoaDonId);

    -- 5. SEED DATA (NẾU CHƯA CÓ)
    -- Seed HopDong từ DatPhong mẫu
    IF NOT EXISTS (SELECT 1 FROM dbo.HopDong)
    BEGIN
        INSERT INTO dbo.HopDong (DatPhongId, PhongId, ChuTroId, NguoiThueId, NgayBatDau, NgayKetThuc, TienThue, TienCoc, TrangThai)
        SELECT TOP 1
            dp.DatPhongId, dp.PhongId, nt.ChuTroId, dp.NguoiThueId,
            '2025-01-01', '2025-12-31', p.GiaTien, p.GiaTien, N'ConHieuLuc'
        FROM dbo.DatPhong dp
        JOIN dbo.Phong p ON dp.PhongId = p.PhongId
        JOIN dbo.NhaTro nt ON p.NhaTroId = nt.NhaTroId;
    END;

    -- Seed HoaDon từ HopDong mẫu
    IF NOT EXISTS (SELECT 1 FROM dbo.HoaDon)
    BEGIN
        INSERT INTO dbo.HoaDon (HopDongId, Thang, Nam, TienPhong, TienDien, TienNuoc, TienDichVu, TongTien, TrangThai)
        SELECT TOP 1
            HopDongId, 1, 2025, TienThue, 200000, 100000, 50000, TienThue + 350000, N'ChuaThanhToan'
        FROM dbo.HopDong
        ORDER BY CreatedAt DESC;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    DECLARE @Mem_Err NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT N'Error merging Contract tables: ' + @Mem_Err;
END CATCH;
GO

-- ADDED BY AGENT FOR REAL NOTIFICATIONS
IF OBJECT_ID(N'dbo.ThongBao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ThongBao (
        ThongBaoId     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        NguoiDungId    UNIQUEIDENTIFIER NOT NULL,
        TieuDe         NVARCHAR(250) NOT NULL,
        NoiDung        NVARCHAR(MAX) NOT NULL,
        Loai           NVARCHAR(50) NULL,
        DaXem          BIT NOT NULL DEFAULT 0,
        ThoiGianTao    DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
        RedirectUrl    NVARCHAR(200) NULL
    );
END;
GO
IF COL_LENGTH('dbo.DatPhong', 'ChuTroId') IS NULL
BEGIN
    ALTER TABLE dbo.DatPhong ADD ChuTroId UNIQUEIDENTIFIER NULL;
END;
go
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'dbo.DatPhong') 
               AND name = N'GhiChu')
BEGIN
    ALTER TABLE dbo.DatPhong ADD GhiChu NVARCHAR(100) NULL;
END
-- SEED DATA FOR DEMO APPOINTMENTS (Lịch Hẹn)
-- Lấy ID một vài phòng để làm mẫu
DECLARE @TenantDemoId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
DECLARE @LandlordDemoId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000002';
DECLARE @RoomA UNIQUEIDENTIFIER = (SELECT TOP 1 PhongId FROM dbo.Phong);
DECLARE @RoomB UNIQUEIDENTIFIER = (SELECT TOP 1 PhongId FROM dbo.Phong WHERE PhongId != @RoomA);

IF @RoomA IS NOT NULL
BEGIN
    INSERT INTO dbo.DatPhong (DatPhongId, PhongId, NguoiThueId, ChuTroId, Loai, BatDau, KetThuc, ThoiGianTao, TrangThaiId, GhiChu)
    VALUES 
    (NEWID(), @RoomA, @TenantDemoId, @LandlordDemoId, 'XemPhong', DATEADD(DAY, 1, SYSDATETIMEOFFSET()), NULL, SYSDATETIMEOFFSET(), 1, N'Tôi muốn xem phòng vào sáng mai lúc 9h.'),
    (NEWID(), @RoomB, @TenantDemoId, @LandlordDemoId, 'XemPhong', DATEADD(DAY, 2, SYSDATETIMEOFFSET()), NULL, DATEADD(HOUR, -10, SYSDATETIMEOFFSET()), 2, N'Lịch hẹn này đã được chủ trọ xác nhận.');
END
GO

-- SEED DATA FOR DEMO NOTIFICATIONS
DECLARE @TenantDemoId_N UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
DECLARE @LandlordDemoId_N UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000002';

IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE NguoiDungId = @TenantDemoId_N)
BEGIN
    PRINT '⚠️ Warning: Demo Tenant user missing!';
END

-- Notifications for Landlord
INSERT INTO dbo.ThongBao (ThongBaoId, NguoiDungId, TieuDe, NoiDung, Loai, ThoiGianTao, DaXem)
VALUES 
(NEWID(), @LandlordDemoId_N, N'Yêu cầu đặt phòng mới', N'Khách hàng Nguyễn Văn A vừa gửi yêu cầu đặt lịch xem phòng 101.', 'info', SYSDATETIMEOFFSET(), 0),
(NEWID(), @LandlordDemoId_N, N'Yêu cầu đặt phòng mới', N'Khách hàng Trần Thị B muốn đặt phòng 202 từ tháng sau.', 'info', DATEADD(HOUR, -2, SYSDATETIMEOFFSET()), 0),
(NEWID(), @LandlordDemoId_N, N'Thanh toán thành công', N'Người thuê phòng 305 đã gửi biên lai thanh toán tiền nhà tháng 12.', 'success', DATEADD(DAY, -1, SYSDATETIMEOFFSET()), 1);

-- Notifications for Tenant
INSERT INTO dbo.ThongBao (ThongBaoId, NguoiDungId, TieuDe, NoiDung, Loai, ThoiGianTao, DaXem)
VALUES 
(NEWID(), @TenantDemoId_N, N'Đã xác nhận lịch hẹn', N'Chủ trọ đã đồng ý lịch xem phòng vào 9h sáng mai.', 'success', SYSDATETIMEOFFSET(), 0),
(NEWID(), @TenantDemoId_N, N'Yêu cầu thanh toán', N'Đã có hóa đơn tiền điện nước tháng 12, vui lòng thanh toán đúng hạn.', 'warning', DATEADD(HOUR, -5, SYSDATETIMEOFFSET()), 0);
GO
/*
-- AGENT: Ensure all rooms have descriptions and utilities
PRINT N'--- CẬP NHẬT MÔ TẢ VÀ TIỆN ÍCH CHO TẤT CẢ PHÒNG ---';

-- 1. Thêm cột MoTa nếu chưa có (Idempotent)
IF COL_LENGTH('dbo.Phong', 'MoTa') IS NULL
BEGIN
    ALTER TABLE dbo.Phong ADD MoTa NVARCHAR(MAX) NULL;
END

-- 2. Cập nhật mô tả mẫu cho các phòng chưa có
/*UPDATE dbo.Phong
SET MoTa = CASE 
    WHEN DienTich > 30 THEN N'Phòng trọ cao cấp, không gian rộng rãi, thoáng mát, đầy đủ tiện nghi. Tọa lạc tại vị trí đắc địa, giao thông thuận tiện, an ninh đảm bảo 24/7. Phù hợp cho hộ gia đình hoặc nhóm bạn ở từ 3-4 người.'
    WHEN GiaTien > 3000000 THEN N'Phòng trọ studio hiện đại, thiết kế tinh tế, tối ưu diện tích. Nội thất đầy đủ bao gồm giường, tủ, máy lạnh. Khu vực yên tĩnh, dân trí cao, gần chợ và các trường đại học.'
    ELSE N'Phòng trọ giá rẻ, sạch sẽ, an ninh tốt. Điện nước tính theo giá nhà nước, chủ nhà thân thiện. Rất phù hợp cho sinh viên và người lao động muốn tiết kiệm chi phí mà vẫn đảm bảo chất lượng sống.'
END
WHERE MoTa IS NULL OR MoTa = '';*/

-- 3. Đảm bảo mỗi phòng có ít nhất 3 tiện ích ngẫu nhiên
INSERT INTO dbo.PhongTienIch (PhongId, TienIchId)
SELECT p.PhongId, ti.TienIchId
FROM dbo.Phong p
CROSS JOIN dbo.TienIch ti
WHERE NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch pti WHERE pti.PhongId = p.PhongId AND pti.TienIchId = ti.TienIchId)
AND ti.TienIchId IN (
    SELECT TOP 3 TienIchId 
    FROM dbo.TienIch 
    ORDER BY ABS(CAST(BINARY_CHECKSUM(p.PhongId, ti.TienIchId, NEWID()) AS INT))
);

PRINT N'✅ Đã cập nhật xong dữ liệu mô tả và tiện ích.';
*/
GO

-- Create SystemSettings table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SystemSettings' and xtype='U')
BEGIN
    CREATE TABLE [SystemSettings] (
        [SettingId] UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
        [SettingKey] NVARCHAR(255) NOT NULL,
        [SettingValue] NVARCHAR(MAX) NOT NULL,
        [DataType] NVARCHAR(50) NULL,
        [Description] NVARCHAR(500) NULL,
        [GroupName] NVARCHAR(100) NULL,
        [IsVisible] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
        [UpdatedAt] DATETIMEOFFSET NULL,
        PRIMARY KEY ([SettingId]),
        UNIQUE ([SettingKey])
    );
    
    -- Create index for better query performance
    CREATE INDEX [IX_SystemSettings_GroupName] ON [SystemSettings] ([GroupName]);
    CREATE INDEX [IX_SystemSettings_SettingKey] ON [SystemSettings] ([SettingKey]);
END

-- Insert default system settings if they don't exist
IF NOT EXISTS (SELECT 1 FROM [SystemSettings] WHERE [SettingKey] = 'app.name')
BEGIN
    INSERT INTO [SystemSettings] ([SettingKey], [SettingValue], [DataType], [Description], [GroupName], [IsVisible])
    VALUES 
        ('app.name', 'Quản Lý Phòng Trọ', 'string', 'Tên ứng dụng', 'general', 1),
        ('app.description', 'Ứng dụng quản lý phòng trọ toàn diện', 'string', 'Mô tả ứng dụng', 'general', 1),
        ('app.url', 'https://example.com', 'string', 'URL ứng dụng', 'general', 1),
        ('support.hotline', '0123 456 789', 'string', 'Hotline hỗ trợ', 'contact', 1),
        ('support.email', 'support@example.com', 'string', 'Email hỗ trợ', 'contact', 1),
        ('company.address', '123 Đường ABC, Q.1, TP.HCM', 'string', 'Địa chỉ công ty', 'contact', 1),
        ('service.post_fee', '10000', 'decimal', 'Phí đăng tin', 'service', 1),
        ('service.boost_fee', '50000', 'decimal', 'Phí đẩy bài', 'service', 1),
        ('service.verify_fee', '100000', 'decimal', 'Phí xác minh', 'service', 1),
        ('policy.auto_approve', 'false', 'boolean', 'Tự động duyệt bài', 'policy', 1),
        ('policy.review_timeout_hours', '24', 'integer', 'Thời gian duyệt tối đa (giờ)', 'policy', 1),
        ('security.require_email_verify', 'true', 'boolean', 'Yêu cầu xác minh email', 'security', 1),
        ('security.require_phone_verify', 'false', 'boolean', 'Yêu cầu xác minh điện thoại', 'security', 1),
        ('security.blocked_ips', '', 'string', 'Danh sách IP bị chặn', 'security', 1),
        ('appearance.theme_color', 'blue', 'string', 'Màu chủ đề', 'appearance', 1),
        ('appearance.logo_url', '/Content/img/logo.png', 'string', 'URL logo', 'appearance', 1),
        ('appearance.language', 'vi', 'string', 'Ngôn ngữ mặc định', 'appearance', 1);
END
GO
