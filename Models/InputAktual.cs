using System.ComponentModel.DataAnnotations.Schema;

namespace AplikasiCheckDimensi.Models
{
    public class InputAktual
    {
        public int Id { get; set; }
        public int StandarDimensiId { get; set; }
        
        // Sisi A & B (X/Y) - TIDAK ADA di schema SQLServer_Setup.sql
        // Hanya dipakai di versi baru (mis. SQLite). Supaya compatible dengan SQL Server lama,
        // properti ini tidak di-map ke kolom database.
        [NotMapped] public decimal? NilaiInnerDiameter_SisiA { get; set; }
        [NotMapped] public decimal? NilaiInnerDiameter_SisiAX { get; set; }
        [NotMapped] public decimal? NilaiInnerDiameter_SisiAY { get; set; }
        
        [NotMapped] public decimal? NilaiOuterDiameter_SisiA { get; set; }
        [NotMapped] public decimal? NilaiOuterDiameter_SisiAX { get; set; }
        [NotMapped] public decimal? NilaiOuterDiameter_SisiAY { get; set; }
        
        [NotMapped] public decimal? NilaiThickness_SisiA { get; set; }
        [NotMapped] public decimal? NilaiThickness_SisiAX { get; set; }
        [NotMapped] public decimal? NilaiThickness_SisiAY { get; set; }
        
        [NotMapped] public decimal? NilaiInnerDiameter_SisiB { get; set; }
        [NotMapped] public decimal? NilaiInnerDiameter_SisiBX { get; set; }
        [NotMapped] public decimal? NilaiInnerDiameter_SisiBY { get; set; }
        
        [NotMapped] public decimal? NilaiOuterDiameter_SisiB { get; set; }
        [NotMapped] public decimal? NilaiOuterDiameter_SisiBX { get; set; }
        [NotMapped] public decimal? NilaiOuterDiameter_SisiBY { get; set; }
        
        [NotMapped] public decimal? NilaiThickness_SisiB { get; set; }
        [NotMapped] public decimal? NilaiThickness_SisiBX { get; set; }
        [NotMapped] public decimal? NilaiThickness_SisiBY { get; set; }
        
        // Old dimension fields for backward compatibility
        public decimal? NilaiInnerDiameter { get; set; }
        public decimal? NilaiOuterDiameter { get; set; }
        public decimal? NilaiThickness { get; set; }
        
        // Other dimensions
        public decimal? NilaiPanjang { get; set; }
        public decimal? NilaiTinggi { get; set; }
        public decimal? NilaiRadius { get; set; }
        public decimal? NilaiDimensiA { get; set; }
        public decimal? NilaiDimensiB { get; set; }
        public decimal? NilaiSudut { get; set; }
        
        public DateTime TanggalInput { get; set; }
        // CatatanOperator tidak ada di schema SQLServer_Setup.sql
        [NotMapped]
        public string? CatatanOperator { get; set; }
        
        // Additional fields
        public string? NamaPIC { get; set; }
        public string? Plant { get; set; }
        public string? Grup { get; set; }
        
        // Status field - OK if all measurements pass, NG if any measurement fails
        public string? Status { get; set; }
        
        public virtual StandarDimensi? StandarDimensi { get; set; }
        
        // Computed properties untuk status OK/NG - New dimensions
        public bool IsInnerDiameter_OK 
        { 
            get 
            {
                if (NilaiInnerDiameter == null || StandarDimensi == null) return false;
                if (StandarDimensi.InnerDiameter_Min == null || StandarDimensi.InnerDiameter_Max == null) return true;
                return NilaiInnerDiameter >= StandarDimensi.InnerDiameter_Min && NilaiInnerDiameter <= StandarDimensi.InnerDiameter_Max;
            }
        }
        
        public bool IsOuterDiameter_OK 
        { 
            get 
            {
                if (NilaiOuterDiameter == null || StandarDimensi == null) return false;
                if (StandarDimensi.OuterDiameter_Min == null || StandarDimensi.OuterDiameter_Max == null) return true;
                return NilaiOuterDiameter >= StandarDimensi.OuterDiameter_Min && NilaiOuterDiameter <= StandarDimensi.OuterDiameter_Max;
            }
        }
        
