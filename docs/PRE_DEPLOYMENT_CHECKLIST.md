# Pre-Deployment Checklist - Aplikasi Check Dimensi

## ✅ Verifikasi Kode

### 1. Error & Warning Check
- [x] No compilation errors
- [x] No runtime errors
- [x] All dependencies resolved

### 2. Security Check
- [x] Session authentication implemented
- [x] Role-based access control (Admin/Quality)
- [x] SQL injection prevention (Entity Framework)
- [x] XSS prevention (proper encoding)
- ⚠️ **TODO**: Change default passwords in production
- ⚠️ **TODO**: Implement password hashing (currently plain text)

### 3. Database Check
- [x] All tables properly defined
- [x] Foreign keys configured
- [x] Indexes created for performance
- [x] Data seeding working (45 users total)

### 4. Feature Verification

#### Authentication
- [x] Login page working
- [x] Session management working
- [x] Logout working
- [x] Session timeout (8 hours)

#### User Management (Admin Only)
- [x] List users
- [x] Create user
- [x] Edit user
- [x] Delete user
- [x] Seed users (44 quality + 1 admin)

#### Product Management
- [x] List products
- [x] Create product (Admin only)
- [x] Edit product (Admin only)
- [x] Delete product (Admin only)
- [x] Upload Excel (Admin only)
- [x] Download template (Admin only)
- [x] Add standar dimensi
- [x] Edit standar dimensi
- [x] Delete standar dimensi
- [x] Upload gambar produk
- [x] Upload gambar checking fixture

#### Input Dimensi
- [x] Search and select product
- [x] Input measurements (ID, OD, Thickness, etc)
- [x] Multiple sampling (up to defined qty)
- [x] Real-time OK/NG validation
- [x] Progress tracking
- [x] Save to database
- [x] Auto-fill Plant, Grup, PIC from session

#### Riwayat (History)
- [x] List all measurements
- [x] Filter by user (NamaPIC)
- [x] Filter by status (OK/NG)
- [x] Search by product
- [x] View detail
- [x] Delete measurement
- [x] Edit removed (as requested)

#### Dashboard
- [x] Total produk (global)
- [x] Total standar dimensi (global)
- [x] Input hari ini (filtered by user)
- [x] Input bulan ini (filtered by user)
- [x] OK/NG ratio (filtered by user)
- [x] Recent inputs (filtered by user)
- [x] Standar dimensi list
- [x] SOP Checking list
- [x] "Tambah Standar" button (Admin only)

#### SOP Checking
- [x] List products with SOP
- [x] View checking fixture image
- [x] Management page (Admin)
- [x] Upload SOP images

### 5. UI/UX Check
- [x] Toast notifications working
- [x] Fallback to alert() if toast not loaded
- [x] Mobile responsive layout
- [x] Desktop layout
- [x] Loading indicators
- [x] Form validation
- [x] Error messages
- [x] Success messages

### 6. Known Issues (Fixed)
- ✅ Toast "not defined" error - FIXED with typeof checks
- ✅ Variable scope in Input.cshtml - FIXED (const to let)
- ✅ Dashboard showing all data - FIXED (filter by NamaPIC)
- ✅ Riwayat showing all data - FIXED (filter by NamaPIC)
- ✅ Edit button in Riwayat - REMOVED as requested
- ✅ Tambah Produk for non-admin - HIDDEN as requested

## 🔧 Configuration Files Ready

### Files Created
- [x] DEPLOYMENT_GUIDE.md - Complete deployment instructions
- [x] SQLServer_Setup.sql - SQL Server database setup script
- [x] appsettings.json - Current configuration (SQLite)

### Files Need Update for Production
- [ ] appsettings.Production.json - Create with SQL Server connection
- [ ] Program.cs - Change UseSqlite to UseSqlServer
- [ ] AplikasiCheckDimensi.csproj - Add SQL Server package

## 📦 Deployment Requirements

### Server Requirements
- Windows Server 2016 or later / Windows 10/11
- .NET 8.0 Runtime
- SQL Server 2019/2022 (Express or higher)
- 2GB RAM minimum
- 1GB disk space minimum

### Network Requirements
- Port 5000 open (or configure custom port)
- SQL Server port 1433 accessible
- Firewall configured

