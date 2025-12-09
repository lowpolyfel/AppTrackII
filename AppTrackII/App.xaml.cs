using AppTrackII.Pages.Scan;
using Microsoft.Maui.Controls;

namespace AppTrackII;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
        Routing.RegisterRoute(nameof(ScanPage), typeof(ScanPage));

    }
}
