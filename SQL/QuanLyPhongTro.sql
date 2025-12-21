/**********************************************************************
 Fixed full init script for DB "QuanLyPhongTro"
 - Ensure admin-related columns exist BEFORE creating stored procedures
 - Idempotent: checks existence before CREATE / ALTER
**********************************************************************/

USE master;
GO

-- Create DB if missing
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'QuanLyPhongTro')
BEGIN
    CREATE DATABASE [QuanLyPhongTro];
    PRINT N'Created database QuanLyPhongTro.';
END
ELSE
    PRINT N'Database QuanLyPhongTro already exists.';
GO

USE [QuanLyPhongTro];
GO

-- ========== Basic tables creation (only create if not exists) ==========
-- (For brevity this block recreates the core tables if missing.
--  If you already ran earlier script, most will exist and be skipped.)

IF OBJECT_ID(N'dbo.VaiTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.VaiTro (
        VaiTroId INT IDENTITY(1,1) PRIMARY KEY,
        TenVaiTro NVARCHAR(100) NOT NULL UNIQUE
    );
END
GO

IF OBJECT_ID(N'dbo.NguoiDung', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NguoiDung (
        NguoiDungId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        Email NVARCHAR(255) NULL UNIQUE,
        DienThoai NVARCHAR(50) NULL,
        PasswordHash NVARCHAR(512) NULL,
        VaiTroId INT NOT NULL,
        IsKhoa BIT DEFAULT 0,
        IsEmailXacThuc BIT DEFAULT 0,
        CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        UpdatedAt DATETIMEOFFSET NULL
    );
END
GO

IF OBJECT_ID(N'dbo.HoSoNguoiDung', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HoSoNguoiDung (
        NguoiDungId UNIQUEIDENTIFIER PRIMARY KEY,
        HoTen NVARCHAR(200) NULL,
        NgaySinh DATE NULL,
        LoaiGiayTo NVARCHAR(100) NULL,
        GhiChu NVARCHAR(1000) NULL,
        CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    );
END
GO

IF OBJECT_ID(N'dbo.QuanHuyen', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QuanHuyen (
        QuanHuyenId INT IDENTITY(1,1) PRIMARY KEY,
        Ten NVARCHAR(200) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Phuong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Phuong (
        PhuongId INT IDENTITY(1,1) PRIMARY KEY,
        QuanHuyenId INT NOT NULL,
        Ten NVARCHAR(200) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.NhaTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhaTro (
        NhaTroId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        ChuTroId UNIQUEIDENTIFIER NOT NULL,
        TieuDe NVARCHAR(300) NOT NULL,
        DiaChi NVARCHAR(500) NULL,
        QuanHuyenId INT NULL,
        PhuongId INT NULL,
        CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        IsHoatDong BIT DEFAULT 1
    );
END
GO

-- Create Phong with admin columns if missing (create only if table absent)
IF OBJECT_ID(N'dbo.Phong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Phong (
        PhongId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        NhaTroId UNIQUEIDENTIFIER NOT NULL,
        TieuDe NVARCHAR(250) NULL,
        DienTich DECIMAL(8,2) NULL,
        GiaTien BIGINT NOT NULL,
        TienCoc BIGINT NULL,
        SoNguoiToiDa INT DEFAULT 1,
        TrangThai NVARCHAR(50) NOT NULL DEFAULT N'con_trong',
        CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        UpdatedAt DATETIMEOFFSET NULL,
        DiemTrungBinh FLOAT NULL,
        SoLuongDanhGia INT DEFAULT 0,
        IsDuyet BIT DEFAULT 0,
        NguoiDuyet UNIQUEIDENTIFIER NULL,
        ThoiGianDuyet DATETIMEOFFSET NULL,
        IsBiKhoa BIT DEFAULT 0
    );
END
GO

IF OBJECT_ID(N'dbo.TienIch', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TienIch (
        TienIchId INT IDENTITY(1,1) PRIMARY KEY,
        Ten NVARCHAR(200) NOT NULL UNIQUE
    );
END
GO

IF OBJECT_ID(N'dbo.PhongTienIch', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhongTienIch (
        PId INT IDENTITY(1,1) PRIMARY KEY,
        PhongId UNIQUEIDENTIFIER NOT NULL,
        TienIchId INT NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.TapTin', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TapTin (
        TapTinId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        DuongDan NVARCHAR(1000) NOT NULL,
        MimeType NVARCHAR(100) NULL,
        TaiBangNguoi UNIQUEIDENTIFIER NULL,
        ThoiGianTai DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    );
END
GO

IF OBJECT_ID(N'dbo.TrangThaiDatPhong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TrangThaiDatPhong (
        TrangThaiId INT IDENTITY(1,1) PRIMARY KEY,
        TenTrangThai NVARCHAR(100) NOT NULL UNIQUE
    );
END
GO

IF OBJECT_ID(N'dbo.DatPhong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DatPhong (
        DatPhongId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        PhongId UNIQUEIDENTIFIER NOT NULL,
        NguoiThueId UNIQUEIDENTIFIER NOT NULL,
        ChuTroId UNIQUEIDENTIFIER NOT NULL,
        Loai NVARCHAR(30) NOT NULL,
        BatDau DATETIMEOFFSET NOT NULL,
        KetThuc DATETIMEOFFSET NULL,
        ThoiGianTao DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        TrangThaiId INT NOT NULL,
        TapTinBienLaiId UNIQUEIDENTIFIER NULL,
        GhiChu NVARCHAR(MAX) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.BienLai', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BienLai (
        BienLaiId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        DatPhongId UNIQUEIDENTIFIER NOT NULL,
        NguoiTai UNIQUEIDENTIFIER NOT NULL,
        TapTinId UNIQUEIDENTIFIER NOT NULL,
        SoTien BIGINT NULL,
        ThoiGianTai DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        DaXacNhan BIT DEFAULT 0,
        NguoiXacNhan UNIQUEIDENTIFIER NULL
    );
END
GO

IF OBJECT_ID(N'dbo.LoaiHoTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoaiHoTro (
        LoaiHoTroId INT IDENTITY(1,1) PRIMARY KEY,
        TenLoai NVARCHAR(200) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.YeuCauHoTro', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.YeuCauHoTro (
        HoTroId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        PhongId UNIQUEIDENTIFIER NULL,
        -- note: create NguoiYeuCau column explicitly later if missing
        LoaiHoTroId INT NOT NULL,
        TieuDe NVARCHAR(300) NOT NULL,
        MoTa NVARCHAR(MAX) NULL,
        TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Moi',
        ThoiGianTao DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    );
END
GO

IF OBJECT_ID(N'dbo.TinNhan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TinNhan (
        TinNhanId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        FromUser UNIQUEIDENTIFIER NOT NULL,
        ToUser UNIQUEIDENTIFIER NOT NULL,
        NoiDung NVARCHAR(MAX) NULL,
        TapTinId UNIQUEIDENTIFIER NULL,
        ThoiGian DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        DaDoc BIT DEFAULT 0
    );
END
GO

IF OBJECT_ID(N'dbo.DanhGiaPhong', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGiaPhong (
        DanhGiaId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        PhongId UNIQUEIDENTIFIER NOT NULL,
        NguoiDanhGia UNIQUEIDENTIFIER NOT NULL,
        Diem INT NOT NULL CHECK (Diem BETWEEN 1 AND 5),
        NoiDung NVARCHAR(1000) NULL,
        ThoiGian DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    );
END
GO

IF OBJECT_ID(N'dbo.ViPham', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ViPham (
        ViPhamId INT IDENTITY(1,1) PRIMARY KEY,
        TenViPham NVARCHAR(200) NOT NULL,
        MoTa NVARCHAR(1000) NULL,
        HinhPhatTien BIGINT NULL,
        SoDiemTru INT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.BaoCaoViPham', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BaoCaoViPham (
        BaoCaoId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        LoaiThucThe NVARCHAR(50) NOT NULL,
        ThucTheId UNIQUEIDENTIFIER NULL,
        NguoiBaoCao UNIQUEIDENTIFIER NOT NULL,
        ViPhamId INT NULL,
        TieuDe NVARCHAR(300) NOT NULL,
        MoTa NVARCHAR(MAX) NULL,
        TrangThai NVARCHAR(50) NOT NULL DEFAULT N'ChoXuLy',
        KetQua NVARCHAR(1000) NULL,
        NguoiXuLy UNIQUEIDENTIFIER NULL,
        ThoiGianBaoCao DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        ThoiGianXuLy DATETIMEOFFSET NULL
    );
END
GO

IF OBJECT_ID(N'dbo.HanhDongAdmin', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HanhDongAdmin (
        HanhDongId BIGINT IDENTITY(1,1) PRIMARY KEY,
        AdminId UNIQUEIDENTIFIER NOT NULL,
        HanhDong NVARCHAR(200) NOT NULL,
        MucTieuBang NVARCHAR(200) NULL,
        BanGhiId NVARCHAR(200) NULL,
        ChiTiet NVARCHAR(MAX) NULL,
        ThoiGian DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    );
END
GO

IF OBJECT_ID(N'dbo.LichSu', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LichSu (
        LichSuId BIGINT IDENTITY(1,1) PRIMARY KEY,
        NguoiDungId UNIQUEIDENTIFIER NULL,
        HanhDong NVARCHAR(200) NOT NULL,
        TenBang NVARCHAR(200) NULL,
        BanGhiId NVARCHAR(200) NULL,
        ChiTiet NVARCHAR(MAX) NULL,
        ThoiGian DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
    );
END
GO

IF OBJECT_ID(N'dbo.TokenThongBao', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TokenThongBao (
        TokenId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        NguoiDungId UNIQUEIDENTIFIER NOT NULL,
        Token NVARCHAR(1000) NOT NULL,
        ThoiGianTao DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        IsActive BIT DEFAULT 1
    );
END
GO

-- ========== Add missing admin-related columns if table existed without them ==========
-- Ensure Phong columns exist (IsDuyet, NguoiDuyet, ThoiGianDuyet, IsBiKhoa)
IF OBJECT_ID(N'dbo.Phong','U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Phong','IsDuyet') IS NULL
    BEGIN
        ALTER TABLE dbo.Phong ADD IsDuyet BIT DEFAULT 0;
        PRINT N'Added column Phong.IsDuyet';
    END

    IF COL_LENGTH('dbo.Phong','NguoiDuyet') IS NULL
    BEGIN
        ALTER TABLE dbo.Phong ADD NguoiDuyet UNIQUEIDENTIFIER NULL;
        PRINT N'Added column Phong.NguoiDuyet';
    END

    IF COL_LENGTH('dbo.Phong','ThoiGianDuyet') IS NULL
    BEGIN
        ALTER TABLE dbo.Phong ADD ThoiGianDuyet DATETIMEOFFSET NULL;
        PRINT N'Added column Phong.ThoiGianDuyet';
    END

    IF COL_LENGTH('dbo.Phong','IsBiKhoa') IS NULL
    BEGIN
        ALTER TABLE dbo.Phong ADD IsBiKhoa BIT DEFAULT 0;
        PRINT N'Added column Phong.IsBiKhoa';
    END
END
GO

-- Ensure YeuCauHoTro.NguoiYeuCau exists (some older scripts used different name)
IF OBJECT_ID(N'dbo.YeuCauHoTro','U') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.YeuCauHoTro','NguoiYeuCau') IS NULL
    BEGIN
        ALTER TABLE dbo.YeuCauHoTro ADD NguoiYeuCau UNIQUEIDENTIFIER NULL;
        PRINT N'Added column YeuCauHoTro.NguoiYeuCau (nullable)';
    END
END
GO

-- ========== Add foreign keys (only add if missing and both sides exist) ==========
-- We'll add the most relevant FKs; skip ones you intentionally want loose.

-- NguoiDung -> VaiTro
IF OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL AND OBJECT_ID(N'dbo.VaiTro','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_NguoiDung_VaiTro')
BEGIN
    ALTER TABLE dbo.NguoiDung ADD CONSTRAINT FK_NguoiDung_VaiTro FOREIGN KEY (VaiTroId) REFERENCES dbo.VaiTro(VaiTroId);
END
GO

-- NhaTro -> NguoiDung (ChuTro)
IF OBJECT_ID(N'dbo.NhaTro','U') IS NOT NULL AND OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_NhaTro_NguoiDung')
BEGIN
    ALTER TABLE dbo.NhaTro ADD CONSTRAINT FK_NhaTro_NguoiDung FOREIGN KEY (ChuTroId) REFERENCES dbo.NguoiDung(NguoiDungId);
END
GO

-- Phong -> NhaTro
IF OBJECT_ID(N'dbo.Phong','U') IS NOT NULL AND OBJECT_ID(N'dbo.NhaTro','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_Phong_NhaTro')
BEGIN
    ALTER TABLE dbo.Phong ADD CONSTRAINT FK_Phong_NhaTro FOREIGN KEY (NhaTroId) REFERENCES dbo.NhaTro(NhaTroId);
END
GO

-- DatPhong -> Phong/NguoiDung/TrangThai
IF OBJECT_ID(N'dbo.DatPhong','U') IS NOT NULL AND OBJECT_ID(N'dbo.Phong','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_DatPhong_Phong')
BEGIN
    ALTER TABLE dbo.DatPhong ADD CONSTRAINT FK_DatPhong_Phong FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);
END
GO
IF OBJECT_ID(N'dbo.DatPhong','U') IS NOT NULL AND OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_DatPhong_NguoiThue')
BEGIN
    ALTER TABLE dbo.DatPhong ADD CONSTRAINT FK_DatPhong_NguoiThue FOREIGN KEY (NguoiThueId) REFERENCES dbo.NguoiDung(NguoiDungId);
END
GO
IF OBJECT_ID(N'dbo.DatPhong','U') IS NOT NULL AND OBJECT_ID(N'dbo.TrangThaiDatPhong','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_DatPhong_TrangThai')
BEGIN
    ALTER TABLE dbo.DatPhong ADD CONSTRAINT FK_DatPhong_TrangThai FOREIGN KEY (TrangThaiId) REFERENCES dbo.TrangThaiDatPhong(TrangThaiId);
END
GO

-- YeuCauHoTro -> NguoiYeuCau FK
IF OBJECT_ID(N'dbo.YeuCauHoTro','U') IS NOT NULL AND OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL
AND COL_LENGTH('dbo.YeuCauHoTro','NguoiYeuCau') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_YeuCauHoTro_NguoiYeuCau')
BEGIN
    ALTER TABLE dbo.YeuCauHoTro ADD CONSTRAINT FK_YeuCauHoTro_NguoiYeuCau FOREIGN KEY (NguoiYeuCau) REFERENCES dbo.NguoiDung(NguoiDungId);
END
GO

-- BaoCaoViPham -> NguoiBaoCao, ViPham
IF OBJECT_ID(N'dbo.BaoCaoViPham','U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_BaoCao_NguoiBaoCao')
    BEGIN
        ALTER TABLE dbo.BaoCaoViPham ADD CONSTRAINT FK_BaoCao_NguoiBaoCao FOREIGN KEY (NguoiBaoCao) REFERENCES dbo.NguoiDung(NguoiDungId);
    END
    IF OBJECT_ID(N'dbo.ViPham','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_BaoCao_ViPham')
    BEGIN
        ALTER TABLE dbo.BaoCaoViPham ADD CONSTRAINT FK_BaoCao_ViPham FOREIGN KEY (ViPhamId) REFERENCES dbo.ViPham(ViPhamId);
    END
END
GO

-- HanhDongAdmin.AdminId -> NguoiDung
IF OBJECT_ID(N'dbo.HanhDongAdmin','U') IS NOT NULL AND OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_HanhDongAdmin_Admin')
BEGIN
    ALTER TABLE dbo.HanhDongAdmin ADD CONSTRAINT FK_HanhDongAdmin_Admin FOREIGN KEY (AdminId) REFERENCES dbo.NguoiDung(NguoiDungId);
END
GO

-- LichSu -> NguoiDung
IF OBJECT_ID(N'dbo.LichSu','U') IS NOT NULL AND OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_LichSu_NguoiDung')
BEGIN
    ALTER TABLE dbo.LichSu ADD CONSTRAINT FK_LichSu_NguoiDung FOREIGN KEY (NguoiDungId) REFERENCES dbo.NguoiDung(NguoiDungId);
END
GO

-- TapTin -> NguoiDung
IF OBJECT_ID(N'dbo.TapTin','U') IS NOT NULL AND OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_TapTin_NguoiDung')
BEGIN
    ALTER TABLE dbo.TapTin ADD CONSTRAINT FK_TapTin_NguoiDung FOREIGN KEY (TaiBangNguoi) REFERENCES dbo.NguoiDung(NguoiDungId);
END
GO

-- TokenThongBao -> NguoiDung
IF OBJECT_ID(N'dbo.TokenThongBao','U') IS NOT NULL AND OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL
AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_TokenThongBao_NguoiDung')
BEGIN
    ALTER TABLE dbo.TokenThongBao ADD CONSTRAINT FK_TokenThongBao_NguoiDung FOREIGN KEY (NguoiDungId) REFERENCES dbo.NguoiDung(NguoiDungId);
END
GO

-- DanhGiaPhong FKs
IF OBJECT_ID(N'dbo.DanhGiaPhong','U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.Phong','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_DanhGiaPhong_Phong')
    BEGIN
        ALTER TABLE dbo.DanhGiaPhong ADD CONSTRAINT FK_DanhGiaPhong_Phong FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);
    END
    IF OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_DanhGiaPhong_NguoiDanhGia')
    BEGIN
        ALTER TABLE dbo.DanhGiaPhong ADD CONSTRAINT FK_DanhGiaPhong_NguoiDanhGia FOREIGN KEY (NguoiDanhGia) REFERENCES dbo.NguoiDung(NguoiDungId);
    END
END
GO

-- PhongTienIch FKs
IF OBJECT_ID(N'dbo.PhongTienIch','U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.Phong','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_PhongTienIch_Phong')
    BEGIN
        ALTER TABLE dbo.PhongTienIch ADD CONSTRAINT FK_PhongTienIch_Phong FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId);
    END
    IF OBJECT_ID(N'dbo.TienIch','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_PhongTienIch_TienIch')
    BEGIN
        ALTER TABLE dbo.PhongTienIch ADD CONSTRAINT FK_PhongTienIch_TienIch FOREIGN KEY (TienIchId) REFERENCES dbo.TienIch(TienIchId);
    END
END
GO

-- TinNhan -> FromUser/ToUser/TapTin
IF OBJECT_ID(N'dbo.TinNhan','U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_TinNhan_FromUser')
    BEGIN
        ALTER TABLE dbo.TinNhan ADD CONSTRAINT FK_TinNhan_FromUser FOREIGN KEY (FromUser) REFERENCES dbo.NguoiDung(NguoiDungId);
    END
    IF OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_TinNhan_ToUser')
    BEGIN
        ALTER TABLE dbo.TinNhan ADD CONSTRAINT FK_TinNhan_ToUser FOREIGN KEY (ToUser) REFERENCES dbo.NguoiDung(NguoiDungId);
    END
    IF OBJECT_ID(N'dbo.TapTin','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_TinNhan_TapTin')
    BEGIN
        ALTER TABLE dbo.TinNhan ADD CONSTRAINT FK_TinNhan_TapTin FOREIGN KEY (TapTinId) REFERENCES dbo.TapTin(TapTinId);
    END
END
GO

-- BienLai -> DatPhong, NguoiTai, TapTin
IF OBJECT_ID(N'dbo.BienLai','U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'dbo.DatPhong','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_BienLai_DatPhong')
    BEGIN
        ALTER TABLE dbo.BienLai ADD CONSTRAINT FK_BienLai_DatPhong FOREIGN KEY (DatPhongId) REFERENCES dbo.DatPhong(DatPhongId);
    END
    IF OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_BienLai_NguoiTai')
    BEGIN
        ALTER TABLE dbo.BienLai ADD CONSTRAINT FK_BienLai_NguoiTai FOREIGN KEY (NguoiTai) REFERENCES dbo.NguoiDung(NguoiDungId);
    END
    IF OBJECT_ID(N'dbo.TapTin','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys fk WHERE fk.name = N'FK_BienLai_TapTin')
    BEGIN
        ALTER TABLE dbo.BienLai ADD CONSTRAINT FK_BienLai_TapTin FOREIGN KEY (TapTinId) REFERENCES dbo.TapTin(TapTinId);
    END
END
GO

-- ========== Ensure seed data for lookups ==========
IF NOT EXISTS (SELECT 1 FROM dbo.VaiTro WHERE TenVaiTro = N'Admin')
    INSERT INTO dbo.VaiTro (TenVaiTro) VALUES (N'Admin');
IF NOT EXISTS (SELECT 1 FROM dbo.VaiTro WHERE TenVaiTro = N'ChuTro')
    INSERT INTO dbo.VaiTro (TenVaiTro) VALUES (N'ChuTro');
IF NOT EXISTS (SELECT 1 FROM dbo.VaiTro WHERE TenVaiTro = N'NguoiThue')
    INSERT INTO dbo.VaiTro (TenVaiTro) VALUES (N'NguoiThue');

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

IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Wifi') INSERT INTO dbo.TienIch (Ten) VALUES (N'Wifi');
IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'BanCong') INSERT INTO dbo.TienIch (Ten) VALUES (N'BanCong');

IF NOT EXISTS (SELECT 1 FROM dbo.LoaiHoTro WHERE TenLoai = N'SuaChua') INSERT INTO dbo.LoaiHoTro (TenLoai) VALUES (N'SuaChua');
IF NOT EXISTS (SELECT 1 FROM dbo.LoaiHoTro WHERE TenLoai = N'VeSinh') INSERT INTO dbo.LoaiHoTro (TenLoai) VALUES (N'VeSinh');

IF NOT EXISTS (SELECT 1 FROM dbo.ViPham WHERE TenViPham = N'Báo tin sai sự thật')
    INSERT INTO dbo.ViPham (TenViPham, MoTa, HinhPhatTien) VALUES (N'Báo tin sai sự thật', N'Người dùng báo tin sai / thông tin giả', 0);
IF NOT EXISTS (SELECT 1 FROM dbo.ViPham WHERE TenViPham = N'Trộm cắp / lừa đảo')
    INSERT INTO dbo.ViPham (TenViPham, MoTa, HinhPhatTien) VALUES (N'Trộm cắp / lừa đảo', N'Các hành vi lừa đảo, trộm cắp', 0);
IF NOT EXISTS (SELECT 1 FROM dbo.ViPham WHERE TenViPham = N'Vi phạm nội quy')
    INSERT INTO dbo.ViPham (TenViPham, MoTa, HinhPhatTien) VALUES (N'Vi phạm nội quy', N'Những hành vi vi phạm nội quy cộng đồng', 0);
GO

-- ========== DROP stored procedures if existed earlier (to recreate clean) ==========
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
GO

-- ========== Recreate stored procedures (now that columns exist) ==========
-- sp_CreateBooking
CREATE PROCEDURE dbo.sp_CreateBooking
    @PhongId UNIQUEIDENTIFIER,
    @NguoiThueId UNIQUEIDENTIFIER,
    @ChuTroId UNIQUEIDENTIFIER,
    @Loai NVARCHAR(30),
    @BatDau DATETIMEOFFSET,
    @KetThuc DATETIMEOFFSET = NULL,
    @NewDatPhongId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE PhongId = @PhongId)
        BEGIN
            RAISERROR(N'Phong khong ton tai.', 16, 1); ROLLBACK TRAN; RETURN;
        END
        SET @NewDatPhongId = NEWID();
        INSERT INTO dbo.DatPhong (DatPhongId, PhongId, NguoiThueId, ChuTroId, Loai, BatDau, KetThuc, TrangThaiId)
        VALUES (@NewDatPhongId, @PhongId, @NguoiThueId, @ChuTroId, @Loai, @BatDau, @KetThuc,
            (SELECT TOP 1 TrangThaiId FROM dbo.TrangThaiDatPhong WHERE TenTrangThai = N'ChoXacNhan'));
        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (@NguoiThueId, N'Tạo đặt phòng', N'DatPhong', CAST(@NewDatPhongId AS NVARCHAR(50)), N'PhongId=' + CAST(@PhongId AS NVARCHAR(50)));
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@err, 16, 1);
    END CATCH
END
GO

-- sp_UploadReceipt
CREATE PROCEDURE dbo.sp_UploadReceipt
    @DatPhongId UNIQUEIDENTIFIER,
    @NguoiTai UNIQUEIDENTIFIER,
    @TapTinId UNIQUEIDENTIFIER,
    @SoTien BIGINT = NULL,
    @NewBienLaiId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.DatPhong WHERE DatPhongId = @DatPhongId)
            BEGIN RAISERROR(N'DatPhong khong ton tai',16,1); ROLLBACK TRAN; RETURN; END
        SET @NewBienLaiId = NEWID();
        INSERT INTO dbo.BienLai (BienLaiId, DatPhongId, NguoiTai, TapTinId, SoTien)
        VALUES (@NewBienLaiId, @DatPhongId, @NguoiTai, @TapTinId, @SoTien);
        UPDATE dbo.DatPhong SET TapTinBienLaiId = @TapTinId WHERE DatPhongId = @DatPhongId;
        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (@NguoiTai, N'Upload biên lai', N'BienLai', CAST(@NewBienLaiId AS NVARCHAR(50)), N'DatPhongId=' + CAST(@DatPhongId AS NVARCHAR(50)));
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

-- sp_CreateReview
CREATE PROCEDURE dbo.sp_CreateReview
    @PhongId UNIQUEIDENTIFIER,
    @NguoiDanhGia UNIQUEIDENTIFIER,
    @Diem INT,
    @NoiDung NVARCHAR(1000) = NULL,
    @NewDanhGiaId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Diem IS NULL OR @Diem < 1 OR @Diem > 5
    BEGIN
        RAISERROR(N'Diem phai trong khoang 1..5.',16,1); RETURN;
    END
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE PhongId = @PhongId)
        BEGIN
            RAISERROR(N'Phong khong ton tai.', 16, 1); ROLLBACK TRAN; RETURN;
        END
        SET @NewDanhGiaId = NEWID();
        INSERT INTO dbo.DanhGiaPhong (DanhGiaId, PhongId, NguoiDanhGia, Diem, NoiDung)
        VALUES (@NewDanhGiaId, @PhongId, @NguoiDanhGia, @Diem, @NoiDung);
        UPDATE dbo.Phong
        SET SoLuongDanhGia = ISNULL(SoLuongDanhGia,0) + 1,
            DiemTrungBinh =
                CASE WHEN ISNULL(SoLuongDanhGia,0)=0 THEN @Diem
                     ELSE ((ISNULL(DiemTrungBinh,0) * ISNULL(SoLuongDanhGia,0)) + @Diem) / (ISNULL(SoLuongDanhGia,0) + 1)
                END,
            UpdatedAt = SYSDATETIMEOFFSET()
        WHERE PhongId = @PhongId;
        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (@NguoiDanhGia, N'Tạo đánh giá', N'DanhGiaPhong', CAST(@NewDanhGiaId AS NVARCHAR(50)), N'PhongId=' + CAST(@PhongId AS NVARCHAR(50)));
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @err NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@err, 16, 1);
    END CATCH
END
GO

-- sp_CreateSupport (now references YeuCauHoTro.NguoiYeuCau which exists or was added)
CREATE PROCEDURE dbo.sp_CreateSupport
    @PhongId UNIQUEIDENTIFIER = NULL,
    @NguoiYeuCau UNIQUEIDENTIFIER,
    @LoaiHoTroId INT,
    @TieuDe NVARCHAR(300),
    @MoTa NVARCHAR(MAX) = NULL,
    @NewHoTroId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        SET @NewHoTroId = NEWID();
        INSERT INTO dbo.YeuCauHoTro (HoTroId, PhongId, NguoiYeuCau, LoaiHoTroId, TieuDe, MoTa)
        VALUES (@NewHoTroId, @PhongId, @NguoiYeuCau, @LoaiHoTroId, @TieuDe, @MoTa);
        INSERT INTO dbo.LichSu (NguoiDungId, HanhDong, TenBang, BanGhiId, ChiTiet)
        VALUES (@NguoiYeuCau, N'Tạo yêu cầu hỗ trợ', N'YeuCauHoTro', CAST(@NewHoTroId AS NVARCHAR(50)), @TieuDe);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000) = ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

-- Admin SPs (account management, post moderation, reports)
CREATE PROCEDURE dbo.sp_Admin_XacThucTaiKhoan
    @AdminId UNIQUEIDENTIFIER,
    @NguoiDungId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE NguoiDungId = @NguoiDungId)
        BEGIN RAISERROR(N'Nguoi dung khong ton tai.',16,1); ROLLBACK TRAN; RETURN; END
        UPDATE dbo.NguoiDung SET IsEmailXacThuc = 1, UpdatedAt = SYSDATETIMEOFFSET() WHERE NguoiDungId = @NguoiDungId;
        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (@AdminId, N'Xác nhận tài khoản', N'NguoiDung', CAST(@NguoiDungId AS NVARCHAR(50)), N'Xác nhận email/tài khoản');
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000)=ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_Admin_KhoaTaiKhoan
    @AdminId UNIQUEIDENTIFIER,
    @NguoiDungId UNIQUEIDENTIFIER,
    @LyDo NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE NguoiDungId = @NguoiDungId)
        BEGIN RAISERROR(N'Nguoi dung khong ton tai.',16,1); ROLLBACK TRAN; RETURN; END
        UPDATE dbo.NguoiDung SET IsKhoa = 1, UpdatedAt = SYSDATETIMEOFFSET() WHERE NguoiDungId = @NguoiDungId;
        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (@AdminId, N'Khóa tài khoản', N'NguoiDung', CAST(@NguoiDungId AS NVARCHAR(50)), ISNULL(@LyDo,N''));
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000)=ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_Admin_MoKhoaTaiKhoan
    @AdminId UNIQUEIDENTIFIER,
    @NguoiDungId UNIQUEIDENTIFIER,
    @LyDo NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE NguoiDungId = @NguoiDungId)
        BEGIN RAISERROR(N'Nguoi dung khong ton tai.',16,1); ROLLBACK TRAN; RETURN; END
        UPDATE dbo.NguoiDung SET IsKhoa = 0, UpdatedAt = SYSDATETIMEOFFSET() WHERE NguoiDungId = @NguoiDungId;
        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (@AdminId, N'Mở khóa tài khoản', N'NguoiDung', CAST(@NguoiDungId AS NVARCHAR(50)), ISNULL(@LyDo,N''));
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000)=ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_Admin_DuyetBaiDang
    @AdminId UNIQUEIDENTIFIER,
    @PhongId UNIQUEIDENTIFIER,
    @ChapNhan BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE PhongId = @PhongId)
        BEGIN RAISERROR(N'Phong khong ton tai.',16,1); ROLLBACK TRAN; RETURN; END

        IF @ChapNhan = 1
        BEGIN
            UPDATE dbo.Phong
            SET IsDuyet = 1, NguoiDuyet = @AdminId, ThoiGianDuyet = SYSDATETIMEOFFSET(), IsBiKhoa = 0, UpdatedAt = SYSDATETIMEOFFSET()
            WHERE PhongId = @PhongId;
            INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
            VALUES (@AdminId, N'Duyệt bài đăng', N'Phong', CAST(@PhongId AS NVARCHAR(50)), N'Chấp nhận hiển thị');
        END
        ELSE
        BEGIN
            UPDATE dbo.Phong
            SET IsDuyet = 0, IsBiKhoa = 1, NguoiDuyet = @AdminId, ThoiGianDuyet = SYSDATETIMEOFFSET(), UpdatedAt = SYSDATETIMEOFFSET()
            WHERE PhongId = @PhongId;
            INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
            VALUES (@AdminId, N'Từ chối bài đăng', N'Phong', CAST(@PhongId AS NVARCHAR(50)), N'Từ chối/không cho hiển thị');
        END

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000)=ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_Admin_KhoaBaiDang
    @AdminId UNIQUEIDENTIFIER,
    @PhongId UNIQUEIDENTIFIER,
    @LyDo NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE PhongId = @PhongId)
        BEGIN RAISERROR(N'Phong khong ton tai.',16,1); ROLLBACK TRAN; RETURN; END
        UPDATE dbo.Phong SET IsBiKhoa = 1, IsDuyet = 0, UpdatedAt = SYSDATETIMEOFFSET() WHERE PhongId = @PhongId;
        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (@AdminId, N'Khóa bài đăng', N'Phong', CAST(@PhongId AS NVARCHAR(50)), ISNULL(@LyDo,N''));
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000)=ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_TaoBaoCaoViPham
    @NguoiBaoCao UNIQUEIDENTIFIER,
    @LoaiThucThe NVARCHAR(50),
    @ThucTheId UNIQUEIDENTIFIER = NULL,
    @ViPhamId INT = NULL,
    @TieuDe NVARCHAR(300),
    @MoTa NVARCHAR(MAX) = NULL,
    @NewBaoCaoId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        SET @NewBaoCaoId = NEWID();
        INSERT INTO dbo.BaoCaoViPham (BaoCaoId, LoaiThucThe, ThucTheId, NguoiBaoCao, ViPhamId, TieuDe, MoTa)
        VALUES (@NewBaoCaoId, @LoaiThucThe, @ThucTheId, @NguoiBaoCao, @ViPhamId, @TieuDe, @MoTa);
        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (@NguoiBaoCao, N'Tạo báo cáo vi phạm', @LoaiThucThe, CAST(@NewBaoCaoId AS NVARCHAR(50)), @TieuDe);
        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000)=ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

