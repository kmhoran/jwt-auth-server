namespace JWTAuthServer.Models
{
public class TokenValidationRequest
{
    public string Username { get; set; }
    public string Token { get; set; }
}
}