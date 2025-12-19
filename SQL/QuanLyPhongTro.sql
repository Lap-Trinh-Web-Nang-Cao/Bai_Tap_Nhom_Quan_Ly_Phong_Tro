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
            IsBiKhoa BIT DEFAULT 0,
            MoTa NVARCHAR(MAX) NULL
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

        IF COL_LENGTH('dbo.Phong','MoTa') IS NULL
        BEGIN
            ALTER TABLE dbo.Phong ADD MoTa NVARCHAR(MAX) NULL;
            PRINT N'Added column Phong.MoTa';
        END

        IF COL_LENGTH('dbo.Phong','IsDeleted') IS NULL
        BEGIN
            ALTER TABLE dbo.Phong ADD IsDeleted BIT NOT NULL DEFAULT 0;
            PRINT N'Added column Phong.IsDeleted';
        END
    END
    GO

    -- Ensure NhaTro and NguoiDung have IsDeleted
    IF OBJECT_ID(N'dbo.NhaTro','U') IS NOT NULL AND COL_LENGTH('dbo.NhaTro','IsDeleted') IS NULL
    BEGIN
        ALTER TABLE dbo.NhaTro ADD IsDeleted BIT NOT NULL DEFAULT 0;
    END
    GO
    IF OBJECT_ID(N'dbo.NguoiDung','U') IS NOT NULL AND COL_LENGTH('dbo.NguoiDung','IsDeleted') IS NULL
    BEGIN
        ALTER TABLE dbo.NguoiDung ADD IsDeleted BIT NOT NULL DEFAULT 0;
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

    -- =========================================================
    -- INSERT SAMPLE DATA - Dữ liệu mẫu cho hệ thống
    -- =========================================================

    -- ========== THÊM QUẬN HUYỆN VÀ PHƯỜNG ==========
    IF NOT EXISTS (SELECT 1 FROM dbo.QuanHuyen WHERE Ten = N'Quận 9')
        INSERT INTO dbo.QuanHuyen (Ten) VALUES (N'Quận 9');
    IF NOT EXISTS (SELECT 1 FROM dbo.QuanHuyen WHERE Ten = N'Thủ Đức')
        INSERT INTO dbo.QuanHuyen (Ten) VALUES (N'Thủ Đức');
    IF NOT EXISTS (SELECT 1 FROM dbo.QuanHuyen WHERE Ten = N'Bình Thạnh')
        INSERT INTO dbo.QuanHuyen (Ten) VALUES (N'Bình Thạnh');
    IF NOT EXISTS (SELECT 1 FROM dbo.QuanHuyen WHERE Ten = N'Gò Vấp')
        INSERT INTO dbo.QuanHuyen (Ten) VALUES (N'Gò Vấp');

    DECLARE @QuanId_Q9 INT = (SELECT QuanHuyenId FROM dbo.QuanHuyen WHERE Ten = N'Quận 9');
    DECLARE @QuanId_ThuDuc INT = (SELECT QuanHuyenId FROM dbo.QuanHuyen WHERE Ten = N'Thủ Đức');
    DECLARE @QuanId_BinhThanh INT = (SELECT QuanHuyenId FROM dbo.QuanHuyen WHERE Ten = N'Bình Thạnh');

    IF @QuanId_Q9 IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.Phuong WHERE Ten = N'Phường Linh Trung' AND QuanHuyenId = @QuanId_Q9)
            INSERT INTO dbo.Phuong (QuanHuyenId, Ten) VALUES (@QuanId_Q9, N'Phường Linh Trung');
        IF NOT EXISTS (SELECT 1 FROM dbo.Phuong WHERE Ten = N'Phường Linh Chiểu' AND QuanHuyenId = @QuanId_Q9)
            INSERT INTO dbo.Phuong (QuanHuyenId, Ten) VALUES (@QuanId_Q9, N'Phường Linh Chiểu');
    END

    -- ========== THÊM TIỆN ÍCH ==========
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Máy lạnh') INSERT INTO dbo.TienIch (Ten) VALUES (N'Máy lạnh');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Máy nước nóng') INSERT INTO dbo.TienIch (Ten) VALUES (N'Máy nước nóng');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Giường') INSERT INTO dbo.TienIch (Ten) VALUES (N'Giường');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Tủ quần áo') INSERT INTO dbo.TienIch (Ten) VALUES (N'Tủ quần áo');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Bàn học') INSERT INTO dbo.TienIch (Ten) VALUES (N'Bàn học');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Chỗ để xe') INSERT INTO dbo.TienIch (Ten) VALUES (N'Chỗ để xe');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Camera an ninh') INSERT INTO dbo.TienIch (Ten) VALUES (N'Camera an ninh');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Bếp riêng') INSERT INTO dbo.TienIch (Ten) VALUES (N'Bếp riêng');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'WC riêng') INSERT INTO dbo.TienIch (Ten) VALUES (N'WC riêng');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Gác lửng') INSERT INTO dbo.TienIch (Ten) VALUES (N'Gác lửng');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Thang máy') INSERT INTO dbo.TienIch (Ten) VALUES (N'Thang máy');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Giờ giấc tự do') INSERT INTO dbo.TienIch (Ten) VALUES (N'Giờ giấc tự do');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Cửa vân tay') INSERT INTO dbo.TienIch (Ten) VALUES (N'Cửa vân tay');
    IF NOT EXISTS (SELECT 1 FROM dbo.TienIch WHERE Ten = N'Bảo vệ 24/7') INSERT INTO dbo.TienIch (Ten) VALUES (N'Bảo vệ 24/7');
    GO

    -- ========== TẠO NGƯỜI DÙNG MẪU (TỰ ĐỘNG) ==========
    DECLARE @VaiTroAdmin INT = (SELECT VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'Admin');
    DECLARE @VaiTroChuTro INT = (SELECT VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'ChuTro');
    DECLARE @VaiTroNguoiThue INT = (SELECT VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'NguoiThue');

    -- ===== CẤU HÌNH: Thay đổi số lượng ở đây =====
    DECLARE @TotalChuTro INT = 10;        -- Số chủ trọ muốn tạo
    DECLARE @TotalNguoiThue INT = 5;      -- Số người thuê muốn tạo
    -- ============================================

    -- 1. TẠO ADMIN
    DECLARE @AdminId UNIQUEIDENTIFIER;
    IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE Email = N'admin@example.com')
    BEGIN
        SET @AdminId = NEWID();
        INSERT INTO dbo.NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsEmailXacThuc)
        VALUES (@AdminId, N'admin@example.com', N'0901234567', N'HashedPassword123', @VaiTroAdmin, 1);
        
        INSERT INTO dbo.HoSoNguoiDung (NguoiDungId, HoTen)
        VALUES (@AdminId, N'Administrator');
        PRINT N'✓ Created Admin';
    END
    ELSE
    BEGIN
        SET @AdminId = (SELECT NguoiDungId FROM dbo.NguoiDung WHERE Email = N'admin@example.com');
        PRINT N'✓ Admin already exists';
    END

    -- 2. TẠO CHỦ TRỌ TỰ ĐỘNG (LOOP)
    DECLARE @Counter INT = 1;
    DECLARE @NewUserId UNIQUEIDENTIFIER;
    DECLARE @Email NVARCHAR(255);
    DECLARE @Phone NVARCHAR(50);
    DECLARE @HoTen NVARCHAR(200);

    DECLARE @HoList TABLE (Ho NVARCHAR(50));
    INSERT INTO @HoList VALUES (N'Nguyễn'), (N'Trần'), (N'Lê'), (N'Phạm'), (N'Hoàng'), (N'Phan'), (N'Vũ'), (N'Võ'), (N'Đặng'), (N'Bùi');
    DECLARE @TenList TABLE (Ten NVARCHAR(50));
    INSERT INTO @TenList VALUES (N'An'), (N'Bình'), (N'Cường'), (N'Dũng'), (N'Hà'), (N'Hương'), (N'Linh'), (N'Mai'), (N'Nam'), (N'Phương');

    WHILE @Counter <= @TotalChuTro
    BEGIN
        SET @Email = N'chutro' + CAST(@Counter AS NVARCHAR(10)) + N'@example.com';
        
        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE Email = @Email)
        BEGIN
            SET @NewUserId = NEWID();
            SET @Phone = N'091' + RIGHT('0000000' + CAST(@Counter AS NVARCHAR(10)), 7);
            
            -- Random tên từ lists
            SET @HoTen = (SELECT TOP 1 Ho FROM @HoList ORDER BY NEWID()) + N' Văn ' + 
                        (SELECT TOP 1 Ten FROM @TenList ORDER BY NEWID()) + N' (Chủ Trọ ' + CAST(@Counter AS NVARCHAR(10)) + N')';
            
            INSERT INTO dbo.NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsEmailXacThuc)
            VALUES (@NewUserId, @Email, @Phone, N'HashedPassword123', @VaiTroChuTro, 1);
            
            INSERT INTO dbo.HoSoNguoiDung (NguoiDungId, HoTen, NgaySinh)
            VALUES (@NewUserId, @HoTen, DATEADD(YEAR, -30 - (@Counter % 20), GETDATE()));
        END
        
        SET @Counter = @Counter + 1;
    END
    PRINT N'✓ Created ' + CAST(@TotalChuTro AS NVARCHAR(10)) + N' Chủ Trọ';

    -- 3. TẠO NGƯỜI THUÊ TỰ ĐỘNG (LOOP)
    SET @Counter = 1;
    WHILE @Counter <= @TotalNguoiThue
    BEGIN
        SET @Email = N'nguoithue' + CAST(@Counter AS NVARCHAR(10)) + N'@example.com';
        
        IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE Email = @Email)
        BEGIN
            SET @NewUserId = NEWID();
            SET @Phone = N'094' + RIGHT('0000000' + CAST(@Counter AS NVARCHAR(10)), 7);
            
            SET @HoTen = (SELECT TOP 1 Ho FROM @HoList ORDER BY NEWID()) + N' Thị ' + 
                        (SELECT TOP 1 Ten FROM @TenList ORDER BY NEWID()) + N' (Người Thuê ' + CAST(@Counter AS NVARCHAR(10)) + N')';
            
            INSERT INTO dbo.NguoiDung (NguoiDungId, Email, DienThoai, PasswordHash, VaiTroId, IsEmailXacThuc)
            VALUES (@NewUserId, @Email, @Phone, N'HashedPassword123', @VaiTroNguoiThue, 1);
            
            INSERT INTO dbo.HoSoNguoiDung (NguoiDungId, HoTen, NgaySinh)
            VALUES (@NewUserId, @HoTen, DATEADD(YEAR, -18 - (@Counter % 10), GETDATE()));
        END
        
        SET @Counter = @Counter + 1;
    END
    PRINT N'✓ Created ' + CAST(@TotalNguoiThue AS NVARCHAR(10)) + N' Người Thuê';
    PRINT N'========================================';
    GO


    -- ========== TẠO NHÀ TRỌ MẪU (TỰ ĐỘNG ASSIGN CHỦ TRỌ) ==========
    DECLARE @QuanId_Q9 INT = (SELECT QuanHuyenId FROM dbo.QuanHuyen WHERE Ten = N'Quận 9');
    DECLARE @QuanId_ThuDuc INT = (SELECT QuanHuyenId FROM dbo.QuanHuyen WHERE Ten = N'Thủ Đức');
    DECLARE @AdminId UNIQUEIDENTIFIER = (SELECT NguoiDungId FROM dbo.NguoiDung WHERE Email = N'admin@example.com');

    -- Danh sách nhà trọ cần tạo
    DECLARE @NhaTroData TABLE (
        TieuDe NVARCHAR(300),
        DiaChi NVARCHAR(500),
        QuanHuyenId INT
    );

    INSERT INTO @NhaTroData VALUES
        (N'Nhà trọ Hoa Mai', N'123 Lê Văn Việt, Quận 9, TP.HCM', @QuanId_Q9),
        (N'Nhà trọ SUNRISE', N'456 Man Thiện, TP Thủ Đức, TP.HCM', @QuanId_ThuDuc),
        (N'Căn hộ Mini UTE HOME', N'Đường Hoàng Hữu Nam, Quận 9, TP.HCM', @QuanId_Q9),
        (N'Nhà trọ Bình Dân', N'Khu phố 6, Linh Trung, Thủ Đức, TP.HCM', @QuanId_ThuDuc),
        (N'Nhà trọ Kim Phát', N'234 Kha Vạn Cân, Linh Chiểu, Thủ Đức, TP.HCM', @QuanId_ThuDuc);

    DECLARE @TieuDeNhaTro NVARCHAR(300);
    DECLARE @DiaChiNhaTro NVARCHAR(500);
    DECLARE @QuanHuyenIdNhaTro INT;
    DECLARE @RandomChuTroId UNIQUEIDENTIFIER;
    DECLARE @NhaTroId UNIQUEIDENTIFIER;

    -- Cursor để loop qua danh sách nhà trọ
    DECLARE nhatro_cursor CURSOR FOR
    SELECT TieuDe, DiaChi, QuanHuyenId FROM @NhaTroData;

    OPEN nhatro_cursor;
    FETCH NEXT FROM nhatro_cursor INTO @TieuDeNhaTro, @DiaChiNhaTro, @QuanHuyenIdNhaTro;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.NhaTro WHERE TieuDe = @TieuDeNhaTro)
        BEGIN
            -- Random pick 1 chủ trọ từ database
            SELECT TOP 1 @RandomChuTroId = NguoiDungId 
            FROM dbo.NguoiDung 
            WHERE VaiTroId = (SELECT VaiTroId FROM dbo.VaiTro WHERE TenVaiTro = N'ChuTro')
            ORDER BY NEWID();
            
            SET @NhaTroId = NEWID();
            INSERT INTO dbo.NhaTro (NhaTroId, ChuTroId, TieuDe, DiaChi, QuanHuyenId, IsHoatDong)
            VALUES (@NhaTroId, @RandomChuTroId, @TieuDeNhaTro, @DiaChiNhaTro, @QuanHuyenIdNhaTro, 1);
        END
        
        FETCH NEXT FROM nhatro_cursor INTO @TieuDeNhaTro, @DiaChiNhaTro, @QuanHuyenIdNhaTro;
    END

    CLOSE nhatro_cursor;
    DEALLOCATE nhatro_cursor;

    PRINT N'✓ Created 5 Nhà Trọ (auto-assigned to random Chủ Trọ)';
    GO

    -- ========== TẠO PHÒNG TRỌ MẪU ==========
    DECLARE @NhaTro1Id UNIQUEIDENTIFIER = (SELECT NhaTroId FROM dbo.NhaTro WHERE TieuDe = N'Nhà trọ Hoa Mai');
    DECLARE @NhaTro2Id UNIQUEIDENTIFIER = (SELECT NhaTroId FROM dbo.NhaTro WHERE TieuDe = N'Nhà trọ SUNRISE');
    DECLARE @NhaTro3Id UNIQUEIDENTIFIER = (SELECT NhaTroId FROM dbo.NhaTro WHERE TieuDe = N'Căn hộ Mini UTE HOME');
    DECLARE @NhaTro4Id UNIQUEIDENTIFIER = (SELECT NhaTroId FROM dbo.NhaTro WHERE TieuDe = N'Nhà trọ Bình Dân');
    DECLARE @NhaTro5Id UNIQUEIDENTIFIER = (SELECT NhaTroId FROM dbo.NhaTro WHERE TieuDe = N'Nhà trọ Kim Phát');
    DECLARE @AdminId UNIQUEIDENTIFIER = (SELECT NguoiDungId FROM dbo.NguoiDung WHERE Email = N'admin@example.com');

    -- Phòng 1
    DECLARE @Phong1Id UNIQUEIDENTIFIER = NEWID();
    IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE TieuDe = N'Phòng trọ sinh viên gần UTE')
    BEGIN
        INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia)
        VALUES (@Phong1Id, @NhaTro1Id, N'Phòng trọ sinh viên gần UTE', 18, 1800000, 1800000, 2, N'con_trong', 1, @AdminId, SYSDATETIMEOFFSET(), 4.5, 12);
    END
    ELSE
        SET @Phong1Id = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ sinh viên gần UTE');

    -- Phòng 2
    DECLARE @Phong2Id UNIQUEIDENTIFIER = NEWID();
    IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE TieuDe = N'Phòng trọ mới xây, full nội thất')
    BEGIN
        INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia)
        VALUES (@Phong2Id, @NhaTro2Id, N'Phòng trọ mới xây, full nội thất', 22, 2500000, 2500000, 2, N'con_trong', 1, @AdminId, SYSDATETIMEOFFSET(), 4.8, 8);
    END
    ELSE
        SET @Phong2Id = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ mới xây, full nội thất');

    -- Phòng 3
    DECLARE @Phong3Id UNIQUEIDENTIFIER = NEWID();
    IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE TieuDe = N'Căn hộ mini 1PN cho sinh viên')
    BEGIN
        INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia)
        VALUES (@Phong3Id, @NhaTro3Id, N'Căn hộ mini 1PN cho sinh viên', 28, 3200000, 3200000, 2, N'con_trong', 1, @AdminId, SYSDATETIMEOFFSET(), 4.2, 5);
    END
    ELSE
        SET @Phong3Id = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Căn hộ mini 1PN cho sinh viên');

    -- Phòng 4
    DECLARE @Phong4Id UNIQUEIDENTIFIER = NEWID();
    IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE TieuDe = N'Phòng giá rẻ cho sinh viên')
    BEGIN
        INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia)
        VALUES (@Phong4Id, @NhaTro4Id, N'Phòng giá rẻ cho sinh viên', 16, 1300000, 1300000, 2, N'con_trong', 1, @AdminId, SYSDATETIMEOFFSET(), 3.9, 20);
    END
    ELSE
        SET @Phong4Id = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng giá rẻ cho sinh viên');

    -- Phòng 5
    DECLARE @Phong5Id UNIQUEIDENTIFIER = NEWID();
    IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE TieuDe = N'Phòng trọ có gác lửng, rộng rãi')
    BEGIN
        INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia)
        VALUES (@Phong5Id, @NhaTro5Id, N'Phòng trọ có gác lửng, rộng rãi', 25, 2200000, 2200000, 3, N'con_trong', 1, @AdminId, SYSDATETIMEOFFSET(), 4.3, 15);
    END
    ELSE
        SET @Phong5Id = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ có gác lửng, rộng rãi');

    -- Phòng 6
    DECLARE @Phong6Id UNIQUEIDENTIFIER = NEWID();
    IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE TieuDe = N'Phòng studio cao cấp')
    BEGIN
        INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia)
        VALUES (@Phong6Id, @NhaTro2Id, N'Phòng studio cao cấp', 30, 4500000, 4500000, 2, N'con_trong', 1, @AdminId, SYSDATETIMEOFFSET(), 4.9, 25);
    END
    ELSE
        SET @Phong6Id = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng studio cao cấp');

    -- Phòng 7
    DECLARE @Phong7Id UNIQUEIDENTIFIER = NEWID();
    IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE TieuDe = N'Phòng trọ nữ only, an ninh')
    BEGIN
        INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia)
        VALUES (@Phong7Id, @NhaTro1Id, N'Phòng trọ nữ only, an ninh', 20, 1900000, 1900000, 2, N'con_trong', 1, @AdminId, SYSDATETIMEOFFSET(), 4.6, 18);
    END
    ELSE
        SET @Phong7Id = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ nữ only, an ninh');

    -- Phòng 8
    DECLARE @Phong8Id UNIQUEIDENTIFIER = NEWID();
    IF NOT EXISTS (SELECT 1 FROM dbo.Phong WHERE TieuDe = N'Phòng trọ có bếp riêng')
    BEGIN
        INSERT INTO dbo.Phong (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia)
        VALUES (@Phong8Id, @NhaTro3Id, N'Phòng trọ có bếp riêng', 24, 2800000, 2800000, 2, N'con_trong', 1, @AdminId, SYSDATETIMEOFFSET(), 4.4, 10);
    END
    ELSE
        SET @Phong8Id = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ có bếp riêng');

    PRINT N'Created sample Phong successfully.';
    GO

    -- ========== LIÊN KẾT TIỆN ÍCH CHO PHÒNG ==========
    DECLARE @Phong1Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ sinh viên gần UTE');
    DECLARE @Phong2Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ mới xây, full nội thất');
    DECLARE @Phong3Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Căn hộ mini 1PN cho sinh viên');
    DECLARE @Phong4Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng giá rẻ cho sinh viên');
    DECLARE @Phong5Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ có gác lửng, rộng rãi');
    DECLARE @Phong6Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng studio cao cấp');
    DECLARE @Phong7Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ nữ only, an ninh');
    DECLARE @Phong8Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ có bếp riêng');

    -- Phòng 1: Wifi, Chỗ để xe, Giờ giấc tự do
    IF @Phong1Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong1Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong1Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong1Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Chỗ để xe'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong1Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Chỗ để xe'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong1Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Giờ giấc tự do'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong1Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Giờ giấc tự do'));
    END

    -- Phòng 2: Máy lạnh, Máy nước nóng, Camera
    IF @Phong2Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong2Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Máy lạnh'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong2Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Máy lạnh'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong2Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Máy nước nóng'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong2Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Máy nước nóng'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong2Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Camera an ninh'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong2Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Camera an ninh'));
    END

    -- Phòng 3: Thang máy, Ban công, Bếp riêng
    IF @Phong3Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong3Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Thang máy'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong3Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Thang máy'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong3Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'BanCong'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong3Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'BanCong'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong3Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Bếp riêng'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong3Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Bếp riêng'));
    END

    -- Phòng 4: Wifi, Giờ giấc tự do
    IF @Phong4Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong4Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong4Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong4Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Giờ giấc tự do'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong4Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Giờ giấc tự do'));
    END

    -- Phòng 5: Gác lửng, Wifi, Máy lạnh
    IF @Phong5Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong5Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Gác lửng'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong5Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Gác lửng'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong5Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong5Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong5Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Máy lạnh'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong5Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Máy lạnh'));
    END

    -- Phòng 6: Wifi, Bảo vệ 24/7, Thang máy
    IF @Phong6Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong6Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong6Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong6Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Bảo vệ 24/7'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong6Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Bảo vệ 24/7'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong6Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Thang máy'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong6Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Thang máy'));
    END

    -- Phòng 7: Camera, Cửa vân tay, Wifi
    IF @Phong7Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong7Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Camera an ninh'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong7Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Camera an ninh'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong7Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Cửa vân tay'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong7Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Cửa vân tay'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong7Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong7Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Wifi'));
    END

    -- Phòng 8: Bếp riêng, Ban công, WC riêng
    IF @Phong8Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong8Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Bếp riêng'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong8Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'Bếp riêng'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong8Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'BanCong'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong8Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'BanCong'));
        IF NOT EXISTS (SELECT 1 FROM dbo.PhongTienIch WHERE PhongId = @Phong8Id AND TienIchId = (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'WC riêng'))
            INSERT INTO dbo.PhongTienIch (PhongId, TienIchId) VALUES (@Phong8Id, (SELECT TienIchId FROM dbo.TienIch WHERE Ten = N'WC riêng'));
    END

    PRINT N'Linked TienIch to Phong successfully.';
    GO

    -- ========== TẠO ĐÁNH GIÁ MẪU ==========
    DECLARE @Phong1Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ sinh viên gần UTE');
    DECLARE @Phong2Id UNIQUEIDENTIFIER = (SELECT PhongId FROM dbo.Phong WHERE TieuDe = N'Phòng trọ mới xây, full nội thất');
    DECLARE @NguoiThue1Id UNIQUEIDENTIFIER = (SELECT NguoiDungId FROM dbo.NguoiDung WHERE Email = N'nguoithue1@example.com');
    DECLARE @NguoiThue2Id UNIQUEIDENTIFIER = (SELECT NguoiDungId FROM dbo.NguoiDung WHERE Email = N'nguoithue2@example.com');

    IF @Phong1Id IS NOT NULL AND @NguoiThue1Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.DanhGiaPhong WHERE PhongId = @Phong1Id AND NguoiDanhGia = @NguoiThue1Id)
        BEGIN
            INSERT INTO dbo.DanhGiaPhong (PhongId, NguoiDanhGia, Diem, NoiDung)
            VALUES (@Phong1Id, @NguoiThue1Id, 5, N'Phòng rất đẹp, sạch sẽ, chủ nhà thân thiện!');
        END
    END

    IF @Phong2Id IS NOT NULL AND @NguoiThue2Id IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.DanhGiaPhong WHERE PhongId = @Phong2Id AND NguoiDanhGia = @NguoiThue2Id)
        BEGIN
            INSERT INTO dbo.DanhGiaPhong (PhongId, NguoiDanhGia, Diem, NoiDung)
            VALUES (@Phong2Id, @NguoiThue2Id, 5, N'Full nội thất như mô tả, vị trí gần trường. Rất hài lòng!');
        END
    END

    PRINT N'Created sample reviews successfully.';
    GO

    PRINT N'========================================';
    PRINT N'✅ INSERT SAMPLE DATA COMPLETED!';
    PRINT N'========================================';
    PRINT N'📊 Summary:';
    PRINT N'  - Admin: 1';
    PRINT N'  - Chủ Trọ: 10 (auto-generated)';
    PRINT N'  - Người Thuê: 5 (auto-generated)';
    PRINT N'  - Nhà Trọ: 5 (randomly assigned)';
    PRINT N'  - Phòng: 8 (all approved)';
    PRINT N'  - Tiện Ích: 14+';
    PRINT N'  - PhongTienIch: Linked';
    PRINT N'  - Reviews: Sample data';
    PRINT N'========================================';
    PRINT N'💡 TIP: Muốn thay đổi số lượng?';
    PRINT N'    Tìm dòng: @TotalChuTro = 10';
    PRINT N'    Tìm dòng: @TotalNguoiThue = 5';
    PRINT N'========================================';
    GO

