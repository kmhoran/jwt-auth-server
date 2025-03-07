public class Session
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Username { get; set; }
    public DateTime Expiration { get; set; }
    public DateTime RefreshExpiration { get; set; }
}