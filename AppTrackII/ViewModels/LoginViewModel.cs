using System.Windows.Input;
using Microsoft.Maui.Controls;
using AppTrackII.Pages.Scan;

namespace AppTrackII.ViewModels;

public class LoginViewModel : BaseViewModel
{
    public string Usuario { get; set; }
    public string Password { get; set; }

    public ICommand LoginCommand { get; }
    public ICommand CancelCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(Usuario) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "Ingresa usuario y contraseña.", "OK");
                return;
            }

            await Shell.Current.GoToAsync(nameof(ScanPage));
        });

    }
}
