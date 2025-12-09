using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppTrackII.Models;

namespace AppTrackII.Services
{
    public class ApiLocalidadService : ILocalidadService
    {
        private readonly ApiClient _apiClient = new();

        public async Task<IReadOnlyList<Localidad>> GetLocalidadesAsync()
        {
            var result = await _apiClient.GetAsync<Localidad[]>("localidades");
            if (result == null)
                return Array.Empty<Localidad>();

            return result;
        }
    }
}
