﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using AppTrackII.Services;

namespace AppTrackII.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient = new();

        // ======= CONTEXTO USUARIO / DISPOSITIVO =======

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _deviceName = "Nombre de dispositivo";
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

        private string _cantidadPiezas = string.Empty;
        public string CantidadPiezas
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
        public async Task InitializeAsync()
        {
            Username = Preferences.Get("Usuario", string.Empty);
            var deviceToken = Preferences.Get("DeviceToken", string.Empty);

            if (string.IsNullOrWhiteSpace(deviceToken))
            {
                DeviceName = "Dispositivo no registrado";
                DeviceLocalidad = "Sin localidad";
                return;
            }

            var body = new
            {
                DeviceToken = deviceToken,
                Username = Username
            };

            try
            {
                var ctx = await _apiClient.PostAsync<DeviceContextResponse>("context", body);

                if (ctx != null && ctx.Ok)
                {
                    if (!string.IsNullOrWhiteSpace(ctx.Username))
                        Username = ctx.Username;

                    DeviceName = string.IsNullOrWhiteSpace(ctx.DeviceName)
                        ? "Sin nombre"
                        : ctx.DeviceName!;

                    DeviceLocalidad = string.IsNullOrWhiteSpace(ctx.LocalidadNombre)
                        ? "Sin localidad"
                        : ctx.LocalidadNombre!;
                }
                else
                {
                    DeviceName = "Dispositivo desconocido";
                    DeviceLocalidad = "Sin localidad";
                }
            }
            catch
            {
                DeviceName = "Error al cargar dispositivo";
                DeviceLocalidad = "Error";
            }
        }

        // ======= LÓGICA DE CÁMARA / ESCANEO =======

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

            // Por ahora lo ponemos en el campo Lote.
            Lote = value;
            IsDetecting = false;
            CameraStatusMessage = $"Código detectado: {value}";
        }

        // ======= DTO respuesta /context =======

        private class DeviceContextResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public string? Username { get; set; }
            public string? Rol { get; set; }
            public string? DeviceName { get; set; }
            public int? LocalidadId { get; set; }
            public string? LocalidadNombre { get; set; }
        }
    }
}
