using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;
using AplikasiCheckDimensi.ViewModels;

namespace AplikasiCheckDimensi.Controllers
{
    public class DimensiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DimensiController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        // GET: Dimensi/Input
        public async Task<IActionResult> Input(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            // Pass session data to view
            ViewBag.NamaLengkap = HttpContext.Session.GetString("NamaLengkap");
            ViewBag.Plant = HttpContext.Session.GetString("Plant");
            ViewBag.Grup = HttpContext.Session.GetString("Grup");
            
            var standarList = await _context.StandarDimensi
                .Include(s => s.Produk)
                .ToListAsync();

            StandarDimensi? selectedStandar = null;
            if (id.HasValue)
            {
                selectedStandar = await _context.StandarDimensi.FindAsync(id.Value);
            }

            var viewModel = new InputDimensiViewModel
            {
                StandarDimensiList = standarList,
                SelectedStandarDimensi = selectedStandar
            };

            return View(viewModel);
        }

        // POST: Dimensi/SimpanInput
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimpanInput(InputAktual inputAktual)
        {
            if (!IsLoggedIn()) 
                return Json(new { success = false, message = "Not logged in" });
            
            // Debug logging
            Console.WriteLine($"=== DEBUG INPUT AKTUAL ===");
            Console.WriteLine($"StandarDimensiId: {inputAktual.StandarDimensiId}");
            Console.WriteLine($"NamaPIC: {inputAktual.NamaPIC}");
            Console.WriteLine($"Plant: {inputAktual.Plant}");
            Console.WriteLine($"Grup: {inputAktual.Grup}");
            Console.WriteLine($"NilaiInnerDiameter: {inputAktual.NilaiInnerDiameter}");
            Console.WriteLine($"NilaiOuterDiameter: {inputAktual.NilaiOuterDiameter}");
            Console.WriteLine($"NilaiThickness: {inputAktual.NilaiThickness}");
            Console.WriteLine($"NilaiPanjang: {inputAktual.NilaiPanjang}");
            Console.WriteLine($"NilaiTinggi: {inputAktual.NilaiTinggi}");
            Console.WriteLine($"NilaiRadius: {inputAktual.NilaiRadius}");
            Console.WriteLine($"NilaiDimensiA: {inputAktual.NilaiDimensiA}");
            Console.WriteLine($"NilaiDimensiB: {inputAktual.NilaiDimensiB}");
            Console.WriteLine($"NilaiSudut: {inputAktual.NilaiSudut}");
            Console.WriteLine($"========================");

            try
            {
                inputAktual.TanggalInput = DateTime.Now;
                
                // Load StandarDimensi to check status
                var standar = await _context.StandarDimensi.FindAsync(inputAktual.StandarDimensiId);
                if (standar != null)
                {
                    inputAktual.StandarDimensi = standar;
                    // Calculate overall status based on all dimensions
                    inputAktual.Status = inputAktual.IsAllOK ? "OK" : "NG";
                }
                else
                {
                    inputAktual.Status = "NG"; // Default to NG if standar not found
                }
                
                _context.Add(inputAktual);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, id = inputAktual.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Dimensi/Riwayat
        public async Task<IActionResult> Riwayat()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            // Get logged in user's name and role
            var userName = HttpContext.Session.GetString("NamaLengkap");
            var userRole = HttpContext.Session.GetString("Role");
            
            var query = _context.InputAktual
                .Include(i => i.StandarDimensi)
                .ThenInclude(s => s!.Produk)
                .AsQueryable();
            
            // If user is not Admin, filter by their name (NamaPIC)
            if (userRole != "Admin")
            {
                query = query.Where(i => i.NamaPIC == userName);
            }
            
            var riwayat = await query
                .OrderByDescending(i => i.TanggalInput)
                .ToListAsync();

            return View(riwayat);
        }

        // GET: Dimensi/Detail/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id == null)
            {
                return NotFound();
            }

            var inputAktual = await _context.InputAktual
                .Include(i => i.StandarDimensi)
                .ThenInclude(s => s!.Produk)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inputAktual == null)
            {
                return NotFound();
            }

