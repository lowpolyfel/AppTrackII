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
    // Contador para el secreto
    private int _secretTapCount = 0;

    // Propiedad para ocultar/mostrar el botón
    private bool _isRegisterVisible = false;
    public bool IsRegisterVisible
    {
        get => _isRegisterVisible;
        set { _isRegisterVisible = value; OnPropertyChanged(); }
    }

    public ICommand GoToRegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    // Comando para el logo
    public ICommand LogoTappedCommand { get; }

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

        LogoTappedCommand = new Command(() =>
        {
            _secretTapCount++;
            if (_secretTapCount >= 5)
            {
                // Alternar visibilidad
                IsRegisterVisible = !IsRegisterVisible;
                // Reiniciar contador
                _secretTapCount = 0;
            }
        });
    }
}