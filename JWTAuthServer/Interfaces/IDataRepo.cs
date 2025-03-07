using JWTAuthServer.Models;

namespace JWTAuthServer.Interfaces
{
public interface IDataRepo
{
    void AddUser(User user);
    User GetUser(string username);
    void UpdateUser(User user);
    void UpdateLastActive(string username);
    void DeleteUser(string username);
}
}