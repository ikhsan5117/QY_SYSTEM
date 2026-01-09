using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if user is admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var users = await _context.Users.OrderByDescending(u => u.TanggalDibuat).ToListAsync();
            return View(users);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            // Check if username already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username sudah digunakan");
                return BadRequest("Username sudah digunakan");
            }

            user.TanggalDibuat = DateTime.Now;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Check if request is AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
            {
                return Ok();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin() || id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (!IsAdmin() || id != user.Id)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Check if username is being changed and if it's already taken
                if (existingUser.Username != user.Username)
                {
                    var duplicateUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                    if (duplicateUser != null)
                    {
                        ModelState.AddModelError("Username", "Username sudah digunakan");
                        return View(user);
                    }
                }

                existingUser.Username = user.Username;
                // Only update password if a new one is provided
                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = user.Password;
                }
                existingUser.NamaLengkap = user.NamaLengkap;
                existingUser.Role = user.Role;
                existingUser.Plant = user.Plant;
                existingUser.Grup = user.Grup;
                existingUser.IsActive = user.IsActive;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
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

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin() || id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // GET: User/SeedUsers - Force seed all users from template
        [HttpGet]
        public async Task<IActionResult> SeedUsers()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                await UserSeeder.SeedUsers(_context);
                TempData["Success"] = "Berhasil membuat 40 user login sesuai template!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error saat membuat user: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
