using System.Windows.Input;
using AppTrackII.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

#if ANDROID
using Android.Provider;
#endif

namespace AppTrackII.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    public string AndroidId { get; private set; } = string.Empty;

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

    private async Task RegisterAsync()
    {
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
                Password);

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
