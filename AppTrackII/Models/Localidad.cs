namespace AppTrackII.Models;

public class Localidad
{
    public uint Id { get; set; }
    public string Nombre { get; set; } = "";
    public bool Active { get; set; }

    // Para mostrar todas, incluyendo inactivas etiquetadas
    public string Display => Active ? Nombre : $"{Nombre} (Inactiva)";
}
