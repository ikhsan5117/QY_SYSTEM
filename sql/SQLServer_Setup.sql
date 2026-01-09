-- ============================================
-- Script SQL Server untuk Aplikasi Check Dimensi
-- Versi: 1.0
-- Tanggal: 17 November 2025
-- ============================================

-- 1. BUAT DATABASE
USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'CheckDimensiDB')
BEGIN
    ALTER DATABASE CheckDimensiDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CheckDimensiDB;
END
GO

CREATE DATABASE CheckDimensiDB;
GO

USE CheckDimensiDB;
GO

-- 2. BUAT TABEL USERS
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    NamaLengkap NVARCHAR(100) NOT NULL,
    Plant NVARCHAR(50) NOT NULL,
    Grup NVARCHAR(50) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- 3. BUAT TABEL PRODUK
CREATE TABLE Produk (
    ProdukId INT IDENTITY(1,1) PRIMARY KEY,
    PartCode NVARCHAR(50) NOT NULL,
    NamaProduk NVARCHAR(200) NOT NULL,
    OperatorPIC NVARCHAR(100) NULL,
    Plant NVARCHAR(50) NULL,
    Grup NVARCHAR(50) NULL,
    Customer NVARCHAR(100) NULL,
    Line NVARCHAR(50) NULL,
    Cavity NVARCHAR(50) NULL,
    CycleTime NVARCHAR(50) NULL,
    IsFG BIT NOT NULL DEFAULT 0,
    CheckingFixture NVARCHAR(200) NULL,
    CheckingFixtureImagePath NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- 4. BUAT TABEL STANDAR DIMENSI
CREATE TABLE StandarDimensi (
    StandarDimensiId INT IDENTITY(1,1) PRIMARY KEY,
    ProdukId INT NOT NULL,
    NamaDimensi NVARCHAR(100) NOT NULL,
    
    -- Dimensi baru (prioritas)
    InnerDiameter_Min DECIMAL(18,4) NULL,
    InnerDiameter_Max DECIMAL(18,4) NULL,
    OuterDiameter_Min DECIMAL(18,4) NULL,
    OuterDiameter_Max DECIMAL(18,4) NULL,
    Thickness_Min DECIMAL(18,4) NULL,
    Thickness_Max DECIMAL(18,4) NULL,
    
    -- Dimensi lama (kompatibilitas)
    Panjang_Min DECIMAL(18,4) NULL,
    Panjang_Max DECIMAL(18,4) NULL,
    Tinggi_Min DECIMAL(18,4) NULL,
    Tinggi_Max DECIMAL(18,4) NULL,
    Radius_Min DECIMAL(18,4) NULL,
    Radius_Max DECIMAL(18,4) NULL,
    DimensiA_Min DECIMAL(18,4) NULL,
    DimensiA_Max DECIMAL(18,4) NULL,
    DimensiB_Min DECIMAL(18,4) NULL,
    DimensiB_Max DECIMAL(18,4) NULL,
    Sudut_Min DECIMAL(18,4) NULL,
    Sudut_Max DECIMAL(18,4) NULL,
    
    JumlahSampling INT NOT NULL DEFAULT 5,
    ImagePath NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_StandarDimensi_Produk FOREIGN KEY (ProdukId) 
        REFERENCES Produk(ProdukId) ON DELETE CASCADE
);
GO

-- 5. BUAT TABEL INPUT AKTUAL
CREATE TABLE InputAktual (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StandarDimensiId INT NOT NULL,
    
    -- Dimensi baru (prioritas)
    NilaiInnerDiameter DECIMAL(18,4) NULL,
    NilaiOuterDiameter DECIMAL(18,4) NULL,
    NilaiThickness DECIMAL(18,4) NULL,
    
    -- Dimensi lama (kompatibilitas)
    NilaiPanjang DECIMAL(18,4) NULL,
    NilaiTinggi DECIMAL(18,4) NULL,
    NilaiRadius DECIMAL(18,4) NULL,
    NilaiDimensiA DECIMAL(18,4) NULL,
    NilaiDimensiB DECIMAL(18,4) NULL,
    NilaiSudut DECIMAL(18,4) NULL,
    
    NamaPIC NVARCHAR(100) NOT NULL,
    Plant NVARCHAR(50) NOT NULL,
    Grup NVARCHAR(50) NOT NULL,
    TanggalInput DATETIME2 NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(10) NULL,
    
    CONSTRAINT FK_InputAktual_StandarDimensi FOREIGN KEY (StandarDimensiId) 
        REFERENCES StandarDimensi(StandarDimensiId) ON DELETE CASCADE
);
GO

-- 6. BUAT INDEX UNTUK PERFORMA
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Produk_PartCode ON Produk(PartCode);
CREATE INDEX IX_InputAktual_TanggalInput ON InputAktual(TanggalInput DESC);
CREATE INDEX IX_InputAktual_NamaPIC ON InputAktual(NamaPIC);
CREATE INDEX IX_InputAktual_Plant_Grup ON InputAktual(Plant, Grup);
GO

-- 7. INSERT DEFAULT ADMINISTRATOR
INSERT INTO Users (Username, Password, NamaLengkap, Plant, Grup, Role, CreatedAt)
VALUES ('Administrator', 'admin123', 'System Administrator', 'All', 'Admin', 'Admin', GETDATE());
GO

-- 8. INSERT USER QUALITY (44 users)

-- BTR (4 users)
INSERT INTO Users (Username, Password, NamaLengkap, Plant, Grup, Role, CreatedAt) VALUES
('BTR1A', 'password123', 'Yaya', 'BTR', 'Quality', 'Quality', GETDATE()),
('BTR1B', 'password123', 'Rais', 'BTR', 'Quality', 'Quality', GETDATE()),
('BTR2A', 'password123', 'Andri', 'BTR', 'Quality', 'Quality', GETDATE()),
('BTR2B', 'password123', 'Riki', 'BTR', 'Quality', 'Quality', GETDATE());
GO

-- Hose (12 users)
INSERT INTO Users (Username, Password, NamaLengkap, Plant, Grup, Role, CreatedAt) VALUES
('Hose1A', 'password123', 'Rico', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose1B', 'password123', 'Dimas W', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose2A', 'password123', 'Egi', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose2B', 'password123', 'Deni', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose3A', 'password123', 'Hendry', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose3B', 'password123', 'Arsal', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose4A', 'password123', 'Dimas S', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose4B', 'password123', 'Gilang', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose5A', 'password123', 'Rico', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose5B', 'password123', 'Nanang', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose6A', 'password123', 'Azis', 'Hose', 'Quality', 'Quality', GETDATE()),
('Hose6B', 'password123', 'Alif', 'Hose', 'Quality', 'Quality', GETDATE());
GO

-- Molded (8 users)
INSERT INTO Users (Username, Password, NamaLengkap, Plant, Grup, Role, CreatedAt) VALUES
('Molded1A', 'password123', 'Achmad', 'Molded', 'Quality', 'Quality', GETDATE()),
('Molded1B', 'password123', 'Mifta', 'Molded', 'Quality', 'Quality', GETDATE()),
('Molded2A', 'password123', 'Hamdan', 'Molded', 'Quality', 'Quality', GETDATE()),
('Molded2B', 'password123', 'Luqman', 'Molded', 'Quality', 'Quality', GETDATE()),
('Molded3A', 'password123', 'Salam', 'Molded', 'Quality', 'Quality', GETDATE()),
('Molded3B', 'password123', 'Yayang', 'Molded', 'Quality', 'Quality', GETDATE()),
('Molded4A', 'password123', 'Sahrul', 'Molded', 'Quality', 'Quality', GETDATE()),
('Molded4B', 'password123', 'Suhidin', 'Molded', 'Quality', 'Quality', GETDATE());
GO

-- RVI (10 users)
INSERT INTO Users (Username, Password, NamaLengkap, Plant, Grup, Role, CreatedAt) VALUES
('RVI1A', 'password123', 'Ridwan', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI1B', 'password123', 'Acu', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI2A', 'password123', 'Piki', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI2B', 'password123', 'Keken', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI3A', 'password123', 'Roby', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI3B', 'password123', 'Wahyu', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI4A', 'password123', 'Thandy', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI4B', 'password123', 'RVI User', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI5A', 'password123', 'RVI User', 'RVI', 'Quality', 'Quality', GETDATE()),
('RVI5B', 'password123', 'RVI User', 'RVI', 'Quality', 'Quality', GETDATE());
GO

-- TPE (4 users)
INSERT INTO Users (Username, Password, NamaLengkap, Plant, Grup, Role, CreatedAt) VALUES
('TPE1A', 'password123', 'Wikatma', 'TPE', 'Quality', 'Quality', GETDATE()),
('TPE1B', 'password123', 'Arif', 'TPE', 'Quality', 'Quality', GETDATE()),
('TPE2A', 'password123', 'TPE User', 'TPE', 'Quality', 'Quality', GETDATE()),
('TPE2B', 'password123', 'TPE User', 'TPE', 'Quality', 'Quality', GETDATE());
GO

-- LEADERS (4 users dengan role Admin)
INSERT INTO Users (Username, Password, NamaLengkap, Plant, Grup, Role, CreatedAt) VALUES
('Leadhose', 'password123', 'Ariyanto', 'Hose', 'Leader', 'Admin', GETDATE()),
('LeadrviA', 'password123', 'Willy', 'RVI', 'Leader', 'Admin', GETDATE()),
('LeadrviB', 'password123', 'Arif', 'RVI', 'Leader', 'Admin', GETDATE()),
('Leadmolded', 'password123', 'Misbah', 'Molded', 'Leader', 'Admin', GETDATE());
GO

-- 9. VERIFIKASI DATA
PRINT '=========================================';
PRINT 'DATABASE SETUP COMPLETED';
PRINT '=========================================';

SELECT 'Users' AS TableName, COUNT(*) AS TotalRecords FROM Users
UNION ALL
SELECT 'Produk', COUNT(*) FROM Produk
UNION ALL
SELECT 'StandarDimensi', COUNT(*) FROM StandarDimensi
UNION ALL
SELECT 'InputAktual', COUNT(*) FROM InputAktual;
GO

-- 10. DISPLAY USERS
SELECT 
    Username, 
    NamaLengkap, 
    Plant, 
    Grup, 
    Role,
    CreatedAt
FROM Users
ORDER BY Role DESC, Plant, Username;
GO

PRINT '=========================================';
PRINT 'DEFAULT LOGIN CREDENTIALS:';
PRINT 'Username: Administrator';
PRINT 'Password: admin123';
PRINT '';
PRINT 'Quality User Example:';
PRINT 'Username: Hose1A (Rico)';
PRINT 'Password: password123';
PRINT '=========================================';
GO
