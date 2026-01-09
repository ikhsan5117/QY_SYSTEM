# TESTING CHECKLIST - Dashboard Rejection

**Tanggal:** 6 Januari 2026  
**URL:** `https://localhost:5001/Rejection/Dashboard`

---

## ✅ TESTING CHECKLIST

### 1. FILTER SECTION

#### 1.1 Filter Dropdown - Item "SEMUA"
- [ ] **JENIS NG**: Item "SEMUA" muncul di awal list
- [ ] **Kategori NG**: Item "SEMUA" muncul di awal list
- [ ] **LINE**: Item "SEMUA" muncul di awal list
- [ ] **PART CODE**: Item "SEMUA" muncul di awal list
- [ ] Item "SEMUA" ter-select saat tidak ada filter aktif
- [ ] Item "SEMUA" memiliki button "HANYA"

#### 1.2 Check Icon (✓)
- [ ] Klik check icon di header dropdown JENIS NG → item "SEMUA" ter-select
- [ ] Klik check icon di header dropdown Kategori NG → item "SEMUA" ter-select
- [ ] Klik check icon di header dropdown LINE → item "SEMUA" ter-select
- [ ] Klik check icon di header dropdown PART CODE → item "SEMUA" ter-select
- [ ] Form auto submit setelah check icon diklik
- [ ] Semua data ditampilkan setelah select "SEMUA"

#### 1.3 Filter Dropdown - Functionality
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

#### 1.4 Filter Submission
- [ ] Select filter → form auto submit
- [ ] Select "SEMUA" → form submit dengan value kosong
- [ ] Select item tertentu → form submit dengan value yang benar
- [ ] URL parameter ter-update sesuai filter yang dipilih
- [ ] Data di chart/tabel ter-update sesuai filter

---

### 2. CHART - TIME SERIES

#### 2.1 Chart Bulanan (Monthly)
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

#### 2.2 Chart Mingguan (Weekly)
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

#### 2.3 Chart Harian (Daily)
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

#### 2.4 Chart Interaction - Click to Filter
- [ ] Klik bar di chart Bulanan → semua chart/tabel ter-filter sesuai periode
- [ ] Klik bar di chart Mingguan → semua chart/tabel ter-filter sesuai periode
- [ ] Klik bar di chart Harian → semua chart/tabel ter-filter sesuai tanggal
- [ ] URL parameter ter-update setelah klik chart
- [ ] Halaman reload dengan data ter-filter

---

### 3. CHART - PARETO & KRITERIA

#### 3.1 Chart Pareto Part
- [ ] Chart muncul dengan data
- [ ] Bar chart QTY NG tampil
- [ ] X-axis menampilkan Part Code
- [ ] Y-axis menampilkan QTY NG
- [ ] Data terurut dari terbesar ke terkecil
- [ ] Maksimal 10 part code ditampilkan
- [ ] Data ter-update saat filter berubah

#### 3.2 Chart Kriteria NG
- [ ] Chart muncul dengan data (donut chart)
- [ ] Legend tampil di samping (kanan)
- [ ] Legend menggunakan square color boxes
- [ ] Label legend di sebelah warna
- [ ] Tidak ada label di bawah chart
- [ ] Data ter-update saat filter berubah

#### 3.3 Chart Interaction - Click to Filter
- [ ] Klik bar di chart Pareto Part → semua chart/tabel ter-filter sesuai part code
- [ ] Klik segment di chart Kriteria NG → semua chart/tabel ter-filter sesuai jenis NG
- [ ] URL parameter ter-update setelah klik chart
- [ ] Halaman reload dengan data ter-filter

---

### 4. TABEL DATA REJECTION

#### 4.1 Tabel Data Rejection / Part
- [ ] Tabel muncul dengan data
- [ ] Kolom NO (urutan 1, 2, 3, dst) tampil
- [ ] Kolom PART CODE tampil
- [ ] Kolom QTY CHECK tampil (bukan 0)
- [ ] Kolom QTY NG tampil
- [ ] Kolom RR% tampil
- [ ] Data terurut dari terbesar ke terkecil (by QTY NG)
- [ ] Data ter-update saat filter berubah
- [ ] "Tidak ada data" tampil jika tidak ada data

#### 4.2 Tabel Data Rejection / Kriteria NG
- [ ] Tabel muncul dengan data
- [ ] Kolom NO (urutan 1, 2, 3, dst) tampil
- [ ] Kolom JENIS NG tampil
- [ ] Kolom QTY NG tampil
- [ ] Data terurut dari terbesar ke terkecil (by QTY NG)
- [ ] Data ter-update saat filter berubah
- [ ] "Tidak ada data" tampil jika tidak ada data

