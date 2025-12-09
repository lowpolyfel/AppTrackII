using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTrackII.Config;

public static class ApiConfig
{
    // Usa exactamente la que te funciona en Chrome
    public const string BaseUrl = "https://192.168.0.50";
    // (Si esto siguiera dando problemas, luego probamos con http)

    public static string Mobile(string relative)
        => $"{BaseUrl.TrimEnd('/')}/api/mobile/{relative}";
}
