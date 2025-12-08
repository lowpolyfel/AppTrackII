using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace AppTrackII.ViewModels
{
    public class ScrapViewModel : BaseViewModel
    {
        // 1. Preparación para API de Localidad
        private string _localidadActual = "Cargando ubicación...";
        public string LocalidadActual
        {
            get => _localidadActual;
            set { _localidadActual = value; OnPropertyChanged(); }
        }

        // 2. Sección de Comentarios
        private string _comentarios = string.Empty;
        public string Comentarios
        {
            get => _comentarios;
            set { _comentarios = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ScrapViewModel()
        {
            SaveCommand = new Command(async () => await SaveDataAsync());

            CancelCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        public async Task InitializeAsync()
        {
            // TODO: Aquí iría la llamada a la API real para obtener la localidad donde se quedó la pieza
            await Task.Delay(1000); // Simulando red
            LocalidadActual = "Estación de Calidad #4 (Simulado)";
        }

        private async Task SaveDataAsync()
        {
            // Lógica futura de guardado
            await Application.Current.MainPage.DisplayAlert("Scrap", "Reporte de Scrap generado.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}