#### 4.3 Tabel Interaction - Click to Filter
- [ ] Klik row di tabel Data Rejection / Part → semua chart/tabel ter-filter sesuai part code
- [ ] Klik row di tabel Data Rejection / Kriteria NG → semua chart/tabel ter-filter sesuai jenis NG
- [ ] URL parameter ter-update setelah klik row
- [ ] Halaman reload dengan data ter-filter

---

### 5. INTEGRASI FILTER DENGAN CHART/TABEL

#### 5.1 Filter Bulan
- [ ] Select bulan → Chart Bulanan menampilkan data bulan tersebut
- [ ] Select bulan → Chart Mingguan menampilkan data minggu dalam bulan tersebut
- [ ] Select bulan → Chart Harian menampilkan data harian dalam bulan tersebut
- [ ] Select bulan → Chart Pareto Part menampilkan data sesuai bulan
- [ ] Select bulan → Chart Kriteria NG menampilkan data sesuai bulan
- [ ] Select bulan → Tabel menampilkan data sesuai bulan

#### 5.2 Filter Tanggal
- [ ] Select tanggal → Semua chart menampilkan data tanggal tersebut
- [ ] Select tanggal → Semua tabel menampilkan data tanggal tersebut
- [ ] Select multiple tanggal → Data sesuai tanggal yang dipilih

#### 5.3 Filter JENIS NG
- [ ] Select jenis NG → Chart Kriteria NG menampilkan data jenis NG tersebut
- [ ] Select jenis NG → Tabel menampilkan data jenis NG tersebut
- [ ] Select jenis NG → Chart lain menampilkan data sesuai filter

#### 5.4 Filter LINE
- [ ] Select line → Semua chart menampilkan data line tersebut
- [ ] Select line → Semua tabel menampilkan data line tersebut

#### 5.5 Filter PART CODE
- [ ] Select part code → Chart Pareto Part menampilkan data part code tersebut
- [ ] Select part code → Tabel Data Rejection / Part menampilkan data part code tersebut
- [ ] Select part code → Chart lain menampilkan data sesuai filter

#### 5.6 Multiple Filters
- [ ] Select Bulan + JENIS NG → Data sesuai kedua filter
- [ ] Select Bulan + LINE + PART CODE → Data sesuai semua filter
- [ ] Select semua filter → Data sesuai semua filter yang dipilih

#### 5.7 Clear All Filters (Select "SEMUA")
- [ ] Select "SEMUA" di semua dropdown → Semua data ditampilkan
- [ ] Chart menampilkan semua data
- [ ] Tabel menampilkan semua data
- [ ] Tidak ada filter yang aktif

---

### 6. ERROR HANDLING

#### 6.1 JavaScript Errors
- [ ] Tidak ada error di browser console
- [ ] Tidak ada error saat klik dropdown
- [ ] Tidak ada error saat klik chart
- [ ] Tidak ada error saat klik tabel
- [ ] Tidak ada error saat form submit

#### 6.2 Data Errors
- [ ] Tidak ada error saat tidak ada data
- [ ] "Tidak ada data" tampil dengan benar
- [ ] Chart tidak error saat data kosong
- [ ] Tabel tidak error saat data kosong

#### 6.3 UI Errors
- [ ] Dropdown tidak overlap
- [ ] Z-index bekerja dengan benar
- [ ] Tidak ada layout broken
- [ ] Responsive layout bekerja dengan benar

---

## 📝 CATATAN TESTING

### Browser Console
- Buka Developer Tools (F12)
- Cek Console tab untuk error JavaScript
- Cek Network tab untuk error request

### Test Scenarios
1. **Default State**: Buka dashboard tanpa filter → semua data tampil
2. **Single Filter**: Test satu filter pada satu waktu
3. **Multiple Filters**: Test kombinasi beberapa filter
4. **Clear Filters**: Test select "SEMUA" di semua dropdown
5. **Chart Click**: Test klik pada setiap chart
6. **Table Click**: Test klik pada setiap row tabel

---

## ✅ HASIL TESTING

**Status:** [ ] PASSED [ ] FAILED

**Error yang ditemukan:**
1. 
2. 
3. 

**Perbaikan yang dilakukan:**
1. 
2. 
3. 

---

**End of Checklist**

