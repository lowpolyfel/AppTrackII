using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AppTrackII.Config; // Asegúrate de usar tu ApiConfig para la URL

namespace AppTrackII.Services;

public static class ApiClient
{
    // Usamos la configuración centralizada para evitar errores de IP
    private static readonly string BaseUrl = ApiConfig.BaseUrl;

    private static HttpClient? _client;

    public static HttpClient Client
    {
        get
        {
            if (_client != null)
                return _client;

            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            ApplyAuthHeader();
            return _client;
        }
    }

    // ======================
    // AUTH
    // ======================

    public static async Task<string?> LoginAsync(string username, string password)
    {
        var payload = new
        {
            username = username,
            password = password
        };

        var response = await Client.PostAsync(
            "api/v1/auth/login",
            Json(payload));

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var token = doc.RootElement
            .GetProperty("token")
            .GetString();

        if (!string.IsNullOrWhiteSpace(token))
        {
            Preferences.Set("jwt", token);
            ApplyAuthHeader();
        }

        return token;
    }

    public static void Logout()
    {
        Preferences.Remove("jwt");
        _client?.DefaultRequestHeaders.Authorization = null;
    }

    // ======================
    // REGISTER (Actualizado con LocationId)
    // ======================
    public static async Task<(uint userId, uint deviceId, string username)?> RegisterAsync(
        string token,
        string deviceUid,
        string? deviceName,
        string password,
        uint locationId) // <--- NUEVO PARAMETRO
    {
        var response = await Client.PostAsync(
            "api/v1/register",
            Json(new
            {
                token = token,
                deviceUid = deviceUid,
                deviceName = deviceName,
                password = password,
                locationId = locationId // <--- ENVIAR AL API
            }));

        if (!response.IsSuccessStatusCode)
        {
            var msg = await response.Content.ReadAsStringAsync();
            throw new Exception(string.IsNullOrWhiteSpace(msg) ? "No se pudo registrar" : msg);
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return (
            doc.RootElement.GetProperty("userId").GetUInt32(),
            doc.RootElement.GetProperty("deviceId").GetUInt32(),
            doc.RootElement.GetProperty("username").GetString() ?? ""
        );
    }

    // ======================
    // HELPERS
    // ======================

    private static void ApplyAuthHeader()
    {
        if (_client == null)
            return;

        var jwt = Preferences.Get("jwt", null);
        _client.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(jwt))
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);
        }
    }

    private static StringContent Json(object obj)
    {
        return new StringContent(
            JsonSerializer.Serialize(obj),
            Encoding.UTF8,
            "application/json");
    }
}