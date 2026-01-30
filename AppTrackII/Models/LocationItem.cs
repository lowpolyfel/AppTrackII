namespace AppTrackII.Models;

public class LocationItem
{
    public uint Id { get; set; }
    public string Name { get; set; } = "";

    public override string ToString() => Name;
}
