using LiteDB;

public class LiteDbDataRepo : IDataRepo
{
    private readonly string _dbFilePath = @"Resources/Data/UserDatabase.db";
    private readonly LiteDatabase _db;

    public LiteDbDataRepo()
    {
        // Create a new LiteDatabase instance or open an existing one
        string dbPassword = Constants.DBPassword;
        _db = new LiteDatabase($"Filename={_dbFilePath};Password={dbPassword}");
    }

    public void AddUser(User user)
    {
        var users = _db.GetCollection<User>("users");
        user.Id = Guid.NewGuid().ToString();
        user.Created = DateTime.Now;
        user.LastActive = DateTime.Now;
        users.Insert(user);
    }

    public User GetUser(string username)
    {
        var users = _db.GetCollection<User>("users");
        return users.FindOne(x => x.Username == username);
    }

    public void UpdateUser(User user)
    {
        var users = _db.GetCollection<User>("users");
        user.LastActive = DateTime.Now;
        users.Update(user);
    }

    public void DeleteUser(string username)
    {
        var users = _db.GetCollection<User>("users");
        var user = users.FindOne(x => x.Username == username);
        if (user != null)
        {
            users.Delete(user.Id);
        }
    }
}
