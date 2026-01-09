-- Script untuk memperbaiki Role dari 'User' menjadi 'Quality'
-- untuk semua user non-admin

-- Update Role untuk semua user yang bukan Administrator
UPDATE Users 
SET Role = 'Quality' 
WHERE Username != 'Administrator' AND Role = 'User';

-- Verifikasi hasil
SELECT Username, Role, Plant, Grup, NamaLengkap 
FROM Users 
ORDER BY Plant, Username;

-- Cek jumlah per role
SELECT Role, COUNT(*) as TotalUsers 
FROM Users 
GROUP BY Role;
