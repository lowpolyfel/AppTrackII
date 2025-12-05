using System.Collections.Generic;
using System.Threading.Tasks;
using AppTrackII.Models;

namespace AppTrackII.Services
{
    /// <summary>
    /// Versión temporal. Más adelante aquí se llamará al API de TrackII.
    /// </summary>
    public class MockLocalidadService : ILocalidadService
    {
        public Task<IReadOnlyList<Localidad>> GetLocalidadesAsync()
        {
            var list = new List<Localidad>
            {
                new Localidad { Id = 1,  Nombre = "Alloy" },
                new Localidad { Id = 2,  Nombre = "Bond" },
                new Localidad { Id = 3,  Nombre = "Coat" },
                new Localidad { Id = 4,  Nombre = "Top Rail" },
                new Localidad { Id = 5,  Nombre = "Insp Bond" },
                new Localidad { Id = 6,  Nombre = "Backfill" },
                new Localidad { Id = 7,  Nombre = "Fast Cast" },
                new Localidad { Id = 8,  Nombre = "Moldeo" },
                new Localidad { Id = 9,  Nombre = "Sand Blast" },
                new Localidad { Id = 10, Nombre = "Simbolo (PUNTO)" },
                new Localidad { Id = 11, Nombre = "Inspección Pre Tie Bar" },
                new Localidad { Id = 12, Nombre = "Tie bar" },
                new Localidad { Id = 13, Nombre = "Estaño" },
                new Localidad { Id = 14, Nombre = "Polish" },
                new Localidad { Id = 15, Nombre = "Burning" },
                new Localidad { Id = 16, Nombre = "Prueba Eléctrica" },
                new Localidad { Id = 17, Nombre = "Simbolizado laser" },
                new Localidad { Id = 18, Nombre = "Inspección Final" },
                new Localidad { Id = 19, Nombre = "Empaque" }
            };

            return Task.FromResult((IReadOnlyList<Localidad>)list);
        }
    }
}
