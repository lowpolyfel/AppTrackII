using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using AppTrackII.Services;

namespace AppTrackII.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        // ======= CONTEXTO USUARIO / DISPOSITIVO =======

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _deviceName = "Dispositivo";
        public string DeviceName
        {
            get => _deviceName;
            set => SetProperty(ref _deviceName, value);
        }

        private string _deviceLocalidad = "Sin localidad";
        public string DeviceLocalidad
        {
            get => _deviceLocalidad;
            set => SetProperty(ref _deviceLocalidad, value);
        }

        private uint _deviceId;

        // ======= CAMPOS DE ESCANEO =======

        private string _lote = string.Empty;
        public string Lote
        {
            get => _lote;
            set => SetProperty(ref _lote, value);
        }

        private string _numeroParte = string.Empty;
        public string NumeroParte
        {
            get => _numeroParte;
            set => SetProperty(ref _numeroParte, value);
        }

        private int _cantidadPiezas;
        public int CantidadPiezas
        {
            get => _cantidadPiezas;
            set => SetProperty(ref _cantidadPiezas, value);
        }

        // ======= CÁMARAS / ESTADO =======

        public ObservableCollection<string> Cameras { get; } = new();

        private string? _selectedCamera;
        public string? SelectedCamera
        {
            get => _selectedCamera;
            set => SetProperty(ref _selectedCamera, value);
        }

        private string _cameraStatusMessage = "Selecciona una cámara y presiona Iniciar.";
        public string CameraStatusMessage
        {
            get => _cameraStatusMessage;
            set => SetProperty(ref _cameraStatusMessage, value);
        }

        private bool _isDetecting;
        public bool IsDetecting
        {
            get => _isDetecting;
            set => SetProperty(ref _isDetecting, value);
        }

        // ======= COMANDOS =======

        public ICommand StartScanCommand { get; }
        public ICommand StopScanCommand { get; }
        public ICommand BarcodeDetectedCommand { get; }

        public ScanViewModel()
        {
            StartScanCommand = new Command(StartScan);
            StopScanCommand = new Command(StopScan);
            BarcodeDetectedCommand = new Command<string>(OnBarcodeDetected);

            LoadMockCameras();
        }

        // Se llama desde ScanPage.OnAppearing()
        public Task InitializeAsync()
        {
            Username = Preferences.Get("Usuario", string.Empty);
            DeviceName = Preferences.Get("DeviceName", "Dispositivo");
            DeviceLocalidad = Preferences.Get("LocalidadNombre", "Sin localidad");
            _deviceId = (uint)Preferences.Get("DeviceId", 0);

            return Task.CompletedTask;
        }

        // ======= LÓGICA DE CÁMARA / ESCANEO =======

        private void LoadMockCameras()
        {
            Cameras.Clear();
            Cameras.Add("Cámara trasera");
            Cameras.Add("Cámara frontal");
            SelectedCamera = Cameras.FirstOrDefault();
        }

        private void StartScan()
        {
            if (SelectedCamera == null)
            {
                CameraStatusMessage = "Selecciona una cámara antes de iniciar.";
                return;
            }

            IsDetecting = true;
            CameraStatusMessage = $"Escaneando con {SelectedCamera}...";
        }

        private void StopScan()
        {
            IsDetecting = false;
            CameraStatusMessage = "Escaneo detenido.";
        }

        private void OnBarcodeDetected(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            value = value.Trim();

            // ======= REGLA TRACKII =======
            // Lote: exactamente 7 dígitos numéricos
            if (value.Length == 7 && value.All(char.IsDigit))
            {
                Lote = value;
                CameraStatusMessage = $"Lote detectado: {value}";
            }
            else
            {
                NumeroParte = value;
                CameraStatusMessage = $"No. Parte detectado: {value}";
            }

            IsDetecting = false;
        }
    }
}
