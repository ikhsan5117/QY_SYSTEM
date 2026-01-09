using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            // Check if already logged in
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            // IsActive tidak disimpan di database SQL Server (hanya properti in-memory),
            // jadi query cukup berdasarkan Username dan Password saja.
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Set session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("NamaLengkap", user.NamaLengkap);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("Plant", user.Plant);
                HttpContext.Session.SetString("Grup", user.Grup);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Username atau password salah";
            return View();
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Auth/FixAdmin - Temporary endpoint to fix/create admin
        public async Task<IActionResult> FixAdmin()
        {
            try
            {
                // Check if Administrator exists
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "Administrator");
                
                if (admin == null)
                {
                    // Create new Administrator
                    admin = new User
                    {
                        Username = "Administrator",
                        Password = "admin123",
                        NamaLengkap = "System Administrator",
                        Role = "Admin",
                        Plant = "All",
                        Grup = "Admin",
                        IsActive = true,
                        TanggalDibuat = DateTime.Now
                    };
                    _context.Users.Add(admin);
                    await _context.SaveChangesAsync();
                    return Content($"✓ Administrator berhasil dibuat!<br/>Username: Administrator<br/>Password: admin123<br/><br/><a href='/Auth/Login'>Klik untuk Login</a>", "text/html");
                }
                else
                {
                    // Update existing Administrator
                    admin.Password = "admin123";
                    admin.Role = "Admin";
                    admin.IsActive = true;
                    admin.Plant = "All";
                    admin.Grup = "Admin";
                    await _context.SaveChangesAsync();
                    return Content($"✓ Administrator berhasil diupdate!<br/>Username: {admin.Username}<br/>Password: admin123<br/>Role: {admin.Role}<br/>IsActive: {admin.IsActive}<br/><br/><a href='/Auth/Login'>Klik untuk Login</a>", "text/html");
                }
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}", "text/html");
            }
        }

        // GET: Auth/VerifyDatabase - Endpoint untuk verifikasi data database
        public async Task<IActionResult> VerifyDatabase()
        {
            try
            {
                var result = new System.Text.StringBuilder();
                result.AppendLine("<h2>📊 Verifikasi Database CheckDimensiDB</h2>");
                result.AppendLine("<style>body{font-family:Arial;padding:20px;} table{border-collapse:collapse;width:100%;margin:10px 0;} th,td{border:1px solid #ddd;padding:8px;text-align:left;} th{background-color:#4CAF50;color:white;}</style>");
                
                // Cek Users
                var totalUsers = await _context.Users.CountAsync();
                var adminUsers = await _context.Users.CountAsync(u => u.Role == "Admin");
                var qualityUsers = await _context.Users.CountAsync(u => u.Role == "Quality" || u.Role == "User");
                
                result.AppendLine("<h3>👥 Tabel Users</h3>");
                result.AppendLine($"<p><strong>Total Users:</strong> {totalUsers}</p>");
                result.AppendLine($"<p><strong>Admin Users:</strong> {adminUsers}</p>");
                result.AppendLine($"<p><strong>Quality/User Users:</strong> {qualityUsers}</p>");
                
                // Cek Administrator
                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "Administrator");
                result.AppendLine("<h4>Administrator:</h4>");
                if (admin != null)
                {
                    result.AppendLine($"<p>✓ Username: {admin.Username}<br/>");
                    result.AppendLine($"Password: {admin.Password}<br/>");
                    result.AppendLine($"Role: {admin.Role}<br/>");
                    result.AppendLine($"IsActive: {admin.IsActive}</p>");
                    
                    // Verifikasi sesuai SQLServer_Setup.sql
                    var expectedPassword = "admin123";
                    var expectedRole = "Admin";
                    if (admin.Password == expectedPassword && admin.Role == expectedRole)
                    {
                        result.AppendLine("<p style='color:green;'>✓ Administrator sesuai dengan SQLServer_Setup.sql</p>");
                    }
                    else
                    {
                        result.AppendLine("<p style='color:red;'>✗ Administrator TIDAK sesuai! Expected: Password='admin123', Role='Admin'</p>");
                    }
                }
                else
                {
                    result.AppendLine("<p style='color:red;'>✗ Administrator tidak ditemukan!</p>");
                }
                
                // List semua users
                var allUsers = await _context.Users.OrderBy(u => u.Plant).ThenBy(u => u.Username).ToListAsync();
                result.AppendLine("<h4>Daftar Semua Users:</h4>");
                result.AppendLine("<table>");
                result.AppendLine("<tr><th>Username</th><th>Nama Lengkap</th><th>Plant</th><th>Grup</th><th>Role</th><th>Password</th></tr>");
                
                foreach (var user in allUsers)
                {
                    result.AppendLine($"<tr><td>{user.Username}</td><td>{user.NamaLengkap}</td><td>{user.Plant}</td><td>{user.Grup}</td><td>{user.Role}</td><td>{user.Password}</td></tr>");
                }
                result.AppendLine("</table>");
                
                // Verifikasi sesuai SQLServer_Setup.sql
                result.AppendLine("<h3>🔍 Verifikasi sesuai SQLServer_Setup.sql:</h3>");
                var expectedUsers = new[]
                {
                    new { Username = "Administrator", Password = "admin123", Role = "Admin" },
                    new { Username = "Hose1A", Password = "password123", Role = "Quality" },
                    new { Username = "Hose1B", Password = "password123", Role = "Quality" },
                    new { Username = "BTR1A", Password = "password123", Role = "Quality" },
                    new { Username = "Leadhose", Password = "password123", Role = "Admin" }
                };
                
                int matchCount = 0;
                foreach (var expected in expectedUsers)
                {
                    var user = allUsers.FirstOrDefault(u => u.Username == expected.Username);
                    if (user != null && user.Password == expected.Password && user.Role == expected.Role)
                    {
                        matchCount++;
                        result.AppendLine($"<p style='color:green;'>✓ {expected.Username}: Password dan Role sesuai</p>");
                    }
                    else if (user != null)
                    {
                        result.AppendLine($"<p style='color:orange;'>⚠ {expected.Username}: Password atau Role berbeda (DB: {user.Password}/{user.Role}, Expected: {expected.Password}/{expected.Role})</p>");
                    }
                    else
                    {
                        result.AppendLine($"<p style='color:red;'>✗ {expected.Username}: Tidak ditemukan di database</p>");
                    }
                }
                
                // Cek tabel lain
                var totalProduk = await _context.Produk.CountAsync();
                var totalStandar = await _context.StandarDimensi.CountAsync();
                var totalInput = await _context.InputAktual.CountAsync();
                
                result.AppendLine("<h3>📦 Tabel Lainnya:</h3>");
                result.AppendLine($"<p><strong>Total Produk:</strong> {totalProduk}</p>");
                result.AppendLine($"<p><strong>Total StandarDimensi:</strong> {totalStandar}</p>");
                result.AppendLine($"<p><strong>Total InputAktual:</strong> {totalInput}</p>");
                
                result.AppendLine("<hr/>");
                result.AppendLine($"<p><strong>Summary:</strong> {matchCount}/{expectedUsers.Length} sample users sesuai dengan SQLServer_Setup.sql</p>");
                result.AppendLine("<p><a href='/Auth/Login'>Kembali ke Login</a></p>");
                
                return Content(result.ToString(), "text/html");
            }
            catch (Exception ex)
            {
                return Content($"<h2>Error</h2><p style='color:red;'>{ex.Message}</p><p>{ex.StackTrace}</p>", "text/html");
            }
        }
    }
}
