namespace JWTAuthServer.Models
{
public class RefreshRequest
{
    public string Username { get; set; }
    public string RefreshToken { get; set; }
}
}