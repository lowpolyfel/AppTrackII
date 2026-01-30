using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrackII.Config;

public static class ApiConfig
{
    // Usa exactamente la que te funciona en Chrome
    public const string BaseUrl = "http://192.168.0.8:5000/";
    // (Si esto siguiera dando problemas, luego probamos con http)

    public static string Mobile(string relative)
        => $"{BaseUrl.TrimEnd('/')}/api/mobile/{relative}";
}
