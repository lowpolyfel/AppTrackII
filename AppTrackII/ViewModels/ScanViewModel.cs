using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ZXing.Net.Maui; // Necesario para CameraLocation
using AppTrackII.Pages.Scrap;      // Para la navegación
using AppTrackII.Pages.Retrabajo;  // Para la navegación

namespace AppTrackII.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        // Datos del dispositivo (Simulados por ahora, luego vendrán de API/Storage)
        public string DeviceName { get; set; } = "Scanner Pro";
        public string DeviceLocalidad { get; set; } = "Almacén Central";

        // Campos de captura
        private string _lote = string.Empty;
        public string Lote
        {
            get => _lote;
            set { _lote = value; OnPropertyChanged(); }
        }

        private string _numeroParte = string.Empty;
        public string NumeroParte
        {
            get => _numeroParte;
            set { _numeroParte = value; OnPropertyChanged(); }
        }

        private string _cantidadPiezas = string.Empty;
        public string CantidadPiezas
        {
            get => _cantidadPiezas;
            set { _cantidadPiezas = value; OnPropertyChanged(); }
        }

        // Control de Cámara
        private bool _isDetecting = true;
        public bool IsDetecting
        {
            get => _isDetecting;
            set { _isDetecting = value; OnPropertyChanged(); }
        }

        private CameraLocation _cameraLocation = CameraLocation.Rear;
        public CameraLocation CameraLocation
        {
            get => _cameraLocation;
            set { _cameraLocation = value; OnPropertyChanged(); }
        }

        private string _cameraStatusMessage = "Listo para escanear";
        public string CameraStatusMessage
        {
            get => _cameraStatusMessage;
            set { _cameraStatusMessage = value; OnPropertyChanged(); }
        }

        // Comandos
        public ICommand StartScanCommand { get; }
        public ICommand StopScanCommand { get; }
        public ICommand SwitchCameraCommand { get; }
        public ICommand BarcodeDetectedCommand { get; }

        // Comandos de Navegación
        public ICommand GoToScrapCommand { get; }
        public ICommand GoToRetrabajoCommand { get; }

        public ScanViewModel()
        {
            // Control de escaneo
            StartScanCommand = new Command(() =>
            {
                IsDetecting = true;
                CameraStatusMessage = "Escaneando...";
            });

            StopScanCommand = new Command(() =>
            {
                IsDetecting = false;
                CameraStatusMessage = "Escaneo pausado.";
            });

            SwitchCameraCommand = new Command(() =>
            {
                CameraLocation = CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
                CameraStatusMessage = $"Cámara {(CameraLocation == CameraLocation.Rear ? "Trasera" : "Frontal")}";
            });

            // Lógica al detectar código
            BarcodeDetectedCommand = new Command<BarcodeResult>(OnBarcodeDetected);

            // Navegación
            GoToScrapCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ScrapPage));
            });

            GoToRetrabajoCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(RetrabajoPage));
            });
        }

        public Task InitializeAsync()
        {
            // Reiniciar estado al entrar
            IsDetecting = true;
            return Task.CompletedTask;
        }

        private void OnBarcodeDetected(BarcodeResult result)
        {
            // Ejecutar en hilo principal para actualizar UI
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!IsDetecting) return;

                // Feedback háptico (vibración)
                try { HapticFeedback.Perform(HapticFeedbackType.Click); } catch { }

                var value = result.Value;

                // Lógica simple de llenado (primero Lote, luego Parte)
                if (string.IsNullOrEmpty(Lote))
                {
                    Lote = value;
                    CameraStatusMessage = "Lote capturado";
                    await PauseScanningBriefly();
                }
                else if (string.IsNullOrEmpty(NumeroParte))
                {
                    NumeroParte = value;
                    CameraStatusMessage = "No. Parte capturado";
                    await PauseScanningBriefly();
                }
                else
                {
                    CameraStatusMessage = $"Lectura: {value}";
                }
            });
        }

        private async Task PauseScanningBriefly()
        {
            // Pequeña pausa para no leer el mismo código múltiples veces seguidas
            IsDetecting = false;
            await Task.Delay(1500);
            IsDetecting = true;
            CameraStatusMessage = "Listo para siguiente...";
        }
    }
}