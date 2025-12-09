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
    private readonly ApiClient _apiClient = new();

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

            // Leer DeviceToken almacenado (lo generará el registro del dispositivo)
            var deviceToken = Preferences.Get("DeviceToken", string.Empty);

            var body = new
            {
                Usuario = this.Usuario,
                Password = this.Password,
                DeviceToken = deviceToken
            };

            var result = await _apiClient.PostAsync<LoginResponseDto>("login", body);

            if (result == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "No se pudo conectar con el servidor.", "OK");
                return;
            }

            if (!result.Ok)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Login fallido", result.Error ?? "Credenciales inválidas.", "OK");
                return;
            }

            // Guardar datos básicos
            Preferences.Set("Usuario", result.Username ?? string.Empty);
            Preferences.Set("Rol", result.Rol ?? string.Empty);
            if (result.LocalidadId.HasValue)
                Preferences.Set("LocalidadId", result.LocalidadId.Value);

            await Application.Current.MainPage.DisplayAlert(
                "Bienvenido",
                $"Usuario: {result.Username}\nRol: {result.Rol}\nLocalidad: {result.LocalidadNombre}",
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

    // DTO para mapear la respuesta JSON del API
    private class LoginResponseDto
    {
        public bool Ok { get; set; }
        public string? Error { get; set; }
        public int? UsuarioId { get; set; }
        public string? Username { get; set; }
        public string? Rol { get; set; }
        public int? LocalidadId { get; set; }
        public string? LocalidadNombre { get; set; }
    }
}
