# 🧪 Test WebSocket SignalR - INPUT E_LWP ↔ DATA E_LWP

## Tujuan Test
Memverifikasi bahwa WebSocket SignalR berfungsi dengan baik untuk real-time update antara halaman INPUT E_LWP dan DATA E_LWP.

## Komponen yang Diperiksa

### 1. ✅ Backend (Server)
- **QCHub.cs** - SignalR Hub sudah terdaftar
- **Program.cs** - SignalR sudah di-register (`builder.Services.AddSignalR()`)
- **Program.cs** - Hub sudah di-map (`app.MapHub<QCHub>("/qcHub")`)
- **QCHoseController.cs** - Broadcast WebSocket setelah data disimpan

### 2. ✅ Frontend (Client)
- **List.cshtml** - SignalR client library sudah di-load
- **List.cshtml** - Connection sudah di-initialize dan di-start
- **List.cshtml** - Event listener `NewQCDataAdded` sudah terdaftar
- **List.cshtml** - Fungsi `addNewRowToTable` sudah diperbaiki untuk 16 kolom

## Langkah Test

### Test 1: Koneksi WebSocket
1. Buka browser dan navigasi ke halaman **DATA E_LWP** (`/QCHose/List`)
2. Buka **Developer Console** (F12)
3. Periksa console log:
   - Harus muncul: `✅ WebSocket connected! Listening for real-time updates...`
   - Jika error, periksa apakah URL `/qcHub` dapat diakses

### Test 2: Real-time Update
1. Buka **2 tab browser** atau **2 browser berbeda**:
   - Tab 1: **DATA E_LWP** (`/QCHose/List`)
   - Tab 2: **INPUT E_LWP** (`/QCHose/Create`)
2. Di tab **INPUT E_LWP**, isi form dan submit data baru
3. Di tab **DATA E_LWP**, periksa:
   - ✅ Data baru muncul di baris pertama tabel (tanpa refresh)
   - ✅ Baris baru memiliki highlight biru muda yang hilang setelah 2 detik
   - ✅ Toast notification muncul di pojok kanan atas
   - ✅ Total count bertambah
   - ✅ Console log menunjukkan: `📩 New QC Data received via WebSocket:`

### Test 3: Struktur Data
Periksa apakah data yang diterima lengkap:
```javascript
{
  id: number,
  tanggalInput: "dd/MM/yyyy HH:mm",
  lineChecking: string,
  groupChecking: string,
  namaInspector: string,
  partCode: string,
  timeStop: string,
  qtyCheck: string,
  statusChecking: "Checking" | "Done" | "Stop",
  jenisNG: string,
  namaOPR: string,
  qtyNG: string,
  lineStop: string,
  plant: string,
  grup: string
}
```

### Test 4: Reconnection
1. Buka halaman **DATA E_LWP**
2. Matikan koneksi internet (atau stop server)
3. Nyalakan kembali koneksi internet (atau start server)
4. Periksa console log:
   - Harus muncul: `🔄 Reconnecting to WebSocket...`
   - Setelah reconnect: `✅ WebSocket reconnected! ConnectionId: ...`

## Perbaikan yang Sudah Dilakukan

### 1. ✅ Struktur Tabel
- Fungsi `addNewRowToTable` sudah diperbaiki untuk menampilkan 16 kolom sesuai struktur tabel:
  1. Tanggal
  2. Line Checking
  3. Inspector
  4. Group
  5. Part Code
  6. Time Stop
  7. Qty Check
  8. Jenis NG
  9. Nama OPR
  10. Qty NG
  11. Line Stop
  12. Status (dengan dropdown)
  13. Status Time
  14. Plant
  15. Grup
  16. Aksi

### 2. ✅ Data yang Dikirim
- Controller sudah diperbaiki untuk mengirim semua field yang diperlukan:
  - `timeStop`
  - `namaOPR`
  - `plant`
  - `grup`

## Troubleshooting

### Problem: WebSocket tidak terhubung
**Solusi:**
1. Periksa apakah SignalR sudah di-register di `Program.cs`
2. Periksa apakah Hub sudah di-map: `app.MapHub<QCHub>("/qcHub")`
3. Periksa console browser untuk error message
4. Pastikan URL `/qcHub` dapat diakses

### Problem: Data tidak muncul real-time
**Solusi:**
1. Periksa console browser untuk log `📩 New QC Data received via WebSocket:`
2. Periksa apakah `connection.start()` sudah dipanggil
3. Periksa apakah event listener `NewQCDataAdded` sudah terdaftar
4. Periksa server console untuk log `🔊 Broadcasting WebSocket:`

### Problem: Struktur tabel tidak sesuai
**Solusi:**
1. Pastikan fungsi `addNewRowToTable` memiliki 16 kolom
2. Pastikan urutan kolom sesuai dengan struktur tabel di view
3. Pastikan data yang dikirim dari controller lengkap

## Status Test
- [x] Backend SignalR Hub terdaftar
- [x] Frontend SignalR client terhubung
- [x] Event listener terdaftar
- [x] Struktur tabel diperbaiki (16 kolom)
- [x] Data lengkap dikirim dari controller
- [ ] **Test manual perlu dilakukan oleh user**

## Catatan
- WebSocket menggunakan SignalR dengan automatic reconnect
- Data baru akan muncul di baris pertama tabel
- Toast notification akan muncul selama 4 detik
- Highlight biru muda akan hilang setelah 2 detik

