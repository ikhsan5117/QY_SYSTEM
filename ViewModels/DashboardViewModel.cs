using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProduk { get; set; }
        public int TotalStandarDimensi { get; set; }
        public int TotalInputHariIni { get; set; }
        public int TotalInputBulanIni { get; set; }
        public int TotalOK { get; set; }
        public int TotalNG { get; set; }
        public List<InputAktual> RecentInputs { get; set; } = new List<InputAktual>();
        public List<Produk> ProdukWithSOP { get; set; } = new List<Produk>();
        public List<StandarDimensi> RecentStandarDimensi { get; set; } = new List<StandarDimensi>();
        
        public double PersentaseOK 
        { 
            get 
            {
                var total = TotalOK + TotalNG;
                return total > 0 ? Math.Round((double)TotalOK / total * 100, 1) : 0;
            }
        }
        
        public int TotalProdukDenganSOP 
        { 
            get 
            {
                return ProdukWithSOP.Count;
            }
        }
    }
}
