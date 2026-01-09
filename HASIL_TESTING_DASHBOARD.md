# HASIL TESTING DASHBOARD REJECTION

**Tanggal:** 6 Januari 2026  
**URL:** `https://localhost:5001/Rejection/Dashboard`  
**Browser:** Cursor Browser  
**Status:** Testing Manual

---

## ✅ TESTING CHECKLIST

### 1. AKSES DASHBOARD
- [x] Login berhasil dengan username: Administrator, password: admin123
- [x] Navigate ke Dashboard Rejection berhasil
- [x] Halaman dashboard terbuka tanpa error
- [x] Tidak ada error di browser console

### 2. FILTER SECTION

#### 2.1 Filter Dropdown - Item "SEMUA"
- [ ] **JENIS NG**: Item "SEMUA" muncul di awal list
- [ ] **Kategori NG**: Item "SEMUA" muncul di awal list
- [ ] **LINE**: Item "SEMUA" muncul di awal list
- [ ] **PART CODE**: Item "SEMUA" muncul di awal list
- [ ] Item "SEMUA" ter-select saat tidak ada filter aktif
- [ ] Item "SEMUA" memiliki button "HANYA"

#### 2.2 Check Icon (✓)
- [ ] Klik check icon di header dropdown JENIS NG → item "SEMUA" ter-select
- [ ] Klik check icon di header dropdown Kategori NG → item "SEMUA" ter-select
- [ ] Klik check icon di header dropdown LINE → item "SEMUA" ter-select
- [ ] Klik check icon di header dropdown PART CODE → item "SEMUA" ter-select
- [ ] Form auto submit setelah check icon diklik
- [ ] Semua data ditampilkan setelah select "SEMUA"

#### 2.3 Filter Dropdown - Functionality
- [ ] **Bulan**: Dropdown terbuka, item bulan muncul, bisa select bulan
- [ ] **Tanggal**: Dropdown terbuka, checkbox tanggal muncul, "Pilih Semua" berfungsi
- [ ] **JENIS NG**: Dropdown terbuka, list jenis NG muncul, bisa select
- [ ] **Kategori NG**: Dropdown terbuka, list kategori NG muncul, bisa select
- [ ] **LINE**: Dropdown terbuka, list line muncul, bisa select
- [ ] **PART CODE**: Dropdown terbuka, list part code muncul, bisa select
- [ ] Search box di setiap dropdown berfungsi (filter items)
- [ ] Button "HANYA" di setiap item berfungsi
- [ ] Dropdown tertutup saat klik di luar
- [ ] Hanya satu dropdown terbuka pada satu waktu

### 3. CHART - TIME SERIES

#### 3.1 Chart Bulanan (Monthly)
- [ ] Chart muncul dengan data
- [ ] Bar chart QTY CHECK (biru) tampil
- [ ] Bar chart QTY NG (oranye) tampil
- [ ] Line chart RR% (hitam) tampil
- [ ] Line chart Target RR (merah putus-putus) tampil
- [ ] X-axis menampilkan nama bulan (MMM yyyy)
- [ ] Y-axis kiri menampilkan QTY
- [ ] Y-axis kanan menampilkan RR%
- [ ] Legend tampil dengan benar
- [ ] Data ter-update saat filter berubah

#### 3.2 Chart Mingguan (Weekly)
- [ ] Chart muncul dengan data
- [ ] Bar chart QTY CHECK (biru) tampil
- [ ] Bar chart QTY NG (oranye) tampil
- [ ] Line chart RR% (hitam) tampil
- [ ] Line chart Target RR (merah putus-putus) tampil
- [ ] X-axis menampilkan "Minggu 1", "Minggu 2", dst
- [ ] Y-axis kiri menampilkan QTY
- [ ] Y-axis kanan menampilkan RR%
- [ ] Legend tampil dengan benar
- [ ] Data ter-update saat filter berubah

