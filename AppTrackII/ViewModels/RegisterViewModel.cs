using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using AppTrackII.Models;
using AppTrackII.Services;

#if ANDROID
using Android.Provider;
#endif

namespace AppTrackII.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly ILocalidadService _localidadService;

        public ObservableCollection<Localidad> Localidades { get; } = new();

        private Localidad? _localidadSeleccionada;
        public Localidad? LocalidadSeleccionada
        {
            get => _localidadSeleccionada;
            set { _localidadSeleccionada = value; OnPropertyChanged(); }
        }

        public string AndroidId { get; private set; } = string.Empty;

        private string _nombreDispositivo = string.Empty;
        public string NombreDispositivo
        {
            get => _nombreDispositivo;
            set { _nombreDispositivo = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel()
        {
            // Por ahora usamos el servicio "mock" que simula la BD.
            _localidadService = new MockLocalidadService();

            LoadAndroidId();
            _ = LoadLocalidadesAsync();

            RegisterCommand = new Command(async () => await RegisterDeviceAsync());
            CancelCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        private async Task LoadLocalidadesAsync()
        {
            Localidades.Clear();
            var items = await _localidadService.GetLocalidadesAsync();

            foreach (var loc in items)
                Localidades.Add(loc);

            LocalidadSeleccionada = Localidades.Count > 0 ? Localidades[0] : null;
        }

        private void LoadAndroidId()
        {
#if ANDROID
            var context = Android.App.Application.Context;
            AndroidId = Settings.Secure.GetString(
                context.ContentResolver,
                Settings.Secure.AndroidId);
#else
            AndroidId = "Solo disponible en Android";
#endif
            OnPropertyChanged(nameof(AndroidId));
        }

        private async Task RegisterDeviceAsync()
        {
            if (LocalidadSeleccionada is null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "Selecciona una localidad.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NombreDispositivo))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "Introduce un nombre para el dispositivo.", "OK");
                return;
            }

            // TODO: Enviar a la API de TrackII:
            // AndroidId, LocalidadSeleccionada.Id, NombreDispositivo, etc.

            await Application.Current.MainPage.DisplayAlert(
                "Registro exitoso",
                $"Dispositivo: {NombreDispositivo}\n" +
                $"Localidad: {LocalidadSeleccionada.Nombre}\n" +
                $"AndroidID: {AndroidId}",
                "OK");

            await Shell.Current.GoToAsync("//HomePage");
        }
    }
}
