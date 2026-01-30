namespace AppTrackII.Models;

public class LoginResponse
{
    public string Token { get; set; } = "";
    public uint UserId { get; set; }
    public string Role { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }
}
