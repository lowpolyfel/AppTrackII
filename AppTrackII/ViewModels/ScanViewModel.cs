using System.Windows.Input;
using AppTrackII.Services;
using AppTrackII.Models; // Asegúrate de tener el modelo ProductInfo
using ZXing.Net.Maui;    // Necesario para BarcodeDetectionEventArgs

namespace AppTrackII.ViewModels;

public class ScanViewModel : BaseViewModel
{
    // PROPIEDADES DE TEXTO
    private string _ordenTrabajo = string.Empty;
    public string OrdenTrabajo
    {
        get => _ordenTrabajo;
        set => SetProperty(ref _ordenTrabajo, value);
    }

    private string _numeroParte = string.Empty;
    public string NumeroParte
    {
        get => _numeroParte;
        set => SetProperty(ref _numeroParte, value);
    }

    // PROPIEDADES DE INFORMACIÓN (Familia, Area, etc)
    private string _familia = string.Empty;
    public string Familia
    {
        get => _familia;
        set => SetProperty(ref _familia, value);
    }

    private string _subFamilia = string.Empty;
    public string SubFamilia
    {
        get => _subFamilia;
        set => SetProperty(ref _subFamilia, value);
    }

    private string _area = string.Empty;
    public string Area
    {
        get => _area;
        set => SetProperty(ref _area, value);
    }

    // CONTROL DEL ESCÁNER
    private bool _isScanning = true;
    public bool IsScanning
    {
        get => _isScanning;
        set => SetProperty(ref _isScanning, value);
    }

    // COMANDOS
    public ICommand BarcodeDetectedCommand { get; }
    public ICommand ClearCommand { get; }

    public ScanViewModel()
    {
        BarcodeDetectedCommand = new Command<BarcodeDetectionEventArgs>(OnBarcodeDetected);
        ClearCommand = new Command(Limpiar);
    }

    private void OnBarcodeDetected(BarcodeDetectionEventArgs args)
    {
        if (args == null || !args.Results.Any()) return;

        var result = args.Results.FirstOrDefault();
        if (result == null) return;

        var texto = result.Value;

        // IMPORTANTE: Volver al hilo principal para evitar CRASH
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            IsScanning = false; // Pausar escáner
            await ProcesarTexto(texto);
            IsScanning = true;  // Reactivar escáner
        });
    }

    private async Task ProcesarTexto(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return;

        // LÓGICA: Si son 7 números es Orden, si no, es Parte
        bool esOrden = texto.Length == 7 && long.TryParse(texto, out _);

        if (esOrden)
        {
            OrdenTrabajo = texto;
            try { HapticFeedback.Perform(HapticFeedbackType.Click); } catch { }
        }
        else
        {
            NumeroParte = texto;
            try { HapticFeedback.Perform(HapticFeedbackType.Click); } catch { }
            await BuscarInfoProducto(texto);
        }
    }

    private async Task BuscarInfoProducto(string partNumber)
    {
        IsBusy = true; // Requiere que BaseViewModel tenga IsBusy
        try
        {
            // Llama a tu API
            var info = await ApiClient.GetProductInfoAsync(partNumber);

            if (info != null)
            {
                Familia = info.Family;
                SubFamilia = info.SubFamily;
                Area = info.Area;
            }
            else
            {
                Familia = "No encontrado";
                SubFamilia = "-";
                Area = "-";
            }
        }
        catch (Exception)
        {
            Familia = "Error Red";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Limpiar()
    {
        OrdenTrabajo = string.Empty;
        NumeroParte = string.Empty;
        Familia = string.Empty;
        SubFamilia = string.Empty;
        Area = string.Empty;
        IsScanning = true;
    }
}