using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace AppTrackII.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        // Estos valores luego vendrán de la API (dispositivo y localidad)
        public string DeviceName { get; set; } = "Nombre de dispositivo";
        public string DeviceLocalidad { get; set; } = "Sin localidad";

        public string Lote { get; set; } = string.Empty;
        public string NumeroParte { get; set; } = string.Empty;
        public string CantidadPiezas { get; set; } = string.Empty;

        public ObservableCollection<string> Cameras { get; } = new();

        private string? _selectedCamera;
        public string? SelectedCamera
        {
            get => _selectedCamera;
            set { _selectedCamera = value; OnPropertyChanged(); }
        }

        private string _cameraStatusMessage = "Selecciona una cámara y presiona Iniciar.";
        public string CameraStatusMessage
        {
            get => _cameraStatusMessage;
            set { _cameraStatusMessage = value; OnPropertyChanged(); }
        }

        public ICommand StartScanCommand { get; }
        public ICommand StopScanCommand { get; }

        public ScanViewModel()
        {
            StartScanCommand = new Command(StartScan);
            StopScanCommand = new Command(StopScan);

            LoadMockCameras();
        }

        public Task InitializeAsync()
        {
            // Más adelante aquí se cargará desde la API:
            // - DeviceName
            // - DeviceLocalidad
            return Task.CompletedTask;
        }

        private void LoadMockCameras()
        {
            Cameras.Clear();
            Cameras.Add("Cámara frontal");
            Cameras.Add("Cámara trasera");
            SelectedCamera = Cameras.Count > 0 ? Cameras[0] : null;
        }

        private void StartScan()
        {
            if (SelectedCamera == null)
            {
                CameraStatusMessage = "Selecciona una cámara antes de iniciar.";
                return;
            }

            CameraStatusMessage = $"Escaneando con {SelectedCamera}...";
        }

        private void StopScan()
        {
            CameraStatusMessage = "Escaneo detenido.";
        }
    }
}
