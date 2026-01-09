# Panduan Deployment Aplikasi Check Dimensi

## Persiapan SQL Server

### 1. Install SQL Server
- Download dan install SQL Server 2019/2022 Express atau versi lainnya
- Install SQL Server Management Studio (SSMS)

### 2. Buat Database
```sql
CREATE DATABASE CheckDimensiDB;
GO
```

## Langkah Deployment

### 1. Update appsettings.json
Ganti connection string dari SQLite ke SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CheckDimensiDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Atau jika menggunakan SQL Server Authentication:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CheckDimensiDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

### 2. Update AplikasiCheckDimensi.csproj
Tambahkan package SQL Server:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
```

Jalankan command:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### 3. Update Program.cs
Ganti UseSqlite dengan UseSqlServer:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 4. Migrasi Database
```bash
# Hapus migrations lama (opsional jika ingin fresh)
Remove-Item -Recurse -Force Migrations

# Buat migration baru untuk SQL Server
dotnet ef migrations add InitialSQLServer

# Apply migration ke database
dotnet ef database update
```

### 5. Publish Aplikasi
```bash
# Publish untuk production
dotnet publish -c Release -o ./publish

# Copy folder publish ke server tujuan
```

### 6. Setup Windows Service (Opsional)
Install aplikasi sebagai Windows Service menggunakan NSSM atau sc.exe

## Konfigurasi Production

### appsettings.Production.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=PRODUCTION_SERVER;Database=CheckDimensiDB;User Id=sa;Password=SecurePassword;TrustServerCertificate=True;"
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      }
    }
  }
}
```

## Checklist Deployment

- [ ] SQL Server terinstall dan berjalan
- [ ] Database CheckDimensiDB sudah dibuat
- [ ] Connection string sudah diupdate
- [ ] Package SQL Server sudah terinstall
- [ ] Migration berhasil dijalankan
- [ ] User Administrator default sudah dibuat
- [ ] 42 user Quality dan Leader sudah di-seed
- [ ] Folder wwwroot/uploads sudah dibuat dan memiliki write permission
- [ ] Port 5000 terbuka di firewall
- [ ] Test login dengan user Administrator

## Troubleshooting

### Error: Cannot connect to SQL Server
- Pastikan SQL Server service berjalan
- Cek firewall settings
- Pastikan SQL Server Authentication enabled (jika pakai user/password)

### Error: Login failed for user
- Cek username dan password di connection string
- Pastikan user memiliki akses ke database
- Enable SQL Server and Windows Authentication mode

### Error: Cannot create database
- Pastikan user memiliki CREATE DATABASE permission
- Jalankan script CREATE DATABASE manual di SSMS

## Default Credentials

**Administrator:**
- Username: `Administrator`
- Password: `admin123`

**Quality Users (contoh):**
- Username: `Hose1A` (Rico)
- Password: `password123`

⚠️ **PENTING**: Ganti password default setelah deployment pertama!

## Network Access

Jika ingin diakses dari komputer lain:
1. Pastikan firewall port 5000 terbuka
2. Akses dari browser: `http://IP_SERVER:5000`
3. Untuk production, gunakan reverse proxy (IIS/Nginx)

## Backup Database

```sql
-- Backup database
BACKUP DATABASE CheckDimensiDB 
TO DISK = 'D:\Backup\CheckDimensiDB.bak'
WITH FORMAT, COMPRESSION;

-- Restore database
RESTORE DATABASE CheckDimensiDB 
FROM DISK = 'D:\Backup\CheckDimensiDB.bak'
WITH REPLACE;
```