-- ============================================================
-- INSERT ĐƠN GIẢN - PHÒNG TRỌ NỔI BẬT
-- Chạy sau khi đã có dữ liệu admin, chủ trọ, quận huyện
-- ============================================================

-- Lấy thông tin cần thiết
DECLARE @Admin UNIQUEIDENTIFIER = (SELECT TOP 1 NguoiDungId FROM NguoiDung WHERE Email = 'admin@example.com');
DECLARE @ChuTro UNIQUEIDENTIFIER = (SELECT TOP 1 NguoiDungId FROM NguoiDung WHERE VaiTroId = (SELECT VaiTroId FROM VaiTro WHERE TenVaiTro = N'ChuTro'));
DECLARE @ThuDuc INT = (SELECT QuanHuyenId FROM QuanHuyen WHERE Ten = N'Thủ Đức');
DECLARE @BinhThanh INT = ISNULL((SELECT QuanHuyenId FROM QuanHuyen WHERE Ten = N'Bình Thạnh'), (SELECT TOP 1 QuanHuyenId FROM QuanHuyen));
DECLARE @GoVap INT = ISNULL((SELECT QuanHuyenId FROM QuanHuyen WHERE Ten = N'Gò Vấp'), (SELECT TOP 1 QuanHuyenId FROM QuanHuyen));