CREATE PROCEDURE dbo.sp_Admin_XuLyBaoCao
    @AdminId UNIQUEIDENTIFIER,
    @BaoCaoId UNIQUEIDENTIFIER,
    @HanhDong NVARCHAR(200),
    @ViPhamId INT = NULL,
    @KetQua NVARCHAR(1000) = NULL,
    @ApDungKhoaTaiKhoan BIT = 0,
    @ApDungKhoaBaiDang BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRAN;
        IF NOT EXISTS (SELECT 1 FROM dbo.BaoCaoViPham WHERE BaoCaoId = @BaoCaoId)
        BEGIN RAISERROR(N'Bao cao khong ton tai.',16,1); ROLLBACK TRAN; RETURN; END

        UPDATE dbo.BaoCaoViPham
        SET ViPhamId = COALESCE(@ViPhamId, ViPhamId),
            TrangThai = N'DaXuLy',
            KetQua = @KetQua,
            NguoiXuLy = @AdminId,
            ThoiGianXuLy = SYSDATETIMEOFFSET()
        WHERE BaoCaoId = @BaoCaoId;

        DECLARE @LoaiThucThe NVARCHAR(50);
        DECLARE @ThucTheId UNIQUEIDENTIFIER;
        SELECT @LoaiThucThe = LoaiThucThe, @ThucTheId = ThucTheId FROM dbo.BaoCaoViPham WHERE BaoCaoId = @BaoCaoId;

        IF @ApDungKhoaTaiKhoan = 1 AND @LoaiThucThe = N'NguoiDung' AND @ThucTheId IS NOT NULL
        BEGIN
            UPDATE dbo.NguoiDung SET IsKhoa = 1, UpdatedAt = SYSDATETIMEOFFSET() WHERE NguoiDungId = @ThucTheId;
            INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
            VALUES (@AdminId, N'Khóa tài khoản do vi phạm', N'NguoiDung', CAST(@ThucTheId AS NVARCHAR(50)), ISNULL(@KetQua,N''));
        END

        IF @ApDungKhoaBaiDang = 1 AND @LoaiThucThe = N'Phong' AND @ThucTheId IS NOT NULL
        BEGIN
            UPDATE dbo.Phong SET IsBiKhoa = 1, IsDuyet = 0, UpdatedAt = SYSDATETIMEOFFSET() WHERE PhongId = @ThucTheId;
            INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
            VALUES (@AdminId, N'Khóa bài đăng do vi phạm', N'Phong', CAST(@ThucTheId AS NVARCHAR(50)), ISNULL(@KetQua,N''));
        END

        INSERT INTO dbo.HanhDongAdmin (AdminId, HanhDong, MucTieuBang, BanGhiId, ChiTiet)
        VALUES (@AdminId, @HanhDong, N'BaoCaoViPham', CAST(@BaoCaoId AS NVARCHAR(50)), ISNULL(@KetQua,N''));

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF XACT_STATE()<>0 ROLLBACK TRAN;
        DECLARE @e NVARCHAR(4000)=ERROR_MESSAGE(); RAISERROR(@e,16,1);
    END CATCH
