using Microsoft.Maui.Controls;

namespace AppTrackII;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
}