-- INSERT NHÀ TRỌ (sử dụng MERGE để tránh duplicate)
MERGE INTO NhaTro AS target
USING (VALUES
    (NEWID(), N'Căn hộ Green View', N'789 Võ Văn Ngân, Thủ Đức, TP.HCM', @ThuDuc),
    (NEWID(), N'Sky Tower Residence', N'123 Đinh Bộ Lĩnh, Bình Thạnh, TP.HCM', @BinhThanh),
    (NEWID(), N'Phòng Trọ An Phú', N'456 Nguyễn Văn Nghi, Gò Vấp, TP.HCM', @GoVap),
    (NEWID(), N'Dragon House', N'789 Quang Trung, Gò Vấp, TP.HCM', @GoVap),
    (NEWID(), N'Pearl Apartment', N'321 Dương Quảng Hàm, Gò Vấp, TP.HCM', @GoVap)
) AS source (NhaTroId, TieuDe, DiaChi, QuanHuyenId)
ON target.TieuDe = source.TieuDe
WHEN NOT MATCHED THEN
    INSERT (NhaTroId, ChuTroId, TieuDe, DiaChi, QuanHuyenId, IsHoatDong)
    VALUES (source.NhaTroId, @ChuTro, source.TieuDe, source.DiaChi, source.QuanHuyenId, 1);

