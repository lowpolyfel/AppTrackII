namespace AppTrackII.Models;

public class LoginResult
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = "";
    public string Token { get; private set; } = "";
    public uint UserId { get; private set; }
    public string Role { get; private set; } = "";

    public static LoginResult Ok(string token, uint userId, string role)
        => new()
        {
            Success = true,
            Token = token,
            UserId = userId,
            Role = role
        };

    public static LoginResult Fail(string message)
        => new()
        {
            Success = false,
            Message = message
        };
}
