using System.ComponentModel.DataAnnotations.Schema;

namespace AplikasiCheckDimensi.Models
{
    public class Produk
    {
        public int Id { get; set; }
        public string NamaProduk { get; set; } = string.Empty;

        // Kolom yang ADA di SQLServer_Setup.sql
        public string? PartCode { get; set; }   // PartCode
        public string? Plant { get; set; }      // Plant
        public string? Operator { get; set; }   // OperatorPIC (di-map di ApplicationDbContext)
        public DateTime TanggalInput { get; set; } // CreatedAt

        // Kolom tambahan yang TIDAK ada di SQLServer_Setup.sql -> NotMapped
        [NotMapped] public string? PartNo { get; set; }
        [NotMapped] public string? CT { get; set; }
        [NotMapped] public string? IdentifikasiItem { get; set; }
        
        // Packing Information (tidak ada di script SQLServer_Setup.sql)
        [NotMapped] public string? TypeBox { get; set; }
        [NotMapped] public int? QtyPerBox { get; set; }
        [NotMapped] public string? GambarPacking { get; set; }
        
        // Checking Fixture Information (tidak ada di script SQLServer_Setup.sql)
        [NotMapped] public string? StandarCF { get; set; }
        [NotMapped] public string? GambarCF { get; set; }
        
        // Video SOP (tidak ada di script SQLServer_Setup.sql)
        [NotMapped] public string? VideoPath { get; set; }
    }
}
