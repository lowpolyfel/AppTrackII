using System.Text.Json;
using AppTrackII.Models;

namespace AppTrackII.Services;

public class ApiLocalidadService : ILocalidadService
{
    public async Task<List<Localidad>> GetLocalidadesAsync()
    {
        var result = new List<Localidad>();

        var response = await ApiClient.Client.GetAsync("api/v1/location");
        if (!response.IsSuccessStatusCode)
            return result;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        foreach (var item in doc.RootElement.EnumerateArray())
        {
            result.Add(new Localidad
            {
                Id = item.GetProperty("id").GetUInt32(),
                Nombre = item.GetProperty("name").GetString() ?? "",
                Active = item.GetProperty("active").GetBoolean()
            });
        }

        return result;
    }
}
