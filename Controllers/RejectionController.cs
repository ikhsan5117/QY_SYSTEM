using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi.Controllers
{
    public class RejectionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RejectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hanya user yang sudah login yang boleh akses
        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // Hanya Admin yang boleh akses Dashboard Rejection
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin";
        }

        // GET: /Rejection/Dashboard
        public async Task<IActionResult> Dashboard(string? bulan, string? tanggal, string? jenisNG, string? line, string? kategoriNG, string? partCode)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            // Dashboard Rejection hanya untuk Admin
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Akses ditolak. Dashboard Rejection hanya untuk Admin.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Check if any filter is set - if all filters are empty, return empty data
                bool hasAnyFilter = !string.IsNullOrEmpty(bulan) || 
                                   !string.IsNullOrEmpty(tanggal) || 
                                   !string.IsNullOrEmpty(jenisNG) || 
                                   !string.IsNullOrEmpty(line) || 
                                   !string.IsNullOrEmpty(kategoriNG) || 
                                   !string.IsNullOrEmpty(partCode);
                
                // Get data from QCHoseData where there is NG (QtyNG > 0)
                var query = _context.QCHoseData
                    .Where(x => x.QtyNG != null && x.QtyNG > 0)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(bulan))
                {
                    var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (bulanList.Length > 0)
                    {
                        // Parse bulan format: "Month-Year" (e.g., "1-2026", "12-2025")
                        var monthYearPairs = bulanList
                            .Select(b => {
                                var parts = b.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    return new { Month = month, Year = year };
                                }
                                // Fallback untuk format lama (hanya bulan)
                                else if (int.TryParse(b, out int monthOnly))
                                {
                                    return new { Month = monthOnly, Year = DateTime.Now.Year };
                                }
                                return null;
                            })
                            .Where(p => p != null)
                            .ToList();
                        
                        if (monthYearPairs != null && monthYearPairs.Count > 0)
                        {
                            var validPairs = monthYearPairs.Where(p => p != null).ToList();
                            if (validPairs.Count > 0)
                            {
                                query = query.Where(x => validPairs.Any(p => p != null && p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year));
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tanggal))
                {
                    var tanggalList = tanggal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (tanggalList.Length > 0)
                    {
                        // Tanggal comes as day numbers (1-31), need to combine with selected month/year
                        int tanggalYear = DateTime.Now.Year;
                        int tanggalMonth = DateTime.Now.Month;
                        if (!string.IsNullOrEmpty(bulan))
                        {
                            var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (bulanList.Length > 0)
                            {
                                // Parse bulan format: "Month-Year" (e.g., "1-2026", "12-2025")
                                var firstBulan = bulanList.First();
                                var parts = firstBulan.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    tanggalMonth = month;
                                    tanggalYear = year;
                                }
                                else if (int.TryParse(firstBulan, out int monthOnly))
                                {
                                    // Fallback untuk format lama (hanya bulan)
                                    tanggalMonth = monthOnly;
                                    tanggalYear = DateTime.Now.Year;
                                }
                            }
                        }
                        
                        var tanggalDates = tanggalList
                            .Where(t => int.TryParse(t, out int day) && day >= 1 && day <= 31)
                            .Select(t => {
                                int day = int.Parse(t);
                                if (day <= DateTime.DaysInMonth(tanggalYear, tanggalMonth))
                                {
                                    return new DateTime(tanggalYear, tanggalMonth, day).Date;
                                }
                                return (DateTime?)null;
                            })
                            .Where(dt => dt.HasValue)
                            .Select(dt => dt!.Value)
                            .ToList();
                        if (tanggalDates.Count > 0)
                        {
                            query = query.Where(x => tanggalDates.Contains(x.TanggalInput.Date));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(jenisNG))
                {
                    var jenisNGList = jenisNG.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (jenisNGList.Length > 0)
                    {
                        query = query.Where(x => jenisNGList.Contains(x.JenisNG));
                    }
                }

                if (!string.IsNullOrEmpty(kategoriNG))
                {
                    // KategoriNG uses same field as JenisNG (based on view implementation)
                    var kategoriNGList = kategoriNG.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (kategoriNGList.Length > 0)
                    {
                        query = query.Where(x => kategoriNGList.Contains(x.JenisNG));
                    }
                }

                if (!string.IsNullOrEmpty(line))
                {
                    var lineList = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (lineList.Length > 0)
                    {
                        query = query.Where(x => lineList.Contains(x.LineChecking));
                    }
                }

                if (!string.IsNullOrEmpty(partCode))
                {
                    var partCodeList = partCode.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (partCodeList.Length > 0)
                    {
                        query = query.Where(x => partCodeList.Contains(x.PartCode));
                    }
                }

                var data = await query.ToListAsync();

                // Calculate summary metrics
                // QTY NG: Sum dari semua QtyNG yang > 0
                var totalQtyNG = data.Sum(x => x.QtyNG ?? 0);
                
                // QTY CHECK: Hitung dari semua data yang sudah di-check (QtyCheck > 0) dengan filter yang sama
                // Build query untuk QTY CHECK (semua data yang sudah di-check, tidak hanya yang NG)
                var qtyCheckQuery = _context.QCHoseData
                    .Where(x => x.QtyCheck != null && x.QtyCheck > 0)
                    .AsQueryable();
                
                // Apply same filters to QTY CHECK query (except jenisNG, because QTY CHECK includes all checked data, not just NG)
                if (!string.IsNullOrEmpty(bulan))
                {
                    var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (bulanList.Length > 0)
                    {
                        // Parse bulan format: "Month-Year" (e.g., "1-2026", "12-2025")
                        var monthYearPairs = bulanList
                            .Select(b => {
                                var parts = b.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    return new { Month = month, Year = year };
                                }
                                // Fallback untuk format lama (hanya bulan)
                                else if (int.TryParse(b, out int monthOnly))
                                {
                                    return new { Month = monthOnly, Year = DateTime.Now.Year };
                                }
                                return null;
                            })
                            .Where(p => p != null)
                            .ToList();
                        
                        if (monthYearPairs != null && monthYearPairs.Count > 0)
                        {
                            qtyCheckQuery = qtyCheckQuery.Where(x => monthYearPairs.Any(p => p != null && p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(tanggal))
                {
                    var tanggalList = tanggal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (tanggalList.Length > 0)
                    {
                        // Tanggal comes as day numbers (1-31), need to combine with selected month/year
                        int tanggalYear = DateTime.Now.Year;
                        int tanggalMonth = DateTime.Now.Month;
                        if (!string.IsNullOrEmpty(bulan))
                        {
                            var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (bulanList.Length > 0)
                            {
                                // Parse bulan format: "Month-Year" (e.g., "1-2026", "12-2025")
                                var firstBulan = bulanList.First();
                                var parts = firstBulan.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    tanggalMonth = month;
                                    tanggalYear = year;
                                }
                                else if (int.TryParse(firstBulan, out int monthOnly))
                                {
                                    // Fallback untuk format lama (hanya bulan)
                                    tanggalMonth = monthOnly;
                                    tanggalYear = DateTime.Now.Year;
                                }
                            }
                        }
                        
                        var tanggalDates = tanggalList
                            .Where(t => int.TryParse(t, out int day) && day >= 1 && day <= 31)
                            .Select(t => {
                                int day = int.Parse(t);
                                if (day <= DateTime.DaysInMonth(tanggalYear, tanggalMonth))
                                {
                                    return new DateTime(tanggalYear, tanggalMonth, day).Date;
                                }
                                return (DateTime?)null;
                            })
                            .Where(dt => dt.HasValue)
                            .Select(dt => dt!.Value)
                            .ToList();
                        if (tanggalDates.Count > 0)
                        {
                            qtyCheckQuery = qtyCheckQuery.Where(x => tanggalDates.Contains(x.TanggalInput.Date));
                        }
                    }
                }
                // Note: jenisNG filter is NOT applied to qtyCheckQuery because QTY CHECK should include all checked data
                // regardless of NG type. Only NG data should be filtered by jenisNG.
                if (!string.IsNullOrEmpty(line))
                {
                    var lineList = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (lineList.Length > 0)
                    {
                        qtyCheckQuery = qtyCheckQuery.Where(x => lineList.Contains(x.LineChecking));
                    }
                }
                if (!string.IsNullOrEmpty(partCode))
                {
                    var partCodeList = partCode.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (partCodeList.Length > 0)
                    {
                        qtyCheckQuery = qtyCheckQuery.Where(x => partCodeList.Contains(x.PartCode));
                    }
                }
                
                var totalQtyCheck = await qtyCheckQuery.SumAsync(x => x.QtyCheck ?? 0);
                var totalRR = totalQtyCheck > 0 ? Math.Round((totalQtyNG * 100.0) / totalQtyCheck, 2) : 0;

                // Get all checked data (not just NG) for QTY CHECK calculation in charts
                var allCheckedData = await qtyCheckQuery.ToListAsync();
                
                // Build query untuk mendapatkan semua data sesuai filter (tanpa filter QtyCheck > 0 atau QtyNG > 0)
                // Ini digunakan untuk mendapatkan semua part code yang sesuai filter
                var allDataQuery = _context.QCHoseData.AsQueryable();
                
                // Apply same filters to allDataQuery (except jenisNG and kategoriNG, karena kita ingin semua part code)
                if (!string.IsNullOrEmpty(bulan))
                {
                    var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (bulanList.Length > 0)
                    {
                        // Parse bulan format: "Month-Year" (e.g., "1-2026", "12-2025")
                        var monthYearPairs = bulanList
                            .Select(b => {
                                var parts = b.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    return new { Month = month, Year = year };
                                }
                                // Fallback untuk format lama (hanya bulan)
                                else if (int.TryParse(b, out int monthOnly))
                                {
                                    return new { Month = monthOnly, Year = DateTime.Now.Year };
                                }
                                return null;
                            })
                            .Where(p => p != null)
                            .ToList();
                        
                        if (monthYearPairs != null && monthYearPairs.Count > 0)
                        {
                            var validPairs = monthYearPairs.Where(p => p != null).ToList();
                            if (validPairs.Count > 0)
                            {
                                allDataQuery = allDataQuery.Where(x => validPairs.Any(p => p != null && p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year));
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(tanggal))
                {
                    var tanggalList = tanggal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (tanggalList.Length > 0)
                    {
                        int tanggalYear = DateTime.Now.Year;
                        int tanggalMonth = DateTime.Now.Month;
                        if (!string.IsNullOrEmpty(bulan))
                        {
                            var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (bulanList.Length > 0)
                            {
                                // Parse bulan format: "Month-Year" (e.g., "1-2026", "12-2025")
                                var firstBulan = bulanList.First();
                                var parts = firstBulan.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    tanggalMonth = month;
                                    tanggalYear = year;
                                }
                                else if (int.TryParse(firstBulan, out int monthOnly))
                                {
                                    // Fallback untuk format lama (hanya bulan)
                                    tanggalMonth = monthOnly;
                                    tanggalYear = DateTime.Now.Year;
                                }
                            }
                        }
                        
                        var tanggalDates = tanggalList
                            .Where(t => int.TryParse(t, out int day) && day >= 1 && day <= 31)
                            .Select(t => {
                                int day = int.Parse(t);
                                if (day <= DateTime.DaysInMonth(tanggalYear, tanggalMonth))
                                {
                                    return new DateTime(tanggalYear, tanggalMonth, day).Date;
                                }
                                return (DateTime?)null;
                            })
                            .Where(dt => dt.HasValue)
                            .Select(dt => dt!.Value)
                            .ToList();
                        if (tanggalDates.Count > 0)
                        {
                            allDataQuery = allDataQuery.Where(x => tanggalDates.Contains(x.TanggalInput.Date));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(line))
                {
                    var lineList = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (lineList.Length > 0)
                    {
                        allDataQuery = allDataQuery.Where(x => lineList.Contains(x.LineChecking));
                    }
                }
                if (!string.IsNullOrEmpty(partCode))
                {
                    var partCodeList = partCode.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (partCodeList.Length > 0)
                    {
                        allDataQuery = allDataQuery.Where(x => partCodeList.Contains(x.PartCode));
                    }
                }
                
                // Get all data sesuai filter untuk mendapatkan semua part code
                var allData = await allDataQuery.ToListAsync();
                
                // Determine month and year for daily data
                // If bulan filter is set, use first selected month; otherwise use current month
                int selectedMonth = DateTime.Now.Month;
                int selectedYear = DateTime.Now.Year;
                List<int> selectedMonths = new List<int>();
                List<(int Month, int Year)> selectedMonthYears = new List<(int, int)>();
                if (!string.IsNullOrEmpty(bulan))
                {
                    var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (bulanList.Length > 0)
                    {
                        selectedMonthYears = bulanList
                            .Select(b => {
                                var parts = b.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[1], out int year))
                                {
                                    // Use helper to parse month (supports both "1" and "Jan")
                                    int month = ParseMonthValue(parts[0].Trim());
                                    if (month > 0)
                                    {
                                        return (Month: month, Year: year);
                                    }
                                }
                                else
                                {
                                    // Try parse as month-only (for backward compatibility)
                                    int monthOnly = ParseMonthValue(b.Trim());
                                    if (monthOnly > 0)
                                    {
                                        return (Month: monthOnly, Year: DateTime.Now.Year);
                                    }
                                }
                                return (Month: 0, Year: 0);
                            })
                            .Where(p => p.Month > 0)
                            .ToList();
                        
                        selectedMonths = selectedMonthYears.Select(p => p.Month).Distinct().ToList();
                        if (selectedMonthYears.Count > 0)
                        {
                            selectedMonth = selectedMonthYears.First().Month;
                            selectedYear = selectedMonthYears.First().Year;
                        }
                    }
                }
                
                // Get data for charts (use allData untuk mendapatkan semua periode yang sesuai filter, allCheckedData untuk QTY CHECK, data untuk QTY NG)
                // Pass selectedMonths dan tanggal untuk menentukan apakah semua checkbox dipilih
                var monthlyData = GetMonthlyData(data, allCheckedData, allData, bulan, selectedMonthYears);
                var weeklyData = GetWeeklyData(data, allCheckedData, allData, selectedMonths, selectedMonthYears, bulan);
                
                var dailyData = GetDailyData(data, allCheckedData, allData, selectedMonth, selectedYear, tanggal);
                
                // Pass selected months to view for chart filtering
                ViewBag.SelectedMonths = selectedMonths;
                ViewBag.SelectedTanggal = !string.IsNullOrEmpty(tanggal) ? tanggal.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                var paretoPartData = GetParetoPartData(data, allCheckedData, allData);
                var kriteriaNGData = GetKriteriaNGData(data);
                var rejectionByPartData = GetRejectionByPartData(data, allCheckedData, allData);
                var rejectionByKriteriaData = GetRejectionByKriteriaData(data);

                ViewBag.TotalQtyNG = totalQtyNG;
                ViewBag.TotalQtyCheck = totalQtyCheck;
                ViewBag.TotalRR = totalRR;
                ViewBag.MonthlyData = monthlyData;
                ViewBag.WeeklyData = weeklyData;
                ViewBag.DailyData = dailyData;
                ViewBag.ParetoPartData = paretoPartData;
                ViewBag.KriteriaNGData = kriteriaNGData;
                ViewBag.RejectionByPartData = rejectionByPartData;
                ViewBag.RejectionByKriteriaData = rejectionByKriteriaData;

                // Get filter options
                ViewBag.JenisNGList = await _context.QCHoseData
                    .Where(x => !string.IsNullOrEmpty(x.JenisNG))
                    .Select(x => x.JenisNG)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                ViewBag.LineList = await _context.QCHoseData
                    .Where(x => !string.IsNullOrEmpty(x.LineChecking))
                    .Select(x => x.LineChecking)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                ViewBag.PartCodeList = await _context.QCHoseData
                    .Where(x => !string.IsNullOrEmpty(x.PartCode))
                    .Select(x => x.PartCode)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                // Get unique month-year combinations from database for bulan dropdown
                // IMPORTANT: Query ini TIDAK boleh terpengaruh filter apapun, harus mengambil semua bulan yang ada di database
                // Gunakan query terpisah tanpa filter apapun untuk memastikan semua bulan muncul
                var bulanListQuery = await _context.QCHoseData
                    .GroupBy(x => new { x.TanggalInput.Year, x.TanggalInput.Month })
                    .Select(g => new { 
                        Year = g.Key.Year, 
                        Month = g.Key.Month
                    })
                    .Where(x => x.Year > 0 && x.Month > 0)
                    .ToListAsync();
                
                // Transform di memory untuk menghindari masalah dengan GetMonthName di EF query
                ViewBag.BulanList = bulanListQuery
                    .Select(x => new { 
                        Year = x.Year, 
                        Month = x.Month,
                        MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                        Value = $"{x.Month}-{x.Year}"
                    })
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .ToList();

                ViewBag.Bulan = bulan;
                ViewBag.Tanggal = tanggal;
                ViewBag.JenisNG = jenisNG;
                ViewBag.Line = line;
                ViewBag.KategoriNG = kategoriNG;
                ViewBag.PartCode = partCode;

                ViewData["Title"] = "Dashboard Rejection Molded";

                // Jika request adalah AJAX, kembalikan data dalam format JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        TotalQtyNG = totalQtyNG,
                        TotalQtyCheck = totalQtyCheck,
                        TotalRR = totalRR,
                        MonthlyData = monthlyData,
                        WeeklyData = weeklyData,
                        DailyData = dailyData,
                        ParetoPartData = paretoPartData,
                        KriteriaNGData = kriteriaNGData,
                        RejectionByPartData = rejectionByPartData,
                        RejectionByKriteriaData = rejectionByKriteriaData
                    });
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewData["Title"] = "Dashboard Rejection Molded";
                ViewBag.ErrorMessage = "Error loading data: " + ex.Message;
                return View();
            }
        }

        // GET: /Rejection/DashboardHose
        public async Task<IActionResult> DashboardHose(string? bulan, string? tanggal, string? jenisNG, string? line, string? kategoriNG, string? partCode)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Akses ditolak. Dashboard Rejection hanya untuk Admin.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PlantName = "HOSE";
            return await GetDashboardByPlant("HOSE", bulan, tanggal, jenisNG, line, kategoriNG, partCode);
        }

        // GET: /Rejection/DashboardMolded
        public async Task<IActionResult> DashboardMolded(string? bulan, string? tanggal, string? jenisNG, string? line, string? kategoriNG, string? partCode)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Akses ditolak. Dashboard Rejection hanya untuk Admin.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PlantName = "MOLDED";
            return await GetDashboardByPlant("MOLDED", bulan, tanggal, jenisNG, line, kategoriNG, partCode);
        }

        // GET: /Rejection/DashboardRVI
        public async Task<IActionResult> DashboardRVI(string? bulan, string? tanggal, string? jenisNG, string? line, string? kategoriNG, string? partCode)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Akses ditolak. Dashboard Rejection hanya untuk Admin.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PlantName = "RVI";
            return await GetDashboardByPlant("RVI", bulan, tanggal, jenisNG, line, kategoriNG, partCode);
        }

        // GET: /Rejection/DashboardBTR
        public async Task<IActionResult> DashboardBTR(string? bulan, string? tanggal, string? jenisNG, string? line, string? kategoriNG, string? partCode)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Akses ditolak. Dashboard Rejection hanya untuk Admin.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.PlantName = "BTR";
            return await GetDashboardByPlant("BTR", bulan, tanggal, jenisNG, line, kategoriNG, partCode);
        }

        // Helper method for plant-specific dashboard
        private async Task<IActionResult> GetDashboardByPlant(string plantName, string? bulan, string? tanggal, string? jenisNG, string? line, string? kategoriNG, string? partCode)
        {
            try
            {
                bool hasAnyFilter = !string.IsNullOrEmpty(bulan) || 
                                   !string.IsNullOrEmpty(tanggal) || 
                                   !string.IsNullOrEmpty(jenisNG) || 
                                   !string.IsNullOrEmpty(line) || 
                                   !string.IsNullOrEmpty(kategoriNG) || 
                                   !string.IsNullOrEmpty(partCode);
                
                // Get NG data with Plant filter
                var query = _context.QCHoseData
                    .Where(x => x.QtyNG != null && x.QtyNG > 0 && x.Plant == plantName)
                    .AsQueryable();

                // Parse selected months for filtering
                List<(int Month, int Year)> selectedMonthYears = new List<(int Month, int Year)>();
                List<int> selectedMonths = new List<int>();
                int selectedMonth = DateTime.Now.Month;
                int selectedYear = DateTime.Now.Year;

                if (!string.IsNullOrEmpty(bulan))
                {
                    var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var b in bulanList)
                    {
                        var parts = b.Split('-');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                        {
                            selectedMonthYears.Add((month, year));
                            if (!selectedMonths.Contains(month))
                                selectedMonths.Add(month);
                            selectedMonth = month;
                            selectedYear = year;
                        }
                    }
                }

                // Apply filters to NG query
                if (!string.IsNullOrEmpty(bulan))
                {
                    var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (bulanList.Length > 0)
                    {
                        var monthYearPairs = bulanList
                            .Select(b => {
                                var parts = b.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    return new { Month = month, Year = year };
                                }
                                else if (int.TryParse(b, out int monthOnly))
                                {
                                    return new { Month = monthOnly, Year = DateTime.Now.Year };
                                }
                                return null;
                            })
                            .Where(p => p != null)
                            .ToList();
                        
                        if (monthYearPairs != null && monthYearPairs.Count > 0)
                        {
                            var validPairs = monthYearPairs.Where(p => p != null).ToList();
                            if (validPairs.Count > 0)
                            {
                                query = query.Where(x => validPairs.Any(p => p != null && p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year));
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tanggal))
                {
                    var tanggalList = tanggal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (tanggalList.Length > 0)
                    {
                        int tanggalYear = DateTime.Now.Year;
                        int tanggalMonth = DateTime.Now.Month;
                        if (!string.IsNullOrEmpty(bulan))
                        {
                            var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (bulanList.Length > 0)
                            {
                                var firstBulan = bulanList.First();
                                var parts = firstBulan.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    tanggalMonth = month;
                                    tanggalYear = year;
                                }
                            }
                        }
                        
                        var tanggalDates = tanggalList
                            .Where(t => int.TryParse(t, out int day) && day >= 1 && day <= 31)
                            .Select(t => {
                                int day = int.Parse(t);
                                if (day <= DateTime.DaysInMonth(tanggalYear, tanggalMonth))
                                {
                                    return new DateTime(tanggalYear, tanggalMonth, day).Date;
                                }
                                return (DateTime?)null;
                            })
                            .Where(dt => dt.HasValue)
                            .Select(dt => dt!.Value)
                            .ToList();
                        if (tanggalDates.Count > 0)
                        {
                            query = query.Where(x => tanggalDates.Contains(x.TanggalInput.Date));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(jenisNG))
                {
                    var jenisNGList = jenisNG.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (jenisNGList.Length > 0)
                    {
                        query = query.Where(x => jenisNGList.Contains(x.JenisNG ?? ""));
                    }
                }

                if (!string.IsNullOrEmpty(line))
                {
                    var lineList = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (lineList.Length > 0)
                    {
                        query = query.Where(x => lineList.Contains(x.LineChecking ?? ""));
                    }
                }

                if (!string.IsNullOrEmpty(partCode))
                {
                    var partCodeList = partCode.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (partCodeList.Length > 0)
                    {
                        query = query.Where(x => partCodeList.Contains(x.PartCode ?? ""));
                    }
                }

                var data = await query.ToListAsync(); // Always load data for plant-specific dashboard
                var totalQtyNG = data.Sum(x => x.QtyNG ?? 0);

                // Get QTY CHECK data (all checked data, not just NG) with Plant filter
                var qtyCheckQuery = _context.QCHoseData
                    .Where(x => x.Plant == plantName)
                    .AsQueryable();

                // Apply same filters to qtyCheckQuery
                if (!string.IsNullOrEmpty(bulan))
                {
                    var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (bulanList.Length > 0)
                    {
                        var monthYearPairs = bulanList
                            .Select(b => {
                                var parts = b.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    return new { Month = month, Year = year };
                                }
                                return null;
                            })
                            .Where(p => p != null)
                            .ToList();
                        
                        if (monthYearPairs != null && monthYearPairs.Count > 0)
                        {
                            qtyCheckQuery = qtyCheckQuery.Where(x => monthYearPairs.Any(p => p != null && p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tanggal))
                {
                    var tanggalList = tanggal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (tanggalList.Length > 0)
                    {
                        int tanggalYear = DateTime.Now.Year;
                        int tanggalMonth = DateTime.Now.Month;
                        if (!string.IsNullOrEmpty(bulan))
                        {
                            var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (bulanList.Length > 0)
                            {
                                var firstBulan = bulanList.First();
                                var parts = firstBulan.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    tanggalMonth = month;
                                    tanggalYear = year;
                                }
                            }
                        }
                        
                        var tanggalDates = tanggalList
                            .Where(t => int.TryParse(t, out int day) && day >= 1 && day <= 31)
                            .Select(t => {
                                int day = int.Parse(t);
                                if (day <= DateTime.DaysInMonth(tanggalYear, tanggalMonth))
                                {
                                    return new DateTime(tanggalYear, tanggalMonth, day).Date;
                                }
                                return (DateTime?)null;
                            })
                            .Where(dt => dt.HasValue)
                            .Select(dt => dt!.Value)
                            .ToList();
                        if (tanggalDates.Count > 0)
                        {
                            qtyCheckQuery = qtyCheckQuery.Where(x => tanggalDates.Contains(x.TanggalInput.Date));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(line))
                {
                    var lineList = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (lineList.Length > 0)
                    {
                        qtyCheckQuery = qtyCheckQuery.Where(x => lineList.Contains(x.LineChecking));
                    }
                }

                if (!string.IsNullOrEmpty(partCode))
                {
                    var partCodeList = partCode.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (partCodeList.Length > 0)
                    {
                        qtyCheckQuery = qtyCheckQuery.Where(x => partCodeList.Contains(x.PartCode));
                    }
                }

                var totalQtyCheck = await qtyCheckQuery.SumAsync(x => x.QtyCheck ?? 0);
                var totalRR = totalQtyCheck > 0 ? Math.Round((totalQtyNG * 100.0) / totalQtyCheck, 2) : 0;

                var allCheckedData = await qtyCheckQuery.ToListAsync();
                
                // Get all data for this plant (with bulan filter if applicable)
                var allDataQuery = _context.QCHoseData.Where(x => x.Plant == plantName).AsQueryable();
                
                // Apply bulan filter to allData for correct weekly chart display
                if (!string.IsNullOrEmpty(bulan))
                {
                    var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (bulanList.Length > 0)
                    {
                        var monthYearPairs = bulanList
                            .Select(b => {
                                var parts = b.Split('-');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                                {
                                    return new { Month = month, Year = year };
                                }
                                return null;
                            })
                            .Where(p => p != null)
                            .ToList();
                        
                        if (monthYearPairs != null && monthYearPairs.Count > 0)
                        {
                            allDataQuery = allDataQuery.Where(x => monthYearPairs.Any(p => p != null && p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year));
                        }
                    }
                }
                
                var allData = await allDataQuery.ToListAsync();

                // Generate chart data
                var monthlyData = GetMonthlyData(data, allCheckedData, allData, bulan, selectedMonthYears);
                var weeklyData = GetWeeklyData(data, allCheckedData, allData, selectedMonths, selectedMonthYears, bulan);
                var dailyData = GetDailyData(data, allCheckedData, allData, selectedMonth, selectedYear, tanggal);
                var paretoPartData = GetParetoPartData(data, allCheckedData, allData);
                var kriteriaNGData = GetKriteriaNGData(data);
                var rejectionByPartData = GetRejectionByPartData(data, allCheckedData, allData);
                var rejectionByKriteriaData = GetRejectionByKriteriaData(data);

                // Set ViewBag data
                ViewBag.TotalQtyNG = totalQtyNG;
                ViewBag.TotalQtyCheck = totalQtyCheck;
                ViewBag.TotalRR = totalRR;
                ViewBag.MonthlyData = monthlyData;
                ViewBag.WeeklyData = weeklyData;
                ViewBag.DailyData = dailyData;
                ViewBag.ParetoPartData = paretoPartData;
                ViewBag.KriteriaNGData = kriteriaNGData;
                ViewBag.RejectionByPartData = rejectionByPartData;
                ViewBag.RejectionByKriteriaData = rejectionByKriteriaData;
                ViewBag.SelectedMonths = selectedMonths;
                ViewBag.SelectedTanggal = !string.IsNullOrEmpty(tanggal) ? tanggal.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                // Get filter options (filtered by plant)
                ViewBag.JenisNGList = await _context.QCHoseData
                    .Where(x => x.Plant == plantName && !string.IsNullOrEmpty(x.JenisNG))
                    .Select(x => x.JenisNG)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                ViewBag.LineList = await _context.QCHoseData
                    .Where(x => x.Plant == plantName && !string.IsNullOrEmpty(x.LineChecking))
                    .Select(x => x.LineChecking)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                ViewBag.PartCodeList = await _context.QCHoseData
                    .Where(x => x.Plant == plantName && !string.IsNullOrEmpty(x.PartCode))
                    .Select(x => x.PartCode)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToListAsync();

                // Get bulan list (filtered by plant)
                var bulanListQuery = await _context.QCHoseData
                    .Where(x => x.Plant == plantName)
                    .GroupBy(x => new { x.TanggalInput.Year, x.TanggalInput.Month })
                    .Select(g => new { 
                        Year = g.Key.Year, 
                        Month = g.Key.Month
                    })
                    .Where(x => x.Year > 0 && x.Month > 0)
                    .ToListAsync();
                
                ViewBag.BulanList = bulanListQuery
                    .Select(x => new { 
                        Year = x.Year, 
                        Month = x.Month,
                        MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                        Value = $"{x.Month}-{x.Year}"
                    })
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .ToList();

                ViewBag.Bulan = bulan;
                ViewBag.Tanggal = tanggal;
                ViewBag.JenisNG = jenisNG;
                ViewBag.Line = line;
                ViewBag.KategoriNG = kategoriNG;
                ViewBag.PartCode = partCode;

                ViewData["Title"] = $"Dashboard Rejection {plantName}";

                // If AJAX request, return JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        TotalQtyNG = totalQtyNG,
                        TotalQtyCheck = totalQtyCheck,
                        TotalRR = totalRR,
                        MonthlyData = monthlyData,
                        WeeklyData = weeklyData,
                        DailyData = dailyData,
                        ParetoPartData = paretoPartData,
                        KriteriaNGData = kriteriaNGData,
                        RejectionByPartData = rejectionByPartData,
                        RejectionByKriteriaData = rejectionByKriteriaData
                    });
                }

                return View($"Dashboard{plantName}");
            }
            catch (Exception ex)
            {
                ViewData["Title"] = $"Dashboard Rejection {plantName}";
                ViewBag.ErrorMessage = "Error loading data: " + ex.Message;
                return View($"Dashboard{plantName}");
            }
        }

        // Helper methods for data aggregation
        private dynamic GetMonthlyData(List<QCHoseData> ngData, List<QCHoseData> allCheckedData, List<QCHoseData> allData, string? bulan, List<(int Month, int Year)> selectedMonthYears)
        {
            // Get NG data grouped by month-year
            var ngGrouped = ngData
                .GroupBy(x => new { x.TanggalInput.Year, x.TanggalInput.Month })
                .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month}", g => g.Sum(x => x.QtyNG ?? 0));
            
            // Get QTY CHECK data grouped by month-year
            var checkGrouped = allCheckedData
                .GroupBy(x => new { x.TanggalInput.Year, x.TanggalInput.Month })
                .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month}", g => g.Sum(x => x.QtyCheck ?? 0));
            
            // Determine which month-years to show berdasarkan checkbox yang dipilih
            List<(int Month, int Year)> monthsToShow = new List<(int, int)>();
            
            // Prioritas: gunakan selectedMonthYears jika ada (dari checkbox yang dipilih)
            if (selectedMonthYears != null && selectedMonthYears.Count > 0)
            {
                monthsToShow = selectedMonthYears.OrderBy(m => m.Year).ThenBy(m => m.Month).ToList();
            }
            else if (!string.IsNullOrEmpty(bulan))
            {
                // Fallback: parse dari string bulan jika selectedMonthYears kosong
                var bulanList = bulan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (bulanList.Length > 0)
                {
                    monthsToShow = bulanList
                        .Select(b => {
                            var parts = b.Split('-');
                            if (parts.Length == 2 && int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
                            {
                                return (Month: month, Year: year);
                            }
                            else if (int.TryParse(b, out int monthOnly))
                            {
                                return (Month: monthOnly, Year: DateTime.Now.Year);
                            }
                            return (Month: 0, Year: 0);
                        })
                        .Where(p => p.Month > 0)
                        .OrderBy(m => m.Year)
                        .ThenBy(m => m.Month)
                        .ToList();
                }
            }
            
            // Jika tidak ada bulan dipilih di checkbox, ambil semua bulan-tahun dari allData (yang sudah difilter)
            if (monthsToShow.Count == 0)
            {
                monthsToShow = allData
                    .GroupBy(x => new { x.TanggalInput.Year, x.TanggalInput.Month })
                    .Select(g => (Month: g.Key.Month, Year: g.Key.Year))
                    .Distinct()
                    .OrderBy(m => m.Year)
                    .ThenBy(m => m.Month)
                    .ToList();
            }
            
            return monthsToShow
                .Select(m => {
                    var key = $"{m.Year}-{m.Month}";
                    var qtyCheck = checkGrouped.ContainsKey(key) ? checkGrouped[key] : 0;
                    var qtyNG = ngGrouped.ContainsKey(key) ? ngGrouped[key] : 0;
                    // Gunakan nama bulan singkat (Jan, Feb, dst) bukan nama lengkap, tanpa tahun
                    var monthName = GetMonthName(m.Month);
                    return new
                    {
                        label = monthName, // Hanya nama bulan singkat, tanpa tahun
                        qtyCheck = qtyCheck,
                        qtyNG = qtyNG,
                        rr = qtyCheck > 0 ? Math.Round((qtyNG * 100.0) / qtyCheck, 2) : 0,
                        month = m.Month,
                        year = m.Year
                    };
                })
                .OrderBy(x => x.year)
                .ThenBy(x => x.month)
                .ToList();
        }

        private dynamic GetWeeklyData(List<QCHoseData> ngData, List<QCHoseData> allCheckedData, List<QCHoseData> allData, List<int> selectedMonths, List<(int Month, int Year)> selectedMonthYears, string? bulan)
        {
            // Filter data berdasarkan bulan yang dipilih (jika ada)
            List<QCHoseData> filteredNgData = ngData;
            List<QCHoseData> filteredCheckedData = allCheckedData;
            List<QCHoseData> filteredAllData = allData;
            
            bool showAllWeeks = false;
            
            // Jika semua bulan dipilih atau tidak ada bulan dipilih, tampilkan semua minggu
            if (selectedMonthYears == null || selectedMonthYears.Count == 0)
            {
                showAllWeeks = true;
            }
            else if (selectedMonthYears.Count > 0)
            {
                // Filter hanya minggu yang ada di bulan-tahun yang dipilih
                filteredNgData = ngData
                    .Where(x => selectedMonthYears.Any(p => p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year))
                    .ToList();
                filteredCheckedData = allCheckedData
                    .Where(x => selectedMonthYears.Any(p => p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year))
                    .ToList();
                filteredAllData = allData
                    .Where(x => selectedMonthYears.Any(p => p.Month == x.TanggalInput.Month && p.Year == x.TanggalInput.Year))
                    .ToList();
            }
            
            // Group by week of year (minggu dalam tahun, bukan per bulan)
            // Minggu dihitung untuk seluruh tahun, mulai dari Senin pertama tahun tersebut
            var ngGrouped = filteredNgData
                .GroupBy(x => GetWeekOfYear(x.TanggalInput))
                .ToDictionary(g => $"{g.Key.year}-{g.Key.week}", g => g.Sum(x => x.QtyNG ?? 0));
            
            var checkGrouped = filteredCheckedData
                .GroupBy(x => GetWeekOfYear(x.TanggalInput))
                .ToDictionary(g => $"{g.Key.year}-{g.Key.week}", g => g.Sum(x => x.QtyCheck ?? 0));
            
            // Get all weeks to show
            List<(int year, int week, DateTime startDate, DateTime endDate)> allWeeks = new List<(int, int, DateTime, DateTime)>();
            
            if (showAllWeeks)
            {
                // Jika semua bulan dipilih, tampilkan semua minggu dari semua tahun yang ada di data
                var allYears = allData.Select(x => x.TanggalInput.Year).Distinct().OrderBy(y => y).ToList();
                if (allYears.Count == 0)
                {
                    allYears = new List<int> { DateTime.Now.Year };
                }
                
                foreach (var year in allYears)
                {
                    var yearStart = new DateTime(year, 1, 1);
                    var yearEnd = new DateTime(year, 12, 31);
                    var firstMonday = GetStartOfWeek(yearStart);
                    var lastSunday = GetEndOfWeek(yearEnd);
                    
                    for (var date = firstMonday; date <= lastSunday; date = date.AddDays(7))
                    {
                        var weekEnd = date.AddDays(6);
                        if (weekEnd > lastSunday) weekEnd = lastSunday;
                        
                        var weekInfo = GetWeekOfYear(date);
                        allWeeks.Add((weekInfo.year, weekInfo.week, date, weekEnd));
                    }
                }
                
                // Remove duplicates
                allWeeks = allWeeks
                    .GroupBy(w => new { w.year, w.week })
                    .Select(g => g.First())
                    .OrderBy(w => w.year)
                    .ThenBy(w => w.week)
                    .ToList();
            }
            else
            {
                // Jika bulan dipilih, tampilkan semua minggu yang memiliki setidaknya satu hari di bulan yang dipilih
                // Ambil semua tahun dari selectedMonthYears
                var yearsToShow = selectedMonthYears != null ? selectedMonthYears.Select(p => p.Year).Distinct().OrderBy(y => y).ToList() : new List<int>();
                if (yearsToShow.Count == 0)
                {
                    yearsToShow = new List<int> { DateTime.Now.Year };
                }
                
                foreach (var year in yearsToShow)
                {
                    // Ambil bulan-bulan yang dipilih untuk tahun ini
                    var monthsInYear = selectedMonthYears != null ? selectedMonthYears.Where(p => p.Year == year).Select(p => p.Month).Distinct().ToList() : new List<int>();
                    
                    // Untuk setiap bulan yang dipilih, ambil semua minggu yang memiliki setidaknya satu hari di bulan tersebut
                    foreach (var month in monthsInYear)
                    {
                        var monthStart = new DateTime(year, month, 1);
                        var monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                        
                        // Cari minggu pertama yang memiliki hari di bulan ini
                        var firstMonday = GetStartOfWeek(monthStart);
                        var lastSunday = GetEndOfWeek(monthEnd);
                        
                        // Generate semua minggu dari firstMonday sampai lastSunday
                        for (var date = firstMonday; date <= lastSunday; date = date.AddDays(7))
                        {
                            var weekEnd = date.AddDays(6);
                            if (weekEnd > lastSunday) weekEnd = lastSunday;
                            
                            // Pastikan minggu ini memiliki setidaknya satu hari di bulan yang dipilih
                            bool hasDayInMonth = false;
                            for (var d = date; d <= weekEnd; d = d.AddDays(1))
                            {
                                if (d.Month == month && d.Year == year)
                                {
                                    hasDayInMonth = true;
                                    break;
                                }
                            }
                            
                            if (hasDayInMonth)
                            {
                                var weekInfo = GetWeekOfYear(date);
                                allWeeks.Add((weekInfo.year, weekInfo.week, date, weekEnd));
                            }
                        }
                    }
                }
                
                // Remove duplicates dan sort
                allWeeks = allWeeks
                    .GroupBy(w => new { w.year, w.week })
                    .Select(g => g.First())
                    .OrderBy(w => w.year)
                    .ThenBy(w => w.week)
                    .ToList();
            }
            
            return allWeeks
                .Select(w => {
                    var key = $"{w.year}-{w.week}";
                    var qtyCheck = checkGrouped.ContainsKey(key) ? checkGrouped[key] : 0;
                    var qtyNG = ngGrouped.ContainsKey(key) ? ngGrouped[key] : 0;
                    
                    // Format label lebih pendek: "W1" atau "W1 (1-7)" atau "W1 (29-4)" jika lintas bulan
                    string label = "";
                    if (w.startDate.Month == w.endDate.Month)
                    {
                        // Format: "Minggu 1 (1-7)" - hanya tanggal tanpa nama bulan
                        label = $"Minggu {w.week} ({w.startDate.Day}-{w.endDate.Day})";
                    }
                    else
                    {
                        // Format: "Minggu 1 (29-4)" - hanya tanggal tanpa nama bulan untuk lebih pendek
                        label = $"Minggu {w.week} ({w.startDate.Day}-{w.endDate.Day})";
                    }
                    
                    return new
                    {
                        label = label,
                        qtyCheck = qtyCheck,
                        qtyNG = qtyNG,
                        rr = qtyCheck > 0 ? Math.Round((qtyNG * 100.0) / qtyCheck, 2) : 0,
                        week = w.week,
                        year = w.year
                    };
                })
                .OrderBy(x => x.year)
                .ThenBy(x => x.week)
                .ToList();
        }
        
        private DateTime GetStartOfWeek(DateTime date)
        {
            // Cari Senin dari minggu tersebut (ISO week dimulai dari Senin)
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }
        
        private DateTime GetEndOfWeek(DateTime date)
        {
            // Cari Minggu dari minggu tersebut (ISO week berakhir di Minggu)
            int diff = (7 - (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(diff).Date;
        }
        
        private string GetMonthName(int month)
        {
            return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
        }
        
        // Helper method to parse month value from either number (1-12) or abbr name (Jan, Feb, etc.)
        private int ParseMonthValue(string monthStr)
        {
            // Try parse as number first (e.g., "1", "12")
            if (int.TryParse(monthStr, out int monthNum) && monthNum >= 1 && monthNum <= 12)
            {
                return monthNum;
            }
            
            // Try parse as abbreviated month name (e.g., "Jan", "Feb", "Dec")
            for (int i = 1; i <= 12; i++)
            {
                var monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i);
                if (string.Equals(monthStr, monthName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            
            // Fallback: return 0 if invalid
            return 0;
        }

        private dynamic GetDailyData(List<QCHoseData> ngData, List<QCHoseData> allCheckedData, List<QCHoseData> allData, int month, int year, string? tanggal)
        {
            // Get days in selected month
            var daysInMonth = DateTime.DaysInMonth(year, month);
            
            // Group NG data by day
            var ngGrouped = ngData
                .Where(x => x.TanggalInput.Year == year && x.TanggalInput.Month == month)
                .GroupBy(x => x.TanggalInput.Day)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.QtyNG ?? 0));
            
            // Group checked data by day
            var checkedGrouped = allCheckedData
                .Where(x => x.TanggalInput.Year == year && x.TanggalInput.Month == month)
                .GroupBy(x => x.TanggalInput.Day)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.QtyCheck ?? 0));
            
            // Get all days from allData that are in the selected month (semua hari yang sesuai filter)
            var allDaysInMonth = allData
                .Where(x => x.TanggalInput.Year == year && x.TanggalInput.Month == month)
                .Select(x => x.TanggalInput.Day)
                .Distinct()
                .ToList();
            
            // If tanggal filter is set, only show selected days
            List<int> selectedDays = new List<int>();
            if (!string.IsNullOrEmpty(tanggal))
            {
                var tanggalList = tanggal.Split(',', StringSplitOptions.RemoveEmptyEntries);
                selectedDays = tanggalList
                    .Where(t => int.TryParse(t, out int day) && day >= 1 && day <= daysInMonth)
                    .Select(t => int.Parse(t))
                    .ToList();
            }
            
            // Generate data - if tanggal filter is set, only show selected days; otherwise show all days in the month (1-31 atau sesuai jumlah hari di bulan)
            var daysToShow = selectedDays.Count > 0 ? selectedDays : Enumerable.Range(1, daysInMonth).ToList();
            
            return daysToShow
                .Select(day => {
                    var qtyCheck = checkedGrouped.ContainsKey(day) ? checkedGrouped[day] : 0;
                    var qtyNG = ngGrouped.ContainsKey(day) ? ngGrouped[day] : 0;
                    return new
                    {
                        label = day.ToString(),
                        qtyCheck = qtyCheck,
                        qtyNG = qtyNG,
                        rr = qtyCheck > 0 ? Math.Round((qtyNG * 100.0) / qtyCheck, 2) : 0
                    };
                })
                .OrderBy(x => int.Parse(x.label))
                .ToList();
        }

        private dynamic GetParetoPartData(List<QCHoseData> ngData, List<QCHoseData> allCheckedData, List<QCHoseData> allData)
        {
            // Get NG data grouped by PartCode
            var ngGrouped = ngData
                .Where(x => !string.IsNullOrEmpty(x.PartCode))
                .GroupBy(x => x.PartCode)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.QtyNG ?? 0));
            
            // Get all PartCodes from allData (semua part code yang sesuai filter, bukan hanya yang memiliki checked data)
            var allPartCodes = allData
                .Where(x => !string.IsNullOrEmpty(x.PartCode))
                .Select(x => x.PartCode)
                .Distinct()
                .ToList();
            
            // Combine: show QTY NG for parts that have NG, include all parts from allData
            return allPartCodes
                .Select(partCode => new
                {
                    partCode = partCode,
                    qtyNG = ngGrouped.ContainsKey(partCode) ? ngGrouped[partCode] : 0
                })
                .Where(x => x.qtyNG > 0) // Only show parts with NG (Pareto chart hanya menampilkan yang memiliki masalah)
                .OrderByDescending(x => x.qtyNG)
                .Take(10)
                .ToList();
        }

        private dynamic GetKriteriaNGData(List<QCHoseData> data)
        {
            return data
                .Where(x => !string.IsNullOrEmpty(x.JenisNG))
                .GroupBy(x => x.JenisNG)
                .Select(g => new
                {
                    jenisNG = g.Key,
                    qtyNG = g.Sum(x => x.QtyNG ?? 0)
                })
                .OrderByDescending(x => x.qtyNG)
                .ToList();
        }

        private dynamic GetRejectionByPartData(List<QCHoseData> ngData, List<QCHoseData> allCheckedData, List<QCHoseData> allData)
        {
            // Get NG data grouped by PartCode
            var ngGrouped = ngData
                .Where(x => !string.IsNullOrEmpty(x.PartCode))
                .GroupBy(x => x.PartCode)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.QtyNG ?? 0));
            
            // Get QTY CHECK data grouped by PartCode from allCheckedData
            var checkGrouped = allCheckedData
                .Where(x => !string.IsNullOrEmpty(x.PartCode))
                .GroupBy(x => x.PartCode)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.QtyCheck ?? 0));
            
            // Get all PartCodes from allData (semua part code yang sesuai filter, bukan hanya yang memiliki NG atau Check)
            var allPartCodes = allData
                .Where(x => !string.IsNullOrEmpty(x.PartCode))
                .Select(x => x.PartCode)
                .Distinct()
                .ToList();
            
            // Combine data: QTY CHECK from allCheckedData, QTY NG from ngData
            return allPartCodes
                .Select(partCode => new
                {
                    partCode = partCode,
                    qtyCheck = checkGrouped.ContainsKey(partCode) ? checkGrouped[partCode] : 0,
                    qtyNG = ngGrouped.ContainsKey(partCode) ? ngGrouped[partCode] : 0,
                    rr = checkGrouped.ContainsKey(partCode) && checkGrouped[partCode] > 0
                        ? Math.Round((ngGrouped.ContainsKey(partCode) ? ngGrouped[partCode] : 0) * 100.0 / checkGrouped[partCode], 2)
                        : 0
                })
                .OrderByDescending(x => x.qtyNG)
                .ThenBy(x => x.partCode)
                .ToList();
        }

        private dynamic GetRejectionByKriteriaData(List<QCHoseData> data)
        {
            return data
                .Where(x => !string.IsNullOrEmpty(x.JenisNG))
                .GroupBy(x => x.JenisNG)
                .Select(g => new
                {
                    jenisNG = g.Key,
                    qtyNG = g.Sum(x => x.QtyNG ?? 0)
                })
                .OrderByDescending(x => x.qtyNG)
                .ToList();
        }

        private (int year, int week) GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            var week = calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
            return (date.Year, week);
        }
    }
}


