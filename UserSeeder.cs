using Microsoft.EntityFrameworkCore;
using AplikasiCheckDimensi.Data;
using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi
{
    public static class UserSeeder
    {
        public static async Task SeedUsers(ApplicationDbContext context)
        {
            // Buat Administrator jika belum ada
            var adminExists = await context.Users.AnyAsync(u => u.Username == "Administrator");
            if (!adminExists)
            {
                var admin = new User
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
                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }

            // Hapus user lama kecuali Administrator
            var usersToDelete = await context.Users
                .Where(u => u.Username != "Administrator")
                .ToListAsync();
            
            if (usersToDelete.Any())
            {
                context.Users.RemoveRange(usersToDelete);
                await context.SaveChangesAsync();
            }

            var users = new List<User>
            {
                // Plant BTR - Quality
                new User { Username = "BTR1A", Password = "qcvin", NamaLengkap = "Yaya", Role = "User", Plant = "BTR", Grup = "Quality" },
                new User { Username = "BTR1B", Password = "qcvin", NamaLengkap = "Rais", Role = "User", Plant = "BTR", Grup = "Quality" },
                new User { Username = "BTR2A", Password = "qcvin", NamaLengkap = "Andri", Role = "User", Plant = "BTR", Grup = "Quality" },
                new User { Username = "BTR2B", Password = "qcvin", NamaLengkap = "Riki", Role = "User", Plant = "BTR", Grup = "Quality" },
                
                // Plant Hose - Quality
                new User { Username = "Hose1A", Password = "qcvin", NamaLengkap = "Dimas W", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose1B", Password = "qcvin", NamaLengkap = "Egi", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose2A", Password = "qcvin", NamaLengkap = "Deni", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose2B", Password = "qcvin", NamaLengkap = "Hendry", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose3A", Password = "qcvin", NamaLengkap = "Arsal", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose3B", Password = "qcvin", NamaLengkap = "Dimas S", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose4A", Password = "qcvin", NamaLengkap = "Gilang", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose4B", Password = "qcvin", NamaLengkap = "Rico", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose5A", Password = "qcvin", NamaLengkap = "Nanang", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose5B", Password = "qcvin", NamaLengkap = "Azis", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose6A", Password = "qcvin", NamaLengkap = "Alif", Role = "User", Plant = "Hose", Grup = "Quality" },
                new User { Username = "Hose6B", Password = "qcvin", NamaLengkap = "Mifta", Role = "User", Plant = "Hose", Grup = "Quality" },
                
                // Plant Molded - Quality
                new User { Username = "Molded1A", Password = "qcvin", NamaLengkap = "Achmad", Role = "User", Plant = "Molded", Grup = "Quality" },
                new User { Username = "Molded1B", Password = "qcvin", NamaLengkap = "", Role = "User", Plant = "Molded", Grup = "Quality" },
                new User { Username = "Molded2A", Password = "qcvin", NamaLengkap = "Hamdan", Role = "User", Plant = "Molded", Grup = "Quality" },
                new User { Username = "Molded2B", Password = "qcvin", NamaLengkap = "Luqman", Role = "User", Plant = "Molded", Grup = "Quality" },
                new User { Username = "Molded3A", Password = "qcvin", NamaLengkap = "Salam", Role = "User", Plant = "Molded", Grup = "Quality" },
                new User { Username = "Molded3B", Password = "qcvin", NamaLengkap = "Yayang", Role = "User", Plant = "Molded", Grup = "Quality" },
                new User { Username = "Molded4A", Password = "qcvin", NamaLengkap = "Sahrul", Role = "User", Plant = "Molded", Grup = "Quality" },
                new User { Username = "Molded4B", Password = "qcvin", NamaLengkap = "Suhidin", Role = "User", Plant = "Molded", Grup = "Quality" },
                
                // Plant RVI - Quality
                new User { Username = "RVI1A", Password = "qcvin", NamaLengkap = "", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI1B", Password = "qcvin", NamaLengkap = "", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI2A", Password = "qcvin", NamaLengkap = "Ridwan", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI2B", Password = "qcvin", NamaLengkap = "Acu", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI3A", Password = "qcvin", NamaLengkap = "Piki", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI3B", Password = "qcvin", NamaLengkap = "Keken", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI4A", Password = "qcvin", NamaLengkap = "Roby", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI4B", Password = "qcvin", NamaLengkap = "Wahyu", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI5A", Password = "qcvin", NamaLengkap = "Thandy", Role = "User", Plant = "RVI", Grup = "Quality" },
                new User { Username = "RVI5B", Password = "qcvin", NamaLengkap = "", Role = "User", Plant = "RVI", Grup = "Quality" },
                
                // Plant TPE - Quality
                new User { Username = "TPE1A", Password = "qcvin", NamaLengkap = "Wikatma", Role = "User", Plant = "TPE", Grup = "Quality" },
                new User { Username = "TPE1B", Password = "qcvin", NamaLengkap = "Arif", Role = "User", Plant = "TPE", Grup = "Quality" },
                new User { Username = "TPE2A", Password = "qcvin", NamaLengkap = "", Role = "User", Plant = "TPE", Grup = "Quality" },
                new User { Username = "TPE2B", Password = "qcvin", NamaLengkap = "", Role = "User", Plant = "TPE", Grup = "Quality" },
                
                // Leaders - Admin Role
                new User { Username = "Leadhose", Password = "qcvin", NamaLengkap = "Ariyanto", Role = "Admin", Plant = "Hose", Grup = "Leader" },
                new User { Username = "LeadrviA", Password = "qcvin", NamaLengkap = "Willy", Role = "Admin", Plant = "RVI", Grup = "Leader" },
                new User { Username = "LeadrviB", Password = "qcvin", NamaLengkap = "Arif", Role = "Admin", Plant = "RVI", Grup = "Leader" },
                new User { Username = "Leadmolded", Password = "qcvin", NamaLengkap = "Misbah", Role = "Admin", Plant = "Molded", Grup = "Leader" }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"Successfully created {users.Count} users!");
        }
    }
}
