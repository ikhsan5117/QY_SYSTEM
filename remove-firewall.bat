@echo off
echo ========================================
echo  MENUTUP FIREWALL UNTUK ASP.NET CORE
echo ========================================
echo.
echo Script ini akan menutup kembali port 5000 dan 5001
echo di Windows Firewall
echo.
echo PENTING: Script ini harus dijalankan sebagai Administrator!
echo.
pause

echo.
echo [1/2] Menutup port 5000 (HTTP)...
netsh advfirewall firewall delete rule name="ASP.NET Core HTTP (Port 5000)"
if %errorlevel% equ 0 (
    echo [OK] Port 5000 berhasil ditutup!
) else (
    echo [INFO] Rule untuk port 5000 tidak ditemukan atau sudah dihapus.
)

echo.
echo [2/2] Menutup port 5001 (HTTPS)...
netsh advfirewall firewall delete rule name="ASP.NET Core HTTPS (Port 5001)"
if %errorlevel% equ 0 (
    echo [OK] Port 5001 berhasil ditutup!
) else (
    echo [INFO] Rule untuk port 5001 tidak ditemukan atau sudah dihapus.
)

echo.
echo ========================================
echo  SELESAI!
echo ========================================
echo.
echo Port 5000 dan 5001 sudah ditutup kembali.
echo Aplikasi hanya bisa diakses dari komputer ini saja.
echo.
pause
