-- Script untuk update NamaLengkap di tabel Users
-- Berdasarkan mapping dari data yang diberikan user

-- Update Administrator
UPDATE Users SET NamaLengkap = 'System Administrator' WHERE Username = 'Administrator';

-- Update BTR Users
UPDATE Users SET NamaLengkap = 'Yaya' WHERE Username = 'BTR1A';
UPDATE Users SET NamaLengkap = 'Rais' WHERE Username = 'BTR1B';
UPDATE Users SET NamaLengkap = 'Andri' WHERE Username = 'BTR2A';
UPDATE Users SET NamaLengkap = 'Riki' WHERE Username = 'BTR2B';

-- Update Hose Users
UPDATE Users SET NamaLengkap = 'Rico' WHERE Username = 'Hose1A';
UPDATE Users SET NamaLengkap = 'Dimas W' WHERE Username = 'Hose1B';
UPDATE Users SET NamaLengkap = 'Egi' WHERE Username = 'Hose2A';
UPDATE Users SET NamaLengkap = 'Deni' WHERE Username = 'Hose2B';
UPDATE Users SET NamaLengkap = 'Hendry' WHERE Username = 'Hose3A';
UPDATE Users SET NamaLengkap = 'Arsal' WHERE Username = 'Hose3B';
UPDATE Users SET NamaLengkap = 'Dimas S' WHERE Username = 'Hose4A';
UPDATE Users SET NamaLengkap = 'Gilang' WHERE Username = 'Hose4B';
UPDATE Users SET NamaLengkap = 'Rico' WHERE Username = 'Hose5A';
UPDATE Users SET NamaLengkap = 'Nanang' WHERE Username = 'Hose5B';
UPDATE Users SET NamaLengkap = 'Azis' WHERE Username = 'Hose6A';
UPDATE Users SET NamaLengkap = 'Alif' WHERE Username = 'Hose6B';

-- Update Lead Hose & Molded
UPDATE Users SET NamaLengkap = 'Ariyanto' WHERE Username = 'Leadhose';
UPDATE Users SET NamaLengkap = 'Misbah' WHERE Username = 'Leadmolded';

-- Update Molded Users
UPDATE Users SET NamaLengkap = 'Achmad' WHERE Username = 'Molded1A';
UPDATE Users SET NamaLengkap = 'Mifta' WHERE Username = 'Molded1B';
UPDATE Users SET NamaLengkap = 'Hamdan' WHERE Username = 'Molded2A';
UPDATE Users SET NamaLengkap = 'Luqman' WHERE Username = 'Molded2B';
UPDATE Users SET NamaLengkap = 'Salam' WHERE Username = 'Molded3A';
UPDATE Users SET NamaLengkap = 'Yayang' WHERE Username = 'Molded3B';
UPDATE Users SET NamaLengkap = 'Sahrul' WHERE Username = 'Molded4A';
UPDATE Users SET NamaLengkap = 'Suhidin' WHERE Username = 'Molded4B';

-- Update Lead RVI
UPDATE Users SET NamaLengkap = 'Willy' WHERE Username = 'LeadrviA';
UPDATE Users SET NamaLengkap = 'Arif' WHERE Username = 'LeadrviB';

-- Update RVI Users
UPDATE Users SET NamaLengkap = 'Ridwan' WHERE Username = 'RVI1A';
UPDATE Users SET NamaLengkap = 'Acu' WHERE Username = 'RVI1B';
UPDATE Users SET NamaLengkap = 'Piki' WHERE Username = 'RVI2A';
UPDATE Users SET NamaLengkap = 'Keken' WHERE Username = 'RVI2B';
UPDATE Users SET NamaLengkap = 'Roby' WHERE Username = 'RVI3A';
UPDATE Users SET NamaLengkap = 'Wahyu' WHERE Username = 'RVI3B';
UPDATE Users SET NamaLengkap = 'Thandy' WHERE Username = 'RVI4A';
UPDATE Users SET NamaLengkap = 'RVI User' WHERE Username = 'RVI4B';
UPDATE Users SET NamaLengkap = 'RVI User' WHERE Username = 'RVI5A';
UPDATE Users SET NamaLengkap = 'RVI User' WHERE Username = 'RVI5B';

-- Update TPE Users
UPDATE Users SET NamaLengkap = 'Wikatma' WHERE Username = 'TPE1A';
UPDATE Users SET NamaLengkap = 'Arif' WHERE Username = 'TPE1B';
UPDATE Users SET NamaLengkap = 'TPE User' WHERE Username = 'TPE2A';
UPDATE Users SET NamaLengkap = 'TPE User' WHERE Username = 'TPE2B';

-- Verifikasi hasil
SELECT Username, NamaLengkap, Plant, Grup FROM Users ORDER BY Plant, Username;