END
GO

-- ========== Minimal indexes ==========
IF NOT EXISTS (SELECT 1 FROM sys.indexes i JOIN sys.objects o ON i.object_id = o.object_id WHERE i.name = 'IDX_Phong_Gia' AND o.name = 'Phong')
    CREATE INDEX IDX_Phong_Gia ON dbo.Phong(GiaTien);
IF NOT EXISTS (SELECT 1 FROM sys.indexes i JOIN sys.objects o ON i.object_id = o.object_id WHERE i.name = 'IDX_DatPhong_Phong' AND o.name = 'DatPhong')
    CREATE INDEX IDX_DatPhong_Phong ON dbo.DatPhong(PhongId);
GO

-- ========== Quick checks ==========
SELECT TOP 5 * FROM dbo.VaiTro;
SELECT TOP 5 NguoiDungId, Email, DienThoai, VaiTroId, CreatedAt FROM dbo.NguoiDung;
SELECT TOP 5 PhongId, TieuDe, GiaTien, TrangThai, IsDuyet, IsBiKhoa, NguoiDuyet, ThoiGianDuyet FROM dbo.Phong;
SELECT TOP 5 BaoCaoId, LoaiThucThe, TieuDe, TrangThai, ThoiGianBaoCao FROM dbo.BaoCaoViPham;
GO

PRINT N'Complete: DB QuanLyPhongTro created/updated (admin columns ensured and SPs recreated).';
GO


INSERT INTO NguoiDung (VaiTroId)
    VALUES(2)

INSERT INTO NhaTro (ChuTroId, TieuDe)
    VALUES('482d7eb8-ee43-4fc4-a02d-a653363c11df','Test load')
