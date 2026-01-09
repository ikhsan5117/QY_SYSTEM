using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;
using AplikasiCheckDimensi.ViewModels;
using ClosedXML.Excel;
using System.Data;

namespace AplikasiCheckDimensi.Controllers
{
    public class ProdukController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProdukController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // GET: Produk
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            var produkList = await _context.Produk
                .OrderByDescending(p => p.TanggalInput)
                .ToListAsync();
            return View(produkList);
        }

        // GET: Produk/Create
        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            return View(new CreateProdukWithStandarViewModel());
        }

        // POST: Produk/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProdukWithStandarViewModel model, IFormFile? VideoFile, IFormFile? GambarPackingFile, IFormFile? GambarCFFile)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            try
            {
                // Handle file uploads
                string? videoPath = null;
                string? gambarPackingPath = null;
                string? gambarCFPath = null;

                if (VideoFile != null && VideoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "videos");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(VideoFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await VideoFile.CopyToAsync(fileStream);
                    }
                    videoPath = "/uploads/videos/" + uniqueFileName;
                }

                if (GambarPackingFile != null && GambarPackingFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "packing");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(GambarPackingFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await GambarPackingFile.CopyToAsync(fileStream);
                    }
                    gambarPackingPath = "/uploads/packing/" + uniqueFileName;
                }

                if (GambarCFFile != null && GambarCFFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "cf");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(GambarCFFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await GambarCFFile.CopyToAsync(fileStream);
                    }
                    gambarCFPath = "/uploads/cf/" + uniqueFileName;
                }

                // Create Produk with all fields
                var produk = new Produk
                {
                    NamaProduk = model.Produk?.NamaProduk ?? "Unnamed Product",
                    PartNo = model.Produk?.PartNo,
                    PartCode = model.Produk?.PartCode,
                    CT = model.Produk?.CT,
                    Plant = model.Produk?.Plant,
                    IdentifikasiItem = model.Produk?.IdentifikasiItem,
                    Operator = model.Produk?.Operator ?? "Unknown",
                    TypeBox = model.Produk?.TypeBox,
                    QtyPerBox = model.Produk?.QtyPerBox,
                    GambarPacking = gambarPackingPath,
                    StandarCF = model.Produk?.StandarCF,
                    GambarCF = gambarCFPath,
                    VideoPath = videoPath,
                    TanggalInput = DateTime.Now
                };
                _context.Add(produk);
                await _context.SaveChangesAsync();

                // Create StandarDimensi only if there's data
                if (model.StandarDimensi != null)
                {
                    var standar = new StandarDimensi
                    {
                        ProdukId = produk.Id,
                        NamaDimensi = model.Produk?.PartCode ?? $"Dimension-{produk.Id}",
                        // Sisi A
                        InnerDiameter_SisiA_Min = model.StandarDimensi.InnerDiameter_SisiA_Min,
                        InnerDiameter_SisiA_Max = model.StandarDimensi.InnerDiameter_SisiA_Max,
                        OuterDiameter_SisiA_Min = model.StandarDimensi.OuterDiameter_SisiA_Min,
                        OuterDiameter_SisiA_Max = model.StandarDimensi.OuterDiameter_SisiA_Max,
                        Thickness_SisiA_Min = model.StandarDimensi.Thickness_SisiA_Min,
                        Thickness_SisiA_Max = model.StandarDimensi.Thickness_SisiA_Max,
                        // Sisi B
                        InnerDiameter_SisiB_Min = model.StandarDimensi.InnerDiameter_SisiB_Min,
                        InnerDiameter_SisiB_Max = model.StandarDimensi.InnerDiameter_SisiB_Max,
                        OuterDiameter_SisiB_Min = model.StandarDimensi.OuterDiameter_SisiB_Min,
                        OuterDiameter_SisiB_Max = model.StandarDimensi.OuterDiameter_SisiB_Max,
                        Thickness_SisiB_Min = model.StandarDimensi.Thickness_SisiB_Min,
                        Thickness_SisiB_Max = model.StandarDimensi.Thickness_SisiB_Max,
                        // Dimensi lainnya
                        Panjang_Min = model.StandarDimensi.Panjang_Min,
                        Panjang_Max = model.StandarDimensi.Panjang_Max,
                        Tinggi_Min = model.StandarDimensi.Tinggi_Min,
                        Tinggi_Max = model.StandarDimensi.Tinggi_Max,
                        Radius_Min = model.StandarDimensi.Radius_Min,
                        Radius_Max = model.StandarDimensi.Radius_Max,
                        DimensiA_Min = model.StandarDimensi.DimensiA_Min,
                        DimensiA_Max = model.StandarDimensi.DimensiA_Max,
                        DimensiB_Min = model.StandarDimensi.DimensiB_Min,
                        DimensiB_Max = model.StandarDimensi.DimensiB_Max,
                        Sudut_Min = model.StandarDimensi.Sudut_Min,
                        Sudut_Max = model.StandarDimensi.Sudut_Max
                    };
                    _context.StandarDimensi.Add(standar);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Detail), new { id = produk.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving product: " + ex.Message);
                return View(model);
            }
        }

        // GET: Produk/Detail/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id == null)
            {
                return NotFound();
            }

            var produk = await _context.Produk
                .FirstOrDefaultAsync(m => m.Id == id);

            if (produk == null)
            {
                return NotFound();
            }

            // Get standar dimensi untuk produk ini
            var standarList = await _context.StandarDimensi
                .Where(s => s.ProdukId == id)
                .ToListAsync();

            ViewBag.StandarDimensi = standarList;
            ViewBag.StandarList = standarList;

            return View(produk);
        }

        // GET: Produk/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id == null)
            {
                return NotFound();
            }

            var produk = await _context.Produk.FindAsync(id);
            if (produk == null)
            {
                return NotFound();
            }
            return View(produk);
        }

        // POST: Produk/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produk produk, IFormFile? VideoFile, IFormFile? GambarPackingFile, IFormFile? GambarCFFile)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id != produk.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing product from database to preserve file paths
                    var existingProduk = await _context.Produk.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduk == null)
                    {
                        return NotFound();
                    }

                    // Handle Video file upload
                    if (VideoFile != null && VideoFile.Length > 0)
                    {
                        // Delete old video if exists
                        if (!string.IsNullOrEmpty(existingProduk.VideoPath))
                        {
                            var oldVideoPath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduk.VideoPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldVideoPath))
                            {
                                System.IO.File.Delete(oldVideoPath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "videos");
                        Directory.CreateDirectory(uploadsFolder);
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(VideoFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await VideoFile.CopyToAsync(fileStream);
                        }
                        produk.VideoPath = "/uploads/videos/" + uniqueFileName;
                    }
                    else
                    {
                        produk.VideoPath = existingProduk.VideoPath;
                    }

                    // Handle Packing image upload
                    if (GambarPackingFile != null && GambarPackingFile.Length > 0)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingProduk.GambarPacking))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduk.GambarPacking.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "packing");
                        Directory.CreateDirectory(uploadsFolder);
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(GambarPackingFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await GambarPackingFile.CopyToAsync(fileStream);
                        }
                        produk.GambarPacking = "/uploads/packing/" + uniqueFileName;
                    }
                    else
                    {
                        produk.GambarPacking = existingProduk.GambarPacking;
                    }

                    // Handle CF image upload
                    if (GambarCFFile != null && GambarCFFile.Length > 0)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingProduk.GambarCF))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduk.GambarCF.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "cf");
                        Directory.CreateDirectory(uploadsFolder);
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(GambarCFFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await GambarCFFile.CopyToAsync(fileStream);
                        }
                        produk.GambarCF = "/uploads/cf/" + uniqueFileName;
                    }
                    else
                    {
                        produk.GambarCF = existingProduk.GambarCF;
                    }

                    _context.Update(produk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdukExists(produk.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(produk);
        }

        // GET: Produk/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id == null)
            {
                return NotFound();
            }

            var produk = await _context.Produk
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produk == null)
            {
                return NotFound();
            }

            return View(produk);
        }

        // POST: Produk/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            var produk = await _context.Produk.FindAsync(id);
            if (produk != null)
            {
                _context.Produk.Remove(produk);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Produk/DeleteMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (ids == null || ids.Count == 0)
            {
                TempData["Error"] = "Tidak ada produk yang dipilih";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var produks = await _context.Produk
                    .Where(p => ids.Contains(p.Id))
                    .ToListAsync();

                if (produks.Count > 0)
                {
                    _context.Produk.RemoveRange(produks);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Berhasil menghapus {produks.Count} produk";
                }
                else
                {
                    TempData["Warning"] = "Tidak ada produk yang dihapus";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Gagal menghapus produk: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Produk/AddStandar/5
        public async Task<IActionResult> AddStandar(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id == null)
            {
                return NotFound();
            }

            var produk = await _context.Produk.FindAsync(id);
            if (produk == null)
            {
                return NotFound();
            }

            var standar = new StandarDimensi
            {
                ProdukId = id.Value
            };

            ViewBag.Produk = produk;
            return View(standar);
        }

        // POST: Produk/AddStandar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStandar(StandarDimensi standar)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (ModelState.IsValid)
            {
                _context.Add(standar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id = standar.ProdukId });
            }

            var produk = await _context.Produk.FindAsync(standar.ProdukId);
            ViewBag.Produk = produk;
            return View(standar);
        }

        // GET: Produk/EditStandar/5
        public async Task<IActionResult> EditStandar(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id == null)
            {
                return NotFound();
            }

            var standar = await _context.StandarDimensi
                .Include(s => s.Produk)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (standar == null)
            {
                return NotFound();
            }

            ViewBag.Produk = standar.Produk;
            return View(standar);
        }

        // POST: Produk/EditStandar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStandar(int id, StandarDimensi standar)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id != standar.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(standar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StandarDimensiExists(standar.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Detail), new { id = standar.ProdukId });
            }

            var produk = await _context.Produk.FindAsync(standar.ProdukId);
            ViewBag.Produk = produk;
            return View(standar);
        }

        // POST: Produk/DeleteStandar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStandar(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            var standar = await _context.StandarDimensi.FindAsync(id);
            if (standar != null)
            {
                var produkId = standar.ProdukId;
                _context.StandarDimensi.Remove(standar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id = produkId });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProdukExists(int id)
        {
            return _context.Produk.Any(e => e.Id == id);
        }

        private bool StandarDimensiExists(int id)
        {
            return _context.StandarDimensi.Any(e => e.Id == id);
        }

        // API: Get Standar Dimensi by Product ID
        [HttpGet]
        public async Task<IActionResult> GetStandarDimensi(int id)
        {
            var standarList = await _context.StandarDimensi
                .Where(s => s.ProdukId == id)
                .ToListAsync();

            return Json(standarList);
        }

        // API: Get Packing Info by Product ID
        [HttpGet]
        public async Task<IActionResult> GetPackingInfo(int id)
        {
            var produk = await _context.Produk
                .Where(p => p.Id == id)
                .Select(p => new {
                    typeBox = p.TypeBox,
                    qtyPerBox = p.QtyPerBox,
                    gambarPacking = p.GambarPacking
                })
                .FirstOrDefaultAsync();

            return Json(produk);
        }

        // API: Get Checking Fixture Info by Product ID
        [HttpGet]
        public async Task<IActionResult> GetCFInfo(int id)
        {
            var produk = await _context.Produk
                .Where(p => p.Id == id)
                .Select(p => new {
                    standarCF = p.StandarCF,
                    gambarCF = p.GambarCF
                })
                .FirstOrDefaultAsync();

            return Json(produk);
        }

        // Download Template Excel
        [HttpGet]
        public IActionResult DownloadTemplate()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Template Produk");

            // Header styling
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#3b82f6");
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers - Informasi Produk
            worksheet.Cell(1, 1).Value = "Part Name";
            worksheet.Cell(1, 2).Value = "Part No";
            worksheet.Cell(1, 3).Value = "Part Code";
            worksheet.Cell(1, 4).Value = "CT (Cycle Time)";
            worksheet.Cell(1, 5).Value = "Plant";
            worksheet.Cell(1, 6).Value = "Identifikasi Item";
            worksheet.Cell(1, 7).Value = "Operator / PIC";
            worksheet.Cell(1, 8).Value = "Type Box";
            worksheet.Cell(1, 9).Value = "Qty Per Box";
            worksheet.Cell(1, 10).Value = "Standar CF";
            
            // Headers - SISI A - Inner Diameter
            worksheet.Cell(1, 11).Value = "Inner Ø A Min";
            worksheet.Cell(1, 12).Value = "Inner Ø A Max";
            
            // Headers - SISI A - Outer Diameter
            worksheet.Cell(1, 13).Value = "Outer Ø A Min";
            worksheet.Cell(1, 14).Value = "Outer Ø A Max";
            
            // Headers - SISI A - Thickness
            worksheet.Cell(1, 15).Value = "Thickness A Min";
            worksheet.Cell(1, 16).Value = "Thickness A Max";
            
            // Headers - SISI B - Inner Diameter
            worksheet.Cell(1, 17).Value = "Inner Ø B Min";
            worksheet.Cell(1, 18).Value = "Inner Ø B Max";
            
            // Headers - SISI B - Outer Diameter
            worksheet.Cell(1, 19).Value = "Outer Ø B Min";
            worksheet.Cell(1, 20).Value = "Outer Ø B Max";
            
            // Headers - SISI B - Thickness
            worksheet.Cell(1, 21).Value = "Thickness B Min";
            worksheet.Cell(1, 22).Value = "Thickness B Max";
            
            // Headers - Standar Dimensi (Panjang)
            worksheet.Cell(1, 23).Value = "Panjang Min";
            worksheet.Cell(1, 24).Value = "Panjang Max";
            
            // Headers - Standar Dimensi (Tinggi)
            worksheet.Cell(1, 25).Value = "Tinggi Min";
            worksheet.Cell(1, 26).Value = "Tinggi Max";
            
            // Headers - Standar Dimensi (Radius)
            worksheet.Cell(1, 27).Value = "Radius Min";
            worksheet.Cell(1, 28).Value = "Radius Max";
            
            // Headers - Standar Dimensi (Dimensi A)
            worksheet.Cell(1, 29).Value = "Dimensi A Min";
            worksheet.Cell(1, 30).Value = "Dimensi A Max";
            
            // Headers - Standar Dimensi (Dimensi B)
            worksheet.Cell(1, 31).Value = "Dimensi B Min";
            worksheet.Cell(1, 32).Value = "Dimensi B Max";
            
            // Headers - Standar Dimensi (Sudut)
            worksheet.Cell(1, 33).Value = "Sudut Min";
            worksheet.Cell(1, 34).Value = "Sudut Max";

            // Column widths
            for (int i = 1; i <= 10; i++)
            {
                worksheet.Column(i).Width = i == 1 ? 25 : i == 6 || i == 7 ? 20 : 15;
            }
            for (int i = 11; i <= 34; i++)
            {
                worksheet.Column(i).Width = 12;
            }

            // Example data row 1
            worksheet.Cell(2, 1).Value = "Grommet";
            worksheet.Cell(2, 2).Value = "NA1530";
            worksheet.Cell(2, 3).Value = "NA1530";
            worksheet.Cell(2, 4).Value = "45";
            worksheet.Cell(2, 5).Value = "Plant 1";
            worksheet.Cell(2, 6).Value = "Rubber Parts";
            worksheet.Cell(2, 7).Value = "Quality Team";
            worksheet.Cell(2, 8).Value = "Box A";
            worksheet.Cell(2, 9).Value = "100";
            worksheet.Cell(2, 10).Value = "CF-001";
            // Standar Dimensi Sisi A - contoh Inner Diameter dan Outer Diameter
            worksheet.Cell(2, 11).Value = "4.8";
            worksheet.Cell(2, 12).Value = "5.2";
            worksheet.Cell(2, 13).Value = "9.8";
            worksheet.Cell(2, 14).Value = "10.2";
            worksheet.Cell(2, 15).Value = "2.8";
            worksheet.Cell(2, 16).Value = "3.2";

            // Example data row 2
            worksheet.Cell(3, 1).Value = "Hose Radiator";
            worksheet.Cell(3, 2).Value = "TA1450";
            worksheet.Cell(3, 3).Value = "TA1450";
            worksheet.Cell(3, 4).Value = "60";
            worksheet.Cell(3, 5).Value = "Plant 1";
            worksheet.Cell(3, 6).Value = "Hose Assembly";
            worksheet.Cell(3, 7).Value = "Quality Team";
            worksheet.Cell(3, 8).Value = "Box B";
            worksheet.Cell(3, 9).Value = "50";
            worksheet.Cell(3, 10).Value = "CF-002";
            // Standar Dimensi Sisi B - contoh Inner dan Thickness
            worksheet.Cell(3, 17).Value = "4.5";
            worksheet.Cell(3, 18).Value = "5.0";
            worksheet.Cell(3, 21).Value = "2.9";
            worksheet.Cell(3, 22).Value = "3.1";
            // Panjang
            worksheet.Cell(3, 23).Value = "99.5";
            worksheet.Cell(3, 24).Value = "100.5";

            // Instructions sheet
            var instructionSheet = workbook.Worksheets.Add("Instruksi");
            instructionSheet.Cell(1, 1).Value = "INSTRUKSI PENGGUNAAN TEMPLATE";
            instructionSheet.Cell(1, 1).Style.Font.Bold = true;
            instructionSheet.Cell(1, 1).Style.Font.FontSize = 14;
            
            int iRow = 3;
            instructionSheet.Cell(iRow++, 1).Value = "CARA PENGGUNAAN:";
            instructionSheet.Cell(iRow++, 1).Value = "1. Isi data produk pada sheet 'Template Produk'";
            instructionSheet.Cell(iRow++, 1).Value = "2. Hapus baris contoh (baris 2 dan 3) sebelum mengisi data Anda";
            instructionSheet.Cell(iRow++, 1).Value = "3. Upload file Excel melalui menu Master Produk";
            instructionSheet.Cell(iRow++, 1).Value = "4. Setelah upload, tambahkan gambar dan video melalui menu Edit Produk";
            
            iRow++;
            instructionSheet.Cell(iRow++, 1).Value = "STANDAR DIMENSI (Kolom 11-34):";
            instructionSheet.Cell(iRow++, 1).Value = "SISI A:";
            instructionSheet.Cell(iRow++, 1).Value = "• Inner Ø A Min/Max (Kolom 11-12) = Diameter dalam Sisi A (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Outer Ø A Min/Max (Kolom 13-14) = Diameter luar Sisi A (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Thickness A Min/Max (Kolom 15-16) = Ketebalan Sisi A (mm)";
            iRow++;
            instructionSheet.Cell(iRow++, 1).Value = "SISI B:";
            instructionSheet.Cell(iRow++, 1).Value = "• Inner Ø B Min/Max (Kolom 17-18) = Diameter dalam Sisi B (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Outer Ø B Min/Max (Kolom 19-20) = Diameter luar Sisi B (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Thickness B Min/Max (Kolom 21-22) = Ketebalan Sisi B (mm)";
            iRow++;
            instructionSheet.Cell(iRow++, 1).Value = "DIMENSI LAINNYA:";
            instructionSheet.Cell(iRow++, 1).Value = "• Panjang Min/Max (Kolom 23-24) = Panjang (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Tinggi Min/Max (Kolom 25-26) = Tinggi (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Radius Min/Max (Kolom 27-28) = Radius (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Dimensi A Min/Max (Kolom 29-30) = Dimensi A (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Dimensi B Min/Max (Kolom 31-32) = Dimensi B (mm)";
            instructionSheet.Cell(iRow++, 1).Value = "• Sudut Min/Max (Kolom 33-34) = Sudut (derajat)";
            
            iRow++;
            instructionSheet.Cell(iRow++, 1).Value = "CATATAN:";
            instructionSheet.Cell(iRow++, 1).Value = "• Standar dimensi bersifat opsional, isi hanya yang diperlukan";
            instructionSheet.Cell(iRow++, 1).Value = "• Gunakan titik (.) untuk angka desimal, contoh: 5.2";
            instructionSheet.Cell(iRow++, 1).Value = "• Nilai Min harus ≤ nilai Max";
            
            instructionSheet.Column(1).Width = 80;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template_Produk.xlsx");
        }

        // Upload Excel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");

            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "Silakan pilih file Excel untuk diupload.";
                return RedirectToAction(nameof(Index));
            }

            var fileExtension = Path.GetExtension(excelFile.FileName).ToLower();
            if (fileExtension != ".xlsx" && fileExtension != ".xls")
            {
                TempData["Error"] = "Format file harus Excel (.xlsx atau .xls).";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet("Template Produk");

                if (worksheet == null)
                {
                    TempData["Error"] = "Sheet 'Template Produk' tidak ditemukan. Pastikan menggunakan template yang benar.";
                    return RedirectToAction(nameof(Index));
                }

                var successCount = 0;
                var errorCount = 0;
                var errors = new List<string>();

                // Start from row 2 (skip header)
                var rows = worksheet.RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    try
                    {
                        var partName = row.Cell(1).GetString().Trim();
                        var operatorName = row.Cell(7).GetString().Trim();

                        var produk = new Produk
                        {
                            NamaProduk = partName,
                            PartNo = row.Cell(2).GetString().Trim(),
                            PartCode = row.Cell(3).GetString().Trim(),
                            CT = !string.IsNullOrEmpty(row.Cell(4).GetString()) ? row.Cell(4).GetString().Trim() : null,
                            Plant = row.Cell(5).GetString().Trim(),
                            IdentifikasiItem = row.Cell(6).GetString().Trim(),
                            Operator = operatorName,
                            TypeBox = row.Cell(8).GetString().Trim(),
                            QtyPerBox = !string.IsNullOrEmpty(row.Cell(9).GetString()) && int.TryParse(row.Cell(9).GetString(), out int qty) ? qty : (int?)null,
                            StandarCF = row.Cell(10).GetString().Trim(),
                            TanggalInput = DateTime.Now
                        };

                        _context.Produk.Add(produk);
                        await _context.SaveChangesAsync(); // Save to get ProdukId
                        
                        // Parse Standar Dimensi (columns 11-34)
                        var hasAnyDimensi = false;
                        var standarDimensi = new StandarDimensi
                        {
                            ProdukId = produk.Id
                        };
                        
                        // SISI A - Inner Diameter (columns 11-12)
                        if (TryGetDecimal(row, 11, out decimal innerDiameterSisiAMin))
                        {
                            standarDimensi.InnerDiameter_SisiA_Min = innerDiameterSisiAMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 12, out decimal innerDiameterSisiAMax))
                        {
                            standarDimensi.InnerDiameter_SisiA_Max = innerDiameterSisiAMax;
                            hasAnyDimensi = true;
                        }
                        
                        // SISI A - Outer Diameter (columns 13-14)
                        if (TryGetDecimal(row, 13, out decimal outerDiameterSisiAMin))
                        {
                            standarDimensi.OuterDiameter_SisiA_Min = outerDiameterSisiAMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 14, out decimal outerDiameterSisiAMax))
                        {
                            standarDimensi.OuterDiameter_SisiA_Max = outerDiameterSisiAMax;
                            hasAnyDimensi = true;
                        }
                        
                        // SISI A - Thickness (columns 15-16)
                        if (TryGetDecimal(row, 15, out decimal thicknessSisiAMin))
                        {
                            standarDimensi.Thickness_SisiA_Min = thicknessSisiAMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 16, out decimal thicknessSisiAMax))
                        {
                            standarDimensi.Thickness_SisiA_Max = thicknessSisiAMax;
                            hasAnyDimensi = true;
                        }
                        
                        // SISI B - Inner Diameter (columns 17-18)
                        if (TryGetDecimal(row, 17, out decimal innerDiameterSisiBMin))
                        {
                            standarDimensi.InnerDiameter_SisiB_Min = innerDiameterSisiBMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 18, out decimal innerDiameterSisiBMax))
                        {
                            standarDimensi.InnerDiameter_SisiB_Max = innerDiameterSisiBMax;
                            hasAnyDimensi = true;
                        }
                        
                        // SISI B - Outer Diameter (columns 19-20)
                        if (TryGetDecimal(row, 19, out decimal outerDiameterSisiBMin))
                        {
                            standarDimensi.OuterDiameter_SisiB_Min = outerDiameterSisiBMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 20, out decimal outerDiameterSisiBMax))
                        {
                            standarDimensi.OuterDiameter_SisiB_Max = outerDiameterSisiBMax;
                            hasAnyDimensi = true;
                        }
                        
                        // SISI B - Thickness (columns 21-22)
                        if (TryGetDecimal(row, 21, out decimal thicknessSisiBMin))
                        {
                            standarDimensi.Thickness_SisiB_Min = thicknessSisiBMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 22, out decimal thicknessSisiBMax))
                        {
                            standarDimensi.Thickness_SisiB_Max = thicknessSisiBMax;
                            hasAnyDimensi = true;
                        }
                        
                        // Panjang (columns 23-24)
                        if (TryGetDecimal(row, 23, out decimal panjangMin))
                        {
                            standarDimensi.Panjang_Min = panjangMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 24, out decimal panjangMax))
                        {
                            standarDimensi.Panjang_Max = panjangMax;
                            hasAnyDimensi = true;
                        }
                        
                        // Tinggi (columns 25-26)
                        if (TryGetDecimal(row, 25, out decimal tinggiMin))
                        {
                            standarDimensi.Tinggi_Min = tinggiMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 26, out decimal tinggiMax))
                        {
                            standarDimensi.Tinggi_Max = tinggiMax;
                            hasAnyDimensi = true;
                        }
                        
                        // Radius (columns 27-28)
                        if (TryGetDecimal(row, 27, out decimal radiusMin))
                        {
                            standarDimensi.Radius_Min = radiusMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 28, out decimal radiusMax))
                        {
                            standarDimensi.Radius_Max = radiusMax;
                            hasAnyDimensi = true;
                        }
                        
                        // Dimensi A (columns 29-30)
                        if (TryGetDecimal(row, 29, out decimal dimensiAMin))
                        {
                            standarDimensi.DimensiA_Min = dimensiAMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 30, out decimal dimensiAMax))
                        {
                            standarDimensi.DimensiA_Max = dimensiAMax;
                            hasAnyDimensi = true;
                        }
                        
                        // Dimensi B (columns 31-32)
                        if (TryGetDecimal(row, 31, out decimal dimensiBMin))
                        {
                            standarDimensi.DimensiB_Min = dimensiBMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 32, out decimal dimensiBMax))
                        {
                            standarDimensi.DimensiB_Max = dimensiBMax;
                            hasAnyDimensi = true;
                        }
                        
                        // Sudut (columns 33-34)
                        if (TryGetDecimal(row, 33, out decimal sudutMin))
                        {
                            standarDimensi.Sudut_Min = sudutMin;
                            hasAnyDimensi = true;
                        }
                        if (TryGetDecimal(row, 34, out decimal sudutMax))
                        {
                            standarDimensi.Sudut_Max = sudutMax;
                            hasAnyDimensi = true;
                        }
                        
                        // Only save StandarDimensi if at least one dimension is provided
                        if (hasAnyDimensi)
                        {
                            _context.StandarDimensi.Add(standarDimensi);
                            await _context.SaveChangesAsync();
                        }

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Baris {row.RowNumber()}: {ex.Message}");
                        errorCount++;
                    }
                }

                if (successCount > 0)
                {
                    TempData["Success"] = $"Berhasil mengupload {successCount} produk.";
                }

                if (errorCount > 0)
                {
                    TempData["Warning"] = $"Terdapat {errorCount} baris yang gagal diupload. Detail: " + string.Join("; ", errors.Take(5));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Gagal memproses file Excel: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
        
        // Helper method to parse decimal from Excel cell
        private bool TryGetDecimal(IXLRow row, int columnNumber, out decimal value)
        {
            value = 0;
            try
            {
                var cellValue = row.Cell(columnNumber).GetString().Trim();
                if (string.IsNullOrEmpty(cellValue))
                    return false;
                    
                return decimal.TryParse(cellValue, System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture, out value);
            }
            catch
            {
                return false;
            }
        }
    }
}
