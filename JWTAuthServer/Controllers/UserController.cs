using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JWTAuthServer.Interfaces;
using JWTAuthServer.Models;

namespace JWTAuthServer.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IDataRepo _dataRepo;
    private readonly IUserAuthentication _userAuthentication;

    public UserController(IDataRepo dataRepo, IUserAuthentication userAuthentication)
    {
        _dataRepo = dataRepo;
        _userAuthentication = userAuthentication;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserRegistrationRequest request)
    {
        var existingUser = _dataRepo.GetUser(request.Username);
        if (existingUser != null)
        {
            return Conflict(new { message = "Username already exists" });
        }
        User user = _userAuthentication.GenerateUser(request.Username, request.Password);
        _dataRepo.AddUser(user);
        return Ok(new { message = "User added successfully" });
    }

    [HttpGet("{username}")]
    [Authorize] 
    public IActionResult GetUser(string username)
    {
        var user = _dataRepo.GetUser(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user.ToUserResponse());
    }

    // [HttpPut("{username}")]
    // public IActionResult UpdateUser(string username, [FromBody] User user)
    // {
    //     var existingUser = _dataRepo.GetUser(username);
    //     if (existingUser == null)
    //     {
    //         return NotFound();
    //     }

    //     _dataRepo.UpdateUser(user);
    //     return Ok(new { message = "User updated successfully" });
    // }

    // [HttpDelete("{username}")]
    // public IActionResult DeleteUser(string username)
    // {
    //     var existingUser = _dataRepo.GetUser(username);
    //     if (existingUser == null)
    //     {
    //         return NotFound();
    //     }

    //     _dataRepo.DeleteUser(username);
    //     return Ok(new { message = "User deleted successfully" });
    // }
}
}