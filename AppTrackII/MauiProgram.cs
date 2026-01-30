using AppTrackII.Pages;
using AppTrackII.Pages.Auth;
using AppTrackII.Pages.Register; // <--- FALTABA ESTO
using AppTrackII.Pages.Retrabajo;
using AppTrackII.Pages.Scan;
using AppTrackII.Pages.Scrap;
using AppTrackII.Services;
using AppTrackII.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
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

            // --- Servicios Registrados ---

            // Auth
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            // Registro (ESTO FALTABA)
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();

            // ViewModels existentes
            builder.Services.AddTransient<ScanViewModel>();
            builder.Services.AddTransient<ScrapViewModel>();
            builder.Services.AddTransient<RetrabajoViewModel>();

            // Pages existentes
            builder.Services.AddTransient<ScanPage>();
            builder.Services.AddTransient<ScrapPage>();
            builder.Services.AddTransient<RetrabajoPage>();

            return builder.Build();
        }
    }
}