using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace AppTrackII.ViewModels
{
    public class RetrabajoViewModel : BaseViewModel
    {
        // 1. Preparación para API de Localidad
        private string _localidadOrigen = "Consultando...";
        public string LocalidadOrigen
        {
            get => _localidadOrigen;
            set { _localidadOrigen = value; OnPropertyChanged(); }
        }

        // 2. Sección de Comentarios
        private string _instruccionesRetrabajo = string.Empty;
        public string InstruccionesRetrabajo
        {
            get => _instruccionesRetrabajo;
            set { _instruccionesRetrabajo = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public RetrabajoViewModel()
        {
            SaveCommand = new Command(async () => await SaveDataAsync());

            CancelCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        public async Task InitializeAsync()
        {
            // TODO: API Call
            await Task.Delay(800);
            LocalidadOrigen = "Línea de Ensamble B (Simulado)";
        }

        private async Task SaveDataAsync()
        {
            await Application.Current.MainPage.DisplayAlert("Retrabajo", "Orden de retrabajo creada.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}