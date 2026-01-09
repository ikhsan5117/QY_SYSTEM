namespace AplikasiCheckDimensi.Models
{
    public class QCHoseData
    {
        public int Id { get; set; }
        public string LineChecking { get; set; } = string.Empty;
        public string NamaInspector { get; set; } = string.Empty;
        public string GroupChecking { get; set; } = "A"; // A atau B
        public string PartCode { get; set; } = string.Empty;
        public string TimeStop { get; set; } = "0:0:0:0";
        public string? JenisNG { get; set; }
        public string? NamaOPR { get; set; }
        public int? QtyNG { get; set; }
        public int? QtyCheck { get; set; }
        public string? LineStop { get; set; }
        public string StatusChecking { get; set; } = "Checking"; // Checking, Done, Stop
        public string StatusCheckingTime { get; set; } = "0:0:0:0";
        public DateTime TanggalInput { get; set; } = DateTime.Now;
        public string? Plant { get; set; }
        public string? Grup { get; set; }
    }
}

