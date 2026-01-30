using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using AppTrackII.Pages.Scan;
using AppTrackII.Services;

namespace AppTrackII.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _usuario = string.Empty;
    private string _password = string.Empty;
    private bool _isBusy;

    public string Usuario
    {
        get => _usuario;
        set => SetProperty(ref _usuario, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }
    }

    public ICommand LoginCommand { get; }
    public ICommand CancelCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await DoLoginAsync(), () => !IsBusy);
        CancelCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }

    private async Task DoLoginAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (string.IsNullOrWhiteSpace(Usuario) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "Usuario y contraseña son requeridos.", "OK");
                return;
            }

            // Login REAL contra Trackii.Web
            var token = await ApiClient.LoginAsync(Usuario, Password);

            if (string.IsNullOrWhiteSpace(token))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Login fallido",
                    "Credenciales inválidas o error de conexión.",
                    "OK");
                return;
            }

            // Guardar datos mínimos (lo demás se obtiene vía API)
            Preferences.Set("Usuario", Usuario);

            await Application.Current.MainPage.DisplayAlert(
                "Bienvenido",
                $"Usuario: {Usuario}",
                "OK");

            await Shell.Current.GoToAsync(nameof(ScanPage));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"Ocurrió un error al hacer login:\n{ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
