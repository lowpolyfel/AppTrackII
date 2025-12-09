using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
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
        private readonly ApiClient _apiClient = new();

        public ObservableCollection<Localidad> Localidades { get; } = new();

        private Localidad? _localidadSeleccionada;
        public Localidad? LocalidadSeleccionada
        {
            get => _localidadSeleccionada;
            set => SetProperty(ref _localidadSeleccionada, value);
        }

        public string AndroidId { get; private set; } = string.Empty;

        private string _nombreDispositivo = string.Empty;
        public string NombreDispositivo
        {
            get => _nombreDispositivo;
            set => SetProperty(ref _nombreDispositivo, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel()
        {
            _localidadService = new ApiLocalidadService();

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

            try
            {
                var items = await _localidadService.GetLocalidadesAsync();

                foreach (var loc in items)
                    Localidades.Add(loc);

                LocalidadSeleccionada = Localidades.Count > 0 ? Localidades[0] : null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudieron cargar las localidades:\n{ex.Message}",
                    "OK");
            }
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

            var accessToken = Preferences.Get("AccessToken", string.Empty);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Falta el token de acceso. Vuelve a la pantalla anterior e ingrésalo.",
                    "OK");
                return;
            }

            var body = new
            {
                Token = accessToken,
                AndroidId = this.AndroidId,
                NombreDispositivo = this.NombreDispositivo,
                LocalidadId = LocalidadSeleccionada.Id
            };

            try
            {
                var result = await _apiClient.PostAsync<RegisterDeviceResponse>("register-device", body);

                if (result == null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error", "No se pudo conectar con el servidor.", "OK");
                    return;
                }

                if (!result.Ok)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Registro fallido",
                        result.Error ?? "No fue posible registrar el dispositivo.",
                        "OK");
                    return;
                }

                // Guardamos información de dispositivo para futuras llamadas
                if (!string.IsNullOrWhiteSpace(result.DeviceToken))
                    Preferences.Set("DeviceToken", result.DeviceToken);

                Preferences.Set("DeviceName", NombreDispositivo);
                Preferences.Set("LocalidadId", result.LocalidadId ?? LocalidadSeleccionada.Id);
                Preferences.Set("LocalidadNombre", LocalidadSeleccionada.Nombre);

                await Application.Current.MainPage.DisplayAlert(
                    "Registro exitoso",
                    $"Dispositivo: {NombreDispositivo}\n" +
                    $"Localidad: {LocalidadSeleccionada.Nombre}\n" +
                    $"AndroidID: {AndroidId}",
                    "OK");

                // Volver al Home para que el usuario pueda iniciar sesión
                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Ocurrió un error al registrar el dispositivo:\n{ex.Message}",
                    "OK");
            }
        }

        private class RegisterDeviceResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public string? DeviceToken { get; set; }
            public int? LocalidadId { get; set; }
        }
    }
}
