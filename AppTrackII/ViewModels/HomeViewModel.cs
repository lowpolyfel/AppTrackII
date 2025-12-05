using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppTrackII.Pages.Auth;
using AppTrackII.Pages.Register;
using Microsoft.Maui.Controls;

namespace AppTrackII.ViewModels;

public class HomeViewModel : BaseViewModel
{
    public ICommand GoToRegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    public HomeViewModel()
    {
        GoToRegisterCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(TokenAccessPage));
        });

        GoToLoginCommand = new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        });
    }
}
