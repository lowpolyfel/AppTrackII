using AppTrackII.ViewModels;
using ZXing.Net.Maui;

namespace AppTrackII.Pages.Scan;

public partial class ScanPage : ContentPage
{
    private bool _isAnimating = false;

    public ScanPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ScanViewModel vm)
        {
            await vm.InitializeAsync();
        }

        // La animación se inicia cuando el contenedor de la cámara ya tiene dimensiones.
        // Esperamos un momento para que el renderizado se complete.
        await Task.Delay(500);
        _isAnimating = true;
        _ = AnimateLaser();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isAnimating = false; // Detener animación al salir
    }

    // Animación de bucle para el láser rojo, ajustada para usar la altura del contenedor
    private async Task AnimateLaser()
    {
        while (_isAnimating)
        {
            // Altura de referencia del contenedor ScanContainer (Borde blanco)
            var containerHeight = ScanContainer.Height;

            if (containerHeight > 0)
            {
                // Baja hasta el 95% de la altura
                await LaserLine.TranslateTo(0, containerHeight * 0.95, 1500, Easing.SinInOut);
                // Sube hasta el 5% de la altura (evita chocar con el borde superior)
                await LaserLine.TranslateTo(0, containerHeight * 0.05, 1500, Easing.SinInOut);
            }
            else
            {
                // Si la altura aún no está lista, esperar brevemente.
                await Task.Delay(100);
            }
        }
    }

    private async void CameraBarcodeReaderView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (e.Results.Length > 0 && BindingContext is ScanViewModel vm)
        {
            // Solo procesar si el ViewModel está escuchando (IsDetecting)
            if (!vm.IsDetecting) return;

            // 1. Ejecutar Lógica de Negocio (ViewModel)
            // Esto es crucial para que se llene Lote y NumeroParte
            vm.BarcodeDetectedCommand.Execute(e.Results[0]);

            // 2. Ejecutar Feedback Visual (Animación de éxito)
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                // Parpadeo verde y borde
                ScanContainer.Stroke = Color.FromArgb("#10B981"); // Verde
                SuccessFlash.Opacity = 0.4;

                // Mostrar etiqueta de "LECTURA OK" con pop
                SuccessLabel.Opacity = 1;
                await SuccessLabel.ScaleTo(1.2, 100, Easing.SpringOut);

                await Task.Delay(300); // Dar tiempo para ver el feedback

                // Restaurar estado visual
                await Task.WhenAll(
                    SuccessFlash.FadeTo(0, 200),
                    SuccessLabel.FadeTo(0, 200),
                    SuccessLabel.ScaleTo(0.5, 200),
                    // Retornar el borde a blanco
                    ScanContainer.FadeTo(0.8, 100).ContinueWith(_ => ScanContainer.Stroke = Colors.White)
                );

                // Asegurar que el Stroke sea blanco al final de la animación
                ScanContainer.Stroke = Colors.White;
            });
        }
    }

    private void OnToggleClicked(object sender, EventArgs e)
    {
        if (BindingContext is ScanViewModel vm)
        {
            if (vm.IsDetecting)
            {
                vm.StopScanCommand.Execute(null);
                LaserLine.Opacity = 0.2; // Atenuar láser si está pausado
            }
            else
            {
                vm.StartScanCommand.Execute(null);
                LaserLine.Opacity = 0.8; // Encender láser
            }
        }
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        if (BindingContext is ScanViewModel vm)
        {
            vm.Lote = string.Empty;
            vm.NumeroParte = string.Empty;
            vm.CameraStatusMessage = "Datos limpiados. Escanea de nuevo.";
            vm.IsDetecting = true;
        }
    }
}