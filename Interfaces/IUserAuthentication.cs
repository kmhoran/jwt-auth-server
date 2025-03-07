public interface IUserAuthentication
{
    User GenerateUser(string username, string password);
    Session AuthenticateUser(string username, string password);
    Session RefreshToken(string username, string refreshToken);
    Session ValidateToken(string username, string token);
    void RevokeToken(string username, string token);
}