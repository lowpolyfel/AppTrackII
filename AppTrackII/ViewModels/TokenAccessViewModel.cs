using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using AppTrackII.Pages.Register;

namespace AppTrackII.ViewModels;

public class TokenAccessViewModel : BaseViewModel
{
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
        ContinueCommand = new Command(Continue);
        CancelCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }

    private async void Continue()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                "Debes ingresar un token.",
                "OK");
            return;
        }

        // IMPORTANTE:
        // El token NO se valida aquí.
        // Solo se guarda para que RegisterViewModel lo use
        // en /api/v1/device/activate
        Preferences.Set("ActivationToken", Token.Trim());

        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
