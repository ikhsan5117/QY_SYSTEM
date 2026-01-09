using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi.Controllers
{
    public class MasterDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MasterDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // GET: MasterData/Index - Halaman manage master data
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            // Get semua master data dikelompokkan berdasarkan tipe
            var masterData = await _context.MasterData
                .Where(m => m.IsActive)
                .OrderBy(m => m.Tipe)
                .ThenBy(m => m.Nilai)
                .ToListAsync();

            ViewBag.LineChecking = masterData.Where(m => m.Tipe == "LineChecking").ToList();
            ViewBag.Inspectors = masterData.Where(m => m.Tipe == "Inspector").ToList();
            ViewBag.GroupChecking = masterData.Where(m => m.Tipe == "GroupChecking").ToList();
            ViewBag.JenisNG = masterData.Where(m => m.Tipe == "JenisNG").ToList();
            ViewBag.LineStop = masterData.Where(m => m.Tipe == "LineStop").ToList();
            ViewBag.PartCode = masterData.Where(m => m.Tipe == "PartCode").ToList();

            ViewData["Title"] = "Master Data E_LWP";
            return View();
        }

        // POST: MasterData/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string tipe, string nilai, string? deskripsi)
        {
            if (!IsLoggedIn() || !IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (string.IsNullOrWhiteSpace(tipe) || string.IsNullOrWhiteSpace(nilai))
            {
                return Json(new { success = false, message = "Tipe dan Nilai harus diisi" });
            }

            try
            {
                // Cek apakah sudah ada data aktif
                var existsActive = await _context.MasterData
                    .AnyAsync(m => m.Tipe == tipe && m.Nilai == nilai && m.IsActive == true);

                if (existsActive)
                {
                    return Json(new { success = false, message = "Data sudah ada" });
                }

                // Cek apakah ada data yang sudah dihapus (soft delete) dengan Tipe dan Nilai yang sama
                var deletedData = await _context.MasterData
                    .FirstOrDefaultAsync(m => m.Tipe == tipe && m.Nilai == nilai && m.IsActive == false);

                if (deletedData != null)
                {
                    // Restore data yang sudah dihapus
                    deletedData.IsActive = true;
                    if (!string.IsNullOrWhiteSpace(deskripsi))
                    {
                        deletedData.Deskripsi = deskripsi; // Update deskripsi jika ada
                    }
                    deletedData.CreatedAt = DateTime.Now; // Update tanggal
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Data berhasil direstore", id = deletedData.Id });
                }

                // Buat data baru jika tidak ada data yang dihapus
                var masterData = new MasterData
                {
                    Tipe = tipe,
                    Nilai = nilai,
                    Deskripsi = deskripsi,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.MasterData.Add(masterData);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Data berhasil ditambahkan", id = masterData.Id });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Cek inner exception untuk detail error
                var innerEx = dbEx.InnerException as Microsoft.Data.SqlClient.SqlException;
                if (innerEx != null)
                {
                    // Cek jika error karena constraint violation (duplicate)
                    if (innerEx.Number == 2627 || innerEx.Number == 2601) // Unique constraint violation
                    {
                        return Json(new { success = false, message = "Data dengan Tipe dan Nilai tersebut sudah ada di database." });
                    }
                    // Cek jika error karena tabel belum ada
                    if (innerEx.Message.Contains("Invalid object name") && innerEx.Message.Contains("MasterData"))
                    {
                        return Json(new { success = false, message = "Tabel MasterData belum dibuat. Silakan jalankan script Create_MasterData_Table.sql di SQL Server Management Studio." });
                    }
                    return Json(new { success = false, message = "Database error: " + innerEx.Message });
                }
                return Json(new { success = false, message = "Database error: " + dbEx.Message });
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                // Cek jika error karena tabel belum ada
                if (sqlEx.Message.Contains("Invalid object name") && sqlEx.Message.Contains("MasterData"))
                {
                    return Json(new { success = false, message = "Tabel MasterData belum dibuat. Silakan jalankan script Create_MasterData_Table.sql di SQL Server Management Studio." });
                }
                // Cek constraint violation
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    return Json(new { success = false, message = "Data dengan Tipe dan Nilai tersebut sudah ada di database." });
                }
                return Json(new { success = false, message = "Database error: " + sqlEx.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message + " | " + ex.GetType().Name });
            }
        }

        // POST: MasterData/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string tipe, string nilai, string? deskripsi)
        {
            if (!IsLoggedIn() || !IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (string.IsNullOrWhiteSpace(tipe) || string.IsNullOrWhiteSpace(nilai))
            {
                return Json(new { success = false, message = "Tipe dan Nilai harus diisi" });
            }

            try
            {
                var masterData = await _context.MasterData.FindAsync(id);
                if (masterData == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                // Cek apakah nilai baru sudah ada (kecuali data yang sedang diedit, hanya data yang aktif)
                var exists = await _context.MasterData
                    .AnyAsync(m => m.Tipe == tipe && m.Nilai == nilai && m.Id != id && m.IsActive == true);

                if (exists)
                {
                    return Json(new { success = false, message = "Data dengan nilai tersebut sudah ada" });
                }

                // Update data
                masterData.Tipe = tipe;
                masterData.Nilai = nilai;
                masterData.Deskripsi = deskripsi;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Data berhasil diupdate" });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // Cek inner exception untuk detail error
                var innerEx = dbEx.InnerException as Microsoft.Data.SqlClient.SqlException;
                if (innerEx != null)
                {
                    // Cek jika error karena constraint violation (duplicate)
                    if (innerEx.Number == 2627 || innerEx.Number == 2601) // Unique constraint violation
                    {
                        return Json(new { success = false, message = "Data dengan Tipe dan Nilai tersebut sudah ada di database." });
                    }
                    // Cek jika error karena tabel belum ada
                    if (innerEx.Message.Contains("Invalid object name") && innerEx.Message.Contains("MasterData"))
                    {
                        return Json(new { success = false, message = "Tabel MasterData belum dibuat. Silakan jalankan script Create_MasterData_Table.sql di SQL Server Management Studio." });
                    }
                    return Json(new { success = false, message = "Database error: " + innerEx.Message });
                }
                return Json(new { success = false, message = "Database error: " + dbEx.Message });
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                if (sqlEx.Message.Contains("Invalid object name") && sqlEx.Message.Contains("MasterData"))
                {
                    return Json(new { success = false, message = "Tabel MasterData belum dibuat. Silakan jalankan script Create_MasterData_Table.sql di SQL Server Management Studio." });
                }
                // Cek constraint violation
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    return Json(new { success = false, message = "Data dengan Tipe dan Nilai tersebut sudah ada di database." });
                }
                return Json(new { success = false, message = "Database error: " + sqlEx.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // POST: MasterData/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn() || !IsAdmin())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var masterData = await _context.MasterData.FindAsync(id);
                if (masterData == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                // Soft delete
                masterData.IsActive = false;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

