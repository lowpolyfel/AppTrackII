using System.Collections.ObjectModel;
using System.Windows.Input;
using AppTrackII.Services;
using AppTrackII.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using AppTrackII.Config;


#if ANDROID
using Android.Provider;
#endif

namespace AppTrackII.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    public string AndroidId { get; private set; } = string.Empty;

    // Colección para el Picker
    public ObservableCollection<Localidad> Localidades { get; } = new();

    // Propiedad para el elemento seleccionado del Picker
    private Localidad? _localidadSeleccionada;
    public Localidad? LocalidadSeleccionada
    {
        get => _localidadSeleccionada;
        set => SetProperty(ref _localidadSeleccionada, value);
    }

    private string _nombreDispositivo = string.Empty;
    public string NombreDispositivo
    {
        get => _nombreDispositivo;
        set => SetProperty(ref _nombreDispositivo, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand RegisterCommand { get; }
    public ICommand CancelCommand { get; }

    public RegisterViewModel()
    {
        LoadAndroidId();

        // Cargar las localidades automáticamente al abrir la vista
        Task.Run(LoadLocalidadesAsync);

        RegisterCommand = new Command(async () => await RegisterAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    private void LoadAndroidId()
    {
#if ANDROID
        var context = Android.App.Application.Context;
        AndroidId = Settings.Secure.GetString(
            context.ContentResolver,
            Settings.Secure.AndroidId);
#else
        AndroidId = "Solo Android";
#endif
        OnPropertyChanged(nameof(AndroidId));
    }

    private async Task LoadLocalidadesAsync()
    {
        try
        {
            // Instanciamos el servicio
            var service = new ApiLocalidadService();
            var items = await service.GetLocalidadesAsync();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Localidades.Clear();
                foreach (var item in items)
                {
                    Localidades.Add(item);
                }

                // SI SIGUE VACIA AUNQUE NO HAYA ERROR DE CONEXION:
                if (Localidades.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Ojo", "Conexión exitosa, pero la base de datos devolvió 0 ubicaciones.", "OK");
                }
            });
        }
        catch (Exception ex)
        {
            // AQUI VERAS SI ES TIMEOUT, ERROR DE RED O IP INCORRECTA
            MainThread.BeginInvokeOnMainThread(async () =>
                await Application.Current.MainPage.DisplayAlert("Error de Carga", $"No pude conectar con {ApiConfig.BaseUrl}\n\nError: {ex.Message}", "OK"));
        }
    }

    private async Task RegisterAsync()
    {
        if (LocalidadSeleccionada == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Selecciona una Ubicación.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(NombreDispositivo))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Introduce un nombre para el dispositivo.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Define un PIN / password.", "OK");
            return;
        }

        var activationToken = Preferences.Get("ActivationToken", string.Empty);
        if (string.IsNullOrWhiteSpace(activationToken))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                "Falta el token de activación.",
                "OK");
            return;
        }

        try
        {
            var reg = await ApiClient.RegisterAsync(
                activationToken,
                AndroidId,
                NombreDispositivo,
                Password,
                LocalidadSeleccionada.Id); // Enviamos el ID de la ubicación seleccionada

            if (reg == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar.", "OK");
                return;
            }

            // Login automático
            var jwt = await ApiClient.LoginAsync(reg.Value.username, Password);
            if (string.IsNullOrWhiteSpace(jwt))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Registrado, pero no se pudo iniciar sesión.",
                    "OK");
                return;
            }

            Preferences.Set("UserId", reg.Value.userId);
            Preferences.Set("Username", reg.Value.username);
            Preferences.Set("DeviceId", reg.Value.deviceId);
            Preferences.Set("DeviceUid", AndroidId);
            Preferences.Set("DeviceName", NombreDispositivo);
            Preferences.Set("LocalidadId", LocalidadSeleccionada.Id); // Guardamos también la localidad localmente por si acaso
            Preferences.Set("Rol", "PISO");

            Preferences.Remove("ActivationToken");

            await Application.Current.MainPage.DisplayAlert(
                "Registro exitoso",
                $"Usuario: {reg.Value.username}",
                "OK");

            await Shell.Current.GoToAsync("//ScanPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"No se pudo registrar:\n{ex.Message}",
                "OK");
        }
    }
}