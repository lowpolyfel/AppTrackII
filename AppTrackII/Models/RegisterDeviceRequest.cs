namespace AppTrackII.Models;

public class RegisterDeviceRequest
{
    public string DeviceUid { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public uint LocationId { get; set; }
}
