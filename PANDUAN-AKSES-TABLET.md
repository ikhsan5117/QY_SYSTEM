# 📱 PANDUAN AKSES APLIKASI DARI TABLET/DEVICE LAIN

## 🎯 Masalah
Aplikasi berjalan di laptop tapi tidak bisa diakses dari tablet dengan pesan:
**"Situs ini tidak dapat dijangkau"**

## ✅ Solusi

### STEP 1: Jalankan Script Firewall (WAJIB!)

1. **Klik kanan** file `allow-firewall.bat`
2. Pilih **"Run as administrator"** (Jalankan sebagai administrator)
3. Klik **Yes** pada UAC prompt
4. Tunggu sampai selesai
5. Tekan **Enter** untuk menutup

**PENTING:** Script HARUS dijalankan sebagai Administrator, kalau tidak akan gagal!

### STEP 2: Cek IP Address Laptop

Buka Command Prompt atau PowerShell, lalu ketik:
```
ipconfig
```

Cari bagian **"Wireless LAN adapter"** atau **"Wi-Fi"**, lalu lihat **IPv4 Address**.

Contoh:
```
IPv4 Address. . . . . . . . . . . : 10.14.180.245
```

IP ini yang akan digunakan untuk akses dari tablet.

### STEP 3: Akses dari Tablet

1. Pastikan **tablet terhubung ke WiFi yang sama** dengan laptop
2. Buka browser di tablet (Chrome, Safari, Firefox, dll)
3. Ketik di address bar:
   ```
   http://10.14.180.245:5000
   ```
   (Ganti `10.14.180.245` dengan IP laptop Anda)

4. Tekan **Enter**
5. Aplikasi seharusnya muncul! 🎉

---

## 🔧 Troubleshooting

### Masalah: Masih tidak bisa akses dari tablet

**Solusi 1: Cek Firewall Rule**
1. Tekan `Win + R`
2. Ketik: `wf.msc`
3. Enter
4. Klik **"Inbound Rules"**
5. Cari rule dengan nama **"ASP.NET Core HTTP (Port 5000)"**
6. Pastikan ada dan **Enabled** (hijau)

**Solusi 2: Restart Aplikasi**
1. Stop aplikasi (Ctrl+C di terminal)
2. Jalankan lagi: `dotnet run`
3. Test lagi dari tablet

**Solusi 3: Cek WiFi**
- Pastikan laptop dan tablet di **WiFi yang sama**
- Jangan gunakan WiFi public yang ada **Client Isolation**

**Solusi 4: Matikan Antivirus Sementara**
- Beberapa antivirus memblokir koneksi jaringan
- Matikan sementara untuk test
- Jika berhasil, tambahkan exception di antivirus

### Masalah: IP Address berubah-ubah

IP dinamis bisa berubah setiap restart. Solusi:
1. Cek IP lagi dengan `ipconfig`
2. Gunakan IP yang baru

Atau set **IP Static** di pengaturan WiFi:
1. Settings → Network & Internet → Wi-Fi
2. Klik nama WiFi → Properties
3. IP assignment → Edit → Manual
4. Set IP static (misal: 192.168.1.100)

---

## 🗑️ Menutup Akses dari Device Lain

Jika nanti tidak ingin aplikasi bisa diakses dari device lain:

1. **Klik kanan** file `remove-firewall.bat`
2. Pilih **"Run as administrator"**
3. Klik **Yes**
4. Tunggu sampai selesai

Setelah itu, aplikasi hanya bisa diakses dari laptop saja.

---

## 📋 Informasi Teknis

**Port yang digunakan:**
- HTTP: `5000`
- HTTPS: `5001`

**Konfigurasi di `appsettings.json`:**
```json
"Kestrel": {
  "Endpoints": {
    "Http": {
      "Url": "http://0.0.0.0:5000"
    },
    "Https": {
      "Url": "https://0.0.0.0:5001"
    }
  }
}
```

**Firewall Rules yang dibuat:**
- Name: `ASP.NET Core HTTP (Port 5000)`
- Direction: Inbound
- Protocol: TCP
- Port: 5000
- Action: Allow

---

## ✅ Checklist

- [ ] Script `allow-firewall.bat` dijalankan sebagai Administrator
- [ ] IP Address laptop sudah dicek dengan `ipconfig`
- [ ] Tablet terhubung ke WiFi yang sama dengan laptop
- [ ] URL `http://[IP]:5000` sudah dicoba di browser tablet
- [ ] Aplikasi `dotnet run` sedang berjalan di laptop

---

## 💡 Tips

1. **Bookmark URL di tablet** supaya tidak perlu ketik ulang
2. **Gunakan QR Code** untuk share URL ke tablet (bisa pakai website QR generator)
3. **Cek firewall** jika setelah restart laptop tidak bisa akses lagi

---

## 📞 Bantuan

Jika masih ada masalah, cek:
1. Apakah aplikasi running? (`dotnet run`)
2. Apakah firewall rule sudah dibuat? (cek di `wf.msc`)
3. Apakah IP sudah benar? (cek dengan `ipconfig`)
4. Apakah WiFi sama? (laptop dan tablet harus di jaringan yang sama)

---

**Dibuat:** 2026-01-12
**Untuk:** Aplikasi QC System (ASP.NET Core)
