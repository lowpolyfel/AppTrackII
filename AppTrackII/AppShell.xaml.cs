using AppTrackII.Pages.Auth;
using AppTrackII.Pages.Register;
using AppTrackII.Pages.Scan;
using AppTrackII.Pages.Scrap;      // Nuevo namespace
using AppTrackII.Pages.Retrabajo;  // Nuevo namespace

namespace AppTrackII;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(TokenAccessPage), typeof(TokenAccessPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ScanPage), typeof(ScanPage));

        // Nuevas rutas
        Routing.RegisterRoute(nameof(ScrapPage), typeof(ScrapPage));
        Routing.RegisterRoute(nameof(RetrabajoPage), typeof(RetrabajoPage));
    }
}