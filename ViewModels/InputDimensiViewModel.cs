using AplikasiCheckDimensi.Models;

namespace AplikasiCheckDimensi.ViewModels
{
    public class InputDimensiViewModel
    {
        public List<StandarDimensi> StandarDimensiList { get; set; } = new List<StandarDimensi>();
        public StandarDimensi? SelectedStandarDimensi { get; set; }
        public InputAktual InputAktual { get; set; } = new InputAktual();
    }
}
