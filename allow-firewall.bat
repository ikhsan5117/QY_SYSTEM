@echo off
echo ========================================
echo  MEMBUKA FIREWALL UNTUK ASP.NET CORE
echo ========================================
echo.
echo Script ini akan membuka port 5000 dan 5001
echo di Windows Firewall untuk akses dari tablet/device lain
echo.
echo PENTING: Script ini harus dijalankan sebagai Administrator!
echo.
pause

echo.
echo [1/3] Membuka port 5000 (HTTP)...
netsh advfirewall firewall add rule name="ASP.NET Core HTTP (Port 5000)" dir=in action=allow protocol=TCP localport=5000
if %errorlevel% equ 0 (
    echo [OK] Port 5000 berhasil dibuka!
) else (
    echo [ERROR] Gagal membuka port 5000. Pastikan script dijalankan sebagai Administrator!
)

echo.
echo [2/3] Membuka port 5001 (HTTPS)...
netsh advfirewall firewall add rule name="ASP.NET Core HTTPS (Port 5001)" dir=in action=allow protocol=TCP localport=5001
if %errorlevel% equ 0 (
    echo [OK] Port 5001 berhasil dibuka!
) else (
    echo [ERROR] Gagal membuka port 5001. Pastikan script dijalankan sebagai Administrator!
)

echo.
echo [3/3] Menampilkan firewall rules yang baru dibuat...
netsh advfirewall firewall show rule name="ASP.NET Core HTTP (Port 5000)"
netsh advfirewall firewall show rule name="ASP.NET Core HTTPS (Port 5001)"

echo.
echo ========================================
echo  SELESAI!
echo ========================================
echo.
echo Sekarang aplikasi sudah bisa diakses dari tablet!
echo Gunakan URL: http://10.14.180.245:5000
echo.
echo CATATAN: Jika IP berubah, cek lagi dengan 'ipconfig'
echo.
pause