-- INSERT PHÒNG TRỌ (Sử dụng EXEC để tránh lỗi compile khi cột MoTa mới được thêm)
EXEC sp_executesql N'
MERGE INTO Phong AS target
USING (
    SELECT 
        NEWID() AS PhongId,
        nt.NhaTroId,
        p.TieuDe, p.DienTich, p.GiaTien, p.TienCoc, p.SoNguoi, p.Diem, p.SoReview, p.MoTa
    FROM (VALUES
        (N''Căn hộ Green View'', N''Căn hộ studio view sông, full đồ'', 35, 5500000, 5500000, 2, 4.9, 32, N''Căn hộ studio hiện đại với view sông thoáng mát. Đầy đủ nội thất cao cấp, chỉ cần xách vali vào ở. Hệ thống an ninh 24/7.''),
        (N''Sky Tower Residence'', N''Căn hộ 2PN sang trọng, view thành phố'', 55, 8500000, 8500000, 4, 5.0, 45, N''Căn hộ 2 phòng ngủ rộng rãi, view toàn cảnh thành phố rực rỡ. Phù hợp cho hộ gia đình hoặc nhóm bạn đi làm.''),
        (N''Phòng Trọ An Phú'', N''Phòng trọ giá sinh viên, gần chợ, xe bus'', 20, 1500000, 1500000, 2, 4.0, 28, N''Phòng trọ sạch sẽ, an ninh tốt, gần trạm xe bus và các trường đại học lớn. Giá cả hợp lý cho sinh viên.''),
        (N''Dragon House'', N''Phòng Deluxe cao cấp, bếp + balcony'', 40, 6000000, 6000000, 3, 4.7, 22, N''Không gian sống đẳng cấp với ban công riêng biệt và bếp nấu ăn đầy đủ dụng cụ. Khu vực yên tĩnh, dân trí cao.''),
        (N''Pearl Apartment'', N''Penthouse VIP tầng cao, view 360 độ'', 75, 12000000, 12000000, 4, 5.0, 18, N''Đỉnh cao của sự sang trọng với tầm nhìn 360 độ khắp thành phố. Nội thất nhập khẩu hoàn toàn từ Châu Âu.''),
        (N''Phòng Trọ An Phú'', N''Phòng mini hiện đại, đầy đủ tiện nghi'', 18, 2100000, 2100000, 2, 4.5, 30, N''Tối ưu hóa diện tích với nội thất thông minh. Phù hợp cho những người yêu thích sự gọn gàng và tiện dụng.''),
        (N''Dragon House'', N''Phòng căn góc 2 mặt tiền, thoáng mát'', 32, 3800000, 3800000, 3, 4.6, 25, N''Căn góc với 2 mặt thoáng, luôn tràn ngập ánh sáng tự nhiên. Gần công viên, không khí trong lành.'')
    ) AS p(NhaTro, TieuDe, DienTich, GiaTien, TienCoc, SoNguoi, Diem, SoReview, MoTa)
    INNER JOIN NhaTro nt ON nt.TieuDe = p.NhaTro
) AS source
ON target.TieuDe = source.TieuDe
WHEN NOT MATCHED THEN
    INSERT (PhongId, NhaTroId, TieuDe, DienTich, GiaTien, TienCoc, SoNguoiToiDa, TrangThai, 
            IsDuyet, NguoiDuyet, ThoiGianDuyet, DiemTrungBinh, SoLuongDanhGia, MoTa)
    VALUES (source.PhongId, source.NhaTroId, source.TieuDe, source.DienTich, source.GiaTien, 
            source.TienCoc, source.SoNguoi, N''con_trong'', 1, @AdminId, SYSDATETIMEOFFSET(), 
            source.Diem, source.SoReview, source.MoTa);', N'@AdminId UNIQUEIDENTIFIER', @AdminId = @Admin;

