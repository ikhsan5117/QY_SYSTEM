using Microsoft.AspNetCore.Mvc;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;
using Microsoft.EntityFrameworkCore;

namespace AplikasiCheckDimensi.Controllers
{
    public class SOPController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SOPController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SOP/Checking
        public async Task<IActionResult> Checking()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.NamaLengkap = HttpContext.Session.GetString("NamaLengkap");
            ViewBag.Role = HttpContext.Session.GetString("Role");

            // Get all products with their standards for SOP checking
            var produkList = await _context.Produk
                .Where(p => _context.StandarDimensi.Any(s => s.ProdukId == p.Id))
                .ToListAsync();

            return View(produkList);
        }

        // GET: SOP/Management
        public IActionResult Management()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.NamaLengkap = HttpContext.Session.GetString("NamaLengkap");
            ViewBag.Role = role;

            return View();
        }
    }
}
