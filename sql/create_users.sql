-- Script untuk membuat user login sesuai gambar
-- Password untuk semua user: qcvin

-- Hapus user lama jika ada (kecuali Administrator)
DELETE FROM Users WHERE Username != 'Administrator';

-- Insert users Plant Hose grup A
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('Hose1A', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose2A', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose3A', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose4A', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose5A', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose6A', 'qcvin', 'User', 'User', 'Hose', 'Quality');

-- Insert users Plant Molded grup A
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('Molded1A', 'qcvin', 'User', 'User', 'Molded', 'Quality'),
('Molded2A', 'qcvin', 'User', 'User', 'Molded', 'Quality'),
('Molded3A', 'qcvin', 'User', 'User', 'Molded', 'Quality'),
('Molded4A', 'qcvin', 'User', 'User', 'Molded', 'Quality');

-- Insert users Plant RVI grup A
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('RVI1A', 'qcvin', 'User', 'User', 'RVI', 'Quality'),
('RVI2A', 'qcvin', 'User', 'User', 'RVI', 'Quality'),
('RVI3A', 'qcvin', 'User', 'User', 'RVI', 'Quality'),
('RVI4A', 'qcvin', 'User', 'User', 'RVI', 'Quality'),
('RVI5A', 'qcvin', 'User', 'User', 'RVI', 'Quality');

-- Insert users Plant TPE grup A
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('TPE1A', 'qcvin', 'User', 'User', 'TPE', 'Quality'),
('TPE2A', 'qcvin', 'User', 'User', 'TPE', 'Quality');

-- Insert users Plant BTR grup A
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('BTR1A', 'qcvin', 'User', 'User', 'BTR', 'Quality'),
('BTR2A', 'qcvin', 'User', 'User', 'BTR', 'Quality');

-- Insert users Plant Hose grup B
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('Hose1B', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose2B', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose3B', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose4B', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose5B', 'qcvin', 'User', 'User', 'Hose', 'Quality'),
('Hose6B', 'qcvin', 'User', 'User', 'Hose', 'Quality');

-- Insert users Plant Molded grup B
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('Molded1B', 'qcvin', 'User', 'User', 'Molded', 'Quality'),
('Molded2B', 'qcvin', 'User', 'User', 'Molded', 'Quality'),
('Molded3B', 'qcvin', 'User', 'User', 'Molded', 'Quality'),
('Molded4B', 'qcvin', 'User', 'User', 'Molded', 'Quality');

-- Insert users Plant RVI grup B
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('RVI1B', 'qcvin', 'User', 'User', 'RVI', 'Quality'),
('RVI2B', 'qcvin', 'User', 'User', 'RVI', 'Quality'),
('RVI3B', 'qcvin', 'User', 'User', 'RVI', 'Quality'),
('RVI4B', 'qcvin', 'User', 'User', 'RVI', 'Quality'),
('RVI5B', 'qcvin', 'User', 'User', 'RVI', 'Quality');

-- Insert users Plant TPE grup B
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('TPE1B', 'qcvin', 'User', 'User', 'TPE', 'Quality'),
('TPE2B', 'qcvin', 'User', 'User', 'TPE', 'Quality');

-- Insert users Plant BTR grup B
INSERT INTO Users (Username, Password, NamaLengkap, Role, Plant, Grup) VALUES
('BTR1B', 'qcvin', 'User', 'User', 'BTR', 'Quality'),
('BTR2B', 'qcvin', 'User', 'User', 'BTR', 'Quality');

-- Verifikasi hasil
SELECT Username, NamaLengkap, Role, Plant, Grup FROM Users ORDER BY Plant, Username;