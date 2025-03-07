using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserAuthentication _userAuthentication;


    public AuthController(IUserAuthentication userAuthentication)
    {
        _userAuthentication = userAuthentication;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin user)
    {
        try
        {
            var session = _userAuthentication.AuthenticateUser(user.Username, user.Password);
            return Ok(new { Session = session });
        }
        catch (ArgumentException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var session = _userAuthentication.RefreshToken(request.Username, request.RefreshToken);
            return Ok(new { Session = session });
        }
        catch (ArgumentException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("validate")]
    public IActionResult ValidateToken([FromBody] TokenValidationRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            return BadRequest("Token is required.");
        }

        try
        {
            var session = _userAuthentication.ValidateToken(request.Username, request.Token);

            return Ok(new { Session = session });
        }
        catch (SecurityTokenException)
        {
            return Unauthorized("Invalid token.");
        }
        catch (ArgumentException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("An error occurred while validating the token.");
        }
    }

   
}