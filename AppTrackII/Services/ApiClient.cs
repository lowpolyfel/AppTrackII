using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using AppTrackII.Config;

namespace AppTrackII.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient()
    {
        // 👉 En DEBUG ignoramos el problema de certificado (solo LAN / desarrollo).
#if DEBUG
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        _http = new HttpClient(handler);
#else
        _http = new HttpClient();
#endif

        _http.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<T?> PostAsync<T>(string relativePath, object body)
    {
        var url = ApiConfig.Mobile(relativePath);

        try
        {
            Debug.WriteLine($"[ApiClient] POST {url}");
            var response = await _http.PostAsJsonAsync(url, body);

            var raw = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"[ApiClient] Status: {(int)response.StatusCode} {response.StatusCode}");
            Debug.WriteLine($"[ApiClient] Body: {raw}");

            if (!response.IsSuccessStatusCode)
            {
                // Lanzamos excepción para que el ViewModel la vea
                throw new HttpRequestException(
                    $"Status {(int)response.StatusCode} {response.StatusCode}. Respuesta: {raw}");
            }

            // Si quieres más control, podríamos deserializar con System.Text.Json,
            // pero para ahora está bien:
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ApiClient] ERROR llamando {url}: {ex}");
            // No devolvemos null silencioso, lanzamos hacia arriba
            throw;
        }
    }

    public async Task<T?> GetAsync<T>(string relativePath)
    {
        var url = ApiConfig.Mobile(relativePath);

        try
        {
            Debug.WriteLine($"[ApiClient] GET {url}");
            return await _http.GetFromJsonAsync<T>(url);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ApiClient] ERROR GET {url}: {ex}");
            throw;
        }
    }
}
