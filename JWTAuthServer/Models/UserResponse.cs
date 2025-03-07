namespace JWTAuthServer.Models
{
public class UserResponse
{
    public string Username { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
}

public static class UserResponseExtentions
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse
        {
            Username = user.Username,
            Created = user.Created,
            LastActive = user.LastActive
        };
    }
}
}