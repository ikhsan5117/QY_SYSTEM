# Aplikasi Check Dimensi

Aplikasi web untuk checking dimensi produk dengan tampilan mobile-first responsive menggunakan ASP.NET Core MVC.

## Fitur Utama

- ✅ **Management Produk & Standar Dimensi (CRUD)**
  - Tambah, edit, hapus produk
  - Set standar dimensi per produk (A, B, Sudut)
  - View detail produk dengan semua standar dimensi
- ✅ Input data dimensi produk
- ✅ Validasi otomatis terhadap standar & toleransi
- ✅ Status OK/NG real-time
- ✅ Riwayat data pengukuran
- ✅ Detail laporan per item
- ✅ Responsive design untuk mobile & tablet
- ✅ Dark theme UI
- ✅ **Network Access** - Bisa diakses dari HP/Tablet dalam 1 jaringan

## Teknologi

- ASP.NET Core 8.0 MVC
- Entity Framework Core
- SQLite Database
- Bootstrap 5
- Bootstrap Icons

## Cara Menjalankan

### 1. Restore Dependencies
```bash
dotnet restore
```

### 2. Buat Database
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Jalankan Aplikasi
```bash
dotnet run
```

### 4. Buka Browser
```
https://localhost:5001
atau
http://localhost:5000
```

## Struktur Project

```
AplikasiCheckDimensi/
├── Controllers/
│   └── DimensiController.cs       # Main controller
├── Data/
│   └── ApplicationDbContext.cs    # Database context
├── Models/
│   ├── Produk.cs                  # Model produk
│   ├── StandarDimensi.cs          # Model standar dimensi
│   └── InputAktual.cs             # Model input pengukuran
├── ViewModels/
│   └── InputDimensiViewModel.cs   # ViewModel untuk input
├── Views/
│   ├── Dimensi/
│   │   ├── Input.cshtml           # Halaman input data
│   │   ├── Riwayat.cshtml         # Halaman riwayat
│   │   └── Detail.cshtml          # Halaman detail
│   └── Shared/
│       └── _Layout.cshtml          # Layout utama
├── wwwroot/
│   ├── css/
│   │   └── site.css               # Custom CSS
│   └── js/
│       └── site.js                # Custom JavaScript
├── Program.cs                      # Startup configuration
└── appsettings.json               # Configuration

```

## Fitur Halaman

### 1. Management Produk (`/Produk/Index`)
- Daftar semua produk
- Search & filter produk
- Tambah produk baru
- View detail, edit, hapus produk

### 2. Detail Produk (`/Produk/Detail/{id}`)
- Info lengkap produk
- Daftar standar dimensi
- Tambah/edit/hapus standar dimensi
- Link langsung ke input data

### 3. Input Data Dimensi (`/Dimensi/Input`)
- Pilih produk
- Form input nilai dimensi aktual
- Tampilan standar & toleransi
- Validasi real-time OK/NG
- Submit ke database

### 4. Riwayat Data (`/Dimensi/Riwayat`)
- List semua input data
- Filter & search
- Status summary per item
- Link ke detail

### 5. Detail Input (`/Dimensi/Detail/{id}`)
- Ringkasan lengkap dimensi
- Status OK/NG per parameter
- Catatan operator
- Export PDF (coming soon)

## Database Schema

### Produk
- Id (PK)
- NamaProduk
- Operator
- TanggalInput

### StandarDimensi
- Id (PK)
- ProdukId (FK)
- NamaDimensi
- DimensiA_Min, DimensiA_Max
- DimensiB_Min, DimensiB_Max
- Sudut_Min, Sudut_Max

### InputAktual
- Id (PK)
- StandarDimensiId (FK)
- NilaiDimensiA
- NilaiDimensiB
- NilaiSudut
- TanggalInput
- CatatanOperator

## Customization

### Mengubah Tema Warna
Edit `wwwroot/css/site.css` pada bagian `:root` variables:
```css
:root {
    --bg-primary: #1a1a1a;
    --bg-secondary: #2a2a2a;
    --color-success: #4ade80;
    --color-danger: #ef4444;
}
```

### Menambah Parameter Dimensi
1. Update model `StandarDimensi.cs`
2. Update view input form
3. Update validasi di JavaScript
4. Buat migration baru

## Browser Support

- ✅ Chrome/Edge (Recommended)
- ✅ Firefox
- ✅ Safari
- ✅ Mobile browsers (iOS Safari, Chrome Mobile)

## Development

### Menambah Migration
```bash
dotnet ef migrations add NamaMigration
```

### Update Database
```bash
dotnet ef database update
```

### Build Production
```bash
dotnet publish -c Release -o ./publish
```

## License

MIT License

## Author

Developed for Quality Control Dimensi Checking Application
