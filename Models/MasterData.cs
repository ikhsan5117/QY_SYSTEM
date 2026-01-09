namespace AplikasiCheckDimensi.Models
{
    public class MasterData
    {
        public int Id { get; set; }
        public string Tipe { get; set; } = string.Empty; // LineChecking, Inspector, GroupChecking, JenisNG, LineStop
        public string Nilai { get; set; } = string.Empty; // Nilai dari master data
        public string? Deskripsi { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

