public interface IDataRepo
{
    void AddUser(User user);
    User GetUser(string username);
    void UpdateUser(User user);
    void DeleteUser(string username);
}