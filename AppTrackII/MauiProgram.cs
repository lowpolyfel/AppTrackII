using AppTrackII.Pages;
using AppTrackII.Services;
using AppTrackII.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using AppTrackII.Pages.Scan;


namespace AppTrackII
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Servicios
            builder.Services.AddSingleton<ILocalidadService, MockLocalidadService>();

            // ViewModels
            builder.Services.AddTransient<ScanViewModel>();

            // Pages
            builder.Services.AddTransient<ScanPage>();

            return builder.Build();
        }
    }

}
