-- ============================================
-- Script SQL untuk Verifikasi Data E_LWP
-- Gunakan untuk testing dan debugging
-- ============================================

USE CheckDimensiDB;
GO

-- ============================================
-- 1. CEK DATA QCHoseData TERBARU
-- ============================================
PRINT '========================================';
PRINT '1. DATA QCHoseData TERBARU (10 RECORD)';
PRINT '========================================';
SELECT TOP 10 
    QCHoseDataId AS ID,
    LineChecking AS [Line Checking],
    NamaInspector AS [Nama Inspector],
    GroupChecking AS [Group],
    PartCode AS [Part Code],
    TimeStop AS [Time Stop],
    JenisNG AS [Jenis NG],
    NamaOPR AS [Nama OPR],
    QtyNG AS [Qty NG],
    QtyCheck AS [Qty Check],
    LineStop AS [Line Stop],
    StatusChecking AS [Status],
    StatusCheckingTime AS [Status Time],
    CreatedAt AS [Created At],
    Plant,
    Grup
FROM QCHoseData
ORDER BY QCHoseDataId DESC;
GO

-- ============================================
-- 2. CEK TOTAL DATA QCHoseData
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '2. TOTAL DATA QCHoseData';
PRINT '========================================';
SELECT 
    COUNT(*) AS [Total Records],
    COUNT(DISTINCT LineChecking) AS [Total Line Checking],
    COUNT(DISTINCT NamaInspector) AS [Total Inspector],
    COUNT(DISTINCT PartCode) AS [Total Part Code]
FROM QCHoseData;
GO

-- ============================================
-- 3. CEK MASTER DATA AKTIF
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '3. MASTER DATA AKTIF (GROUP BY TIPE)';
PRINT '========================================';
SELECT 
    Tipe,
    COUNT(*) AS [Jumlah Data],
    STRING_AGG(Nilai, ', ') AS [Daftar Nilai]
FROM MasterData
WHERE IsActive = 1
GROUP BY Tipe
ORDER BY Tipe;
GO

-- ============================================
-- 4. CEK MASTER DATA DETAIL PER TIPE
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '4. MASTER DATA - LINE CHECKING';
PRINT '========================================';
SELECT 
    Id,
    Nilai AS [Line Checking],
    Deskripsi,
    IsActive,
    CreatedAt
FROM MasterData
WHERE Tipe = 'LineChecking' AND IsActive = 1
ORDER BY Nilai;
GO

PRINT '';
PRINT '========================================';
PRINT '5. MASTER DATA - INSPECTOR';
PRINT '========================================';
SELECT 
    Id,
    Nilai AS [Inspector],
    Deskripsi,
    IsActive,
    CreatedAt
FROM MasterData
WHERE Tipe = 'Inspector' AND IsActive = 1
ORDER BY Nilai;
GO

PRINT '';
PRINT '========================================';
PRINT '6. MASTER DATA - GROUP CHECKING';
PRINT '========================================';
SELECT 
    Id,
    Nilai AS [Group Checking],
    Deskripsi,
    IsActive,
    CreatedAt
FROM MasterData
WHERE Tipe = 'GroupChecking' AND IsActive = 1
ORDER BY Nilai;
GO

PRINT '';
PRINT '========================================';
PRINT '7. MASTER DATA - JENIS NG';
PRINT '========================================';
SELECT 
    Id,
    Nilai AS [Jenis NG],
    Deskripsi,
    IsActive,
    CreatedAt
FROM MasterData
WHERE Tipe = 'JenisNG' AND IsActive = 1
ORDER BY Nilai;
GO

PRINT '';
PRINT '========================================';
PRINT '8. MASTER DATA - LINE STOP';
PRINT '========================================';
SELECT 
    Id,
    Nilai AS [Line Stop],
    Deskripsi,
    IsActive,
    CreatedAt
FROM MasterData
WHERE Tipe = 'LineStop' AND IsActive = 1
ORDER BY Nilai;
GO

PRINT '';
PRINT '========================================';
PRINT '9. MASTER DATA - PART CODE';
PRINT '========================================';
SELECT 
    Id,
    Nilai AS [Part Code],
    Deskripsi,
    IsActive,
    CreatedAt
FROM MasterData
WHERE Tipe = 'PartCode' AND IsActive = 1
ORDER BY Nilai;
GO

PRINT '';
PRINT '========================================';
PRINT '10. MASTER DATA - LIST ABNORMALITY';
PRINT '========================================';
SELECT 
    Id,
    Nilai AS [List Abnormality],
    Deskripsi,
    IsActive,
    CreatedAt
FROM MasterData
WHERE Tipe = 'ListAbnormality' AND IsActive = 1
ORDER BY Nilai;
GO

-- ============================================
-- 11. CEK MASTER DATA YANG DI-SOFT DELETE
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '11. MASTER DATA YANG DI-SOFT DELETE';
PRINT '========================================';
SELECT 
    Id,
    Tipe,
    Nilai,
    Deskripsi,
    IsActive,
    CreatedAt
FROM MasterData
WHERE IsActive = 0
ORDER BY Tipe, Nilai;
GO

-- ============================================
-- 12. CEK PART CODE DARI PRODUK DAN MASTERDATA
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '12. PART CODE (DARI PRODUK & MASTERDATA)';
PRINT '========================================';
-- Part Code dari MasterData
SELECT DISTINCT 
    Nilai AS PartCode, 
    'MasterData' AS Sumber,
    Deskripsi
FROM MasterData
WHERE Tipe = 'PartCode' AND IsActive = 1

UNION

