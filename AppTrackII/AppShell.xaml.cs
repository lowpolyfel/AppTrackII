using AppTrackII.Pages.Auth;
using AppTrackII.Pages.Register;

namespace AppTrackII;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registramos las rutas de las páginas que NO están en el TabBar
        // Para poder navegar a ellas con Shell.Current.GoToAsync("NombreRuta")

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

        // OJO: NO registres aquí ScanPage, ScrapPage o RetrabajoPage 
        // porque ya están definidas con "Route=" en el AppShell.xaml
    }
}