            return View(inputAktual);
        }

        // GET: Dimensi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id == null)
            {
                return NotFound();
            }

            var inputAktual = await _context.InputAktual
                .Include(i => i.StandarDimensi)
                .ThenInclude(s => s!.Produk)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inputAktual == null)
            {
                return NotFound();
            }

            return View(inputAktual);
        }

        // POST: Dimensi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InputAktual inputAktual)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id != inputAktual.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inputAktual);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InputAktualExists(inputAktual.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Riwayat));
            }
            return View(inputAktual);
        }

        // GET: Dimensi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            if (id == null)
            {
                return NotFound();
            }

            var inputAktual = await _context.InputAktual
                .Include(i => i.StandarDimensi)
                .ThenInclude(s => s!.Produk)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inputAktual == null)
            {
                return NotFound();
            }

            return View(inputAktual);
        }

        // POST: Dimensi/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            var inputAktual = await _context.InputAktual.FindAsync(id);
            if (inputAktual != null)
            {
                _context.InputAktual.Remove(inputAktual);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Riwayat));
        }

        private bool InputAktualExists(int id)
        {
            return _context.InputAktual.Any(e => e.Id == id);
        }

        // GET: Dimensi/GetStandarDetail/5
        [HttpGet]
        public async Task<IActionResult> GetStandarDetail(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            var standar = await _context.StandarDimensi
                .Include(s => s.Produk)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (standar == null)
            {
                return NotFound();
            }

            return Json(standar);
        }

        // GET: Dimensi/Analisis
        public async Task<IActionResult> Analisis()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            var standarList = await _context.StandarDimensi
                .Include(s => s.Produk)
                .OrderBy(s => s.Produk!.NamaProduk)
                .ThenBy(s => s.NamaDimensi)
                .ToListAsync();

            return View(standarList);
        }

        // GET: Dimensi/GetTrendData
        [HttpGet]
        public async Task<IActionResult> GetTrendData(int standarId, string dimensiType)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
            
            var data = await _context.InputAktual
                .Include(i => i.StandarDimensi)
                .ThenInclude(s => s!.Produk)
                .Where(i => i.StandarDimensiId == standarId)
                .OrderBy(i => i.TanggalInput)
                .Select(i => new
                {
                    i.Id,
                    i.TanggalInput,
                    i.NamaPIC,
                    i.Plant,
                    i.Grup,
                    // Nilai aktual Sisi A
                    i.NilaiInnerDiameter_SisiA,
                    i.NilaiOuterDiameter_SisiA,
                    i.NilaiThickness_SisiA,
                    // Nilai aktual Sisi B
                    i.NilaiInnerDiameter_SisiB,
                    i.NilaiOuterDiameter_SisiB,
                    i.NilaiThickness_SisiB,
                    // Nilai aktual lainnya
                    i.NilaiPanjang,
                    i.NilaiTinggi,
                    i.NilaiRadius,
                    i.NilaiDimensiA,
                    i.NilaiDimensiB,
                    i.NilaiSudut,
                    // Standar
                    StandarDimensi = new
                    {
                        // Sisi A
                        i.StandarDimensi!.InnerDiameter_SisiA_Min,
                        i.StandarDimensi.InnerDiameter_SisiA_Max,
                        i.StandarDimensi.OuterDiameter_SisiA_Min,
                        i.StandarDimensi.OuterDiameter_SisiA_Max,
                        i.StandarDimensi.Thickness_SisiA_Min,
                        i.StandarDimensi.Thickness_SisiA_Max,
                        // Sisi B
                        i.StandarDimensi.InnerDiameter_SisiB_Min,
                        i.StandarDimensi.InnerDiameter_SisiB_Max,
                        i.StandarDimensi.OuterDiameter_SisiB_Min,
                        i.StandarDimensi.OuterDiameter_SisiB_Max,
                        i.StandarDimensi.Thickness_SisiB_Min,
                        i.StandarDimensi.Thickness_SisiB_Max,
                        // Lainnya
                        i.StandarDimensi.Panjang_Min,
                        i.StandarDimensi.Panjang_Max,
                        i.StandarDimensi.Tinggi_Min,
                        i.StandarDimensi.Tinggi_Max,
                        i.StandarDimensi.Radius_Min,
                        i.StandarDimensi.Radius_Max,
                        i.StandarDimensi.DimensiA_Min,
                        i.StandarDimensi.DimensiA_Max,
                        i.StandarDimensi.DimensiB_Min,
                        i.StandarDimensi.DimensiB_Max,
                        i.StandarDimensi.Sudut_Min,
                        i.StandarDimensi.Sudut_Max,
                        i.StandarDimensi.NamaDimensi,
                        Produk = i.StandarDimensi.Produk!.NamaProduk
                    }
                })
                .ToListAsync();

            return Json(data);
        }
    }
}