-- Part Code dari Produk
SELECT DISTINCT 
    PartCode AS Nilai, 
    'Produk' AS Sumber,
    NamaProduk AS Deskripsi
FROM Produk
WHERE PartCode IS NOT NULL AND PartCode != ''

ORDER BY PartCode;
GO

-- ============================================
-- 13. CEK DATA QCHoseData PER LINE CHECKING
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '13. DATA QCHoseData PER LINE CHECKING';
PRINT '========================================';
SELECT 
    LineChecking AS [Line Checking],
    COUNT(*) AS [Jumlah Data],
    SUM(QtyCheck) AS [Total Qty Check],
    SUM(ISNULL(QtyNG, 0)) AS [Total Qty NG],
    MIN(CreatedAt) AS [Data Pertama],
    MAX(CreatedAt) AS [Data Terakhir]
FROM QCHoseData
GROUP BY LineChecking
ORDER BY LineChecking;
GO

-- ============================================
-- 14. CEK DATA QCHoseData PER INSPECTOR
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '14. DATA QCHoseData PER INSPECTOR';
PRINT '========================================';
SELECT 
    NamaInspector AS [Nama Inspector],
    COUNT(*) AS [Jumlah Data],
    SUM(QtyCheck) AS [Total Qty Check],
    SUM(ISNULL(QtyNG, 0)) AS [Total Qty NG],
    MIN(CreatedAt) AS [Data Pertama],
    MAX(CreatedAt) AS [Data Terakhir]
FROM QCHoseData
GROUP BY NamaInspector
ORDER BY NamaInspector;
GO

-- ============================================
-- 15. CEK DATA QCHoseData PER STATUS
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '15. DATA QCHoseData PER STATUS';
PRINT '========================================';
SELECT 
    StatusChecking AS [Status],
    COUNT(*) AS [Jumlah Data],
    AVG(CAST(REPLACE(REPLACE(TimeStop, ':', ''), '0', '') AS FLOAT)) AS [Avg Time Stop]
FROM QCHoseData
GROUP BY StatusChecking
ORDER BY StatusChecking;
GO

-- ============================================
-- 16. CEK DATA QCHoseData HARI INI
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '16. DATA QCHoseData HARI INI';
PRINT '========================================';
SELECT 
    QCHoseDataId AS ID,
    LineChecking AS [Line Checking],
    NamaInspector AS [Inspector],
    PartCode AS [Part Code],
    QtyCheck AS [Qty Check],
    StatusChecking AS [Status],
    CreatedAt AS [Created At]
FROM QCHoseData
WHERE CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY CreatedAt DESC;
GO

-- ============================================
-- 17. CEK DUPLIKAT MASTER DATA (SOFT DELETE)
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '17. CEK DUPLIKAT MASTER DATA';
PRINT '========================================';
SELECT 
    Tipe,
    Nilai,
    COUNT(*) AS [Jumlah],
    STRING_AGG(CAST(Id AS VARCHAR), ', ') AS [ID List],
    STRING_AGG(CAST(IsActive AS VARCHAR), ', ') AS [IsActive List]
FROM MasterData
GROUP BY Tipe, Nilai
HAVING COUNT(*) > 1
ORDER BY Tipe, Nilai;
GO

-- ============================================
-- 18. CEK INTEGRITAS DATA (DATA YANG TIDAK ADA DI MASTER DATA)
-- ============================================
PRINT '';
PRINT '========================================';
PRINT '18. CEK INTEGRITAS DATA - LINE CHECKING';
PRINT '========================================';
SELECT DISTINCT 
    q.LineChecking AS [Line Checking di QCHoseData],
    CASE 
        WHEN m.Id IS NULL THEN 'TIDAK ADA DI MASTER DATA'
        ELSE 'ADA DI MASTER DATA'
    END AS [Status]
FROM QCHoseData q
LEFT JOIN MasterData m ON m.Tipe = 'LineChecking' AND m.Nilai = q.LineChecking AND m.IsActive = 1
WHERE m.Id IS NULL;
GO

PRINT '';
PRINT '========================================';
PRINT '19. CEK INTEGRITAS DATA - INSPECTOR';
PRINT '========================================';
SELECT DISTINCT 
    q.NamaInspector AS [Inspector di QCHoseData],
    CASE 
        WHEN m.Id IS NULL THEN 'TIDAK ADA DI MASTER DATA'
        ELSE 'ADA DI MASTER DATA'
    END AS [Status]
FROM QCHoseData q
LEFT JOIN MasterData m ON m.Tipe = 'Inspector' AND m.Nilai = q.NamaInspector AND m.IsActive = 1
WHERE m.Id IS NULL;
GO

PRINT '';
PRINT '========================================';
PRINT '20. CEK INTEGRITAS DATA - PART CODE';
PRINT '========================================';
SELECT DISTINCT 
    q.PartCode AS [Part Code di QCHoseData],
    CASE 
        WHEN m.Id IS NULL AND p.ProdukId IS NULL THEN 'TIDAK ADA DI MASTER DATA ATAU PRODUK'
        WHEN m.Id IS NOT NULL THEN 'ADA DI MASTER DATA'
        WHEN p.ProdukId IS NOT NULL THEN 'ADA DI PRODUK'
    END AS [Status]
FROM QCHoseData q
LEFT JOIN MasterData m ON m.Tipe = 'PartCode' AND m.Nilai = q.PartCode AND m.IsActive = 1
LEFT JOIN Produk p ON p.PartCode = q.PartCode
WHERE m.Id IS NULL AND p.ProdukId IS NULL;
GO

PRINT '';
PRINT '========================================';
PRINT 'VERIFIKASI SELESAI';
PRINT '========================================';

