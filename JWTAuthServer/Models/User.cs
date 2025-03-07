namespace JWTAuthServer.Models
{
public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
}
}