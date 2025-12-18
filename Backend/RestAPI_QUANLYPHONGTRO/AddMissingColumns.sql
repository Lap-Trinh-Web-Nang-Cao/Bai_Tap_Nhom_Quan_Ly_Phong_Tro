-- Script to add missing columns to tables if they don't exist
USE QuanLyPhongTro;
GO

-- Add IsDeleted column to Phong if missing
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Phong' AND COLUMN_NAME='IsDeleted')
BEGIN
    ALTER TABLE Phong ADD IsDeleted BIT DEFAULT 0;
    PRINT 'Added IsDeleted column to Phong table';
END
ELSE
BEGIN
    PRINT 'IsDeleted column already exists in Phong table';
END
GO

-- Add IsDuyet column to Phong if missing
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Phong' AND COLUMN_NAME='IsDuyet')
BEGIN
    ALTER TABLE Phong ADD IsDuyet BIT DEFAULT 0;
    PRINT 'Added IsDuyet column to Phong table';
END
ELSE
BEGIN
    PRINT 'IsDuyet column already exists in Phong table';
END
GO

-- Add NguoiDuyet column to Phong if missing
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Phong' AND COLUMN_NAME='NguoiDuyet')
BEGIN
    ALTER TABLE Phong ADD NguoiDuyet UNIQUEIDENTIFIER NULL;
    PRINT 'Added NguoiDuyet column to Phong table';
END
ELSE
BEGIN
    PRINT 'NguoiDuyet column already exists in Phong table';
END
GO

-- Add ThoiGianDuyet column to Phong if missing
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Phong' AND COLUMN_NAME='ThoiGianDuyet')
BEGIN
    ALTER TABLE Phong ADD ThoiGianDuyet DATETIMEOFFSET NULL;
    PRINT 'Added ThoiGianDuyet column to Phong table';
END
ELSE
BEGIN
    PRINT 'ThoiGianDuyet column already exists in Phong table';
END
GO

-- Add IsBiKhoa column to Phong if missing
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Phong' AND COLUMN_NAME='IsBiKhoa')
BEGIN
    ALTER TABLE Phong ADD IsBiKhoa BIT DEFAULT 0;
    PRINT 'Added IsBiKhoa column to Phong table';
END
ELSE
BEGIN
    PRINT 'IsBiKhoa column already exists in Phong table';
END
GO

-- Add DiemTrungBinh column to Phong if missing
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Phong' AND COLUMN_NAME='DiemTrungBinh')
BEGIN
    ALTER TABLE Phong ADD DiemTrungBinh FLOAT NULL;
    PRINT 'Added DiemTrungBinh column to Phong table';
END
ELSE
BEGIN
    PRINT 'DiemTrungBinh column already exists in Phong table';
END
GO

-- Add SoLuongDanhGia column to Phong if missing
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Phong' AND COLUMN_NAME='SoLuongDanhGia')
BEGIN
    ALTER TABLE Phong ADD SoLuongDanhGia INT DEFAULT 0;
    PRINT 'Added SoLuongDanhGia column to Phong table';
END
ELSE
BEGIN
    PRINT 'SoLuongDanhGia column already exists in Phong table';
END
GO

PRINT 'All missing columns have been checked/added successfully!';
