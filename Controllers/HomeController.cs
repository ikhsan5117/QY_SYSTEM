using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.ViewModels;

namespace AplikasiCheckDimensi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Check if user is logged in
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Get current user's name and role
                var userName = HttpContext.Session.GetString("NamaLengkap");
                var userRole = HttpContext.Session.GetString("Role");

                var dashboard = new DashboardViewModel
                {
                    TotalProduk = await _context.Produk.CountAsync(),
                    TotalStandarDimensi = await _context.StandarDimensi.CountAsync()
                };

            // Filter data based on user's name (NamaPIC)
            IQueryable<Models.InputAktual> userInputsQuery = _context.InputAktual
                .Include(i => i.StandarDimensi)
                .ThenInclude(s => s!.Produk);

            // Apply filter only if not admin
            if (userRole != "Admin")
            {
                userInputsQuery = userInputsQuery
                    .Where(i => i.NamaPIC == userName);
            }

            // Total input hari ini - filtered by user
            dashboard.TotalInputHariIni = await userInputsQuery
                .Where(i => i.TanggalInput.Date == DateTime.Today)
                .CountAsync();

            // Total input bulan ini - filtered by user
            dashboard.TotalInputBulanIni = await userInputsQuery
                .Where(i => i.TanggalInput.Month == DateTime.Now.Month && 
                           i.TanggalInput.Year == DateTime.Now.Year)
                .CountAsync();

            // Get recent inputs - filtered by user
            dashboard.RecentInputs = await userInputsQuery
                .OrderByDescending(i => i.TanggalInput)
                .Take(5)
                .ToListAsync();

            // Calculate OK/NG ratio for this month - filtered by user
            var allInputs = await userInputsQuery
                .Where(i => i.TanggalInput.Month == DateTime.Now.Month && 
                           i.TanggalInput.Year == DateTime.Now.Year)
                .ToListAsync();

            // Count OK/NG based on Status column or IsAllOK property
            foreach (var input in allInputs)
            {
                // Use Status column if available, otherwise use IsAllOK property
                bool isOK = !string.IsNullOrEmpty(input.Status) 
                    ? input.Status == "OK" 
                    : input.IsAllOK;
                
                if (isOK)
                    dashboard.TotalOK++;
                else
                    dashboard.TotalNG++;
            }

            // Get products with SOP (products that have standards)
            dashboard.ProdukWithSOP = await _context.Produk
                .Where(p => _context.StandarDimensi.Any(s => s.ProdukId == p.Id))
                .Take(10)
                .ToListAsync();

            // Get recent standard dimensions
            dashboard.RecentStandarDimensi = await _context.StandarDimensi
                .Include(s => s.Produk)
                .OrderByDescending(s => s.Id)
                .Take(5)
                .ToListAsync();

            return View(dashboard);
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                // SQL Server specific errors
                Console.WriteLine($"SQL Error in Home/Index: {sqlEx.Message}");
                Console.WriteLine($"Error Number: {sqlEx.Number}");
                Console.WriteLine($"StackTrace: {sqlEx.StackTrace}");
                
                ViewBag.Error = $"Database Error: {sqlEx.Message}";
                ViewBag.StackTrace = sqlEx.StackTrace;
                ViewBag.ErrorNumber = sqlEx.Number;
                return View("Error");
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error in Home/Index: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // Return error page or redirect
                ViewBag.Error = $"Terjadi error: {ex.Message}";
                ViewBag.StackTrace = ex.StackTrace;
                return View("Error");
            }
        }
        
        // GET: Home/TestConnection - Debug endpoint untuk test database connection
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var result = new System.Text.StringBuilder();
                result.AppendLine("<h2>🔍 Test Database Connection</h2>");
                result.AppendLine("<style>body{font-family:Arial;padding:20px;} .success{color:green;} .error{color:red;}</style>");
                
                // Test connection
                result.AppendLine("<h3>1. Testing Connection...</h3>");
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    result.AppendLine("<p class='success'>✓ Database connection successful!</p>");
                }
                else
                {
                    result.AppendLine("<p class='error'>✗ Cannot connect to database!</p>");
                    return Content(result.ToString(), "text/html");
                }
                
                // Test Users table
                result.AppendLine("<h3>2. Testing Users Table...</h3>");
                try
                {
                    var userCount = await _context.Users.CountAsync();
                    result.AppendLine($"<p class='success'>✓ Users table accessible. Total: {userCount}</p>");
                    
                    var admin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "Administrator");
                    if (admin != null)
                    {
                        result.AppendLine($"<p class='success'>✓ Administrator found: {admin.Username}, Role: {admin.Role}</p>");
                    }
                }
                catch (Exception ex)
                {
                    result.AppendLine($"<p class='error'>✗ Error accessing Users: {ex.Message}</p>");
                }
                
                // Test Produk table
                result.AppendLine("<h3>3. Testing Produk Table...</h3>");
                try
                {
                    var produkCount = await _context.Produk.CountAsync();
                    result.AppendLine($"<p class='success'>✓ Produk table accessible. Total: {produkCount}</p>");
                }
                catch (Exception ex)
                {
                    result.AppendLine($"<p class='error'>✗ Error accessing Produk: {ex.Message}</p>");
                }
                
                // Test StandarDimensi table
                result.AppendLine("<h3>4. Testing StandarDimensi Table...</h3>");
                try
                {
                    var standarCount = await _context.StandarDimensi.CountAsync();
                    result.AppendLine($"<p class='success'>✓ StandarDimensi table accessible. Total: {standarCount}</p>");
                    
                    // Test Include
                    var standarWithProduk = await _context.StandarDimensi
                        .Include(s => s.Produk)
                        .Take(1)
                        .ToListAsync();
                    result.AppendLine($"<p class='success'>✓ Include Produk works. Sample: {standarWithProduk.Count}</p>");
                }
                catch (Exception ex)
                {
                    result.AppendLine($"<p class='error'>✗ Error accessing StandarDimensi: {ex.Message}</p>");
                    result.AppendLine($"<pre>{ex.StackTrace}</pre>");
                }
                
                // Test InputAktual table
                result.AppendLine("<h3>5. Testing InputAktual Table...</h3>");
                try
                {
                    var inputCount = await _context.InputAktual.CountAsync();
                    result.AppendLine($"<p class='success'>✓ InputAktual table accessible. Total: {inputCount}</p>");
                    
                    // Test Include with ThenInclude
                    var inputWithStandar = await _context.InputAktual
                        .Include(i => i.StandarDimensi)
                        .ThenInclude(s => s!.Produk)
                        .Take(1)
                        .ToListAsync();
                    result.AppendLine($"<p class='success'>✓ Include with ThenInclude works. Sample: {inputWithStandar.Count}</p>");
                }
                catch (Exception ex)
                {
                    result.AppendLine($"<p class='error'>✗ Error accessing InputAktual: {ex.Message}</p>");
                    result.AppendLine($"<pre>{ex.StackTrace}</pre>");
                }
                
                result.AppendLine("<hr/>");
                result.AppendLine("<p><a href='/Home/Index'>Coba Akses Dashboard</a> | <a href='/Auth/Login'>Kembali ke Login</a></p>");
                
                return Content(result.ToString(), "text/html");
            }
            catch (Exception ex)
            {
                return Content($"<h2>Error</h2><p style='color:red;'>{ex.Message}</p><pre>{ex.StackTrace}</pre>", "text/html");
            }
        }
    }
}
