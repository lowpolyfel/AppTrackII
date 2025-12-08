using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ZXing.Net.Maui; // Necesario para CameraLocation

namespace AppTrackII.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        public string DeviceName { get; set; } = "Scanner Pro";
        public string DeviceLocalidad { get; set; } = "Almacén Central";

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

        public string CantidadPiezas { get; set; } = string.Empty;

        // Propiedad para controlar si la cámara está detectando
        private bool _isDetecting = true;
        public bool IsDetecting
        {
            get => _isDetecting;
            set { _isDetecting = value; OnPropertyChanged(); }
        }

        // Selección de cámara usando el Enum de ZXing
        private CameraLocation _cameraLocation = CameraLocation.Rear;
        public CameraLocation CameraLocation
        {
            get => _cameraLocation;
            set { _cameraLocation = value; OnPropertyChanged(); }
        }

        private string _cameraStatusMessage = "Apunta al código para escanear.";
        public string CameraStatusMessage
        {
            get => _cameraStatusMessage;
            set { _cameraStatusMessage = value; OnPropertyChanged(); }
        }

        public ICommand StartScanCommand { get; }
        public ICommand StopScanCommand { get; }
        public ICommand SwitchCameraCommand { get; }
        public ICommand BarcodeDetectedCommand { get; }

        public ScanViewModel()
        {
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

            BarcodeDetectedCommand = new Command<BarcodeResult>(OnBarcodeDetected);
        }

        public Task InitializeAsync()
        {
            IsDetecting = true;
            return Task.CompletedTask;
        }

        private void OnBarcodeDetected(BarcodeResult result)
        {
            // Ejecutar en el hilo principal para actualizar UI
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Evitar lecturas múltiples muy rápidas
                if (!IsDetecting) return;

                // Feedback háptico (vibración corta) para confirmar lectura
                try { HapticFeedback.Perform(HapticFeedbackType.Click); } catch { }

                var format = result.Format;
                var value = result.Value;

                // Lógica simple para determinar dónde poner el valor
                // Por ejemplo, si empieza con 'L', es Lote, si empieza con 'P', es Parte (ejemplo hipotético)
                // Aquí simplemente llenamos Lote primero, si está vacío.

                if (string.IsNullOrEmpty(Lote))
                {
                    Lote = value;
                    CameraStatusMessage = $"Lote detectado: {format}";
                    // Pausa breve para no re-escanear inmediatamente lo mismo
                    await PauseScanningBriefly();
                }
                else if (string.IsNullOrEmpty(NumeroParte))
                {
                    NumeroParte = value;
                    CameraStatusMessage = $"Parte detectada: {format}";
                    await PauseScanningBriefly();
                }
                else
                {
                    CameraStatusMessage = $"Lectura: {value} ({format})";
                }
            });
        }

        private async Task PauseScanningBriefly()
        {
            IsDetecting = false;
            await Task.Delay(1500); // Esperar 1.5 segundos
            IsDetecting = true;
            CameraStatusMessage = "Listo para siguiente escaneo...";
        }
    }
}