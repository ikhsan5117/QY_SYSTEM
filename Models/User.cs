using System.ComponentModel.DataAnnotations.Schema;

namespace AplikasiCheckDimensi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NamaLengkap { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Admin atau User
        public string Plant { get; set; } = string.Empty;
        public string Grup { get; set; } = string.Empty;
        
        // Kolom IsActive tidak ada di database SQLServer_Setup.sql
        // Properti ini hanya digunakan di kode (in-memory), tidak di-map ke kolom database
        [NotMapped]
        public bool IsActive { get; set; } = true;
        
        public DateTime TanggalDibuat { get; set; } = DateTime.Now;
    }
}