-- INSERT TIỆN ÍCH CHO PHÒNG (chỉ các tiện ích phổ biến)
INSERT INTO PhongTienIch (PhongId, TienIchId)
SELECT DISTINCT p.PhongId, ti.TienIchId
FROM Phong p
CROSS JOIN TienIch ti
WHERE p.TieuDe IN (
    N'Căn hộ studio view sông, full đồ',
    N'Căn hộ 2PN sang trọng, view thành phố',
    N'Phòng trọ giá sinh viên, gần chợ, xe bus',
    N'Phòng Deluxe cao cấp, bếp + balcony',
    N'Penthouse VIP tầng cao, view 360 độ',
    N'Phòng mini hiện đại, đầy đủ tiện nghi',
    N'Phòng căn góc 2 mặt tiền, thoáng mát'
)
AND ti.Ten IN (N'Wifi', N'Máy lạnh', N'Chỗ để xe')
AND NOT EXISTS (
    SELECT 1 FROM PhongTienIch pti 
    WHERE pti.PhongId = p.PhongId AND pti.TienIchId = ti.TienIchId
);

-- Hiển thị kết quả
SELECT COUNT(*) AS N'Tổng phòng đã duyệt' FROM Phong WHERE IsDuyet = 1;
PRINT N'✅ Hoàn tất! Đã thêm dữ liệu phòng trọ nổi bật.';
GO

