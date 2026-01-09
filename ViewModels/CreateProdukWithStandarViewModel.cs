using System.ComponentModel.DataAnnotations;
using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi.ViewModels
{
    public class CreateProdukWithStandarViewModel
    {
        // Produk info
        public Produk Produk { get; set; } = new Produk();
        
        // Standar Dimensi
        public StandarDimensi StandarDimensi { get; set; } = new StandarDimensi();
        
        // File uploads (not saved in model, handled separately)
        public IFormFile? VideoFile { get; set; }
        public IFormFile? GambarPackingFile { get; set; }
    }
}
