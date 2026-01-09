-- ============================================
-- Script untuk membuat tabel QCHoseData
-- Tabel untuk menyimpan data input E_LWP
-- ============================================

USE CheckDimensiDB;
GO

-- Hapus tabel jika sudah ada (untuk re-create)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QCHoseData]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[QCHoseData];
    PRINT 'Tabel QCHoseData lama dihapus.';
END
GO

-- Buat tabel QCHoseData
CREATE TABLE QCHoseData (
    QCHoseDataId INT IDENTITY(1,1) PRIMARY KEY,
    LineChecking NVARCHAR(50) NOT NULL,
    NamaInspector NVARCHAR(100) NOT NULL,
    GroupChecking NVARCHAR(10) NOT NULL DEFAULT 'A', -- A atau B
    PartCode NVARCHAR(50) NOT NULL,
    TimeStop NVARCHAR(20) NULL DEFAULT '0:0:0:0',
    JenisNG NVARCHAR(100) NULL,
    NamaOPR NVARCHAR(100) NULL,
    QtyNG INT NULL,
    QtyCheck INT NULL,
    LineStop NVARCHAR(50) NULL,
    StatusChecking NVARCHAR(20) NOT NULL DEFAULT 'Checking', -- Checking, Done, Stop
    StatusCheckingTime NVARCHAR(20) NULL DEFAULT '0:0:0:0',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    Plant NVARCHAR(50) NULL,
    Grup NVARCHAR(50) NULL
);
GO

-- Buat index untuk performa
CREATE INDEX IX_QCHoseData_CreatedAt ON QCHoseData(CreatedAt DESC);
CREATE INDEX IX_QCHoseData_LineChecking ON QCHoseData(LineChecking);
CREATE INDEX IX_QCHoseData_NamaInspector ON QCHoseData(NamaInspector);
CREATE INDEX IX_QCHoseData_PartCode ON QCHoseData(PartCode);
GO

PRINT '=========================================';
PRINT 'TABEL QCHoseData BERHASIL DIBUAT';
PRINT '=========================================';
PRINT '';
PRINT 'Struktur tabel:';
PRINT '- QCHoseDataId (Primary Key)';
PRINT '- LineChecking';
PRINT '- NamaInspector';
PRINT '- GroupChecking (A/B)';
PRINT '- PartCode';
PRINT '- TimeStop';
PRINT '- JenisNG';
PRINT '- NamaOPR';
PRINT '- QtyNG';
PRINT '- QtyCheck';
PRINT '- LineStop';
PRINT '- StatusChecking (Checking/Done/Stop)';
PRINT '- StatusCheckingTime';
PRINT '- CreatedAt';
PRINT '- Plant';
PRINT '- Grup';
PRINT '';
PRINT 'Index yang dibuat:';
PRINT '- IX_QCHoseData_CreatedAt';
PRINT '- IX_QCHoseData_LineChecking';
PRINT '- IX_QCHoseData_NamaInspector';
PRINT '- IX_QCHoseData_PartCode';
PRINT '=========================================';
GO

