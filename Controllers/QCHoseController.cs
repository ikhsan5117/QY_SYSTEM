using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;
using Microsoft.AspNetCore.SignalR;
using AplikasiCheckDimensi.Hubs;

namespace AplikasiCheckDimensi.Controllers
{
    public class QCHoseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<QCHub> _hubContext;

        public QCHoseController(ApplicationDbContext context, IHubContext<QCHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Hanya user yang sudah login yang boleh akses
        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // User dengan role Admin, Quality, atau User boleh akses E_LWP
        private bool IsAuthorized()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin" || role == "Quality" || role == "User";
        }

        // Helper method untuk load dropdown data dari MasterData
        private async Task LoadDropdownDataAsync()
        {
            var lineChecking = await _context.MasterData
                .Where(m => m.Tipe == "LineChecking" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            var inspectors = await _context.MasterData
                .Where(m => m.Tipe == "Inspector" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            // Jika belum ada di MasterData, ambil dari Users
            if (!inspectors.Any())
            {
                inspectors = await _context.Users
                    .Where(u => u.Role == "Quality" || u.Role == "Admin")
                    .Select(u => u.NamaLengkap)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToListAsync();
            }

            var groupChecking = await _context.MasterData
                .Where(m => m.Tipe == "GroupChecking" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            // Default jika belum ada
            if (!groupChecking.Any())
            {
                groupChecking = new List<string> { "A", "B" };
            }

            var jenisNG = await _context.MasterData
                .Where(m => m.Tipe == "JenisNG" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            var lineStop = await _context.MasterData
                .Where(m => m.Tipe == "LineStop" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            var listAbnormality = await _context.MasterData
                .Where(m => m.Tipe == "ListAbnormality" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            // Default jika belum ada
            if (!listAbnormality.Any())
            {
                listAbnormality = new List<string> 
                { 
                    "Mesin Rusak",
                    "Material Habis",
                    "Quality Issue",
                    "Safety Issue",
                    "Lainnya"
                };
            }

            var partCodes = await _context.Produk
                .Select(p => p.PartCode)
                .Where(pc => !string.IsNullOrEmpty(pc))
                .Distinct()
                .OrderBy(pc => pc)
                .ToListAsync();

            ViewBag.LineChecking = lineChecking;
            ViewBag.Inspectors = inspectors;
            ViewBag.GroupChecking = groupChecking;
            ViewBag.JenisNG = jenisNG;
            ViewBag.LineStop = lineStop;
            ViewBag.ListAbnormality = listAbnormality;
            ViewBag.PartCodes = partCodes;
        }

        // GET: /QCHose/Index
        public IActionResult Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = "E_LWP";
            return View();
        }

        // GET: /QCHose/List - Menampilkan tabel data E_LWP dengan filter dan search
        public async Task<IActionResult> List(string search, string lineFilter, string inspectorFilter, string partCodeFilter, DateTime? dateFrom, DateTime? dateTo, string dateFilter)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var query = _context.QCHoseData.AsQueryable();

                // ROLE-BASED FILTERING: User dan Quality hanya bisa lihat data mereka sendiri
                var role = HttpContext.Session.GetString("Role");
                if (role == "User" || role == "Quality")
                {
                    var namaLengkap = HttpContext.Session.GetString("NamaLengkap");
                    if (!string.IsNullOrEmpty(namaLengkap))
                    {
                        query = query.Where(x => x.NamaInspector == namaLengkap);
                    }
                }
                // Admin bisa lihat semua data (no additional filter)

                // Filter by search
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(x => 
                        x.NamaInspector.Contains(search) || 
                        x.PartCode.Contains(search) || 
                        x.LineChecking.Contains(search));
                }

                // Filter by line
                if (!string.IsNullOrEmpty(lineFilter))
                {
                    query = query.Where(x => x.LineChecking == lineFilter);
                }

                // Filter by inspector
                if (!string.IsNullOrEmpty(inspectorFilter))
                {
                    query = query.Where(x => x.NamaInspector == inspectorFilter);
                }

                // Filter by part code
                if (!string.IsNullOrEmpty(partCodeFilter))
                {
                    query = query.Where(x => x.PartCode == partCodeFilter);
                }

                // Filter by date range or predefined date filter
                if (!string.IsNullOrEmpty(dateFilter))
                {
                    var filterDate = DateTime.Today;
                    switch (dateFilter.ToLower())
                    {
                        case "week":
                            var weekStart = filterDate.AddDays(-(int)filterDate.DayOfWeek);
                            query = query.Where(x => x.TanggalInput >= weekStart && x.TanggalInput < filterDate.AddDays(1));
                            break;
                        case "month":
                            var monthStart = new DateTime(filterDate.Year, filterDate.Month, 1);
                            query = query.Where(x => x.TanggalInput >= monthStart && x.TanggalInput < filterDate.AddDays(1));
                            break;
                        default: // "today" or empty
                            query = query.Where(x => x.TanggalInput.Date == filterDate);
                            break;
                    }
                }
                else
                {
                    // Filter by date range if provided
                    if (dateFrom.HasValue)
                    {
                        query = query.Where(x => x.TanggalInput >= dateFrom.Value);
                    }
                    if (dateTo.HasValue)
                    {
                        query = query.Where(x => x.TanggalInput <= dateTo.Value.AddDays(1));
                    }
                }

                // Ambil semua data dari database QCHoseData
                var data = await query
                    .OrderByDescending(x => x.TanggalInput)
                    .ToListAsync();

                // Get distinct values for filters dari database
                ViewBag.Lines = await _context.QCHoseData
                    .Where(x => !string.IsNullOrEmpty(x.LineChecking))
                    .Select(x => x.LineChecking)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();
                    
                ViewBag.Inspectors = await _context.QCHoseData
                    .Where(x => !string.IsNullOrEmpty(x.NamaInspector))
                    .Select(x => x.NamaInspector)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();
                    
                ViewBag.PartCodes = await _context.QCHoseData
                    .Where(x => !string.IsNullOrEmpty(x.PartCode))
                    .Select(x => x.PartCode)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();
                
                // Debug info (opsional, bisa dihapus di production)
                ViewBag.TotalRecords = data.Count;

                // Calculate statistics for cards
                var today = DateTime.Today;
                var totalHariIni = await _context.QCHoseData
                    .Where(x => x.TanggalInput.Date == today)
                    .CountAsync();
                
                var statusCheckingCount = await _context.QCHoseData
                    .Where(x => x.StatusChecking == "Checking" && x.TanggalInput.Date == today)
                    .CountAsync();
                
                var statusNGCount = await _context.QCHoseData
                    .Where(x => !string.IsNullOrEmpty(x.JenisNG) && x.TanggalInput.Date == today)
                    .CountAsync();
                
                // Calculate average time stop and get top NG by line
                var todayData = await _context.QCHoseData
                    .Where(x => x.TanggalInput.Date == today && !string.IsNullOrEmpty(x.TimeStop))
                    .ToListAsync();
                
                var avgTimeStop = todayData.Any() 
                    ? todayData
                        .Where(x => !string.IsNullOrEmpty(x.TimeStop) && x.TimeStop != "0:0:0:0")
                        .Select(x => {
                            var parts = x.TimeStop.Split(':');
                            if (parts.Length >= 3)
                            {
                                var hours = int.Parse(parts[0]);
                                var minutes = int.Parse(parts[1]);
                                var seconds = int.Parse(parts[2]);
                                return hours * 3600 + minutes * 60 + seconds;
                            }
                            return 0;
                        })
                        .Where(x => x > 0)
                        .DefaultIfEmpty(0)
                        .Average()
                    : 0;
                
                // Get top NG by line for today - convert to list of tuples for easier access in view
                var topNGByLineData = await _context.QCHoseData
                    .Where(x => x.TanggalInput.Date == today && !string.IsNullOrEmpty(x.JenisNG) && x.QtyNG > 0)
                    .GroupBy(x => new { x.LineChecking, x.JenisNG })
                    .Select(g => new { 
                        Line = g.Key.LineChecking ?? "", 
                        JenisNG = g.Key.JenisNG ?? "", 
                        TotalNG = g.Sum(x => x.QtyNG ?? 0) 
                    })
                    .OrderByDescending(x => x.TotalNG)
                    .Take(5)
                    .ToListAsync();
                
                // Convert to list of tuples for easier access in Razor view
                var topNGByLine = topNGByLineData.Select(x => 
                    Tuple.Create(x.Line, x.JenisNG, x.TotalNG)
                ).ToList();

                ViewBag.TotalHariIni = totalHariIni;
                ViewBag.StatusCheckingCount = statusCheckingCount;
                ViewBag.StatusNGCount = statusNGCount;
                ViewBag.AvgTimeStop = avgTimeStop;
                ViewBag.TopNGByLine = topNGByLine;
                ViewBag.StatusCheckingPercent = totalHariIni > 0 ? (int)((statusCheckingCount * 100.0) / totalHariIni) : 0;

                ViewBag.Search = search;
                ViewBag.LineFilter = lineFilter;
                ViewBag.InspectorFilter = inspectorFilter;
                ViewBag.PartCodeFilter = partCodeFilter;
                ViewBag.DateFrom = dateFrom;
                ViewBag.DateTo = dateTo;
                ViewBag.DateFilter = dateFilter;

                ViewData["Title"] = "Data E_LWP";
                return View(data);
            }
            catch (Exception)
            {
                ViewData["Title"] = "Data E_LWP";
                ViewBag.ErrorMessage = "Tabel belum dibuat. Data masih kosong.";
                return View(new List<QCHoseData>());
            }
        }

        // GET: /QCHose/GetStatistics - API untuk mengambil statistik dalam format JSON (untuk real-time update)
        [HttpGet]
        public async Task<IActionResult> GetStatistics(string dateFilter = "")
        {
            if (!IsLoggedIn())
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                // Calculate date range based on dateFilter
                DateTime startDate;
                DateTime endDate = DateTime.Now;

                switch (dateFilter?.ToLower())
                {
                    case "week":
                        startDate = endDate.AddDays(-7);
                        break;
                    case "month":
                        startDate = endDate.AddMonths(-1);
                        break;
                    default: // "today" or empty
                        startDate = endDate.Date;
                        break;
                }

                // Get data for today
                var dataHariIni = await _context.QCHoseData
                    .Where(x => x.TanggalInput >= startDate && x.TanggalInput <= endDate)
                    .ToListAsync();

                var totalHariIni = dataHariIni.Count;
                var statusCheckingCount = dataHariIni.Count(x => x.StatusChecking == "Checking");
                var statusNGCount = dataHariIni.Count(x => x.QtyNG > 0);

                // Calculate average time stop
                var timeStopData = dataHariIni
                    .Where(x => !string.IsNullOrEmpty(x.TimeStop) && x.TimeStop != "-" && x.TimeStop != "0:0:0:0")
                    .Select(x => x.TimeStop)
                    .ToList();

                double avgTimeStop = 0;
                if (timeStopData.Any())
                {
                    var totalSeconds = 0.0;
                    var count = 0;
                    foreach (var timeStr in timeStopData)
                    {
                        try
                        {
                            var parts = timeStr.Split(':');
                            if (parts.Length >= 3)
                            {
                                var hours = int.Parse(parts[0]);
                                var minutes = int.Parse(parts[1]);
                                var seconds = int.Parse(parts[2]);
                                totalSeconds += hours * 3600 + minutes * 60 + seconds;
                                count++;
                            }
                        }
                        catch { }
                    }
                    if (count > 0)
                    {
                        avgTimeStop = totalSeconds / count;
                    }
                }

                // Get top NG by line
                var topNGByLineData = await _context.QCHoseData
                    .Where(x => x.TanggalInput >= startDate && x.TanggalInput <= endDate && x.QtyNG > 0)
                    .GroupBy(x => new { x.LineChecking, x.JenisNG })
                    .Select(g => new
                    {
                        Line = g.Key.LineChecking ?? "-",
                        JenisNG = g.Key.JenisNG ?? "-",
                        TotalNG = g.Sum(x => x.QtyNG ?? 0)
                    })
                    .OrderByDescending(x => x.TotalNG)
                    .Take(5)
                    .ToListAsync();

                var topNGByLine = topNGByLineData.Select(x => new
                {
                    line = x.Line,
                    jenisNG = x.JenisNG,
                    count = x.TotalNG
                }).ToList();

                return Json(new
                {
                    success = true,
                    totalHariIni = totalHariIni,
                    statusCheckingCount = statusCheckingCount,
                    statusNGCount = statusNGCount,
                    statusCheckingPercent = totalHariIni > 0 ? (int)((statusCheckingCount * 100.0) / totalHariIni) : 0,
                    avgTimeStop = avgTimeStop,
                    topNGByLine = topNGByLine
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /QCHose/GetDataJson - API untuk mengambil data dalam format JSON (untuk popup)
        [HttpGet]
        public async Task<IActionResult> GetDataJson(int limit = 50)
        {
            if (!IsLoggedIn())
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                var data = await _context.QCHoseData
                    .OrderByDescending(x => x.TanggalInput)
                    .Take(limit)
                    .Select(x => new
                    {
                        id = x.Id,
                        tanggalInput = x.TanggalInput.ToString("dd/MM/yyyy HH:mm"),
                        lineChecking = x.LineChecking ?? "-",
                        namaInspector = x.NamaInspector ?? "-",
                        groupChecking = x.GroupChecking ?? "-",
                        partCode = x.PartCode ?? "-",
                        timeStop = x.TimeStop ?? "-",
                        qtyCheck = x.QtyCheck ?? 0,
                        jenisNG = x.JenisNG ?? "-",
                        namaOPR = x.NamaOPR ?? "-",
                        qtyNG = x.QtyNG ?? 0,
                        lineStop = x.LineStop ?? "-",
                        statusChecking = x.StatusChecking ?? "-",
                        statusCheckingTime = x.StatusCheckingTime ?? "-"
                    })
                    .ToListAsync();

                return Json(new { success = true, data = data, total = data.Count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /QCHose/GetDetailNG - API untuk Detail NG (hanya NG records) dengan optional date filter
        [HttpGet]
        public async Task<IActionResult> GetDetailNG(string? date = null)
        {
            if (!IsLoggedIn())
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                var query = _context.QCHoseData
                    .Where(x => !string.IsNullOrWhiteSpace(x.JenisNG) && x.QtyNG > 0); // Only actual NG records

                // Role-based filtering
                var role = HttpContext.Session.GetString("Role");
                if (role != "Admin")
                {
                    var namaLengkap = HttpContext.Session.GetString("NamaLengkap");
                    if (!string.IsNullOrEmpty(namaLengkap))
                    {
                        query = query.Where(x => x.NamaInspector == namaLengkap);
                    }
                }

                // Date filter if specific date selected
                if (!string.IsNullOrEmpty(date))
                {
                    // Parse date from calendar picker (format: yyyy-MM-dd)
                    if (DateTime.TryParse(date, out DateTime selectedDate))
                    {
                        query = query.Where(x => x.TanggalInput.Date == selectedDate.Date);
                    }
                }

                var data = await query
                    .OrderByDescending(x => x.TanggalInput)
                    .Select(x => new
                    {
                        jenisNG = x.JenisNG ?? "-",
                        namaOPR = x.NamaOPR ?? "-",
                        qtyNG = x.QtyNG ?? 0,
                        tanggal = x.TanggalInput.ToString("dd/MM/yyyy HH:mm"),
                        lineChecking = x.LineChecking ?? "-",
                        partCode = x.PartCode ?? "-"  // Added Part Code
                    })
                    .ToListAsync();

                return Json(new { success = true, data = data, total = data.Count, selectedDate = date });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /QCHose/SaveNG - API untuk save NG record langsung via AJAX
        [HttpPost]
        public async Task<IActionResult> SaveNG([FromBody] NgRecordRequest request)
        {
            if (!IsLoggedIn())
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                // Validasi input
                if (string.IsNullOrWhiteSpace(request.JenisNG))
                {
                    return Json(new { success = false, message = "Pilih Jenis NG terlebih dahulu" });
                }

                if (request.QtyNG <= 0)
                {
                    return Json(new { success = false, message = "QTY NG harus lebih dari 0" });
                }

                if (string.IsNullOrWhiteSpace(request.PartCode))
                {
                    return Json(new { success = false, message = "Part Code harus diisi" });
                }

                if (string.IsNullOrWhiteSpace(request.LineChecking))
                {
                    return Json(new { success = false, message = "Line Checking harus dipilih" });
                }

                // Get current user info
                var username = HttpContext.Session.GetString("Username");
                var namaInspector = "";
                
                if (!string.IsNullOrEmpty(username))
                {
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == username);
                    if (user != null)
                    {
                        namaInspector = user.NamaLengkap ?? "";
                    }
                }

                // Create new NG record
                var ngRecord = new QCHoseData
                {
                    JenisNG = request.JenisNG,
                    NamaOPR = request.NamaOPR,
                    QtyNG = request.QtyNG,
                    PartCode = request.PartCode,
                    LineChecking = request.LineChecking,
                    GroupChecking = request.GroupChecking ?? "A",
                    NamaInspector = namaInspector,
                    Plant = HttpContext.Session.GetString("Plant") ?? "",
                    Grup = HttpContext.Session.GetString("Grup") ?? "",
                    TanggalInput = DateTime.Now,
                    StatusChecking = "NG Only", // Special status for direct NG entries
                    QtyCheck = 0  // No qty check for direct NG entries
                };

                _context.QCHoseData.Add(ngRecord);
                await _context.SaveChangesAsync();

                // 🔊 BROADCAST via WebSocket - Send new NG data ke semua connected clients
                Console.WriteLine($"🔊 Broadcasting WebSocket NG Update: ID={ngRecord.Id}, JenisNG={ngRecord.JenisNG}, QtyNG={ngRecord.QtyNG}");
                await _hubContext.Clients.All.SendAsync("NewQCDataAdded", new
                {
                    id = ngRecord.Id,
                    tanggalInput = ngRecord.TanggalInput.ToString("dd/MM/yyyy HH:mm"),
                    lineChecking = ngRecord.LineChecking ?? "-",
                    groupChecking = ngRecord.GroupChecking ?? "-",
                    namaInspector = ngRecord.NamaInspector ?? "-",
                    partCode = ngRecord.PartCode ?? "-",
                    timeStop = ngRecord.TimeStop ?? "-",
                    qtyCheck = ngRecord.QtyCheck?.ToString() ?? "0",
                    statusChecking = ngRecord.StatusChecking ?? "NG Only",
                    jenisNG = ngRecord.JenisNG ?? "-",
                    namaOPR = ngRecord.NamaOPR ?? "-",
                    qtyNG = ngRecord.QtyNG?.ToString() ?? "-",
                    lineStop = ngRecord.LineStop ?? "-",
                    plant = ngRecord.Plant ?? "-",
                    grup = ngRecord.Grup ?? "-"
                });
                Console.WriteLine("✅ WebSocket broadcast sent for NG update!");

                return Json(new 
                { 
                    success = true, 
                    message = "Data NG berhasil disimpan!",
                    data = new
                    {
                        jenisNG = ngRecord.JenisNG,
                        namaOPR = ngRecord.NamaOPR,
                        qtyNG = ngRecord.QtyNG,
                        partCode = ngRecord.PartCode,
                        lineChecking = ngRecord.LineChecking,
                        tanggal = ngRecord.TanggalInput.ToString("dd/MM/yyyy")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: /QCHose/SaveLineStop - API untuk save Line Stop record langsung via AJAX
        [HttpPost]
        public async Task<IActionResult> SaveLineStop([FromBody] LineStopRecordRequest request)
        {
            if (!IsLoggedIn())
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                // Validasi input
                if (string.IsNullOrWhiteSpace(request.LineStop))
                {
                    return Json(new { success = false, message = "Pilih Line Stop terlebih dahulu" });
                }

                if (string.IsNullOrWhiteSpace(request.StatusCheckingTime) || request.StatusCheckingTime == "0:0:0:0")
                {
                    return Json(new { success = false, message = "Durasi Line Stop tidak valid" });
                }

                // Get current user info
                var username = HttpContext.Session.GetString("Username");
                var namaInspector = "";
                
                if (!string.IsNullOrEmpty(username))
                {
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == username);
                    if (user != null)
                    {
                        namaInspector = user.NamaLengkap ?? "";
                    }
                }

                // Create new Line Stop record
                var lineStopRecord = new QCHoseData
                {
                    LineStop = request.LineStop,
                    StatusCheckingTime = request.StatusCheckingTime,
                    StatusChecking = "Line Stop Only", // Special status for Line Stop entries
                    NamaInspector = namaInspector,
                    Plant = HttpContext.Session.GetString("Plant") ?? "",
                    Grup = HttpContext.Session.GetString("Grup") ?? "",
                    TanggalInput = DateTime.Now,
                    TimeStop = "0:0:0",
                    QtyCheck = 0  // No qty check for Line Stop entries
                };

                _context.QCHoseData.Add(lineStopRecord);
                await _context.SaveChangesAsync();

                // 🔊 BROADCAST via WebSocket
                Console.WriteLine($"🔊 Broadcasting WebSocket Line Stop Update: ID={lineStopRecord.Id}, LineStop={lineStopRecord.LineStop}, Duration={lineStopRecord.StatusCheckingTime}");
                await _hubContext.Clients.All.SendAsync("NewQCDataAdded", new
                {
                    id = lineStopRecord.Id,
                    tanggalInput = lineStopRecord.TanggalInput.ToString("dd/MM/yyyy HH:mm"),
                    lineChecking = lineStopRecord.LineChecking ?? "-",
                    groupChecking = lineStopRecord.GroupChecking ?? "-",
                    namaInspector = lineStopRecord.NamaInspector ?? "-",
                    partCode = lineStopRecord.PartCode ?? "-",
                    timeStop = lineStopRecord.TimeStop ?? "-",
                    qtyCheck = lineStopRecord.QtyCheck?.ToString() ?? "0",
                    statusChecking = lineStopRecord.StatusChecking ?? "Line Stop Only",
                    jenisNG = lineStopRecord.JenisNG ?? "-",
                    namaOPR = lineStopRecord.NamaOPR ?? "-",
                    qtyNG = lineStopRecord.QtyNG?.ToString() ?? "-",
                    lineStop = lineStopRecord.LineStop ?? "-",
                    plant = lineStopRecord.Plant ?? "-",
                    grup = lineStopRecord.Grup ?? "-"
                });
                Console.WriteLine("✅ WebSocket broadcast sent for Line Stop update!");

                return Json(new 
                { 
                    success = true, 
                    message = "Data Line Stop berhasil disimpan!",
                    data = new
                    {
                        lineStop = lineStopRecord.LineStop,
                        duration = lineStopRecord.StatusCheckingTime,
                        tanggal = lineStopRecord.TanggalInput.ToString("dd/MM/yyyy HH:mm")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // Request model for SaveLineStop
        public class LineStopRecordRequest
        {
            public string LineStop { get; set; } = string.Empty;
            public string StatusCheckingTime { get; set; } = "0:0:0:0";
        }

        // Request model for SaveNG
        public class NgRecordRequest
        {
            public required string JenisNG { get; set; }
            public required string NamaOPR { get; set; }
            public int QtyNG { get; set; }
            public required string PartCode { get; set; }
            public required string LineChecking { get; set; }
            public required string GroupChecking { get; set; }
        }

        // GET: /QCHose/GetOutputCheck - API untuk Output Check dengan filter atau date
        [HttpGet]
        public async Task<IActionResult> GetOutputCheck(string filter = "harian", string? date = null)
        {
            if (!IsLoggedIn())
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                var query = _context.QCHoseData.AsQueryable();

                // Role-based filtering
                var role = HttpContext.Session.GetString("Role");
                if (role != "Admin")
                {
                    var namaLengkap = HttpContext.Session.GetString("NamaLengkap");
                    if (!string.IsNullOrEmpty(namaLengkap))
                    {
                        query = query.Where(x => x.NamaInspector == namaLengkap);
                    }
                }

                // Date filter - specific date takes priority over filter
                if (!string.IsNullOrEmpty(date))
                {
                    // Parse date from calendar picker (format: yyyy-MM-dd)
                    if (DateTime.TryParse(date, out DateTime selectedDate))
                    {
                        query = query.Where(x => x.TanggalInput.Date == selectedDate.Date);
                    }
                }
                else
                {
                    // Use filter if no specific date selected
                    var now = DateTime.Now;
                    switch (filter.ToLower())
                    {
                        case "harian":
                            query = query.Where(x => x.TanggalInput.Date == now.Date);
                            break;
                        case "mingguan":
                            query = query.Where(x => x.TanggalInput >= now.AddDays(-7));
                            break;
                        case "bulanan":
                            query = query.Where(x => x.TanggalInput >= now.AddMonths(-1));
                            break;
                    }
                }

                var data = await query
                    .OrderByDescending(x => x.TanggalInput)
                    .Select(x => new
                    {
                        id = x.Id,
                        tanggalInput = x.TanggalInput.ToString("dd/MM/yyyy HH:mm"),
                        lineChecking = x.LineChecking ?? "-",
                        groupChecking = x.GroupChecking ?? "-",
                        namaInspector = x.NamaInspector ?? "-",
                        partCode = x.PartCode ?? "-",
                        qtyCheck = x.QtyCheck ?? 0,
                        lineStop = x.LineStop ?? "-",
                        statusChecking = x.StatusChecking ?? "-",
                        timeStop = x.TimeStop ?? "-",
                        qtyNG = x.QtyNG ?? 0
                    })
                    .ToListAsync();
                
                // Remove duplicates based on ID (in case there are any)
                var uniqueData = data.GroupBy(x => x.id).Select(g => g.First()).ToList();

                // Calculate statistics
                var totalQtyCheck = uniqueData.Sum(x => x.qtyCheck);
                var totalNG = uniqueData.Sum(x => x.qtyNG);
                var totalRRPercent = totalQtyCheck > 0 ? Math.Round((totalNG * 100.0) / totalQtyCheck, 1) : 0;

                return Json(new { 
                    success = true, 
                    data = uniqueData, 
                    total = uniqueData.Count, 
                    filter = filter, 
                    selectedDate = date,
                    statistics = new {
                        totalQtyCheck = totalQtyCheck,
                        totalNG = totalNG,
                        totalRRPercent = totalRRPercent
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /QCHose/Create - Halaman form input data baru
        // Data yang diinput akan DISIMPAN LANGSUNG KE DATABASE (tabel QCHoseData)
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            // AUTO-FILL: Ambil nama inspector dari user yang login
            var  username = HttpContext.Session.GetString("Username");
            var namaInspector = "";
            
            if (!string.IsNullOrEmpty(username))
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    namaInspector = user.NamaLengkap ?? "";
                }
            }
            
            // Kirim nama inspector ke view untuk auto-fill
            ViewBag.NamaInspectorLogin = namaInspector;
            
            // PRESERVE: Restore Line Checking dan Grup Checking dari TempData (jika ada)
            var preserveLineChecking = TempData["PreserveLineChecking"];
            if (preserveLineChecking != null)
            {
                ViewBag.PreserveLineChecking = preserveLineChecking.ToString();
                TempData.Keep("PreserveLineChecking"); // Keep for next request if needed
            }
            
            var preserveGrupChecking = TempData["PreserveGrupChecking"];
            if (preserveGrupChecking != null)
            {
                ViewBag.PreserveGrupChecking = preserveGrupChecking.ToString();
                TempData.Keep("PreserveGrupChecking"); // Keep for next request if needed
            }

            // Get data untuk dropdown dari MasterData
            var lineChecking = await _context.MasterData
                .Where(m => m.Tipe == "LineChecking" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            var groupChecking = await _context.MasterData
                .Where(m => m.Tipe == "GroupChecking" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            // Default jika belum ada
            if (!groupChecking.Any())
            {
                groupChecking = new List<string> { "A", "B" };
            }

            var jenisNG = await _context.MasterData
                .Where(m => m.Tipe == "JenisNG" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            var lineStop = await _context.MasterData
                .Where(m => m.Tipe == "LineStop" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            var listAbnormality = await _context.MasterData
                .Where(m => m.Tipe == "ListAbnormality" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();

            // Default jika belum ada
            if (!listAbnormality.Any())
            {
                listAbnormality = new List<string> 
                { 
                    "Mesin Rusak",
                    "Material Habis",
                    "Quality Issue",
                    "Safety Issue",
                    "Lainnya"
                };
            }

            // Ambil Part Code dari MasterData dan Produk
            var partCodesFromMaster = await _context.MasterData
                .Where(m => m.Tipe == "PartCode" && m.IsActive)
                .Select(m => m.Nilai)
                .Distinct()
                .ToListAsync();

            var partCodesFromProduk = await _context.Produk
                .Select(p => p.PartCode)
                .Where(pc => !string.IsNullOrEmpty(pc))
                .Distinct()
                .ToListAsync();

            // Gabungkan dan urutkan Part Code
            var partCodes = partCodesFromMaster
                .Union(partCodesFromProduk)
                .Distinct()
                .OrderBy(pc => pc)
                .ToList();

            await LoadDropdownDataAsync();
            
            ViewBag.ListAbnormality = listAbnormality;
            ViewBag.PartCodes = partCodes;

            ViewData["Title"] = "E_LWP";
            return View();
        }

        // GET: /QCHose/RefreshCreate - Clear TempData and redirect to fresh Create page
        public IActionResult RefreshCreate()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            // Clear all TempData to remove success messages and form flags
            TempData.Clear();
            
            // Redirect to Create page for fresh start
            return RedirectToAction("Create");
        }

        // POST: /QCHose/Create - Simpan data baru
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QCHoseData qcHoseData)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            // Validasi tambahan
            if (string.IsNullOrWhiteSpace(qcHoseData.LineChecking))
            {
                ModelState.AddModelError("LineChecking", "Line Checking harus dipilih");
            }
            if (string.IsNullOrWhiteSpace(qcHoseData.NamaInspector))
            {
                ModelState.AddModelError("NamaInspector", "Nama Inspector harus diisi");
            }
            if (string.IsNullOrWhiteSpace(qcHoseData.PartCode))
            {
                ModelState.AddModelError("PartCode", "Part Code harus diisi");
            }
            if (qcHoseData.QtyCheck == null || qcHoseData.QtyCheck <= 0)
            {
                ModelState.AddModelError("QtyCheck", "Qty Check harus lebih dari 0");
            }

            // Validasi Part Code ada di database (dari Produk atau MasterData)
            var partCodeFromProduk = await _context.Produk.AnyAsync(p => p.PartCode == qcHoseData.PartCode);
            var partCodeFromMasterData = await _context.MasterData.AnyAsync(m => m.Tipe == "PartCode" && m.Nilai == qcHoseData.PartCode && m.IsActive);
            if (!partCodeFromProduk && !partCodeFromMasterData)
            {
                ModelState.AddModelError("PartCode", "Part Code tidak ditemukan di database. Silakan tambahkan di Master Data terlebih dahulu.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Set nilai default jika kosong
                    if (string.IsNullOrEmpty(qcHoseData.GroupChecking))
                        qcHoseData.GroupChecking = "A";
                    
                    if (string.IsNullOrEmpty(qcHoseData.StatusChecking))
                        qcHoseData.StatusChecking = "Checking";

                    // Set Plant dan Grup dari session
                    qcHoseData.Plant = HttpContext.Session.GetString("Plant") ?? "";
                    qcHoseData.Grup = HttpContext.Session.GetString("Grup") ?? "";

                    qcHoseData.TanggalInput = DateTime.Now;

                    // SIMPAN LANGSUNG KE DATABASE (Tabel QCHoseData)
                    _context.QCHoseData.Add(qcHoseData);
                    await _context.SaveChangesAsync(); // Data langsung tersimpan ke database

                    // 🔊 BROADCAST via WebSocket - Send new data ke semua connected clients
                    Console.WriteLine($"🔊 Broadcasting WebSocket: ID={qcHoseData.Id}, Inspector={qcHoseData.NamaInspector}");
                    await _hubContext.Clients.All.SendAsync("NewQCDataAdded", new
                    {
                        id = qcHoseData.Id,
                        tanggalInput = qcHoseData.TanggalInput.ToString("dd/MM/yyyy HH:mm"),
                        lineChecking = qcHoseData.LineChecking ?? "-",
                        groupChecking = qcHoseData.GroupChecking ?? "-",
                        namaInspector = qcHoseData.NamaInspector ?? "-",
                        partCode = qcHoseData.PartCode ?? "-",
                        timeStop = qcHoseData.TimeStop ?? "-",
                        qtyCheck = qcHoseData.QtyCheck?.ToString() ?? "-",
                        statusChecking = qcHoseData.StatusChecking ?? "Checking",
                        jenisNG = qcHoseData.JenisNG ?? "-",
                        namaOPR = qcHoseData.NamaOPR ?? "-",
                        qtyNG = qcHoseData.QtyNG?.ToString() ?? "-",
                        lineStop = qcHoseData.LineStop ?? "-",
                        plant = qcHoseData.Plant ?? "-",
                        grup = qcHoseData.Grup ?? "-"
                    });
                    Console.WriteLine("✅ WebSocket broadcast sent!");

                    // Set success message dan tetap di halaman yang sama
                    TempData["SuccessMessage"] = "Data E_LWP berhasil disimpan! Silakan klik 'Lihat Data' untuk melihat data.";
                    TempData["FormSubmitted"] = "true"; // Flag untuk reset form di view
                    
                    // PRESERVE: Simpan Line Checking dan Grup Checking untuk di-restore setelah submit
                    TempData["PreserveLineChecking"] = qcHoseData.LineChecking;
                    TempData["PreserveGrupChecking"] = qcHoseData.GroupChecking;
                    
                    // Reload dropdown data untuk halaman berikutnya
                    await LoadDropdownDataAsync();
                    
                    // Set ListAbnormality dan PartCodes untuk view
                    var listAbnormality = new List<string> 
                    { 
                        "Mesin Rusak",
                        "Material Habis",
                        "Quality Issue",
                        "Safety Issue",
                        "Lainnya"
                    };
                    
                    var partCodesFromMaster = await _context.MasterData
                        .Where(m => m.Tipe == "PartCode" && m.IsActive)
                        .Select(m => m.Nilai)
                        .Distinct()
                        .ToListAsync();

                    var partCodesFromProduk = await _context.Produk
                        .Select(p => p.PartCode)
                        .Where(pc => !string.IsNullOrEmpty(pc))
                        .Distinct()
                        .ToListAsync();

                    var partCodes = partCodesFromMaster
                        .Union(partCodesFromProduk)
                        .Distinct()
                        .OrderBy(pc => pc)
                        .ToList();
                    
                    ViewBag.ListAbnormality = listAbnormality;
                    ViewBag.PartCodes = partCodes;
                    
                    // PRESERVE: Set Nama Inspector untuk field readonly tetap terisi setelah submit
                    var username = HttpContext.Session.GetString("Username");
                    if (!string.IsNullOrEmpty(username))
                    {
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u => u.Username == username);
                        if (user != null)
                        {
                            ViewBag.NamaInspectorLogin = user.NamaLengkap ?? "";
                        }
                    }
                    
                    ViewData["Title"] = "E_LWP";
                    return View(new QCHoseData()); // Return new empty model untuk reset form
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Terjadi kesalahan saat menyimpan data: " + ex.Message;
                    
                    // PRESERVE: Set Nama Inspector untuk field readonly tetap terisi saat ada exception
                    var usernameEx = HttpContext.Session.GetString("Username");
                    if (!string.IsNullOrEmpty(usernameEx))
                    {
                        var userEx = await _context.Users
                            .FirstOrDefaultAsync(u => u.Username == usernameEx);
                        if (userEx != null)
                        {
                            ViewBag.NamaInspectorLogin = userEx.NamaLengkap ?? "";
                        }
                    }
                }
            }

            // Reload dropdown data jika ada error
            await LoadDropdownDataAsync();
            
            // CRITICAL FIX: Populate PartCodes dan ListAbnormality untuk autocomplete
            var listAbnormalityError = await _context.MasterData
                .Where(m => m.Tipe == "ListAbnormality" && m.IsActive)
                .Select(m => m.Nilai)
                .OrderBy(n => n)
                .ToListAsync();
            
            if (!listAbnormalityError.Any())
            {
                listAbnormalityError = new List<string> 
                { 
                    "Mesin Rusak",
                    "Material Habis",
                    "Quality Issue",
                    "Safety Issue",
                    "Lainnya"
                };
            }
            
            var partCodesFromMasterError = await _context.MasterData
                .Where(m => m.Tipe == "PartCode" && m.IsActive)
                .Select(m => m.Nilai)
                .Distinct()
                .ToListAsync();

            var partCodesFromProdukError = await _context.Produk
                .Select(p => p.PartCode)
                .Where(pc => !string.IsNullOrEmpty(pc))
                .Distinct()
                .ToListAsync();

            var partCodesError = partCodesFromMasterError
                .Union(partCodesFromProdukError)
                .Distinct()
                .OrderBy(pc => pc)
                .ToList();
            
            ViewBag.ListAbnormality = listAbnormalityError;
            ViewBag.PartCodes = partCodesError;

            // PRESERVE: Set Nama Inspector untuk field readonly tetap terisi saat ada error
            var usernameError = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(usernameError))
            {
                var userError = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == usernameError);
                if (userError != null)
                {
                    ViewBag.NamaInspectorLogin = userError.NamaLengkap ?? "";
                }
            }

            ViewData["Title"] = "E_LWP";
            return View(qcHoseData);
        }

        // GET: /QCHose/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var qcHoseData = await _context.QCHoseData.FindAsync(id);
            if (qcHoseData == null)
            {
                return NotFound();
            }

            // Get data untuk dropdown
            await LoadDropdownDataAsync();

            // Set ListAbnormality dan PartCodes untuk view
            var listAbnormality = new List<string> 
            { 
                "Mesin Rusak",
                "Material Habis",
                "Quality Issue",
                "Safety Issue",
                "Lainnya"
            };
            
            var partCodesFromMaster = await _context.MasterData
                .Where(m => m.Tipe == "PartCode" && m.IsActive)
                .Select(m => m.Nilai)
                .Distinct()
                .ToListAsync();

            var partCodesFromProduk = await _context.Produk
                .Select(p => p.PartCode)
                .Where(pc => !string.IsNullOrEmpty(pc))
                .Distinct()
                .ToListAsync();

            var partCodes = partCodesFromMaster
                .Union(partCodesFromProduk)
                .Distinct()
                .OrderBy(pc => pc)
                .ToList();
            
            ViewBag.ListAbnormality = listAbnormality;
            ViewBag.PartCodes = partCodes;

            ViewData["Title"] = "Edit Data E_LWP";
            return View(qcHoseData);
        }

        // POST: /QCHose/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QCHoseData qcHoseData)
        {
            if (id != qcHoseData.Id)
            {
                return NotFound();
            }

            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            // Validasi tambahan
            if (string.IsNullOrWhiteSpace(qcHoseData.LineChecking))
            {
                ModelState.AddModelError("LineChecking", "Line Checking harus dipilih");
            }
            if (string.IsNullOrWhiteSpace(qcHoseData.NamaInspector))
            {
                ModelState.AddModelError("NamaInspector", "Nama Inspector harus diisi");
            }
            if (string.IsNullOrWhiteSpace(qcHoseData.PartCode))
            {
                ModelState.AddModelError("PartCode", "Part Code harus diisi");
            }
            if (qcHoseData.QtyCheck == null || qcHoseData.QtyCheck <= 0)
            {
                ModelState.AddModelError("QtyCheck", "Qty Check harus lebih dari 0");
            }

            // Validasi Part Code ada di database (dari Produk atau MasterData)
            var partCodeFromProduk = await _context.Produk.AnyAsync(p => p.PartCode == qcHoseData.PartCode);
            var partCodeFromMasterData = await _context.MasterData.AnyAsync(m => m.Tipe == "PartCode" && m.Nilai == qcHoseData.PartCode && m.IsActive);
            if (!partCodeFromProduk && !partCodeFromMasterData)
            {
                ModelState.AddModelError("PartCode", "Part Code tidak ditemukan di database. Silakan tambahkan di Master Data terlebih dahulu.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qcHoseData);
                    await _context.SaveChangesAsync();
                    
                    // Set success message dan tetap di halaman Edit
                    TempData["SuccessMessage"] = "Data E_LWP berhasil diupdate! Silakan klik 'Lihat Data' untuk melihat data.";
                    TempData["FormSubmitted"] = "true"; // Flag untuk trigger scroll di view
                    
                    // Reload dropdown data
                    await LoadDropdownDataAsync();
                    
                    // Set ListAbnormality dan PartCodes untuk view
                    var listAbnormality = new List<string> 
                    { 
                        "Mesin Rusak",
                        "Material Habis",
                        "Quality Issue",
                        "Safety Issue",
                        "Lainnya"
                    };
                    
                    var partCodesFromMaster = await _context.MasterData
                        .Where(m => m.Tipe == "PartCode" && m.IsActive)
                        .Select(m => m.Nilai)
                        .Distinct()
                        .ToListAsync();

                    var partCodesFromProduk = await _context.Produk
                        .Select(p => p.PartCode)
                        .Where(pc => !string.IsNullOrEmpty(pc))
                        .Distinct()
                        .ToListAsync();

                    var partCodes = partCodesFromMaster
                        .Union(partCodesFromProduk)
                        .Distinct()
                        .OrderBy(pc => pc)
                        .ToList();
                    
                    ViewBag.ListAbnormality = listAbnormality;
                    ViewBag.PartCodes = partCodes;
                    
                    ViewData["Title"] = "Edit Data E_LWP";
                    return View(qcHoseData); // Tetap di halaman Edit
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.QCHoseData.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Terjadi kesalahan saat mengupdate data: " + ex.Message;
                }
            }

            // Reload dropdown data jika ada error
            await LoadDropdownDataAsync();

            ViewData["Title"] = "Edit Data E_LWP";
            return View(qcHoseData);
        }

        // POST: /QCHose/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            var qcHoseData = await _context.QCHoseData.FindAsync(id);
            if (qcHoseData != null)
            {
                _context.QCHoseData.Remove(qcHoseData);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data E_LWP berhasil dihapus!";
            }

            return RedirectToAction(nameof(List));
        }

        // POST: /QCHose/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (!IsLoggedIn() || !IsAuthorized())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var qcHoseData = await _context.QCHoseData.FindAsync(request.Id);
                if (qcHoseData == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                qcHoseData.StatusChecking = request.Status;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Status berhasil diupdate" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper class untuk UpdateStatus
        public class UpdateStatusRequest
        {
            public int Id { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        // POST: /QCHose/UpdateNG
        [HttpPost]
        public async Task<IActionResult> UpdateNG(int id, string jenisNG, string namaOPR, int? qtyNG)
        {
            if (!IsLoggedIn() || !IsAuthorized())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var qcHoseData = await _context.QCHoseData.FindAsync(id);
                if (qcHoseData == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                qcHoseData.JenisNG = jenisNG;
                qcHoseData.NamaOPR = namaOPR;
                qcHoseData.QtyNG = qtyNG;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Data NG berhasil diupdate" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}


