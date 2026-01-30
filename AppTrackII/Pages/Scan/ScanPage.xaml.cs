using ZXing.Net.Maui; // Importante si usas tipos de ZXing aquí, aunque lo básico no lo requiere.

namespace AppTrackII.Pages.Scan;

public partial class ScanPage : ContentPage
{
    public ScanPage()
    {
        InitializeComponent();

        // Opcional: Si el escáner se traba al navegar, puedes descomentar esto:
        // barcodeView.Options = new BarcodeReaderOptions
        // {
        //     Formats = BarcodeFormats.All,
        //     AutoRotate = true,
        //     Multiple = false
        // };
    }

    // Si necesitas manejar eventos de ciclo de vida (cuando la pagina aparece/desaparece)
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // A veces es útil reactivar la detección aquí si se desactivó
        if (BindingContext is ViewModels.ScanViewModel vm)
        {
            vm.IsScanning = true;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Ahorrar batería pausando el escáner al salir
        if (BindingContext is ViewModels.ScanViewModel vm)
        {
            vm.IsScanning = false;
        }
    }
}