-- ============================================================
-- THÊM BẢNG HÌNH ẢNH PHÒNG VÀ DỮ LIỆU MẪU
-- ============================================================

-- Tạo bảng PhongHinhAnh nếu chưa tồn tại
IF OBJECT_ID(N'dbo.PhongHinhAnh', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhongHinhAnh (
        HinhAnhId UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        PhongId UNIQUEIDENTIFIER NOT NULL,
        DuongDanAnh NVARCHAR(1000) NOT NULL,
        LaThumbnail BIT DEFAULT 0,
        ThuTu INT DEFAULT 0,
        CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
        FOREIGN KEY (PhongId) REFERENCES dbo.Phong(PhongId)
    );
    PRINT N'✓ Đã tạo bảng PhongHinhAnh';
END

-- Thêm ảnh mẫu cho các phòng (dùng URL placeholder hoặc ảnh có sẵn)
INSERT INTO PhongHinhAnh (PhongId, DuongDanAnh, LaThumbnail, ThuTu)
SELECT p.PhongId, '/images/example.png', 1, 1
FROM Phong p
WHERE p.IsDuyet = 1
AND NOT EXISTS (SELECT 1 FROM PhongHinhAnh WHERE PhongId = p.PhongId);

PRINT N'✓ Đã thêm hình ảnh mẫu cho các phòng';
SELECT COUNT(*) AS N'Tổng hình ảnh' FROM PhongHinhAnh;
GO
PRINT N'✅ Hoàn tất thiết lập cơ sở dữ liệu!';