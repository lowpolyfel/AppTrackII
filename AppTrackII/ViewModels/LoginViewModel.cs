using AppTrackII.Pages.Register;
using AppTrackII.Services;
using IntelliJ.Lang.Annotations;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace AppTrackII.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await LoginAsync());
        GoToRegisterCommand = new Command(async () => await GoToRegisterAsync());
    }

    private async Task LoginAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Introduce usuario y contraseña", "OK");
            return;
        }

        IsBusy = true; // Ahora funcionará porque está definido en BaseViewModel
        try
        {
            // Llamada al API de Traka
            var token = await ApiClient.LoginAsync(Username, Password);

            if (!string.IsNullOrWhiteSpace(token))
            {
                Preferences.Set("Username", Username);
                // Navegación a la página principal
                await Shell.Current.GoToAsync("//ScanPage");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Credenciales incorrectas", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo conectar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}