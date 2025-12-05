using System.Windows.Input;
using Microsoft.Maui.Controls;
using AppTrackII.Pages.Register;

namespace AppTrackII.ViewModels;

public class TokenAccessViewModel : BaseViewModel
{
    private string _token = string.Empty;
    public string Token
    {
        get => _token;
        set
        {
            _token = value;
            OnPropertyChanged();
        }
    }

    public ICommand ContinueCommand { get; }
    public ICommand CancelCommand { get; }

    public TokenAccessViewModel()
    {
        ContinueCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(Token))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "Debes ingresar un token válido.", "OK");
                return;
            }

            await Shell.Current.GoToAsync(nameof(RegisterPage));
        });

        CancelCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });
    }
}
