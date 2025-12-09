using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using AppTrackII.Pages.Register;
using AppTrackII.Services;

namespace AppTrackII.ViewModels;

public class TokenAccessViewModel : BaseViewModel
{
    private readonly ApiClient _apiClient = new();

    private string _token = string.Empty;
    public string Token
    {
        get => _token;
        set => SetProperty(ref _token, value);
    }

    public ICommand ContinueCommand { get; }
    public ICommand CancelCommand { get; }

    public TokenAccessViewModel()
    {
        ContinueCommand = new Command(async () => await ValidateAndContinueAsync());
        CancelCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }

    private async Task ValidateAndContinueAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error", "Debes ingresar un token.", "OK");
            return;
        }

        try
        {
            var body = new { Token = this.Token.Trim() };
            var result = await _apiClient.PostAsync<ValidateTokenResponse>("validate-token", body);

            if (result == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "No se pudo conectar con el servidor.", "OK");
                return;
            }

            if (!result.Ok)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Token inválido",
                    result.Error ?? "El token no es válido o está inactivo.",
                    "OK");
                return;
            }

            // Guardamos el token para que el registro lo use
            Preferences.Set("AccessToken", this.Token.Trim());

            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                $"Ocurrió un error al validar el token:\n{ex.Message}",
                "OK");
        }
    }

    private class ValidateTokenResponse
    {
        public bool Ok { get; set; }
        public string? Error { get; set; }
    }
}
