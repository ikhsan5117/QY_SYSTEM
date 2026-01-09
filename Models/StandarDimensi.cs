using System.ComponentModel.DataAnnotations.Schema;

namespace AplikasiCheckDimensi.Models
{
    public class StandarDimensi
    {
        public int Id { get; set; }
        public int ProdukId { get; set; }
        public string NamaDimensi { get; set; } = string.Empty;
        
        // Field Sisi A / Sisi B TIDAK ada di schema SQLServer_Setup.sql.
        // Supaya kompatibel dengan database SQL Server lama, kita tandai sebagai NotMapped
        // (hanya dipakai di versi baru / SQLite).

        // SISI A - Inner Diameter (ID)
        [NotMapped] public decimal? InnerDiameter_SisiA_Min { get; set; }
        [NotMapped] public decimal? InnerDiameter_SisiA_Max { get; set; }
        
        // SISI A - Outer Diameter (OD)
        [NotMapped] public decimal? OuterDiameter_SisiA_Min { get; set; }
        [NotMapped] public decimal? OuterDiameter_SisiA_Max { get; set; }
        
        // SISI A - Thickness (Ketebalan)
        [NotMapped] public decimal? Thickness_SisiA_Min { get; set; }
        [NotMapped] public decimal? Thickness_SisiA_Max { get; set; }
        
        // SISI B - Inner Diameter (ID)
        [NotMapped] public decimal? InnerDiameter_SisiB_Min { get; set; }
        [NotMapped] public decimal? InnerDiameter_SisiB_Max { get; set; }
        
        // SISI B - Outer Diameter (OD)
        [NotMapped] public decimal? OuterDiameter_SisiB_Min { get; set; }
        [NotMapped] public decimal? OuterDiameter_SisiB_Max { get; set; }
        
        // SISI B - Thickness (Ketebalan)
        [NotMapped] public decimal? Thickness_SisiB_Min { get; set; }
        [NotMapped] public decimal? Thickness_SisiB_Max { get; set; }
        
        // Panjang (Length)
        public decimal? Panjang_Min { get; set; }
        public decimal? Panjang_Max { get; set; }
        
        // Tinggi (Height)
        public decimal? Tinggi_Min { get; set; }
        public decimal? Tinggi_Max { get; set; }
        
        // Radius
        public decimal? Radius_Min { get; set; }
        public decimal? Radius_Max { get; set; }
        
        // Keep old fields for backward compatibility (will be removed after migration)
        public decimal? InnerDiameter_Min { get; set; }
        public decimal? InnerDiameter_Max { get; set; }
        public decimal? OuterDiameter_Min { get; set; }
        public decimal? OuterDiameter_Max { get; set; }
        public decimal? Thickness_Min { get; set; }
        public decimal? Thickness_Max { get; set; }
        public decimal? DimensiA_Min { get; set; }
        public decimal? DimensiA_Max { get; set; }
        public decimal? DimensiB_Min { get; set; }
        public decimal? DimensiB_Max { get; set; }
        public decimal? Sudut_Min { get; set; }
        public decimal? Sudut_Max { get; set; }
        
        public virtual Produk? Produk { get; set; }
        public virtual ICollection<InputAktual>? InputAktualls { get; set; }
    }
}
