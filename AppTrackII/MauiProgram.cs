using AppTrackII.Pages;
using AppTrackII.Services;
using AppTrackII.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using AppTrackII.Pages.Scan;
using AppTrackII.Pages.Scrap;      // Nuevo
using AppTrackII.Pages.Retrabajo;  // Nuevo
using ZXing.Net.Maui.Controls;

namespace AppTrackII
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Servicios
            builder.Services.AddSingleton<ILocalidadService, MockLocalidadService>();

            // ViewModels existentes
            builder.Services.AddTransient<ScanViewModel>();

            // ViewModels Nuevos
            builder.Services.AddTransient<ScrapViewModel>();
            builder.Services.AddTransient<RetrabajoViewModel>();

            // Pages existentes
            builder.Services.AddTransient<ScanPage>();

            // Pages Nuevas
            builder.Services.AddTransient<ScrapPage>();
            builder.Services.AddTransient<RetrabajoPage>();

            return builder.Build();
        }
    }
}