        public bool IsThickness_OK 
        { 
            get 
            {
                if (NilaiThickness == null || StandarDimensi == null) return false;
                if (StandarDimensi.Thickness_Min == null || StandarDimensi.Thickness_Max == null) return true;
                return NilaiThickness >= StandarDimensi.Thickness_Min && NilaiThickness <= StandarDimensi.Thickness_Max;
            }
        }
        
        public bool IsPanjang_OK 
        { 
            get 
            {
                if (NilaiPanjang == null || StandarDimensi == null) return false;
                if (StandarDimensi.Panjang_Min == null || StandarDimensi.Panjang_Max == null) return true;
                return NilaiPanjang >= StandarDimensi.Panjang_Min && NilaiPanjang <= StandarDimensi.Panjang_Max;
            }
        }
        
        public bool IsTinggi_OK 
        { 
            get 
            {
                if (NilaiTinggi == null || StandarDimensi == null) return false;
                if (StandarDimensi.Tinggi_Min == null || StandarDimensi.Tinggi_Max == null) return true;
                return NilaiTinggi >= StandarDimensi.Tinggi_Min && NilaiTinggi <= StandarDimensi.Tinggi_Max;
            }
        }
        
        public bool IsRadius_OK 
        { 
            get 
            {
                if (NilaiRadius == null || StandarDimensi == null) return false;
                if (StandarDimensi.Radius_Min == null || StandarDimensi.Radius_Max == null) return true;
                return NilaiRadius >= StandarDimensi.Radius_Min && NilaiRadius <= StandarDimensi.Radius_Max;
            }
        }
        
        // Old computed properties for backward compatibility
        public bool IsDimensiA_OK 
        { 
            get 
            {
                if (NilaiDimensiA == null || StandarDimensi == null) return false;
                if (StandarDimensi.DimensiA_Min == null || StandarDimensi.DimensiA_Max == null) return true;
                return NilaiDimensiA >= StandarDimensi.DimensiA_Min && NilaiDimensiA <= StandarDimensi.DimensiA_Max;
            }
        }
        
        public bool IsDimensiB_OK 
        { 
            get 
            {
                if (NilaiDimensiB == null || StandarDimensi == null) return false;
                if (StandarDimensi.DimensiB_Min == null || StandarDimensi.DimensiB_Max == null) return true;
                return NilaiDimensiB >= StandarDimensi.DimensiB_Min && NilaiDimensiB <= StandarDimensi.DimensiB_Max;
            }
        }
        
        public bool IsSudut_OK 
        { 
            get 
            {
                if (NilaiSudut == null || StandarDimensi == null) return false;
                if (StandarDimensi.Sudut_Min == null || StandarDimensi.Sudut_Max == null) return true;
                return NilaiSudut >= StandarDimensi.Sudut_Min && NilaiSudut <= StandarDimensi.Sudut_Max;
            }
        }
        
