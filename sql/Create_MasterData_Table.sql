USE CheckDimensiDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MasterData')
BEGIN
    CREATE TABLE MasterData (
        MasterDataId INT IDENTITY(1,1) PRIMARY KEY,
        Tipe NVARCHAR(50) NOT NULL, -- LineChecking, Inspector, GroupChecking, JenisNG, LineStop
        Nilai NVARCHAR(200) NOT NULL,
        Deskripsi NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT UQ_MasterData_Tipe_Nilai UNIQUE (Tipe, Nilai)
    );
    
    -- Insert default data
    INSERT INTO MasterData (Tipe, Nilai, Deskripsi, IsActive) VALUES
    ('LineChecking', 'Line 1', 'Line Checking 1', 1),
    ('LineChecking', 'Line 2', 'Line Checking 2', 1),
    ('LineChecking', 'Line 3', 'Line Checking 3', 1),
    ('LineChecking', 'Line 4', 'Line Checking 4', 1),
    ('GroupChecking', 'A', 'Group A', 1),
    ('GroupChecking', 'B', 'Group B', 1),
    ('JenisNG', 'Dimensi', 'NG Dimensi', 1),
    ('JenisNG', 'Visual', 'NG Visual', 1),
    ('JenisNG', 'Fungsi', 'NG Fungsi', 1),
    ('JenisNG', 'Lainnya', 'NG Lainnya', 1),
    ('LineStop', 'Line 1', 'Line Stop 1', 1),
    ('LineStop', 'Line 2', 'Line Stop 2', 1),
    ('LineStop', 'Line 3', 'Line Stop 3', 1),
    ('LineStop', 'Line 4', 'Line Stop 4', 1);
    
    PRINT 'Table MasterData created successfully with default data.';
END
ELSE
BEGIN
    PRINT 'Table MasterData already exists.';
END
GO

