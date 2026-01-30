using System.Collections.Generic;
using System.Threading.Tasks;
using AppTrackII.Models;

namespace AppTrackII.Services
{
    public interface ILocalidadService
    {
        Task<List<Localidad>> GetLocalidadesAsync();
    }
}
