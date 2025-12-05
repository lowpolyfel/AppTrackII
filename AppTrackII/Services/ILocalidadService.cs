using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Threading.Tasks;
using AppTrackII.Models;

namespace AppTrackII.Services
{
    public interface ILocalidadService
    {
        Task<IReadOnlyList<Localidad>> GetLocalidadesAsync();
    }
}