        // Overall status - checks all Sisi A, Sisi B, and other dimensions
        public bool IsAllOK
        {
            get
            {
                if (StandarDimensi == null) return false;
                
                // Check Sisi A dimensions
                if (NilaiInnerDiameter_SisiA.HasValue && StandarDimensi.InnerDiameter_SisiA_Min.HasValue && StandarDimensi.InnerDiameter_SisiA_Max.HasValue)
                {
                    if (NilaiInnerDiameter_SisiA < StandarDimensi.InnerDiameter_SisiA_Min || NilaiInnerDiameter_SisiA > StandarDimensi.InnerDiameter_SisiA_Max)
                        return false;
                }
                
                if (NilaiOuterDiameter_SisiA.HasValue && StandarDimensi.OuterDiameter_SisiA_Min.HasValue && StandarDimensi.OuterDiameter_SisiA_Max.HasValue)
                {
                    if (NilaiOuterDiameter_SisiA < StandarDimensi.OuterDiameter_SisiA_Min || NilaiOuterDiameter_SisiA > StandarDimensi.OuterDiameter_SisiA_Max)
                        return false;
                }
                
                if (NilaiThickness_SisiA.HasValue && StandarDimensi.Thickness_SisiA_Min.HasValue && StandarDimensi.Thickness_SisiA_Max.HasValue)
                {
                    if (NilaiThickness_SisiA < StandarDimensi.Thickness_SisiA_Min || NilaiThickness_SisiA > StandarDimensi.Thickness_SisiA_Max)
                        return false;
                }
                
                // Check Sisi B dimensions
                if (NilaiInnerDiameter_SisiB.HasValue && StandarDimensi.InnerDiameter_SisiB_Min.HasValue && StandarDimensi.InnerDiameter_SisiB_Max.HasValue)
                {
                    if (NilaiInnerDiameter_SisiB < StandarDimensi.InnerDiameter_SisiB_Min || NilaiInnerDiameter_SisiB > StandarDimensi.InnerDiameter_SisiB_Max)
                        return false;
                }
                
                if (NilaiOuterDiameter_SisiB.HasValue && StandarDimensi.OuterDiameter_SisiB_Min.HasValue && StandarDimensi.OuterDiameter_SisiB_Max.HasValue)
                {
                    if (NilaiOuterDiameter_SisiB < StandarDimensi.OuterDiameter_SisiB_Min || NilaiOuterDiameter_SisiB > StandarDimensi.OuterDiameter_SisiB_Max)
                        return false;
                }
                
                if (NilaiThickness_SisiB.HasValue && StandarDimensi.Thickness_SisiB_Min.HasValue && StandarDimensi.Thickness_SisiB_Max.HasValue)
                {
                    if (NilaiThickness_SisiB < StandarDimensi.Thickness_SisiB_Min || NilaiThickness_SisiB > StandarDimensi.Thickness_SisiB_Max)
                        return false;
                }
                
                // Check other dimensions
                if (NilaiPanjang.HasValue && StandarDimensi.Panjang_Min.HasValue && StandarDimensi.Panjang_Max.HasValue)
                {
                    if (NilaiPanjang < StandarDimensi.Panjang_Min || NilaiPanjang > StandarDimensi.Panjang_Max)
                        return false;
                }
                
                if (NilaiTinggi.HasValue && StandarDimensi.Tinggi_Min.HasValue && StandarDimensi.Tinggi_Max.HasValue)
                {
                    if (NilaiTinggi < StandarDimensi.Tinggi_Min || NilaiTinggi > StandarDimensi.Tinggi_Max)
                        return false;
                }
                
                if (NilaiRadius.HasValue && StandarDimensi.Radius_Min.HasValue && StandarDimensi.Radius_Max.HasValue)
                {
                    if (NilaiRadius < StandarDimensi.Radius_Min || NilaiRadius > StandarDimensi.Radius_Max)
                        return false;
                }
                
                if (NilaiDimensiA.HasValue && StandarDimensi.DimensiA_Min.HasValue && StandarDimensi.DimensiA_Max.HasValue)
                {
                    if (NilaiDimensiA < StandarDimensi.DimensiA_Min || NilaiDimensiA > StandarDimensi.DimensiA_Max)
                        return false;
                }
                
                if (NilaiDimensiB.HasValue && StandarDimensi.DimensiB_Min.HasValue && StandarDimensi.DimensiB_Max.HasValue)
                {
                    if (NilaiDimensiB < StandarDimensi.DimensiB_Min || NilaiDimensiB > StandarDimensi.DimensiB_Max)
                        return false;
                }
                
                if (NilaiSudut.HasValue && StandarDimensi.Sudut_Min.HasValue && StandarDimensi.Sudut_Max.HasValue)
                {
                    if (NilaiSudut < StandarDimensi.Sudut_Min || NilaiSudut > StandarDimensi.Sudut_Max)
                        return false;
                }
                
                return true; // All checks passed
            }
        }
    }
}
