-- Script untuk membuat user Administrator di SQLite
-- Jalankan dengan: sqlite3 checkdimensi.db < create_admin.sql

-- Cek apakah Administrator sudah ada
DELETE FROM Users WHERE Username = 'Administrator';

-- Insert Administrator
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup, IsActive, TanggalDibuat)
VALUES ('Administrator', 'admin123', 'System Administrator', 'Admin', 'All', 'Admin', 1, datetime('now'));

-- Verifikasi
SELECT Username, NamaLengkap, Role, IsActive FROM Users WHERE Username = 'Administrator';

