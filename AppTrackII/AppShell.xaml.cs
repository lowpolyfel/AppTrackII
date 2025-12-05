using AppTrackII.Pages.Auth;
using AppTrackII.Pages.Register;
using AppTrackII.Pages.Scan;

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
    }
}
