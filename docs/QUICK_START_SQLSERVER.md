# Quick Start Deployment ke SQL Server

## Cara Cepat (Recommended)

### 1. Jalankan Script Otomatis
```bash
# Double-click atau run:
setup-sqlserver.bat
```

Script akan otomatis:
- Install package SQL Server
- Restore dependencies
- Build project
- Memberi panduan update Program.cs
- Create migrations baru

### 2. Update Program.cs (PENTING!)
Buka `Program.cs` dan ganti baris ini:

**SEBELUM:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**SESUDAH:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 3. Setup Database SQL Server

**Opsi A: Menggunakan Script SQL (Recommended)**
1. Buka SQL Server Management Studio (SSMS)
2. Connect ke SQL Server
3. Open file: `SQLServer_Setup.sql`
4. Execute script (F5)
5. Verify: 45 users ter-create

**Opsi B: Menggunakan Entity Framework**
```bash
dotnet ef database update
```

### 4. Test Aplikasi
```bash
# Test dengan production settings
dotnet run --environment Production

# Atau test dengan development
dotnet run
```

Buka browser: http://localhost:5000
Login dengan:
- Username: `Administrator`
- Password: `admin123`

### 5. Publish ke Production
```bash
dotnet publish -c Release -o ./publish
```

Folder `publish` siap di-copy ke server production.

## Troubleshooting Cepat

### Error: Cannot connect to SQL Server
```bash
# Test connection SQL Server
sqlcmd -S localhost -E

# Jika error, pastikan:
# 1. SQL Server service running
# 2. SQL Server Browser running
# 3. TCP/IP protocol enabled
```

### Error: Login failed
Update connection string di `appsettings.Production.json`:
```json
"DefaultConnection": "Server=localhost;Database=CheckDimensiDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

### Error: Migration failed
```bash
# Hapus migrations lama
Remove-Item -Recurse -Force Migrations

# Buat baru
dotnet ef migrations add InitialSQLServer

# Apply
dotnet ef database update
```

## Struktur Files Deployment

```
📁 Project Root/
├── 📁 docs/                        # Dokumentasi
│   ├── 📄 DEPLOYMENT_GUIDE.md          # Panduan lengkap deployment
│   ├── 📄 PRE_DEPLOYMENT_CHECKLIST.md  # Checklist sebelum deploy
│   ├── 📄 QUICK_START_SQLSERVER.md     # File ini (quick guide)
│   └── 📄 ... lainnya
├── 📁 sql/                         # Script SQL
│   └── 📄 SQLServer_Setup.sql          # Script setup database SQL Server
├── 📄 setup-sqlserver.bat          # Script otomatis setup
├── 📄 appsettings.json             # Config development (SQLite)
├── 📄 appsettings.Production.json  # Config production (SQL Server)
└── 📁 publish/                     # Hasil publish (setelah dotnet publish)
```

## Quick Commands

```bash
# 1. Setup SQL Server
setup-sqlserver.bat

# 2. Update Program.cs (manual edit)

# 3. Test
dotnet run --environment Production

# 4. Publish
dotnet publish -c Release -o ./publish

# 5. Copy ke server
xcopy /E /I publish "\\SERVER\path\to\app"

# 6. Run di server
cd path\to\app
dotnet AplikasiCheckDimensi.dll
```

## Default Credentials

**Admin:**
- Username: `Administrator`
- Password: `admin123`

**Quality (contoh):**
- Username: `Hose1A`
- Password: `password123`

⚠️ **Segera ganti password setelah deployment pertama!**

## Support Files

| File / Folder | Deskripsi |
|---------------|-----------|
| `docs/DEPLOYMENT_GUIDE.md` | Panduan deployment lengkap dengan troubleshooting |
| `sql/SQLServer_Setup.sql` | Script SQL untuk create database, tables, dan insert users |
| `docs/PRE_DEPLOYMENT_CHECKLIST.md` | Checklist verifikasi sebelum production |
| `appsettings.Production.json` | Configuration file untuk production |
| `setup-sqlserver.bat` | Automated setup script |

## Kontak / Support

Jika ada masalah saat deployment, check:
1. `PRE_DEPLOYMENT_CHECKLIST.md` - Known issues
2. `DEPLOYMENT_GUIDE.md` - Troubleshooting section
3. Logs di folder: `logs/` atau console output