#### 3.3 Chart Harian (Daily)
- [ ] Chart muncul dengan data
- [ ] Bar chart QTY CHECK (biru) tampil
- [ ] Bar chart QTY NG (oranye) tampil
- [ ] Line chart RR% (hitam) tampil
- [ ] Line chart Target RR (merah putus-putus) tampil
- [ ] X-axis menampilkan tanggal 1-31 (atau sesuai hari dalam bulan)
- [ ] Y-axis kiri menampilkan QTY
- [ ] Y-axis kanan menampilkan RR%
- [ ] Legend tampil dengan benar
- [ ] Data ter-update saat filter berubah
- [ ] Data harian sesuai dengan bulan yang dipilih di filter

### 4. CHART - PARETO & KRITERIA

#### 4.1 Chart Pareto Part
- [ ] Chart muncul dengan data
- [ ] Bar chart QTY NG tampil
- [ ] X-axis menampilkan Part Code
- [ ] Y-axis menampilkan QTY NG
- [ ] Data terurut dari terbesar ke terkecil
- [ ] Maksimal 10 part code ditampilkan
- [ ] Data ter-update saat filter berubah

#### 4.2 Chart Kriteria NG
- [ ] Chart muncul dengan data (donut chart)
- [ ] Legend tampil di samping (kanan)
- [ ] Legend menggunakan square color boxes
- [ ] Label legend di sebelah warna
- [ ] Tidak ada label di bawah chart
- [ ] Data ter-update saat filter berubah

### 5. TABEL DATA REJECTION

#### 5.1 Tabel Data Rejection / Part
- [ ] Tabel muncul dengan data
- [ ] Kolom NO (urutan 1, 2, 3, dst) tampil
- [ ] Kolom PART CODE tampil
- [ ] Kolom QTY CHECK tampil (bukan 0)
- [ ] Kolom QTY NG tampil
- [ ] Kolom RR% tampil
- [ ] Data terurut dari terbesar ke terkecil (by QTY NG)
- [ ] Data ter-update saat filter berubah
- [ ] "Tidak ada data" tampil jika tidak ada data

#### 5.2 Tabel Data Rejection / Kriteria NG
- [ ] Tabel muncul dengan data
- [ ] Kolom NO (urutan 1, 2, 3, dst) tampil
- [ ] Kolom JENIS NG tampil
- [ ] Kolom QTY NG tampil
- [ ] Data terurut dari terbesar ke terkecil (by QTY NG)
- [ ] Data ter-update saat filter berubah
- [ ] "Tidak ada data" tampil jika tidak ada data

### 6. INTERAKSI KLIK CHART/TABEL

#### 6.1 Chart Click to Filter
- [ ] Klik bar di chart Bulanan → semua chart/tabel ter-filter sesuai periode
- [ ] Klik bar di chart Mingguan → semua chart/tabel ter-filter sesuai periode
- [ ] Klik bar di chart Harian → semua chart/tabel ter-filter sesuai tanggal
- [ ] Klik bar di chart Pareto Part → semua chart/tabel ter-filter sesuai part code
- [ ] Klik segment di chart Kriteria NG → semua chart/tabel ter-filter sesuai jenis NG
- [ ] URL parameter ter-update setelah klik chart
- [ ] Halaman reload dengan data ter-filter

#### 6.2 Table Click to Filter
- [ ] Klik row di tabel Data Rejection / Part → semua chart/tabel ter-filter sesuai part code
- [ ] Klik row di tabel Data Rejection / Kriteria NG → semua chart/tabel ter-filter sesuai jenis NG
- [ ] URL parameter ter-update setelah klik row
- [ ] Halaman reload dengan data ter-filter

---

## 📝 CATATAN TESTING

### Browser Console
- Tidak ada error JavaScript di console
- Semua resource load dengan benar

### Screenshot
- Screenshot tersimpan di: `dashboard-rejection-initial.png`

---

## ⚠️ ERROR YANG DITEMUKAN

1. 
2. 
3. 

---

## 🔧 PERBAIKAN YANG DILAKUKAN

1. 
2. 
3. 

---

**Status Testing:** [ ] ONGOING [ ] COMPLETED

**End of Report**