### Permissions Required
- Write permission to wwwroot/uploads folder
- SQL Server database creation permission
- Windows Service registration (if using service)

## 🚀 Deployment Steps

1. [ ] Install .NET 8.0 Runtime on target server
2. [ ] Install SQL Server on target server
3. [ ] Run SQLServer_Setup.sql to create database and tables
4. [ ] Update AplikasiCheckDimensi.csproj - add SQL Server package
5. [ ] Update Program.cs - change to UseSqlServer
6. [ ] Create appsettings.Production.json with correct connection string
7. [ ] Run: `dotnet publish -c Release -o ./publish`
8. [ ] Copy publish folder to target server
9. [ ] Create wwwroot/uploads folder with write permission
10. [ ] Test connection to SQL Server
11. [ ] Run application: `dotnet AplikasiCheckDimensi.dll`
12. [ ] Test login with Administrator account
13. [ ] Verify all features working
14. [ ] Change default passwords
15. [ ] Setup as Windows Service (optional)

## 🔐 Security Recommendations

### Before Production
- [ ] Implement password hashing (bcrypt/PBKDF2)
- [ ] Change all default passwords
- [ ] Enable HTTPS
- [ ] Configure SQL Server firewall
- [ ] Use Windows Authentication for SQL Server (more secure)
- [ ] Implement rate limiting for login
- [ ] Add CSRF token validation
- [ ] Enable audit logging
- [ ] Setup regular database backups

### Password Hashing Implementation Needed
**Current**: Passwords stored in plain text (password123)
**Recommendation**: Use ASP.NET Core Identity or BCrypt.Net

Example:
```csharp
// Install package: BCrypt.Net-Next
var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
var isValid = BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
```

## 📊 Performance Optimization

- [x] Database indexes created
- [x] Entity Framework lazy loading disabled
- [x] Query filtering at database level
- [ ] TODO: Add caching for dashboard metrics
- [ ] TODO: Optimize large Excel uploads
- [ ] TODO: Add pagination for large datasets

## 🐛 Testing Checklist

### Test Scenarios
- [ ] Login dengan user Admin
- [ ] Login dengan user Quality
- [ ] Input dimensi dan verify data saved
- [ ] Dashboard menampilkan data sesuai user
- [ ] Riwayat menampilkan data sesuai user
- [ ] Admin dapat tambah produk
- [ ] Quality tidak dapat tambah produk
- [ ] Upload Excel (Admin)
- [ ] Upload gambar produk
- [ ] Delete measurement
- [ ] Logout dan verify session cleared
- [ ] Multiple concurrent users
- [ ] Network disconnection handling

## 📝 Documentation

### Files Included
- [x] README.md - General information
- [x] DEPLOYMENT_GUIDE.md - Deployment instructions
- [x] SQLServer_Setup.sql - Database setup script
- [x] FORM_INPUT_UPDATE.md - Form updates documentation
- [x] This checklist file

### Missing Documentation
- [ ] User manual (end-user guide)
- [ ] Admin manual (administrator guide)
- [ ] API documentation (if exposing APIs)
- [ ] Troubleshooting guide

## ⚠️ Critical Issues to Address Before Production

### HIGH PRIORITY
1. **Password Security**: Passwords are stored in plain text
   - Impact: High security risk
   - Solution: Implement password hashing immediately

2. **HTTPS**: Currently HTTP only
   - Impact: Data transmitted in clear text
   - Solution: Configure SSL certificate and enable HTTPS

3. **Input Validation**: Limited validation on forms
   - Impact: Potential data corruption
   - Solution: Add comprehensive server-side validation

### MEDIUM PRIORITY
1. **Error Handling**: Generic error messages
   - Impact: Poor user experience on errors
   - Solution: Improve error messages and logging

2. **Database Backups**: No automatic backup configured
   - Impact: Risk of data loss
   - Solution: Setup SQL Server backup jobs

3. **Session Security**: No session encryption
   - Impact: Session hijacking possible
   - Solution: Enable session encryption

### LOW PRIORITY
1. **UI Polish**: Some inconsistencies in mobile view
2. **Loading States**: Could be improved in some areas
3. **Help System**: No inline help or tooltips

## ✅ Sign-off

Reviewed by: _________________
Date: _________________
Approved for deployment: ☐ Yes ☐ No (with conditions)

Conditions/Notes:
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________
